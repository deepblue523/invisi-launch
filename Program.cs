using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace InvisiLaunch
{
    internal static class Program
    {
        [STAThread]
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                ShowHelp();
                return 0;
            }

            // Check for /help first
            if (args[0].Equals("/help", StringComparison.OrdinalIgnoreCase))
            {
                ShowHelp();
                return 0;
            }

            // Parse /onError parameter first (defaults to "message")
            string onErrorAction = "message";
            List<string> filteredArgs = new List<string>();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("/onError:", StringComparison.OrdinalIgnoreCase))
                {
                    string errorActionValue = args[i].Substring("/onError:".Length).ToLower();
                    if (errorActionValue == "message" || errorActionValue == "ignore")
                    {
                        onErrorAction = errorActionValue;
                    }
                    else
                    {
                        MessageBox.Show($"Invalid /onError value: {errorActionValue}\nValid values: message, ignore", 
                            "InvisiLaunch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return 8;
                    }
                }
                else
                {
                    filteredArgs.Add(args[i]);
                }
            }

            // Update args array to exclude /onError parameter
            args = filteredArgs.ToArray();

            if (args.Length == 0)
            {
                ShowHelp();
                return 0;
            }

            // Check for /bypass - bypasses all logic and launches with shell command
            if (args[0].Equals("/bypass", StringComparison.OrdinalIgnoreCase))
            {
                if (args.Length < 2)
                {
                    HandleError("No program specified for /bypass.\nUse /help for usage information.", onErrorAction);
                    return 8;
                }

                try
                {
                    // Extract program and all its arguments
                    string[] programArgs = new string[args.Length - 1];
                    Array.Copy(args, 1, programArgs, 0, args.Length - 1);
                    LaunchWithBypass(programArgs);
                    return 0;
                }
                catch (Exception ex)
                {
                    HandleError(ex, onErrorAction);
                    return 8;
                }
            }

            try
            {
                ProcessPriorityClass priority = ProcessPriorityClass.Normal;
                int programStartIndex = 0;

                // Parse command-line parameters
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].StartsWith("/priority:", StringComparison.OrdinalIgnoreCase))
                    {
                        string priorityValue = args[i].Substring("/priority:".Length).ToUpper();
                        if (!IsValidPriorityValue(priorityValue))
                        {
                            HandleError($"Invalid priority value: {priorityValue}\nValid values: ABOVE_NORMAL, NORMAL, BELOW_NORMAL, LOW", onErrorAction);
                            return 8;
                        }
                        priority = MapPriorityValue(priorityValue);
                        programStartIndex = i + 1;
                        break;
                    }
                    else if (args[i].StartsWith("/", StringComparison.OrdinalIgnoreCase))
                    {
                        // Unknown parameter
                        HandleError($"Unknown parameter: {args[i]}\nUse /help for usage information.", onErrorAction);
                        return 8;
                    }
                    else
                    {
                        // Found the program name
                        programStartIndex = i;
                        break;
                    }
                }

                if (programStartIndex >= args.Length)
                {
                    HandleError("No program specified.\nUse /help for usage information.", onErrorAction);
                    return 8;
                }

                // Extract program and its arguments
                string[] programArgs = new string[args.Length - programStartIndex];
                Array.Copy(args, programStartIndex, programArgs, 0, args.Length - programStartIndex);

                LaunchApplication(programArgs, priority);
                return 0;
            }
            catch (Exception ex)
            {
                HandleError(ex, onErrorAction);
                return 8;
            }
        }

        private static void HandleError(Exception ex, string errorAction)
        {
            HandleError($"Failed to launch application: {ex.Message}", errorAction);
        }

        private static void HandleError(string message, string errorAction)
        {
            if (errorAction == "message")
            {
                MessageBox.Show(message, "InvisiLaunch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // If errorAction is "ignore", do nothing
        }

        private static bool IsValidPriorityValue(string priorityValue)
        {
            return priorityValue == "ABOVE_NORMAL" || 
                   priorityValue == "NORMAL" || 
                   priorityValue == "BELOW_NORMAL" || 
                   priorityValue == "LOW";
        }

        private static ProcessPriorityClass MapPriorityValue(string priorityValue)
        {
            switch (priorityValue)
            {
                case "ABOVE_NORMAL":
                    return ProcessPriorityClass.AboveNormal;
                case "NORMAL":
                    return ProcessPriorityClass.Normal;
                case "BELOW_NORMAL":
                    return ProcessPriorityClass.BelowNormal;
                case "LOW":
                    return ProcessPriorityClass.Idle; // LOW maps to Idle in ProcessPriorityClass
                default:
                    return ProcessPriorityClass.Normal; // Default fallback
            }
        }

        private static void ShowHelp()
        {
            string helpText = @"InvisiLaunch - Launch applications invisibly in the background

USAGE:
    InvisiLaunch.exe [/onError:<action>] [/priority:<value>] <program> [arguments...]
    InvisiLaunch.exe [/onError:<action>] /bypass <command>

PARAMETERS:
    /onError:<action>    Control error handling behavior
                         Values: message, ignore
                         Default: message

    /priority:<value>    Set process priority for the launched program
                         Values: ABOVE_NORMAL, NORMAL, BELOW_NORMAL, LOW
                         Default: NORMAL

    /bypass              Bypass all logic and launch the specified app with a shell command
                         Executes the command as-is without any processing

    /help                Display this help message

EXAMPLES:
    InvisiLaunch.exe notepad.exe C:\temp\file.txt
    
    InvisiLaunch.exe /priority:ABOVE_NORMAL notepad.exe C:\temp\file.txt
    
    InvisiLaunch.exe /onError:ignore notepad.exe C:\temp\file.txt
    
    InvisiLaunch.exe C:\scripts\myscript.ps1 -param1 value1
    
    InvisiLaunch.exe /priority:BELOW_NORMAL C:\scripts\backup.ps1 -source C:\data
    
    InvisiLaunch.exe /bypass notepad.exe C:\temp\file.txt
    
    InvisiLaunch.exe /onError:ignore /bypass dir C:\temp

EXIT CODES:
    0    Success
    8    Error occurred (exception or validation failure)

NOTES:
    - Parameters must come before the program name
    - PowerShell scripts (.ps1) are automatically detected and executed
    - All output is redirected and no console window is shown
    - /bypass mode executes the command as a shell command without any processing
    - /onError:message displays an error message box (default behavior)
    - /onError:ignore silently ignores errors and returns exit code 8";

            bool attachedConsole = false;
            try
            {
                if (GetConsoleWindow() == IntPtr.Zero)
                {
                    attachedConsole = AttachConsole(ATTACH_PARENT_PROCESS);
                    if (attachedConsole)
                    {
                        ResetConsoleStreams();
                    }
                }

                if (GetConsoleWindow() != IntPtr.Zero)
                {
                    Console.WriteLine(helpText);
                    Console.Out.Flush();
                }
                else if (attachedConsole)
                {
                    Console.WriteLine(helpText);
                    Console.Out.Flush();
                }
                else
                {
                    ShowHelpDialog(helpText);
                }
            }
            finally
            {
                if (attachedConsole)
                {
                    Console.Out.Flush();
                    FreeConsole();
                }
            }
        }

        private static void ShowHelpDialog(string helpText)
        {
            using (var form = new Form())
            {
                form.Text = "InvisiLaunch Help";
                form.Size = new System.Drawing.Size(600, 500);
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;
                form.StartPosition = FormStartPosition.CenterScreen;

                var textBox = new TextBox
                {
                    Multiline = true,
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Vertical,
                    Text = helpText,
                    Font = new System.Drawing.Font("Consolas", 9F),
                    Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                    Location = new System.Drawing.Point(10, 10),
                    Size = new System.Drawing.Size(form.ClientSize.Width - 20, form.ClientSize.Height - 50)
                };

                var okButton = new Button
                {
                    Text = "OK",
                    DialogResult = DialogResult.OK,
                    Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                    Width = 75,
                    Height = 23
                };
                okButton.Location = new System.Drawing.Point(
                    form.ClientSize.Width - okButton.Width - 10,
                    form.ClientSize.Height - okButton.Height - 10);

                form.Controls.Add(textBox);
                form.Controls.Add(okButton);
                form.AcceptButton = okButton;

                // Handle resize to keep controls positioned correctly
                form.Resize += (s, e) =>
                {
                    textBox.Size = new System.Drawing.Size(form.ClientSize.Width - 20, form.ClientSize.Height - 50);
                    okButton.Location = new System.Drawing.Point(
                        form.ClientSize.Width - okButton.Width - 10,
                        form.ClientSize.Height - okButton.Height - 10);
                };

                form.ShowDialog();
            }
        }

        private static void ResetConsoleStreams()
        {
            var standardOutput = Console.OpenStandardOutput();
            var outputWriter = new StreamWriter(standardOutput) { AutoFlush = true };
            Console.SetOut(outputWriter);

            var standardError = Console.OpenStandardError();
            var errorWriter = new StreamWriter(standardError) { AutoFlush = true };
            Console.SetError(errorWriter);

            var standardInput = Console.OpenStandardInput();
            var inputReader = new StreamReader(standardInput);
            Console.SetIn(inputReader);
        }

        private const int ATTACH_PARENT_PROCESS = -1;

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetConsoleWindow();

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        private static extern bool AttachConsole(int dwProcessId);

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        private static extern bool FreeConsole();

        private static void LaunchWithBypass(string[] args)
        {
            // Bypass all logic - just launch with shell command (fire-and-forget)
            string command = string.Join(" ", args);
            
            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c start \"\" {command}",
                UseShellExecute = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            // Fire-and-forget: start the process and immediately dispose
            using (var process = Process.Start(startInfo))
            {
                // Process is already detached and running independently
                // No need to wait or track it
            }
        }

        private static void LaunchApplication(string[] args, ProcessPriorityClass priority)
        {
            string programPath = args[0];
            string arguments = args.Length > 1 ? string.Join(" ", args, 1, args.Length - 1) : "";

            var startInfo = new ProcessStartInfo();

            // Check if it's a PowerShell script
            if (programPath.EndsWith(".ps1", StringComparison.OrdinalIgnoreCase))
            {
                startInfo.FileName = "powershell.exe";
                startInfo.Arguments = $"-ExecutionPolicy Bypass -File \"{programPath}\" {arguments}";
            }
            else
            {
                startInfo.FileName = programPath;
                startInfo.Arguments = arguments;
            }

            /*startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;*/
            //WindowStyle = ProcessWindowStyle.Hidden,
            startInfo.CreateNoWindow = true;

            var process = Process.Start(startInfo);
            
            // Set process priority
            if (process != null)
            {
                try
                {
                    process.PriorityClass = priority;
                }
                catch (Exception)
                {
                    // If setting priority fails, continue with default priority
                    // (This can happen if the process has already exited or permission issues)
                }
            }
            
            // Redirect output streams to null (bit bucket)
            /*if (process != null)
            {
                process.OutputDataReceived += (sender, e) => { };
                process.ErrorDataReceived += (sender, e) => { };
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
            }*/
        }
    }
}
