using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace UpdaterApp
{
    class Program
    {
        public static event Action<string> DownloadComplete;

        static async Task Main()
        {
            try
            {
                // Subscribe to the DownloadComplete event
                DownloadComplete += (updatedExecutablePath) =>
                {
                    Log("Download complete event triggered. Launching new version...");
                    LaunchNewVersion(updatedExecutablePath);
                };

                await UpdateApplication();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during the update: {ex.Message}");
            }
        }

        public static async Task UpdateApplication()
        {
            try
            {
                Log("Killing current version of Task_Runway_x64.exe...");
                KillRunningApplication();

                Log("Checking for updates and downloading...");
                await DownloadUpdatedExecutable();
            }
            catch (Exception ex)
            {
                HandleUpdateError(ex);
            }
        }

        private static void KillRunningApplication()
        {
            foreach (var process in Process.GetProcessesByName("Task_Runway_x64"))
            {
                process.Kill();
                process.WaitForExit(); // Wait for the process to exit
            }
        }

        private static async Task DownloadUpdatedExecutable()
        {
            string versionFileUrl = "https://raw.githubusercontent.com/davidinfosec/task-runway/main/version.txt";

            Log("Checking for updates...");

            using (var httpClient = new HttpClient())
            {
                try
                {
                    string versionFileContent = await httpClient.GetStringAsync(versionFileUrl);
                    string[] versionData = versionFileContent.Split('=');
                    string version = versionData[0].Trim();
                    string downloadLink = versionData[1].Trim();

                    Log($"Latest version: {version}");

                    Log("Starting download...");
                    using (var response = await httpClient.GetAsync(downloadLink, HttpCompletionOption.ResponseHeadersRead))
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = new FileStream("Task_Runway_x64.exe", FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await stream.CopyToAsync(fileStream);
                        await fileStream.FlushAsync();
                    }

                    Log("Download completed.");
                    OnDownloadComplete("Task_Runway_x64.exe");
                }
                catch (Exception ex)
                {
                    HandleUpdateError(ex);
                }
            }
        }

        private static void LaunchNewVersion(string updatedExecutablePath)
        {
            try
            {
                string executablePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, updatedExecutablePath);
                Log($"Launching new version: {executablePath}");

                ProcessStartInfo startInfo = new ProcessStartInfo(executablePath)
                {
                    UseShellExecute = true,
                    Verb = "runas" // Indicates to run the process with elevated privileges
                };

                Process.Start(startInfo);
                Log("New version launch attempted.");

                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Log($"Error launching new version: {ex.Message}");
            }
        }


        private static void HandleUpdateError(Exception ex)
        {
            Log($"An error occurred during the update: {ex.Message}");
        }

        private static void Log(string message)
        {
            try
            {
                string logFilePath = "UpdaterLog.txt";
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine($"{DateTime.Now} - {message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging: {ex.Message}");
            }
        }

        private static void OnDownloadComplete(string updatedExecutablePath)
        {
            DownloadComplete?.Invoke(updatedExecutablePath);
        }
    }
}
