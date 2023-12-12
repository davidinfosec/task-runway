// Task_Runway_x64, Version=1.0.2, Culture=neutral, PublicKeyToken=null
// Form1
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.DirectoryServices;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using TaskRunway;
using UpdaterApp;


public class Form1 : Form
{
    private Version currentVersion;

    private string downloadLink;

    private bool isManualCheck = false;


    private bool isSearchbarVisible = true;

    public class ExploreToolsForm : Form
    {
        public ExploreToolsForm()
        {
            Text = "Explore New Tools";
        }
    }

public class ScriptingTool
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public string CustomFlags { get; set; }

        public ScriptingTool(string name, string path, string customFlags)
        {
            Name = name;
            Path = path;
            CustomFlags = customFlags;
        }
    }

    private IContainer components = null;

    private Button button1;

    private Button button3;

    private ListBox listBox1;

    private Button button5;

    private Label label1;

    private Button button6;

    private LinkLabel linkLabel1;

    private LinkLabel linkLabel2;

    private Label label3;

    private List<ScriptingTool> tools = new List<ScriptingTool>();

    private string configPath = "config.txt";

    private TaskRunwayExplorer form2;

    private ContextMenuStrip listBoxContextMenu;

    private const int SW_SHOWNORMAL = 1;

    private List<ScriptingTool> deletedTools = new List<ScriptingTool>();

    private Stack<Tuple<int, string>> renamedToolsHistory = new Stack<Tuple<int, string>>();

    private Label label2;

    private Label label4;

    private void ExploreNewTools()
    {
        form2 = new TaskRunwayExplorer(this);

        // Handle the Shown event to refresh checkedListBox3 when the form is shown
        form2.Shown += (sender, e) => form2.RefreshCheckedListBox3();

        form2.Show();
    }


    public Form1()
    {
        InitializeComponent();
        currentVersion = new Version("1.0.2");
        InitializeContextMenu();
        textBox1.TextChanged += textBox1_TextChanged;
        textBox1.KeyDown += textBox1_KeyDown;
        LoadToolsFromConfig();
        base.MaximizeBox = false;
        base.MinimizeBox = true;
        base.FormBorderStyle = FormBorderStyle.FixedSingle;
        listBox1.KeyDown += listBox1_KeyDown;
    }




    private async Task<(string status, string downloadLink, string version)> GetUpdateStatus()
    {
        // Replace this URL with the actual URL of your version.txt file on the server
        var versionFileUrl = "https://raw.githubusercontent.com/davidinfosec/task-runway/main/version.txt";
        var tempVersionFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "versioncheck.txt");

        try
        {
            using (var httpClient = new HttpClient())
            using (var responseStream = await httpClient.GetStreamAsync(new Uri(versionFileUrl)))
            using (var fileStream = File.Create(tempVersionFile))
            {
                await responseStream.CopyToAsync(fileStream);
            }
        }
        catch (Exception)
        {
            return ("error", null, null);
        }

        if (File.Exists(tempVersionFile))
        {
            string[] versionFileLines = File.ReadAllLines(tempVersionFile);

            if (versionFileLines.Length > 0)
            {
                string versionLine = versionFileLines[0]; // Get the first line
                var versionParts = versionLine.Split('=');

                if (versionParts.Length == 2)
                {
                    string version = versionParts[0].Trim();
                    string newDownloadLink = versionParts[1].Trim();

                    Console.WriteLine($"Extracted version: {version}"); // Add this line for debugging

                    if (IsUpdateNeeded(version))
                    {
                        return ("needs_update", newDownloadLink, version);
                    }
                    else
                    {
                        return ("updated", null, version);
                    }
                }
            }
        }

        return ("error", null, null);
    }

    private bool IsUpdateNeeded(string newVersion)
    {
        try
        {
            // Parse the version number from the newVersion string in version.txt
            Version latestVersion = new Version(newVersion);

            // Use the currentVersion variable
            return currentVersion < latestVersion;
        }
        catch (Exception)
        {
            return false;
        }
    }



    private void ShowUpdateForm(string version)
    {
        var updateForm = new Update();

        updateForm.OnViewChanges = () => ViewChangelog();
        updateForm.OnUpdateNow = () => UpdateNow();

        updateForm.ShowDialog();
    }

    private void ViewChangelog()
    {
        // Open URL for changelog
        Process.Start(new ProcessStartInfo
        {
            FileName = "https://TaskRunway.com/release",
            UseShellExecute = true
        });
    }

    private void UpdateNow()
    {
        if (downloadLink != null)
        {
            try
            {
                Console.WriteLine("Starting updater application...");
                Process.Start("UpdaterApp.exe");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during the update: {ex.Message}", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        LoadAsync(); // Call the asynchronous method
    }

    private async void LoadAsync()
    {
        await CheckForUpdates(); // Automatic check on load
    }

    private void checkForUpdatesMenuItem_Click(object sender, EventArgs e)
    {
        CheckForUpdates(isManualCheck: true); // Manual check
    }

    private async Task CheckForUpdates(bool isManualCheck = false)
    {
        var (status, newDownloadLink, version) = await GetUpdateStatus();

        if (status == "needs_update")
        {
            this.downloadLink = newDownloadLink;
            ShowUpdateForm(version);
        }
        else if (status == "updated" && isManualCheck)
        {
            MessageBox.Show($"You are up to date! ({currentVersion})", "Up to Date", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        // Handle the 'error' case as needed
    }




















    public void RemoveToolFromList(ScriptingTool tool)
    {
        if (tools.Contains(tool))
        {
            tools.Remove(tool);
        }
    }



    private Stack<List<ScriptingTool>> sortHistory = new Stack<List<ScriptingTool>>();
    private Stack<Tuple<int, int>> moveHistory = new Stack<Tuple<int, int>>();

    private void SortToolsAscending()
    {
        SaveSortState();  // Save the state before sorting

        // Implement logic to sort originalOrderTools in ascending order
        originalOrderTools = originalOrderTools.OrderBy(tool => tool.Name).ToList();

        // Update the tools list based on the sorted originalOrderTools
        tools = new List<ScriptingTool>(originalOrderTools);

        // Save the sorted order to config
        SaveToolsToConfig();

        // Update the filteredTools list if a search is active
        if (!string.IsNullOrWhiteSpace(textBox1.Text))
        {
            PerformSearch();
        }
        else
        {
            UpdateListBox();
            // Explicitly update filteredTools based on the sorted tools list
            filteredTools = new List<ScriptingTool>(tools);
        }
    }

    private void SortToolsDescending()
    {
        SaveSortState();  // Save the state before sorting

        // Implement logic to sort originalOrderTools in descending order
        originalOrderTools = originalOrderTools.OrderByDescending(tool => tool.Name).ToList();

        // Update the tools list based on the sorted originalOrderTools
        tools = new List<ScriptingTool>(originalOrderTools);

        // Save the sorted order to config
        SaveToolsToConfig();

        // Update the filteredTools list if a search is active
        if (!string.IsNullOrWhiteSpace(textBox1.Text))
        {
            PerformSearch();
        }
        else
        {
            UpdateListBox();
            // Explicitly update filteredTools based on the sorted tools list
            filteredTools = new List<ScriptingTool>(tools);
        }
    }

    private void SaveSortState()
    {
        sortHistory.Push(new List<ScriptingTool>(tools));
    }

    private void MoveItemUp()
    {
        int selectedIndex = listBox1.SelectedIndex;
        MoveItem(selectedIndex, -1);
    }

    private void MoveItemDown()
    {
        int selectedIndex = listBox1.SelectedIndex;
        MoveItem(selectedIndex, 1);
    }


    private void MoveItem(int selectedIndex, int direction)
    {
        int newIndex = selectedIndex + direction;

        if (newIndex >= 0 && newIndex < filteredTools.Count)
        {
            // Swap the selected item with the one above or below it
            SwapItemsInList(tools, selectedIndex, newIndex);
            SwapItemsInList(originalOrderTools, selectedIndex, newIndex);
            SwapItemsInList(filteredTools, selectedIndex, newIndex);

            // Update the display in the listBox
            UpdateListBox();

            // Update the selected index after the swap
            listBox1.SelectedIndex = newIndex;

            // Save the move to the history stack
            moveHistory.Push(new Tuple<int, int>(selectedIndex, newIndex));

            // Save the tools to the configuration
            SaveToolsToConfig();
        }
    }




    // Helper method to swap items in a list
    private void SwapItemsInList<T>(List<T> list, int index1, int index2)
    {
        if (index1 >= 0 && index1 < list.Count && index2 >= 0 && index2 < list.Count)
        {
            T temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;
        }
    }

    // Make the class and event public
    public class UpdateEventManager
    {
        // Make the event public
        public static event Action<string> DownloadComplete;

        // Make the method public
        public static void OnDownloadComplete(string updatedExecutablePath)
        {
            DownloadComplete?.Invoke(updatedExecutablePath);
        }
    }



    private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
    }

    private void listBox1_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            int index = listBox1.IndexFromPoint(e.Location);
            if (index >= 0)
            {
                listBox1.SelectedIndex = index;
                listBoxContextMenu.Show(listBox1, listBox1.PointToClient(System.Windows.Forms.Cursor.Position));
            }
        }
    }

 

    private void InitializeContextMenu()
    {
        listBoxContextMenu = new ContextMenuStrip();


        // Edit Path
        ToolStripMenuItem editPathItem = new ToolStripMenuItem("Edit Path");
        editPathItem.Click += delegate
        {
            EditPath();
        };
        listBoxContextMenu.Items.Add(editPathItem);

        // ... (existing code)

        listBox1.ContextMenuStrip = listBoxContextMenu;
        listBox1.MouseDown += listBox1_MouseDown;
   

    // Refresh
    ToolStripMenuItem refreshItem = new ToolStripMenuItem("Refresh");
        refreshItem.Click += delegate
        {
            RefreshTools();
        };
        listBoxContextMenu.Items.Add(refreshItem);

        // Undo
        ToolStripMenuItem undoItem = new ToolStripMenuItem("Undo");
        undoItem.Click += delegate
        {
            UndoChanges();
        };
        listBoxContextMenu.Items.Add(undoItem);

        // Custom Flags
        ToolStripMenuItem customFlagsItem = new ToolStripMenuItem("Custom Flags");
        customFlagsItem.Click += delegate
        {
            CustomFlags();
        };
        listBoxContextMenu.Items.Add(customFlagsItem);

        listBoxContextMenu.Items.Add(new ToolStripSeparator());

        // Rename
        ToolStripMenuItem renameItem = new ToolStripMenuItem("Rename");
        renameItem.Click += delegate
        {
            RenameTool();
        };
        listBoxContextMenu.Items.Add(renameItem);

        // Move Item Up
        ToolStripMenuItem moveItemUp = new ToolStripMenuItem("Move Item Up");
        moveItemUp.Click += delegate
        {
            MoveItemUp();
        };
        listBoxContextMenu.Items.Add(moveItemUp);

        // Move Item Down
        ToolStripMenuItem moveItemDown = new ToolStripMenuItem("Move Item Down");
        moveItemDown.Click += delegate
        {
            MoveItemDown();
        };
        listBoxContextMenu.Items.Add(moveItemDown);

        listBox1.ContextMenuStrip = listBoxContextMenu;
        listBox1.MouseDown += listBox1_MouseDown;
    }




    // Declare history stack
    private Stack<Tuple<int, string, string>> pathChangesHistory = new Stack<Tuple<int, string, string>>();

    private List<ScriptingTool> FilterTools(string searchText)
    {
        return tools
            .Where(tool => tool.Name.ToLower().Contains(searchText))
            .ToList();
    }



  private void EditPath()
{
    // Get the selected index from the ListBox
    int selectedIndex = listBox1.SelectedIndex;

    // Check if the selected index is within the valid range of filteredTools
    if (selectedIndex >= 0 && selectedIndex < filteredTools.Count)
    {
        // Retrieve the selected tool from filteredTools
        ScriptingTool selectedTool = filteredTools[selectedIndex];

        // Example: Use InputBox or any other UI element to get the new path
        string currentPath = selectedTool.Path;
        string newPath = Interaction.InputBox("Enter the new path:", "Edit Path", currentPath);

        // Check if the new path is not empty or null
        if (!string.IsNullOrWhiteSpace(newPath))
        {
            // Push the current state into the history stack
            pathChangesHistory.Push(new Tuple<int, string, string>(selectedIndex, currentPath, newPath));

            // Update the tool's path
            selectedTool.Path = newPath;

            // Save the changes to the configuration
            SaveToolsToConfig();

            // Reapply the search if there is text in the search TextBox
            if (!string.IsNullOrWhiteSpace(textBox1.Text))
            {
                PerformSearch();
            }
            else
            {
                // Update the display in listBox1 with the filtered tools
                UpdateListBox(filteredTools);
            }
        }
    }
}






    private void UpdateListBoxForSearch(List<ScriptingTool> items)
    {
        listBox1.Items.Clear();

        foreach (ScriptingTool tool in items)
        {
            listBox1.Items.Add(tool.Name);
        }
    }



 private void RenameTool()
{
    // Get the selected index from the ListBox
    int selectedIndex = listBox1.SelectedIndex;

    // Check if the selected index is within the valid range of filteredTools
    if (selectedIndex >= 0 && selectedIndex < filteredTools.Count)
    {
        // Retrieve the selected tool from filteredTools
        ScriptingTool selectedTool = filteredTools[selectedIndex];

        // Get the current name of the tool
        string currentName = selectedTool.Name;

        // Use InputBox or any other UI element to get the new name
        string newName = Interaction.InputBox("Enter a new name:", "Rename Tool", currentName);

        // Check if the new name is not empty or null
        if (!string.IsNullOrEmpty(newName))
        {
            // Push the current state into the history stack
            renamedToolsHistory.Push(new Tuple<int, string>(selectedIndex, currentName));

            // Update the tool's name
            selectedTool.Name = newName;

            // Save the updated tools to the configuration file
            SaveToolsToConfig();

            // Update the display in listBox1 with the filtered tools
            UpdateListBox(filteredTools);
        }
    }
}






    public static string InputBox(string prompt, string title, string defaultValue = "")
    {
        Form promptForm = new Form
        {
            Width = 250,
            Height = 130,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            Text = title,
            StartPosition = FormStartPosition.CenterScreen
        };
        Label textLabel = new Label
        {
            Left = 20,
            Top = 20,
            Text = prompt,
            Width = 200
        };
        TextBox textBox = new TextBox
        {
            Left = 20,
            Top = 50,
            Width = 150,
            Text = defaultValue
        };
        Button confirmation = new Button
        {
            Text = "Ok",
            Left = 180,
            Width = 40,
            Top = 50,
            DialogResult = DialogResult.OK
        };
        Button cancellation = new Button
        {
            Text = "Cancel",
            Left = 130,
            Width = 40,
            Top = 50,
            DialogResult = DialogResult.Cancel
        };
        confirmation.Click += delegate
        {
            promptForm.Close();
        };
        cancellation.Click += delegate
        {
            promptForm.Close();
        };
        textBox.KeyDown += delegate (object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                e.SuppressKeyPress = true;
                confirmation.PerformClick();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                cancellation.PerformClick();
            }
        };
        promptForm.Controls.Add(textBox);
        promptForm.Controls.Add(confirmation);
        promptForm.Controls.Add(cancellation);
        promptForm.Controls.Add(textLabel);
        return (promptForm.ShowDialog() == DialogResult.OK) ? textBox.Text : "";
    }

    private void button1_Click(object sender, EventArgs e)
    {
        LaunchSelectedTool();
    }

    private void listBox1_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Return)
        {
            LaunchSelectedTool();
            e.Handled = true;
        }
        else if (e.Control && (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down))
        {
            int selectedIndex = listBox1.SelectedIndex;
            int direction = (e.KeyCode == Keys.Up) ? -1 : 1;
            MoveItem(selectedIndex, direction);
            e.Handled = true;
        }
    }


    private int originalMoveIndex; // Add this variable at the class level

    private void listBox1_MouseWheel(object sender, MouseEventArgs e)
    {
        // Check if the Ctrl key is held down
        if (Control.ModifierKeys == Keys.Control)
        {
            // Determine the direction of the scroll (up or down)
            int direction = (e.Delta > 0) ? -1 : 1;

            // Move the selected item based on the direction
            MoveItem(listBox1.SelectedIndex, direction, true);

            // Prevent the default scrolling behavior
            ((HandledMouseEventArgs)e).Handled = true;
        }
    }

    private void MoveItem(int selectedIndex, int direction, bool recordOriginalIndex = false)
    {
        int newIndex = selectedIndex + direction;

        if (newIndex >= 0 && newIndex < filteredTools.Count)
        {
            // Record the original index only if specified
            if (recordOriginalIndex)
            {
                originalMoveIndex = selectedIndex;
            }

            // Swap the selected item with the one above or below it
            SwapItemsInList(tools, selectedIndex, newIndex);
            SwapItemsInList(originalOrderTools, selectedIndex, newIndex);
            SwapItemsInList(filteredTools, selectedIndex, newIndex);

            // Update the display in the listBox
            UpdateListBox();

            // Update the selected index after the swap
            listBox1.SelectedIndex = newIndex;

            // Save the move to the history stack
            moveHistory.Push(new Tuple<int, int>(originalMoveIndex, newIndex));

            // Save the tools to the configuration
            SaveToolsToConfig();
        }
    }








    private void LaunchSelectedTool()
    {
        int selectedIndex = listBox1.SelectedIndex;
        if (selectedIndex < 0 || selectedIndex >= filteredTools.Count)
        {
            return;
        }

        ScriptingTool selectedTool = filteredTools[selectedIndex];

        try
        {
            Uri uri;
            if (selectedTool.Path.EndsWith(".bat", StringComparison.OrdinalIgnoreCase))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/k \"" + selectedTool.Path + "\" " + selectedTool.CustomFlags,
                    WorkingDirectory = Path.GetDirectoryName(selectedTool.Path),
                    CreateNoWindow = false
                });
            }
            else if (selectedTool.Path.EndsWith(".py", StringComparison.OrdinalIgnoreCase))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/k python \"" + selectedTool.Path + "\" " + selectedTool.CustomFlags,
                    WorkingDirectory = Path.GetDirectoryName(selectedTool.Path),
                    CreateNoWindow = false
                });
            }
            else if (selectedTool.Path.EndsWith(".ps1", StringComparison.OrdinalIgnoreCase))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = "-File \"" + selectedTool.Path + "\" " + selectedTool.CustomFlags,
                    WorkingDirectory = Path.GetDirectoryName(selectedTool.Path),
                    CreateNoWindow = false
                });
            }
            else if (selectedTool.Path.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = selectedTool.Path,
                    WorkingDirectory = Path.GetDirectoryName(selectedTool.Path),
                    CreateNoWindow = false
                });
            }
            else if (selectedTool.Path.EndsWith(".jar", StringComparison.OrdinalIgnoreCase))
            {
                // Assuming 'java.exe' is in the system's PATH
                Process.Start(new ProcessStartInfo
                {
                    FileName = "java.exe",
                    Arguments = $"-jar \"{selectedTool.Path}\"",
                    WorkingDirectory = Path.GetDirectoryName(selectedTool.Path),
                    CreateNoWindow = false
                });
            }
            else if (Uri.TryCreate(selectedTool.Path, UriKind.Absolute, out uri) && uri.Scheme.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = selectedTool.Path,
                    UseShellExecute = true
                });
            }
            else if (selectedTool.Path.EndsWith(".docx", StringComparison.OrdinalIgnoreCase) ||
                     selectedTool.Path.EndsWith(".md", StringComparison.OrdinalIgnoreCase) ||
                     selectedTool.Path.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) ||
                     selectedTool.Path.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                // Open document and PDF files with the default associated application
                ShellExecute(0, "open", selectedTool.Path, "", "", 1);
            }
            else if (Directory.Exists(selectedTool.Path)) // Check if it's a directory
            {
                Process.Start("explorer.exe", selectedTool.Path);
            }
            else
            {
                MessageBox.Show("Invalid URL format or unsupported file type: " + selectedTool.Path, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }
    }


    [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern nint ShellExecute(nint hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);

    private void button2_Click(object sender, EventArgs e)
    {
    }

    private void button3_Click(object sender, EventArgs e)
    {
        ContextMenuStrip contextMenu = new ContextMenuStrip();
        ToolStripMenuItem addWebsiteToolItem = new ToolStripMenuItem("Add Website (URL)");
        ToolStripMenuItem addScriptToolItem = new ToolStripMenuItem("Add Script (.ps1, .bat, .py)");
        ToolStripMenuItem addExecutableToolItem = new ToolStripMenuItem("Add Program (.exe, .jar)");
        ToolStripMenuItem addDocumentToolItem = new ToolStripMenuItem("Add Document (.docx, .md, .txt, .pdf)");
        ToolStripMenuItem addFolderPathToolItem = new ToolStripMenuItem("Add Folder Path");

        addWebsiteToolItem.Click += delegate
        {
            AddTool("Website");
        };

        addScriptToolItem.Click += delegate
        {
            AddTool("Script");
        };

        addExecutableToolItem.Click += delegate
        {
            AddTool("Executable");
        };

        addDocumentToolItem.Click += delegate
        {
            AddTool("Document");
        };

        addFolderPathToolItem.Click += delegate
        {
            AddTool("Folder");
        };

        contextMenu.Items.Add(addWebsiteToolItem);
        contextMenu.Items.Add(addScriptToolItem);
        contextMenu.Items.Add(addExecutableToolItem);
        contextMenu.Items.Add(addDocumentToolItem);
        contextMenu.Items.Add(addFolderPathToolItem);

        button3.ContextMenuStrip = contextMenu;
        button3.ContextMenuStrip.Show(button3, new Point(0, button3.Height));
    }

    private void AddTool(string toolType)
    {
        switch (toolType)
        {
            case "Website":
                {
                    string url = Interaction.InputBox("Enter the URL (http(s)://example.com):", "Enter URL");
                    if (!string.IsNullOrWhiteSpace(url))
                    {
                        string toolName3 = Interaction.InputBox("Enter a name for the tool (optional):", "Enter Name", url);
                        if (string.IsNullOrWhiteSpace(toolName3))
                        {
                            toolName3 = new Uri(url).Host;
                        }
                        AddToolToList(new ScriptingTool(toolName3, url, ""));
                    }
                    break;
                }
            case "Script":
                {
                    OpenFileDialog openFileDialog2 = new OpenFileDialog
                    {
                        Title = "Select Script File",
                        Filter = "Script files (*.bat;*.ps1;*.py)|*.bat;*.ps1;*.py",
                        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal)
                    };
                    if (openFileDialog2.ShowDialog() == DialogResult.OK)
                    {
                        string toolName2 = Interaction.InputBox("Enter a name for the tool (optional):", "Enter Name", openFileDialog2.SafeFileName);
                        if (string.IsNullOrWhiteSpace(toolName2))
                        {
                            toolName2 = Path.GetFileNameWithoutExtension(openFileDialog2.FileName);
                        }
                        AddToolToList(new ScriptingTool(toolName2, openFileDialog2.FileName, ""));
                    }
                    break;
                }
            case "Executable":
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog
                    {
                        Title = "Select Executable File",
                        Filter = "Executable files (*.exe;*.jar)|*.exe;*.jar",
                        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
                    };
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string toolName = Interaction.InputBox("Enter a name for the tool (optional):", "Enter Name", openFileDialog.SafeFileName);
                        if (string.IsNullOrWhiteSpace(toolName))
                        {
                            toolName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                        }
                        AddToolToList(new ScriptingTool(toolName, openFileDialog.FileName, ""));
                    }
                    break;
                }

            case "Document":
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog
                    {
                        Title = "Select Document File",
                        Filter = "Document files (*.docx;*.md;*.txt;*.pdf)|*.docx;*.md;*.txt;*.pdf",
                        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal)
                    };
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string toolName = Interaction.InputBox("Enter a name for the tool (optional):", "Enter Name", openFileDialog.SafeFileName);
                        if (string.IsNullOrWhiteSpace(toolName))
                        {
                            toolName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                        }

                        // Add the tool to the list (or perform any other necessary actions)
                        AddToolToList(new ScriptingTool(toolName, openFileDialog.FileName, ""));
                    }
                    break;
                }
            case "Folder":
                {
                    FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
                    {
                        Description = "Select Folder",
                        RootFolder = Environment.SpecialFolder.MyComputer
                    };

                    if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                    {
                        string folderPath = folderBrowserDialog.SelectedPath;

                        string toolNameForFolder = Interaction.InputBox("Enter a name for the tool (optional):", "Enter Name", folderPath);
                        if (string.IsNullOrWhiteSpace(toolNameForFolder))
                        {
                            toolNameForFolder = new DirectoryInfo(folderPath).Name;
                        }

                        AddToolToList(new ScriptingTool(toolNameForFolder, folderPath, ""));
                    }
                    break;
                }
        }
    }

    public void AddToolToList(ScriptingTool tool)
    {
        tools.Add(tool);
        originalOrderTools.Add(tool); // Add the new tool to originalOrderTools as well
        SaveToolsToConfig(); // Save the tools to the configuration file immediately after adding a new tool
        UpdateListBox();
    }


    private void SaveToolsToConfig()
    {
        using (StreamWriter writer = new StreamWriter(configPath))
        {
            foreach (ScriptingTool tool in originalOrderTools)
            {
                writer.WriteLine("[" + tool.Name + "]");
                writer.WriteLine("path=" + tool.Path);
                writer.WriteLine("custom_flags=" + tool.CustomFlags);
                writer.WriteLine();
            }
        }
    }




    private List<ScriptingTool> originalOrderTools = new List<ScriptingTool>();

    private void LoadToolsFromConfig()
    {
        if (!File.Exists(configPath))
        {
            return;
        }

        tools.Clear();
        originalOrderTools.Clear();

        using (StreamReader reader = new StreamReader(configPath))
        {
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine()?.Trim();
                if (line != null && line.StartsWith("[") && line.EndsWith("]"))
                {
                    string toolName = line.Substring(1, line.Length - 2);
                    string pathLine = reader.ReadLine()?.Trim();
                    string customFlagsLine = reader.ReadLine()?.Trim();

                    if (pathLine != null && customFlagsLine != null && pathLine.StartsWith("path=") && customFlagsLine.StartsWith("custom_flags="))
                    {
                        string path = pathLine.Substring("path=".Length);
                        string customFlags = customFlagsLine.Substring("custom_flags=".Length);
                        ScriptingTool tool = new ScriptingTool(toolName, path, customFlags);

                        // Add to both the tools list and the original order list
                        tools.Add(tool);
                        originalOrderTools.Add(tool);
                    }
                }
            }
        }

        // Ensure filteredTools is initialized with all tools
        filteredTools = new List<ScriptingTool>(originalOrderTools);

        UpdateListBox();  // Update the display with the initial tools
    }



    private void UpdateListBox(List<ScriptingTool> items = null)
    {
        listBox1.Items.Clear();

        List<ScriptingTool> toolsToDisplay = items ?? tools;

        foreach (ScriptingTool tool in toolsToDisplay)
        {
            listBox1.Items.Add(tool.Name);
        }
    }







    private void textBox1_TextChanged(object sender, EventArgs e)
    {
        PerformSearch();
    }

    private void textBox1_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            PerformSearch();
            e.SuppressKeyPress = true; // Suppress the Enter key so it doesn't add a new line in the TextBox
        }
        else if (e.KeyCode == Keys.Escape)
        {
            // Reset TextBox and remove focus
            textBox1.Text = string.Empty;
            textBox1.Focus();
            e.SuppressKeyPress = true; // Suppress the Escape key
        }
    }

    private void textBox1_LostFocus(object sender, EventArgs e)
    {
        // Reset TextBox when it loses focus
        textBox1.Text = string.Empty;
    }

    private void Form1_Click(object sender, EventArgs e)
    {
        // Set focus to the form to remove focus from TextBox when clicking outside of it
        this.Focus();
    }

    private Dictionary<string, string> searchResults = new Dictionary<string, string>();


    private List<ScriptingTool> filteredTools = new List<ScriptingTool>();

    private void PerformSearch()
    {
        string searchText = textBox1.Text.ToLower();

        // Filter tools based on the entered search text
        filteredTools = tools
            .Where(tool => tool.Name.ToLower().Contains(searchText))
            .ToList();

        // Update the ListBox with the filtered tools for search
        UpdateListBox(filteredTools);
    }








    private Dictionary<string, string> namePathDictionary = new Dictionary<string, string>();

    private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://www.davidinfosec.com",
                UseShellExecute = true
            });
        }
        catch (Win32Exception ex)
        {
            MessageBox.Show("Error opening blog link: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }
    }



    private void label2_Click(object sender, EventArgs e)
    {
    }

    private void label3_Click(object sender, EventArgs e)
    {
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        ComponentResourceManager resources = new ComponentResourceManager(typeof(Form1));
        button1 = new Button();
        button3 = new Button();
        listBox1 = new ListBox();
        button5 = new Button();
        label1 = new Label();
        button6 = new Button();
        linkLabel1 = new LinkLabel();
        linkLabel2 = new LinkLabel();
        label2 = new Label();
        label3 = new Label();
        label4 = new Label();
        label5 = new Label();
        textBox1 = new TextBox();
        SuspendLayout();
        // 
        // button1
        // 
        button1.Location = new Point(12, 241);
        button1.Name = "button1";
        button1.Size = new Size(215, 28);
        button1.TabIndex = 0;
        button1.Text = "Launch Tool";
        button1.UseVisualStyleBackColor = true;
        button1.Click += button1_Click;
        // 
        // button3
        // 
        button3.Location = new Point(123, 195);
        button3.Name = "button3";
        button3.Size = new Size(49, 24);
        button3.TabIndex = 2;
        button3.Text = "+";
        button3.UseVisualStyleBackColor = true;
        button3.Click += button3_Click;
        // 
        // listBox1
        // 
        listBox1.FormattingEnabled = true;
        listBox1.ItemHeight = 15;
        listBox1.Location = new Point(12, 80);
        listBox1.Name = "listBox1";
        listBox1.Size = new Size(215, 109);
        listBox1.TabIndex = 4;
        listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
        // 
        // button5
        // 
        button5.Location = new Point(12, 195);
        button5.Name = "button5";
        button5.Size = new Size(79, 23);
        button5.TabIndex = 6;
        button5.Text = "Settings";
        button5.UseVisualStyleBackColor = true;
        button5.Click += button5_Click;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label1.Location = new Point(12, 62);
        label1.Name = "label1";
        label1.Size = new Size(51, 15);
        label1.TabIndex = 7;
        label1.Text = "Toolbox";
        // 
        // button6
        // 
        button6.Location = new Point(178, 195);
        button6.Name = "button6";
        button6.Size = new Size(49, 24);
        button6.TabIndex = 8;
        button6.Text = "-";
        button6.UseVisualStyleBackColor = true;
        button6.Click += button6_Click;
        // 
        // linkLabel1
        // 
        linkLabel1.AutoSize = true;
        linkLabel1.Location = new Point(141, 283);
        linkLabel1.Name = "linkLabel1";
        linkLabel1.Size = new Size(31, 15);
        linkLabel1.TabIndex = 9;
        linkLabel1.TabStop = true;
        linkLabel1.Text = "Blog";
        linkLabel1.LinkClicked += linkLabel1_LinkClicked;
        // 
        // linkLabel2
        // 
        linkLabel2.AutoSize = true;
        linkLabel2.Location = new Point(182, 283);
        linkLabel2.Name = "linkLabel2";
        linkLabel2.Size = new Size(45, 15);
        linkLabel2.TabIndex = 10;
        linkLabel2.TabStop = true;
        linkLabel2.Text = "Donate";
        linkLabel2.LinkClicked += linkLabel2_LinkClicked;
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Font = new Font("Microsoft Sans Serif", 15F, FontStyle.Bold);
        label2.Location = new Point(48, 9);
        label2.Name = "label2";
        label2.Size = new Size(142, 25);
        label2.TabIndex = 14;
        label2.Text = "Task Runway";
        label2.TextAlign = ContentAlignment.MiddleCenter;
        label2.Click += label2_Click_1;
        // 
        // label3
        // 
        label3.AutoSize = true;
        label3.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label3.Location = new Point(12, 283);
        label3.Name = "label3";
        label3.Size = new Size(96, 15);
        label3.TabIndex = 15;
        label3.Text = "by DavidInfosec";
        label3.TextAlign = ContentAlignment.MiddleCenter;
        label3.Click += label3_Click_1;
        // 
        // label4
        // 
        label4.Font = new Font("Microsoft JhengHei", 7F, FontStyle.Bold);
        label4.Location = new Point(12, 34);
        label4.Margin = new Padding(10, 0, 3, 0);
        label4.Name = "label4";
        label4.Size = new Size(215, 18);
        label4.TabIndex = 16;
        label4.Text = "Take flight into your favorite tools.";
        label4.TextAlign = ContentAlignment.MiddleCenter;
        label4.Click += label4_Click_1;
        // 
        // label5
        // 
        label5.AutoSize = true;
        label5.Font = new Font("Segoe UI", 14F);
        label5.Location = new Point(96, 194);
        label5.Name = "label5";
        label5.Size = new Size(0, 25);
        label5.TabIndex = 17;
        // 
        // textBox1
        // 
        textBox1.Font = new Font("Segoe UI", 6F);
        textBox1.Location = new Point(110, 59);
        textBox1.Name = "textBox1";
        textBox1.PlaceholderText = "Search your toolbox";
        textBox1.Size = new Size(117, 18);
        textBox1.TabIndex = 18;
        textBox1.TextAlign = HorizontalAlignment.Center;
        textBox1.TextChanged += textBox1_TextChanged_1;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoSize = true;
        ClientSize = new Size(239, 311);
        Controls.Add(textBox1);
        Controls.Add(label5);
        Controls.Add(label4);
        Controls.Add(label3);
        Controls.Add(label2);
        Controls.Add(linkLabel2);
        Controls.Add(linkLabel1);
        Controls.Add(button6);
        Controls.Add(label1);
        Controls.Add(button5);
        Controls.Add(listBox1);
        Controls.Add(button3);
        Controls.Add(button1);
        Icon = (Icon)resources.GetObject("$this.Icon");
        Name = "Form1";
        Text = "Task Runway";
        Load += Form1_Load;
        ResumeLayout(false);
        PerformLayout();
    }

    private void CenterLabels()
    {
        label2.Location = new Point((base.ClientSize.Width - label2.Width) / 2, (base.ClientSize.Height - label2.Height) / 2);
        label4.Location = new Point((base.ClientSize.Width - label4.Width) / 2, label4.Top);
    }

    private void button6_Click(object sender, EventArgs e)
    {
        int selectedIndex = listBox1.SelectedIndex;
        if (selectedIndex >= 0 && selectedIndex < tools.Count)
        {
            ScriptingTool deletedTool = tools[selectedIndex];
            tools.RemoveAt(selectedIndex);
            deletedTools.Add(deletedTool);
            SaveToolsToConfig();
            UpdateListBox();
        }
    }

    private void button5_Click(object sender, EventArgs e)
    {
        ContextMenuStrip contextMenu = new ContextMenuStrip();
        ToolStripMenuItem refreshItem = new ToolStripMenuItem("Refresh");
        refreshItem.Click += delegate
        {
            RefreshTools();
        };
        contextMenu.Items.Add(refreshItem);

        ToolStripMenuItem undoItem = new ToolStripMenuItem("Undo");
        undoItem.Click += delegate
        {
            UndoChanges();
        };
        contextMenu.Items.Add(undoItem);

        ToolStripMenuItem searchbarItem = new ToolStripMenuItem("Toggle Search Bar");
        searchbarItem.CheckOnClick = true;
        searchbarItem.Checked = isSearchbarVisible;
        searchbarItem.Click += delegate
        {
            ToggleSearchbar(searchbarItem);
        };
        contextMenu.Items.Add(searchbarItem);

        ToolStripMenuItem alwaysOnTopItem = new ToolStripMenuItem("Always On Top");
        alwaysOnTopItem.CheckOnClick = true;
        alwaysOnTopItem.Checked = base.TopMost;
        alwaysOnTopItem.Click += delegate
        {
            ToggleAlwaysOnTop(alwaysOnTopItem);
        };
        contextMenu.Items.Add(alwaysOnTopItem);

        ToolStripMenuItem customFlagsItem = new ToolStripMenuItem("Custom Flags");
        customFlagsItem.Click += delegate
        {
            CustomFlags();
        };
        contextMenu.Items.Add(customFlagsItem);

        ToolStripMenuItem downloadExplorerItem = new ToolStripMenuItem("Download Explorer");
        downloadExplorerItem.Click += delegate
        {
            ExploreNewTools();
        };
        contextMenu.Items.Add(downloadExplorerItem);

        ToolStripMenuItem checkForUpdatesItem = new ToolStripMenuItem("Check for Updates");
        checkForUpdatesItem.Click += async delegate
        {
            await CheckForUpdates(isManualCheck: true); // Manual check
        };



        contextMenu.Items.Add(checkForUpdatesItem);

        contextMenu.Items.Add(new ToolStripSeparator());

        ToolStripMenuItem openMenu = new ToolStripMenuItem("Open");
        ToolStripMenuItem openConfigFileItem = new ToolStripMenuItem("Open Config");
        openConfigFileItem.Click += delegate
        {
            OpenConfigFile();
        };
        openMenu.DropDownItems.Add(openConfigFileItem);

        ToolStripMenuItem openPathItem = new ToolStripMenuItem("Open Path");
        openPathItem.Click += delegate
        {
            OpenPathForSelectedTool();
        };
        openMenu.DropDownItems.Add(openPathItem);
        contextMenu.Items.Add(openMenu);

        ToolStripMenuItem saveMenu = new ToolStripMenuItem("Save");

        ToolStripMenuItem saveConfigItem = new ToolStripMenuItem("Save Config");
        saveConfigItem.Click += delegate
        {
            SaveConfig();
        };
        saveMenu.DropDownItems.Add(saveConfigItem);

        contextMenu.Items.Add(saveMenu);

        ToolStripMenuItem copyMenu = new ToolStripMenuItem("Copy");

        ToolStripMenuItem copyConfigItem = new ToolStripMenuItem("Copy Config");
        copyConfigItem.Click += delegate
        {
            CopyConfig();
        };
        copyMenu.DropDownItems.Add(copyConfigItem);

        ToolStripMenuItem copyPathItem = new ToolStripMenuItem("Copy Path");
        copyPathItem.Click += delegate
        {
            CopyPath();
        };
        copyMenu.DropDownItems.Add(copyPathItem);

        ToolStripMenuItem copyFlagsItem = new ToolStripMenuItem("Copy Flags");
        copyFlagsItem.Click += delegate
        {
            CopyFlags();
        };
        copyMenu.DropDownItems.Add(copyFlagsItem);

        contextMenu.Items.Add(copyMenu);

        ToolStripMenuItem sortMenu = new ToolStripMenuItem("Sort");

        ToolStripMenuItem sortAscendingItem = new ToolStripMenuItem("Sort Ascending");
        sortAscendingItem.Click += delegate
        {
            SortToolsAscending();
        };
        sortMenu.DropDownItems.Add(sortAscendingItem);

        ToolStripMenuItem sortDescendingItem = new ToolStripMenuItem("Sort Descending");
        sortDescendingItem.Click += delegate
        {
            SortToolsDescending();
        };
        sortMenu.DropDownItems.Add(sortDescendingItem);

        contextMenu.Items.Add(sortMenu);

        contextMenu.Show(button5, new Point(0, button5.Height));
    }






    private void ToggleAlwaysOnTop(ToolStripMenuItem item)
    {
        base.TopMost = !base.TopMost;
        item.Checked = base.TopMost;
    }

    private void ToggleSearchbar(ToolStripMenuItem searchbarItem)
    {
        // Toggle the searchbar visibility
        isSearchbarVisible = !isSearchbarVisible;

        // Example: Assuming you have a 'textBox1' TextBox control
        textBox1.Visible = isSearchbarVisible;

        // Update the Checked state of the menu item
        searchbarItem.Checked = isSearchbarVisible;

        // Reset the search filter when search bar is toggled off
        if (!isSearchbarVisible)
        {
            // Clear the search text
            textBox1.Text = string.Empty;
        }
    }

            private void CustomFlags()
    {
        int selectedIndex = listBox1.SelectedIndex;
        if (selectedIndex >= 0 && selectedIndex < tools.Count)
        {
            ScriptingTool selectedTool = tools[selectedIndex];
            if (selectedTool.Path.EndsWith(".bat", StringComparison.OrdinalIgnoreCase) || selectedTool.Path.EndsWith(".py", StringComparison.OrdinalIgnoreCase) || selectedTool.Path.EndsWith(".ps1", StringComparison.OrdinalIgnoreCase))
            {
                string currentFlags = selectedTool.CustomFlags;
                string newFlags = Interaction.InputBox("Enter Custom Flags:", "Edit Custom Flags", currentFlags);
                selectedTool.CustomFlags = newFlags;
                SaveToolsToConfig();
            }
            else
            {
                MessageBox.Show("This action is only compatible with a script.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }
        else
        {
            MessageBox.Show("Please select a valid tool.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }
    }

    private void RefreshTools()
    {
        LoadToolsFromConfig();
        UpdateListBox();
    }

    private void UndoChanges()
    {
        if (deletedTools.Count > 0)
        {
            ScriptingTool restoredTool = deletedTools[deletedTools.Count - 1];
            deletedTools.RemoveAt(deletedTools.Count - 1);
            tools.Add(restoredTool);
        }
        else if (renamedToolsHistory.Count > 0)
        {
            Tuple<int, string> lastRename = renamedToolsHistory.Pop();
            int index = lastRename.Item1;
            string oldName = lastRename.Item2;
            tools[index].Name = oldName;
        }
        else if (pathChangesHistory.Count > 0)
        {
            // Undo path change
            Tuple<int, string, string> lastPathChange = pathChangesHistory.Pop();
            int selectedIndexPath = lastPathChange.Item1;
            string originalPath = lastPathChange.Item2;

            // Revert the path change
            tools[selectedIndexPath].Path = originalPath;
        }
        else if (moveHistory.Count > 0)
        {
            // Undo Move
            Tuple<int, int> lastMove = moveHistory.Pop();
            int selectedIndex = lastMove.Item1;
            int newIndex = lastMove.Item2;

            // Swap the items back to undo the move
            SwapItemsInList(tools, selectedIndex, newIndex);
            SwapItemsInList(originalOrderTools, selectedIndex, newIndex);
            SwapItemsInList(filteredTools, selectedIndex, newIndex);

            // Update the display in the listBox
            UpdateListBox();

            // Update the selected index after the undo
            listBox1.SelectedIndex = selectedIndex;

            // Save the tools to the configuration
            SaveToolsToConfig();
        }
        else
        {
            MessageBox.Show("No changes to undo.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            return;  // No changes to apply, return early
        }

        // Save the changes to config and update the display
        SaveToolsToConfig();
        UpdateListBox();
    }







    private void SaveConfig()
    {
        SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            Title = "Save Config File",
            Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
            DefaultExt = "txt",
            FileName = "config.txt"
        };
        DialogResult result = saveFileDialog.ShowDialog();
        if (result == DialogResult.OK)
        {
            string selectedFilePath = saveFileDialog.FileName;
            SaveToolsToConfig(selectedFilePath);
            MessageBox.Show("Config saved to " + selectedFilePath, "Info", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }
    }
    private void SaveToolsToConfig(string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (ScriptingTool tool in tools)
            {
                writer.WriteLine($"[{tool.Name}]");
                writer.WriteLine($"path={tool.Path}");
                writer.WriteLine($"custom_flags={tool.CustomFlags}");
                writer.WriteLine();
            }
        }
    }


    private void SaveFlags()
    {
        int selectedIndex = listBox1.SelectedIndex;
        if (selectedIndex >= 0 && selectedIndex < tools.Count)
        {
            ScriptingTool selectedTool = tools[selectedIndex];
            if (!selectedTool.Path.EndsWith(".bat", StringComparison.OrdinalIgnoreCase) && !selectedTool.Path.EndsWith(".py", StringComparison.OrdinalIgnoreCase) && !selectedTool.Path.EndsWith(".ps1", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("This action is only compatible with a script.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }
    }

    private void CopyConfig()
    {
        if (File.Exists(configPath))
        {
            string configContent = File.ReadAllText(configPath);
            Clipboard.SetText(configContent);
        }
    }

    private void CopyPath()
    {
        int selectedIndex = listBox1.SelectedIndex;
        if (selectedIndex >= 0 && selectedIndex < tools.Count)
        {
            ScriptingTool selectedTool = tools[selectedIndex];
            if (selectedTool.Path.EndsWith(".bat", StringComparison.OrdinalIgnoreCase) || selectedTool.Path.EndsWith(".py", StringComparison.OrdinalIgnoreCase) || selectedTool.Path.EndsWith(".ps1", StringComparison.OrdinalIgnoreCase))
            {
                Clipboard.SetText(selectedTool.Path);
            }
            else
            {
                MessageBox.Show("This action is only compatible with a script.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }
    }

    private void CopyFlags()
    {
        int selectedIndex = listBox1.SelectedIndex;
        if (selectedIndex >= 0 && selectedIndex < tools.Count)
        {
            ScriptingTool selectedTool = tools[selectedIndex];
            if (!selectedTool.Path.EndsWith(".bat", StringComparison.OrdinalIgnoreCase) && !selectedTool.Path.EndsWith(".py", StringComparison.OrdinalIgnoreCase) && !selectedTool.Path.EndsWith(".ps1", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("This action is only compatible with a script.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }
    }

    private void OpenConfigFile()
    {
        Process.Start("notepad.exe", configPath);
    }
    private void OpenPathForSelectedTool()
    {
        int selectedIndex = listBox1.SelectedIndex;
        if (selectedIndex >= 0 && selectedIndex < tools.Count)
        {
            ScriptingTool selectedTool = tools[selectedIndex];
            string folderPath = Path.GetDirectoryName(selectedTool.Path);

            if (!string.IsNullOrEmpty(folderPath))
            {
                string extension = Path.GetExtension(selectedTool.Path).ToLower();

                // Allowed extensions
                List<string> allowedExtensions = new List<string> { ".bat", ".exe", ".ps1", ".py" };

                if (allowedExtensions.Contains(extension))
                {
                    Process.Start("explorer.exe", folderPath);
                }
                else
                {
                    MessageBox.Show("This action is only compatible with .bat, .exe, .ps1, and .py scripts.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
            else
            {
                MessageBox.Show("Unable to determine the folder path for the selected tool.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }
    }



    private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://www.poof.io/tip/@davidinfosec",
                UseShellExecute = true
            });
        }
        catch (Win32Exception ex)
        {
            MessageBox.Show("Error opening donate link: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }
    }

    private void label4_Click(object sender, EventArgs e)
    {
    }

    private void button2_Click_1(object sender, EventArgs e)
    {
    }

    private void label2_Click_1(object sender, EventArgs e)
    {
    }

    private void label3_Click_1(object sender, EventArgs e)
    {
    }

    private void label4_Click_1(object sender, EventArgs e)
    {
    }

    private Label label5;
    private TextBox textBox1;

    private void textBox1_TextChanged_1(object sender, EventArgs e)
    {

    }
}
