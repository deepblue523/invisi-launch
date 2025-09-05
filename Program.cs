using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace InvisiLaunch
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                MessageBox.Show("Usage: InvisiLaunch.exe <program> [arguments...]\n\nExamples:\n• InvisiLaunch.exe notepad.exe C:\\temp\\file.txt\n• InvisiLaunch.exe C:\\scripts\\myscript.ps1 -param1 value1", 
                    "InvisiLaunch", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                LaunchApplication(args);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to launch application: {ex.Message}", 
                    "InvisiLaunch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void LaunchApplication(string[] args)
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

            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            //WindowStyle = ProcessWindowStyle.Hidden,
            startInfo.CreateNoWindow = true;

            var process = Process.Start(startInfo);
            
            // Redirect output streams to null (bit bucket)
            if (process != null)
            {
                process.OutputDataReceived += (sender, e) => { /* Discard output */ };
                process.ErrorDataReceived += (sender, e) => { /* Discard errors */ };
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
            }
        }
    }
}
