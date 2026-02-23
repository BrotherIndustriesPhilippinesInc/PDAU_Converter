using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PDAUS_Converter
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        private const string LogDirectory = @"\\apbiphsh07\D0_ShareBrotherGroup\19_BPS\02_Application\99_Member\05_ARENGAMA\Endorsement Documents\PDAU";

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 1. FORBID the default Windows Forms error dialog. This is mandatory for servers.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            // 2. Catch UI thread crashes
            Application.ThreadException += (sender, e) =>
            {
                LogToFile("UI Thread Crash: " + e.Exception.Message);
                Application.Restart();
            };

            // 3. Catch background thread crashes
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                LogToFile("Background Crash: " + ((Exception)e.ExceptionObject).Message);
                // Application.Restart() doesn't always work reliably from background threads.
                // Environment.Exit is safer to kill the hung process, but you'd need an external restarter.
                Application.Restart();
            };

            Application.Run(new Form1());
        }

        static void LogToFile(string message)
        {
            try
            {
                // Ensure the directory exists before trying to write
                if (!Directory.Exists(LogDirectory))
                {
                    Directory.CreateDirectory(LogDirectory);
                }

                string filePath = Path.Combine(LogDirectory, "PDAUS_CrashLog.txt");

                // Format the log entry with a timestamp and a separator line
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}{new string('-', 50)}{Environment.NewLine}";

                // AppendAllText opens, writes, and closes the file automatically.
                File.AppendAllText(filePath, logEntry);
            }
            catch
            {
                // If the network drive is down, we literally can't log the error.
                // Swallow this exception. Do NOT throw it, or the crash handler itself will crash.
            }
        }
    }
}
