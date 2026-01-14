using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace InvisiLaunch
{
    public class TrayConsoleForm : Form
    {
        private NotifyIcon trayIcon;
        private TextBox outputTextBox;
        private ContextMenuStrip trayMenu;
        private string settingsPath;
        private const int DEFAULT_WIDTH = 800;
        private const int DEFAULT_HEIGHT = 600;

        public TrayConsoleForm()
        {
            InitializeComponent();
            LoadSettings();
            SetupTrayIcon();
            RedirectConsoleOutput();
            // Start hidden in tray
            this.Hide();
        }

        private void InitializeComponent()
        {
            this.Text = "InvisiLaunch Console";
            this.Size = new Size(DEFAULT_WIDTH, DEFAULT_HEIGHT);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimizeBox = true;
            this.MaximizeBox = true;
            this.ShowInTaskbar = false; // Hide from taskbar, show only in tray

            // Create output textbox with Unicode support
            outputTextBox = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Consolas", 9F), // Consolas supports Unicode
                Dock = DockStyle.Fill,
                BackColor = Color.Black,
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
                AcceptsReturn = true,
                AcceptsTab = false
            };

            this.Controls.Add(outputTextBox);

            // Handle window events
            this.Resize += TrayConsoleForm_Resize;
            this.FormClosing += TrayConsoleForm_FormClosing;
            this.Shown += TrayConsoleForm_Shown;

            // Settings path
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "InvisiLaunch");
            Directory.CreateDirectory(appDataPath);
            settingsPath = Path.Combine(appDataPath, "trayconsole.settings");
        }

        private void SetupTrayIcon()
        {
            Icon appIcon = null;
            
            // Try to get the application icon from the executable
            try
            {
                string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                if (!string.IsNullOrEmpty(exePath) && File.Exists(exePath))
                {
                    appIcon = Icon.ExtractAssociatedIcon(exePath);
                }
            }
            catch
            {
                // If extraction fails, try loading from the icon file directly
                try
                {
                    string iconPath = Path.Combine(
                        Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                        "images", "Invisible x32.ico");
                    if (File.Exists(iconPath))
                    {
                        appIcon = new Icon(iconPath);
                    }
                }
                catch
                {
                    // If that also fails, use default
                }
            }
            
            // Fall back to default icon if we couldn't load the custom one
            if (appIcon == null)
            {
                appIcon = SystemIcons.Application;
            }

            trayIcon = new NotifyIcon
            {
                Icon = appIcon,
                Text = "InvisiLaunch Console",
                Visible = true
            };

            // Create context menu
            trayMenu = new ContextMenuStrip();
            var showItem = new ToolStripMenuItem("Show Console");
            showItem.Click += (s, e) => ShowWindow();
            trayMenu.Items.Add(showItem);

            var exitItem = new ToolStripMenuItem("Exit");
            exitItem.Click += (s, e) => ExitApplication();
            trayMenu.Items.Add(exitItem);

            trayIcon.ContextMenuStrip = trayMenu;
            trayIcon.DoubleClick += (s, e) => ShowWindow();
        }

        private void TrayConsoleForm_Shown(object sender, EventArgs e)
        {
            // Window is shown
        }

        private void TrayConsoleForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                SaveSettings();
            }
            else
            {
                SaveSettings();
            }
        }

        private void TrayConsoleForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
                SaveSettings();
            }
        }

        private void ShowWindow()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
            this.BringToFront();
        }

        private void ExitApplication()
        {
            SaveSettings();
            trayIcon.Visible = false;
            trayIcon.Dispose();
            Application.Exit();
        }

        public void AppendOutput(string text)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendOutput), text);
                return;
            }

            outputTextBox.AppendText(text);
            outputTextBox.SelectionStart = outputTextBox.Text.Length;
            outputTextBox.ScrollToCaret();
        }

        private void RedirectConsoleOutput()
        {
            var consoleWriter = new ConsoleWriter(this);
            Console.SetOut(consoleWriter);
            Console.SetError(consoleWriter);
            
            // Set console encoding to UTF-8 for Unicode support (after redirecting)
            try
            {
                Console.OutputEncoding = Encoding.UTF8;
            }
            catch
            {
                // If setting encoding fails, continue without it
            }
        }

        private void LoadSettings()
        {
            try
            {
                if (File.Exists(settingsPath))
                {
                    string[] lines = File.ReadAllLines(settingsPath);
                    if (lines.Length >= 2)
                    {
                        if (int.TryParse(lines[0], out int width) && int.TryParse(lines[1], out int height))
                        {
                            if (width > 0 && height > 0 && width <= 5000 && height <= 5000)
                            {
                                this.Size = new Size(width, height);
                            }
                        }
                    }
                    if (lines.Length >= 4)
                    {
                        if (int.TryParse(lines[2], out int x) && int.TryParse(lines[3], out int y))
                        {
                            // Validate position is on screen
                            Rectangle screenBounds = Screen.PrimaryScreen.Bounds;
                            if (x >= screenBounds.Left - 100 && x <= screenBounds.Right + 100 &&
                                y >= screenBounds.Top - 100 && y <= screenBounds.Bottom + 100)
                            {
                                this.StartPosition = FormStartPosition.Manual;
                                this.Location = new Point(x, y);
                            }
                        }
                    }
                }
            }
            catch
            {
                // If loading fails, use defaults
            }
        }

        private void SaveSettings()
        {
            try
            {
                if (this.WindowState == FormWindowState.Normal)
                {
                    string content = $"{this.Width}\n{this.Height}\n{this.Left}\n{this.Top}";
                    File.WriteAllText(settingsPath, content);
                }
            }
            catch
            {
                // If saving fails, continue silently
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                SaveSettings();
                if (trayIcon != null)
                {
                    trayIcon.Visible = false;
                    trayIcon.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }

    // Custom TextWriter to redirect console output to the form
    public class ConsoleWriter : TextWriter
    {
        private TrayConsoleForm form;
        private StringBuilder buffer = new StringBuilder();

        public ConsoleWriter(TrayConsoleForm form)
        {
            this.form = form;
        }

        public override Encoding Encoding => Encoding.UTF8;

        public override void Write(char value)
        {
            buffer.Append(value);
            if (value == '\n' || buffer.Length > 1000)
            {
                Flush();
            }
        }

        public override void Write(string value)
        {
            buffer.Append(value);
            if (buffer.Length > 1000)
            {
                Flush();
            }
        }

        public override void WriteLine(string value)
        {
            buffer.Append(value);
            buffer.Append(Environment.NewLine);
            Flush();
        }

        public override void Flush()
        {
            if (buffer.Length > 0)
            {
                form.AppendOutput(buffer.ToString());
                buffer.Clear();
            }
        }
    }
}

