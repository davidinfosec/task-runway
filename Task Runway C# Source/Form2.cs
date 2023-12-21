using System.Diagnostics;
using System.IO.Compression;

namespace TaskRunway
{
    public partial class TaskRunwayExplorer : Form
    {
        private List<ToolInfo> tools = new List<ToolInfo>();
        private Dictionary<string, string> documentationLinks = new Dictionary<string, string>();
        private Dictionary<string, string> programFolderMapping = new Dictionary<string, string>();
        private string downloadFolderPath;
        private Form1 form1;

        public TaskRunwayExplorer(Form1 form1)
        {
            InitializeComponent();
            this.form1 = form1;


            // Add event handlers
            this.checkedListBox1.SelectedIndexChanged += new EventHandler(this.CheckedListBox_SelectedIndexChanged);
            this.checkedListBox2.SelectedIndexChanged += new EventHandler(this.CheckedListBox_SelectedIndexChanged);

            // Add the following lines to disable maximize button and resizing
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;



            AddTool(new ToolInfo
            {
                Name = "Task Runway CLI",
                DownloadUrl = "https://github.com/davidinfosec/task-runway/archive/refs/heads/main.zip",
                TargetFileName = "taskrunway_cli",
                ExtractType = ExtractType.Zip,
                TargetFileExtension = ".py",
                ToolDescription = "Command Line Interface for Task Runway",
                DocumentationLink = "https://taskrunway.com"
            });

            AddTool(new ToolInfo
            {
                Name = "YouTube Downloader",
                DownloadUrl = "https://github.com/davidinfosec/downloadclip/archive/refs/heads/main.zip",
                TargetFileName = "downloadclip",
                ExtractType = ExtractType.Zip,
                TargetFileExtension = ".py",
                ToolDescription = "DavidInfosec DownloadClip tool",
                DocumentationLink = "https://github.com/davidinfosec/downloadclip"
            });

            AddTool(new ToolInfo
            {
                Name = "Geo IP API Tool",
                DownloadUrl = "https://github.com/davidinfosec/IP-Tool/archive/refs/heads/main.zip",
                TargetFileName = "iptool",
                ExtractType = ExtractType.Zip,
                TargetFileExtension = ".py",
                ToolDescription = "IP API Tool to retrieve geolocation data to CSV.",
                DocumentationLink = "https://github.com/davidinfosec/IP-Tool"
            });

            AddTool(new ToolInfo
            {
                Name = "Dropfilter CLI",
                DownloadUrl = "https://github.com/crock/dropfilter-cli/archive/refs/heads/master.zip",
                TargetFileName = "main",
                ExtractType = ExtractType.Zip,
                TargetFileExtension = ".py",
                ToolDescription = "Command-line tool to filter expiring domains by configurable criteria,",
                DocumentationLink = "https://github.com/crock/dropfilter-cli"
            });


            AddTool(new ToolInfo
            {
                Name = "Spotify/YT Converter",
                DownloadUrl = "https://github.com/davidinfosec/sp.py/archive/refs/heads/main.zip",
                TargetFileName = "sp",
                ExtractType = ExtractType.Zip,
                TargetFileExtension = ".py",
                ToolDescription = "Command line, enter Spotify link, get YouTube link,",
                DocumentationLink = "https://github.com/davidinfosec/sp.py"
            });


            AddTool(new ToolInfo
            {
                Name = "WHOIS Domain Info",
                DownloadUrl = "https://github.com/davidinfosec/whois-domain-info/archive/refs/heads/main.zip",
                TargetFileName = "whois",
                ExtractType = ExtractType.Zip,
                TargetFileExtension = ".py",
                ToolDescription = "Command line WHOIS API tool,",
                DocumentationLink = "https://github.com/davidinfosec/whois-domain-info"
            });


            AddTool(new ToolInfo
            {
                Name = "Easy-Template",
                DownloadUrl = "https://github.com/davidinfosec/Easy-Template/archive/refs/heads/main.zip",
                TargetFileName = "etgen",
                ExtractType = ExtractType.Zip,
                TargetFileExtension = ".py",
                ToolDescription = "Weekly Folder template structure generator,",
                DocumentationLink = "https://github.com/davidinfosec/Easy-Template"
            });


            AddTool(new ToolInfo
            {
                Name = "Domain Name Ninja",
                DownloadUrl = "https://github.com/davidinfosec/Domain-Name-Ninja/archive/refs/heads/main.zip",
                TargetFileName = "dnn",
                ExtractType = ExtractType.Zip,
                TargetFileExtension = ".py",
                ToolDescription = "Wordlist textfuser to find available domain names,",
                DocumentationLink = "https://github.com/davidinfosec/Domain-Name-Ninja"
            });



            // Add other tools similarly

            downloadFolderPath = Path.Combine(Application.StartupPath, "Downloaded Tools");
            if (!Directory.Exists(downloadFolderPath))
            {
                Directory.CreateDirectory(downloadFolderPath);
            }

            // Add items to checkedListBox2 and their documentation links to the dictionary
            checkedListBox2.Items.Add("OsintFramework.com");
            documentationLinks["OsintFramework.com"] = "http://osintframework.com";

            checkedListBox2.Items.Add("ReportName.com");
            documentationLinks["ReportName.com"] = "http://reportname.com/about";
        }


        private void CheckedListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender == checkedListBox1 && checkedListBox1.SelectedIndex != -1)
            {
                // Clear selection in the second CheckedListBox
                checkedListBox2.ClearSelected();
            }
            else if (sender == checkedListBox2 && checkedListBox2.SelectedIndex != -1)
            {
                // Clear selection in the first CheckedListBox
                checkedListBox1.ClearSelected();
            }
        }







        private async void button1_Click_1(object sender, EventArgs e)
        {
            int downloadedToolsCount = 0;

            foreach (int index in checkedListBox1.CheckedIndices)
            {
                ToolInfo tool = tools[index];

                try
                {
                    string filePath = await DownloadFileAsync(tool, downloadFolderPath);

                    if (filePath != null)
                    {
                        string scriptConfig = tool.TargetFileName;

                        // Append scriptConfig to the path
                        string finalPath = Path.Combine(filePath, $"{tool.TargetFileName}{tool.TargetFileExtension}");

                        // Check if custom_flags is specified
                        string customFlagsLine = string.IsNullOrEmpty(tool.ToolDescription) ? "" : $"custom_flags={tool.ToolDescription}";

                        if (MessageBox.Show($"Downloaded tool: {tool.Name}\nExecutable path: {finalPath}\nDo you want to add it to the toolbox?", "Add to Toolbox", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            // Append custom_flags line only if it's specified
                            form1.AddToolToList(new Form1.ScriptingTool(tool.Name, finalPath, string.IsNullOrEmpty(tool.ToolDescription) ? "" : null));
                        }

                        downloadedToolsCount++;
                    }
                    else
                    {
                        MessageBox.Show($"Error: Unable to find the executable file for tool: {tool.Name}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error downloading tool: {tool.Name}\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // Add the rest of your code here

            if (checkedListBox2.CheckedItems.Count > 0)
            {
                foreach (var item in checkedListBox2.CheckedItems)
                {
                    string website = item.ToString();
                    form1.AddToolToList(new Form1.ScriptingTool(website, "https://" + website, ""));
                    downloadedToolsCount++;
                }
            }

            if (downloadedToolsCount > 0)
            {
                MessageBox.Show($"Downloaded {downloadedToolsCount} tool(s) to {downloadFolderPath}", "Download Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No actions performed.", "Download Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            RefreshCheckedListBox3();
        }

        public void RefreshCheckedListBox3()
        {
            checkedListBox3.Items.Clear();

            foreach (var tool in tools)
            {
                if (programFolderMapping.TryGetValue(tool.Name, out var folderName))
                {
                    string folderPath = Path.Combine(downloadFolderPath, folderName);

                    // Check if the folder exists before adding it to the list
                    if (Directory.Exists(folderPath))
                    {
                        checkedListBox3.Items.Add(tool.Name);
                    }
                }
            }
        }

        private void AddTool(ToolInfo tool)
        {
            tools.Add(tool);
            checkedListBox1.Items.Add(tool.Name);
            programFolderMapping.Add(tool.Name, tool.TargetFileName);
        }

        private async Task<string> DownloadFileAsync(ToolInfo tool, string destinationFolder)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(tool.DownloadUrl);
                response.EnsureSuccessStatusCode();

                string fileName = tool.TargetFileName + tool.TargetFileExtension;
                string filePath = Path.Combine(destinationFolder, fileName);

                if (tool.ExtractType == ExtractType.Zip)
                {
                    await DownloadAndExtractZipAsync(response, filePath, fileName);
                    return Path.Combine(destinationFolder, Path.GetFileNameWithoutExtension(fileName));
                }
                else
                {
                    await DownloadFile(response, filePath);
                    return filePath;
                }
            }
        }

        private async Task DownloadFile(HttpResponseMessage response, string filePath)
        {
            using (Stream stream = await response.Content.ReadAsStreamAsync())
            using (FileStream fs = File.Create(filePath))
            {
                await stream.CopyToAsync(fs);
            }
        }

        private async Task DownloadAndExtractZipAsync(HttpResponseMessage response, string filePath, string extractedFileName)
        {
            using (Stream stream = await response.Content.ReadAsStreamAsync())
            using (ZipArchive archive = new ZipArchive(stream))
            {
                string subfolderPath = Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath));
                Directory.CreateDirectory(subfolderPath);

                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    // Skip entries corresponding to directories (if any)
                    if (string.IsNullOrEmpty(entry.Name))
                        continue;

                    // Remove the root directory from the entry path
                    string entryPath = entry.FullName;
                    int indexOfFirstSlash = entryPath.IndexOf('/');
                    if (indexOfFirstSlash >= 0)
                    {
                        entryPath = entryPath.Substring(indexOfFirstSlash + 1);
                    }

                    string entryTargetPath = Path.Combine(subfolderPath, entryPath);

                    // Create directories if they don't exist yet
                    string directoryPath = Path.GetDirectoryName(entryTargetPath);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    // Extract the entry to the target path
                    entry.ExtractToFile(entryTargetPath, true);
                }
            }
        }

        private bool IsExecutable(string filePath)
        {
            try
            {
                FileAttributes attributes = File.GetAttributes(filePath);
                return (attributes & FileAttributes.Directory) == 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (checkedListBox3.CheckedItems.Count == 0)
            {
                MessageBox.Show("Please select a program to uninstall.", "Uninstall Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (var item in checkedListBox3.CheckedItems)
            {
                string programName = item.ToString();

                if (programFolderMapping.TryGetValue(programName, out var folderName))
                {
                    string itemPath = Path.Combine(downloadFolderPath, folderName);

                    if (IsProgram(folderName))
                    {
                        if (IsProgramRunning(itemPath))
                        {
                            DialogResult result = MessageBox.Show("It looks like a program is still open. Please close it before uninstalling. Continue with uninstallation?", "Program Running", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (result == DialogResult.No)
                            {
                                return;
                            }
                        }

                        DialogResult confirmResult = MessageBox.Show($"Are you sure you want to uninstall {programName}?", "Confirm Uninstall", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (confirmResult == DialogResult.Yes)
                        {
                            form1.RemoveToolFromList(new Form1.ScriptingTool(programName, itemPath, "customFlags"));
                            Directory.Delete(itemPath, true);
                        }
                    }
                    else
                    {
                        // ... (rest of your code remains unchanged)
                    }
                }
            }

            RefreshCheckedListBox3();
        }

        private bool IsProgram(string itemName)
        {
            return Directory.Exists(Path.Combine(downloadFolderPath, itemName));
        }

        private bool IsProgramRunning(string folderPath)
        {
            try
            {
                Process[] processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(folderPath));
                return processes.Length > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void checkedListBox3_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (checkedListBox3.SelectedIndex != -1)
            {
                string selectedItem = checkedListBox3.SelectedItem.ToString();
                OpenSelectedItem(selectedItem);
            }
        }

        private void OpenSelectedItem(string selectedItem)
        {
            if (programFolderMapping.TryGetValue(selectedItem, out var folderName))
            {
                string folderPath = Path.Combine(downloadFolderPath, folderName);

                if (IsProgram(folderName))
                {
                    OpenProgram(selectedItem);
                }
                else
                {
                    OpenWebsite(selectedItem);
                }
            }
        }

        private void OpenProgram(string programName)
        {
            MessageBox.Show($"Opening program: {programName}", "Open Program", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OpenWebsite(string websiteName)
        {
            MessageBox.Show($"Navigating to website: {websiteName}", "Open Website", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void checkedListBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (checkedListBox3.SelectedIndex != -1)
            {
                string selectedItem = checkedListBox3.SelectedItem.ToString();
                if (tools.FirstOrDefault(t => t.Name == selectedItem && !string.IsNullOrEmpty(t.DocumentationLink)) != null)
                {
                    button3.Visible = true; // Show button if documentation link exists
                }
                else
                {
                    button3.Visible = false; // Hide button if no documentation link
                }
            }
        }


        private void checkedListBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ... (existing code)
        }

        private void label5_Click(object sender, EventArgs e)
        {
            // ... (existing code)
        }

        public enum ExtractType
        {
            Zip,
            // Add other extraction types if needed
        }

        public class ToolInfo
        {
            public string Name { get; set; }
            public string DownloadUrl { get; set; }
            public string TargetFileName { get; set; }
            public ExtractType ExtractType { get; set; }
            public string TargetFileExtension { get; set; }
            public string ToolDescription { get; set; }
            public string DocumentationLink { get; set; }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string selectedItem = null;
            string documentationLink = null;

            if (checkedListBox1.SelectedItem != null)
            {
                selectedItem = checkedListBox1.SelectedItem.ToString();
                var tool = tools.FirstOrDefault(t => t.Name == selectedItem);
                documentationLink = tool?.DocumentationLink;
            }
            else if (checkedListBox2.SelectedItem != null)
            {
                selectedItem = checkedListBox2.SelectedItem.ToString();
                documentationLinks.TryGetValue(selectedItem, out documentationLink);
            }

            if (!string.IsNullOrEmpty(documentationLink))
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = documentationLink,
                        UseShellExecute = true
                    });
                }
                catch
                {
                    MessageBox.Show("Could not open the documentation link.");
                }
            }
            else
            {
                MessageBox.Show("No documentation link available for the selected item.");
            }
        }


    }
}
