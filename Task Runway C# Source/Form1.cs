// Task_Runway_x64, Version=1.0.4.4, Culture=neutral, PublicKeyToken=null
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
using System.Text.Json;
using static Form1;
using Microsoft.VisualBasic.Devices;


public class Form1 : Form
{

    private Update updateForm;
    private bool isTextBox1Active;
    private bool isSearchActive;
    private Preferences preferences;

    private string preferencesFilePath = "preferences.json";
    private ToolStripMenuItem searchbarItem;
    private ToolStripMenuItem alwaysOnTopItem;


    private ContextMenuStrip notifyIconContextMenu = new ContextMenuStrip();

    private URL urlForm;
    private EnterToolName toolNameForm;

    private ToolStripMenuItem minimizeToTrayItem;
    private bool isCtrlPressed = false; // Track if Ctrl key is pressed

    private bool minimizeToTrayOnClose = false;

    List<(ScriptingTool tool, int index)> deletedTools = new List<(ScriptingTool tool, int index)>();

    private Version currentVersion;

    private string downloadLink;

    private bool isManualCheck = false;

    public int CountdownValue { get; set; }

    private int countdownTime = 25 * 60; // 25 minutes in seconds
    private bool isTimerRunning = false;

    public class Preferences
    {
        public bool IsSearchbarVisible { get; set; }
        public bool AlwaysOnTop { get; set; }
        public bool MinimizeToTrayOnClose { get; set; }
        public bool? DarkMode { get; set; }
        public bool SupporterLicensePurchased { get; set; } // Tracks if any supporter license has been purchased
        public bool? PomodoroTimerEnabled { get; set; }
  


        // Load preferences from a JSON file
        public static Preferences LoadPreferences(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    return JsonSerializer.Deserialize<Preferences>(json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading preferences: {ex.Message}");
            }

            // Return default preferences if loading fails or the file doesn't exist
            return new Preferences();

        }

        // Save preferences to a JSON file
        public void SavePreferences(string filePath)
        {
            try
            {
                string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving preferences: {ex.Message}");
            }



        }


    }



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

    private Stack<Tuple<int, string>> renamedToolsHistory = new Stack<Tuple<int, string>>();

    private Label label2;

    private Label label4;
    private quickActions quickActionsForm;
    private ToolStripMenuItem quickActionsMenuItem;

    TaskRunway.Timer timerForm = new TaskRunway.Timer();
    private void ExploreNewTools()
    {
        form2 = new TaskRunwayExplorer(this);

        // Handle the Shown event to set the TopMost property of form2
        form2.Shown += (sender, e) =>
        {
            form2.RefreshCheckedListBox3();
            form2.TopMost = this.TopMost; // Set the TopMost property of form2 to match the main form
        };

        form2.Show();
    }


    public Form1()
    {

        InitializeComponent();
        InitializeQuickLaunchMenu();

        
        this.MouseDown += new MouseEventHandler(Form1_MouseDown);


        // Load preferences
        preferences = Preferences.LoadPreferences(preferencesFilePath);
        if (preferences == null)
        {
            preferences = new Preferences(); // Default preferences if loading failed
        }


        this.FormClosing += Form1_FormClosing;

        this.Resize += new System.EventHandler(this.Form1_Resize);
        this.KeyPreview = true;
        this.KeyDown += new KeyEventHandler(Form_KeyDown);
        label1.TabStop = true;

        currentVersion = new Version("1.0.4.4");
        InitializeContextMenu();
        textBox1.TextChanged += textBox1_TextChanged;
        textBox1.KeyDown += textBox1_KeyDown;
        LoadToolsFromConfig();
        base.MaximizeBox = false;
        base.MinimizeBox = true;
        base.FormBorderStyle = FormBorderStyle.FixedSingle;
        listBox1.KeyDown += listBox1_KeyDown;
        listBox1.MouseWheel += listBox1_MouseWheel;
        listBox1.KeyPress += listBox1_KeyPress;
        listBox1.SelectionMode = SelectionMode.MultiExtended;

        preferences.PomodoroTimerEnabled = preferences.PomodoroTimerEnabled ?? false; // Default to false if not set


        // Initialize Preferences
        preferences = Preferences.LoadPreferences(preferencesFilePath);
        if (preferences == null)
        {
            preferences = new Preferences(); // Default preferences if loading failed
        }

        // Apply the loaded preferences
        this.TopMost = preferences.AlwaysOnTop;
        minimizeToTrayOnClose = preferences.MinimizeToTrayOnClose;

        // Apply the loaded visibility preference to the search bar
        textBox1.Visible = preferences.IsSearchbarVisible;

        InitializeQuickActionsForm();
        InitializeQuickLaunchMenu();


    }


    private void InitializeQuickActionsForm()
    {
        quickActionsForm = new quickActions();
    }

    private void InitializeQuickLaunchMenu()
    {
        // Create a context menu for notifyIcon
        ContextMenuStrip notifyIconContextMenu = new ContextMenuStrip();

        // Load quick actions from file and add to the menu
        LoadQuickActionsIntoMenu();

        // Add 'Open Main Window' menu item
        ToolStripMenuItem openMainWindowItem = new ToolStripMenuItem("Open main window");
        openMainWindowItem.Click += (sender, e) => ShowMainWindow();
        notifyIconContextMenu.Items.Add(openMainWindowItem);

        // Add 'Exit' menu item
        ToolStripMenuItem exitMenuItem = new ToolStripMenuItem("Exit");
        exitMenuItem.Click += (sender, e) => Application.Exit();
        notifyIconContextMenu.Items.Add(exitMenuItem);

        /*
        // Create and add "Configure Quick Actions" menu item with a separator
        quickActionsMenuItem = new ToolStripMenuItem("Quick actions");
        AddConfigureQuickActionsMenuItem(notifyIconContextMenu);
        */
        // Assign the context menu to notifyIcon
        notifyIcon.ContextMenuStrip = notifyIconContextMenu;

        // Attach a single-click event handler to notifyIcon
        notifyIcon.MouseClick += NotifyIcon_MouseClick;
    }

    /*
    private void AddConfigureQuickActionsMenuItem(ContextMenuStrip menu)
    {
        // Separator above "Configure Quick Actions"
        ToolStripSeparator separator = new ToolStripSeparator();
        menu.Items.Add(separator);

        // "Configure Quick Actions" as a new ToolStripMenuItem
        ToolStripMenuItem configureQuickActionsItem = new ToolStripMenuItem("Configure quick actions");
        configureQuickActionsItem.Click += (sender, e) => OpenQuickActionsForm();

        // Add "Configure Quick Actions" as an item in the context menu
        menu.Items.Add(configureQuickActionsItem);
    }

    */




    private void LoadQuickActionsIntoMenu()
    {
        // Check if quickActionsMenuItem is null
        if (quickActionsMenuItem == null)
        {
            return;
        }

        // Clear existing items except 'Configure Quick Actions' and separator
        while (quickActionsMenuItem.DropDownItems.Count > 2)
        {
            quickActionsMenuItem.DropDownItems.RemoveAt(0);
        }

        if (!File.Exists("quickactions_config.txt"))
        {
            return;
        }

        var quickActions = File.ReadAllLines("quickactions_config.txt");
        foreach (var action in quickActions)
        {
            var item = new ToolStripMenuItem(action);
            item.Click += QuickActionItem_Click;
            quickActionsMenuItem.DropDownItems.Insert(0, item); // Insert at the top
        }
    }



    private void QuickActionItem_Click(object sender, EventArgs e)
    {
        ToolStripMenuItem clickedItem = sender as ToolStripMenuItem;
        if (clickedItem != null)
        {
            // Handle the click event for each quick action
            MessageBox.Show($"Action: {clickedItem.Text}");
        }
    }



    private void OpenQuickActionsForm()
    {
        quickActions quickActionsForm = new quickActions
        {
            Owner = this // Set Form1 as the owner of quickActionsForm
        };

        if (quickActionsForm.ShowDialog() == DialogResult.OK)
        {
            LoadQuickActionsIntoMenu(); // Reload the quick actions menu
        }
    }

    private void UpdateContextMenu(IEnumerable<string> selectedActions)
    {
        quickActionsMenuItem.DropDownItems.Clear();
        foreach (var action in selectedActions)
        {
            if (!action.Equals("Configure Quick Actions"))
            {
                ToolStripMenuItem item = new ToolStripMenuItem(action);
                item.Click += QuickActionItem_Click;
                quickActionsMenuItem.DropDownItems.Add(item);
            }
        }
    }






    private void ExecuteQuickAction(string actionName)
    {
        // Implement the logic to execute the quick action based on the actionName
        MessageBox.Show($"Executing Quick Action: {actionName}"); // Placeholder logic
    }



    private void ShowMainWindow()
    {
        this.ShowInTaskbar = true;
        this.Show();
        this.WindowState = FormWindowState.Normal;
        notifyIcon.Visible = false;
    }

    private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            ShowMainWindow();
        }
    }

   

    private void Form1_Resize(object sender, EventArgs e)
    {
        if (this.WindowState == FormWindowState.Minimized)
        {
            this.Hide();
            notifyIcon.Visible = true;
        }
    }

    private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
    {
        // Check if the click is a left mouse click
        if (e.Button == MouseButtons.Left)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }
    }



    private void Form_KeyDown(object sender, KeyEventArgs e)
    {
        // Check if Ctrl is held down and the pressed key is K
        if (e.Control && e.KeyCode == Keys.K)
        {
            textBox1.Focus(); // Set focus to textBox1
        }
        // Check if the pressed key is Escape
        else if (e.KeyCode == Keys.Escape)
        {
            textBox1.Text = string.Empty; // Clear the text in textBox1

            // Optionally, if you want to reset focus to another control like listBox1
            listBox1.Focus();
        }
    }


















        private async Task<(string status, string downloadLink, string version)> GetUpdateStatus()
    {
        // Replace this URL with the actual URL of your version.txt file on the server
        var versionFileUrl = "https://raw.githubusercontent.com/davidinfosec/task-runway/main/version.txt";
        var tempVersionFile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "versioncheck.txt");

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
        // Check if updateForm is null or disposed
        if (updateForm == null || updateForm.IsDisposed)
        {
            updateForm = new Update(); // Create a new instance of the Update form
            updateForm.TopMost = this.TopMost;

            // Assigning delegate methods
            updateForm.OnViewChanges = () => ViewChangelog();
            updateForm.OnUpdateNow = () => UpdateNow();
        }

        // Show updateForm modally
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

    private async void Form1_Load(object sender, EventArgs e)
    {
        LoadAsync(); // Call the asynchronous method
        preferences = Preferences.LoadPreferences(preferencesFilePath);

        // Check and apply Dark Mode based on the loaded preferences
        if (preferences.DarkMode ?? false)
        {
            ApplyDarkMode();
        }
        else
        {
            ApplyLightMode();
        }

        // Wait for the form to fully load
        await Task.Delay(2);
        this.ActiveControl = null;
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
        int index = tools.IndexOf(tool);
        if (index != -1)
        {
            // Record the removal in the undo stack before actually removing the tool
            undoStack.Push(new UndoOperation
            {
                Type = OperationType.Delete,
                Tool = tool,
                OriginalIndex = index // Storing the index where the tool was before removal
            });

            tools.Remove(tool);

            // Update UI and save configuration
            UpdateListBox();
            SaveToolsToConfig();
            RefreshTools();
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
            SwapItemsInList(tools, selectedIndex, newIndex);
            SwapItemsInList(originalOrderTools, selectedIndex, newIndex);
            SwapItemsInList(filteredTools, selectedIndex, newIndex);

            // Record the move in the undo stack
            undoStack.Push(new UndoOperation
            {
                Type = OperationType.Move,
                OriginalIndex = selectedIndex,
                NewIndex = newIndex
            });

            UpdateListBox();
            listBox1.SelectedIndex = newIndex;
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

    private void Form1_MouseDown(object sender, MouseEventArgs e)
    {
        // Deselect or 'blur' the currently focused control
        this.ActiveControl = null;
    }

    private void listBox1_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            int index = listBox1.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                listBox1.ClearSelected(); // Clear existing selection
                listBox1.SelectedIndex = index; // Select the item that was right-clicked
            }
            else
            {
                // Prevent context menu from showing if no item is clicked
                listBox1.ContextMenuStrip = null;
            }
        }
    }





    private void InitializeContextMenu()
    {
        listBoxContextMenu = new ContextMenuStrip();


        // Edit Path
        ToolStripMenuItem editPathItem = new ToolStripMenuItem("Edit Path");
        editPathItem.Click += (sender, e) =>
        {
            EditPath();
        };
        listBoxContextMenu.Items.Add(editPathItem);



        // ... (existing code)

        listBox1.ContextMenuStrip = listBoxContextMenu;
        listBox1.MouseDown += listBox1_MouseDown;


        // Rename
        ToolStripMenuItem renameItem = new ToolStripMenuItem("Rename");
        renameItem.Click += delegate
        {
            RenameTool();
        };
        listBoxContextMenu.Items.Add(renameItem);






        // Custom Flags
        ToolStripMenuItem customFlagsItem = new ToolStripMenuItem("Add Custom Flags");
        customFlagsItem.Click += delegate
        {
            CustomFlags();
        };
        listBoxContextMenu.Items.Add(customFlagsItem);


        // Create the 'Delete' context menu item
        ToolStripMenuItem deleteMenuItem = new ToolStripMenuItem("Delete");
        deleteMenuItem.Click += delegate
        {
            // Get the selected index from the ListBox
            int selectedIndex = listBox1.SelectedIndex;

            // Call the DeleteItem function if a valid item is selected
            if (selectedIndex >= 0)
            {
                DeleteItem(selectedIndex);
            }
            else
            {
                MessageBox.Show("No item selected to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        };

        // Add the 'Delete' item to the context menu
        listBoxContextMenu.Items.Add(deleteMenuItem);



        listBoxContextMenu.Items.Add(new ToolStripSeparator());



        // Check if a search is active
        bool searchIsActive = IsSearchActive();

        // Move Item Up
        ToolStripMenuItem moveItemUp = new ToolStripMenuItem("Move Item Up");
        moveItemUp.Enabled = !searchIsActive; // Disable if search is active
        moveItemUp.Click += delegate
        {
            MoveItemUp();
        };
        listBoxContextMenu.Items.Add(moveItemUp);

        // Move Item Down
        ToolStripMenuItem moveItemDown = new ToolStripMenuItem("Move Item Down");
        moveItemDown.Enabled = !searchIsActive; // Disable if search is active
        moveItemDown.Click += delegate
        {
            MoveItemDown();
        };
        listBoxContextMenu.Items.Add(moveItemDown);

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

        // Dynamically update context menu items before showing
        listBoxContextMenu.Opening += (sender, e) =>
        {
            bool searchIsActive = IsSearchActive();
            int indexUnderMouse = listBox1.IndexFromPoint(listBox1.PointToClient(Cursor.Position));

            foreach (ToolStripItem genericItem in listBoxContextMenu.Items)
            {
                // Check if the item is a ToolStripMenuItem
                if (genericItem is ToolStripMenuItem item)
                {
                    if (item.Text == "Move Item Up" || item.Text == "Move Item Down")
                    {
                        item.Enabled = !searchIsActive && indexUnderMouse != ListBox.NoMatches;
                    }
                    else
                    {
                        item.Enabled = indexUnderMouse != ListBox.NoMatches;
                    }
                }
                // No need to do anything for ToolStripSeparator or other types
            }
        };
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
        int selectedIndex = listBox1.SelectedIndex;
        if (selectedIndex >= 0 && selectedIndex < filteredTools.Count)
        {
            ScriptingTool selectedTool = filteredTools[selectedIndex];
            string currentPath = selectedTool.Path;

            using (PathForm pathForm = new PathForm())
            {
                pathForm.Text = "Edit Path";
                pathForm.GetPathTextBox().Text = currentPath;

                pathForm.TopMost = true;

                if (pathForm.ShowDialog() == DialogResult.OK)
                {
                    string newPath = pathForm.GetPath();

                    if (!string.IsNullOrWhiteSpace(newPath) && newPath != currentPath)
                    {
                        // Record the path change in undoStack
                        undoStack.Push(new UndoOperation
                        {
                            Type = OperationType.PathChange,
                            Tool = selectedTool,
                            OldPath = currentPath,
                            OriginalIndex = selectedIndex // Optional, if you need to track the index
                        });

                        // Update the tool's path
                        selectedTool.Path = newPath;



                        // Save the changes to the configuration
                        SaveToolsToConfig();

                        // Reapply the search if there is text in the search TextBox, or update the ListBox
                        if (!string.IsNullOrWhiteSpace(textBox1.Text))
                        {
                            PerformSearch();
                        }
                        else
                        {
                            UpdateListBox(filteredTools);
                        }
                    }
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
        int selectedIndex = listBox1.SelectedIndex;

        // Check if any tool is selected
        if (selectedIndex >= 0 && selectedIndex < filteredTools.Count)
        {
            ScriptingTool selectedTool = filteredTools[selectedIndex];
            string currentName = selectedTool.Name;

            // Always create a new instance of the dialog
            EnterToolName toolNameForm = new EnterToolName(currentName);
            toolNameForm.TopMost = this.TopMost;

            if (toolNameForm.ShowDialog() == DialogResult.OK)
            {
                string newName = toolNameForm.GetToolName();

                if (!string.IsNullOrEmpty(newName) && newName != currentName)
                {
                    // Handle renaming of the selected tool
                    selectedTool.Name = newName;
                    UpdateListBox(filteredTools);
                    SaveToolsToConfig();
                }
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


    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (minimizeToTrayOnClose && e.CloseReason == CloseReason.UserClosing)
        {
            e.Cancel = true; // Prevent the form from closing
            this.WindowState = FormWindowState.Minimized;
            this.Hide();
            notifyIcon.Visible = true; // Show the notifyIcon in the system tray
        }
    }

    private void listBox1_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Control)
        {
            if (e.KeyCode == Keys.C)
            {
                // Handle Ctrl+C to copy the path of the selected tool, regardless of search state
                CopyPath();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.X && e.Shift)
            {
                // Handle Ctrl+Shift+X to copy the name of the selected tool, regardless of search state
                CopyName();
                e.Handled = true;
            }
            else if (!isSearchActive)
            {
                if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
                {
                    // Handle Ctrl+Up and Ctrl+Down to prevent the default behavior
                    e.Handled = true;
                    e.SuppressKeyPress = true;

                    // Determine the direction based on the key pressed
                    int direction = (e.KeyCode == Keys.Up) ? -1 : 1;

                    // Move the selected item based on the direction
                    MoveItem(listBox1.SelectedIndex, direction, true);
                }
            }
        }
        else if (e.KeyCode == Keys.Enter)
        {
            // If Enter key is pressed, launch the selected tools
            LaunchSelectedTool();
            e.Handled = true;
        }
    }



    private void CopyName()
    {
        int selectedIndex = listBox1.SelectedIndex;

        // Check if an item is selected
        if (selectedIndex >= 0)
        {
            ScriptingTool selectedTool;

            // Determine which list to use based on search state
            if (isSearchActive)
            {
                // Use the filtered list when a search is active
                if (selectedIndex < filteredTools.Count)
                {
                    selectedTool = filteredTools[selectedIndex];
                }
                else
                {
                    MessageBox.Show("Invalid selection.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                // Use the complete list when no search is active
                if (selectedIndex < tools.Count)
                {
                    selectedTool = tools[selectedIndex];
                }
                else
                {
                    MessageBox.Show("Invalid selection.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // Copy the name to the clipboard
            Clipboard.SetText(selectedTool.Name);
        }
        else
        {
            MessageBox.Show("No item selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }





    private void listBox1_KeyUp(object sender, KeyEventArgs e)
    {
        // Reset the Ctrl key state when it's released
        if (e.KeyCode == Keys.ControlKey)
        {
            isCtrlPressed = false;
        }
    }


    private int originalMoveIndex; // Add this variable at the class level


    private void listBox1_MouseWheel(object sender, MouseEventArgs e)
    {
        // Check if the Ctrl key is held down and not in search mode
        if (Control.ModifierKeys == Keys.Control && !isSearchActive)
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


    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == (Keys.Control | Keys.Z))
        {
            UndoChanges();
            return true;
        }

        return base.ProcessCmdKey(ref msg, keyData); // Return false for other keyData
    }








    private void LaunchSelectedTool()
    {
        foreach (int selectedIndex in listBox1.SelectedIndices)
        {
            if (selectedIndex < 0 || selectedIndex >= filteredTools.Count)
            {
                continue;
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
                        WorkingDirectory = System.IO.Path.GetDirectoryName(selectedTool.Path),
                        CreateNoWindow = false
                    });
                }
                else if (selectedTool.Path.EndsWith(".py", StringComparison.OrdinalIgnoreCase))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = "/k python \"" + selectedTool.Path + "\" " + selectedTool.CustomFlags,
                        WorkingDirectory = System.IO.Path.GetDirectoryName(selectedTool.Path),
                        CreateNoWindow = false
                    });
                }
                else if (selectedTool.Path.EndsWith(".ps1", StringComparison.OrdinalIgnoreCase))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        Arguments = "-File \"" + selectedTool.Path + "\" " + selectedTool.CustomFlags,
                        WorkingDirectory = System.IO.Path.GetDirectoryName(selectedTool.Path),
                        CreateNoWindow = false
                    });
                }
                else if (selectedTool.Path.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = selectedTool.Path,
                        WorkingDirectory = System.IO.Path.GetDirectoryName(selectedTool.Path),
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
                        WorkingDirectory = System.IO.Path.GetDirectoryName(selectedTool.Path),
                        CreateNoWindow = false
                    });
                }
                else if (selectedTool.Path.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = selectedTool.Path,
                        WorkingDirectory = System.IO.Path.GetDirectoryName(selectedTool.Path),
                        UseShellExecute = true  // Important for launching .lnk files
                    });
                }
                else if (Uri.TryCreate(selectedTool.Path, UriKind.Absolute, out uri))
                {
                    if (uri.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase) || uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
                    {
                        // Open web URLs using the default web browser
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = selectedTool.Path,
                            UseShellExecute = true
                        });
                    }
                    else if (uri.Scheme.Equals("file", StringComparison.OrdinalIgnoreCase))
                    {
                        // Handle file URLs by opening them in the default web browser
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = selectedTool.Path,
                            UseShellExecute = true
                        });
                    }
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
        ToolStripMenuItem addExecutableToolItem = new ToolStripMenuItem("Add Program (.exe, .jar, .lnk)");
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

    private string PromptForToolName(string defaultName)
    {
        // Always create a new instance of the dialog
        using (EnterToolName toolNameForm = new EnterToolName(defaultName))
        {
            toolNameForm.TopMost = this.TopMost;

            if (toolNameForm.ShowDialog() == DialogResult.OK)
            {
                return toolNameForm.GetToolName();
            }
        }

        // Return null or an empty string if the user canceled the dialog
        return null;
    }

    private void AddTool(string toolType)
    {
        switch (toolType)
        {
            case "Website":
                {
                    // Create a new instance of the URL form to reset it
                    urlForm = new URL();
                    urlForm.TopMost = this.TopMost;

                    if (urlForm.ShowDialog() == DialogResult.OK)
                    {
                        string enteredURL = urlForm.GetURL();

                        if (string.IsNullOrWhiteSpace(enteredURL))
                        {
                            // Handle the case where the URL is empty
                            MessageBox.Show("URL cannot be empty. Please enter a valid URL.");
                            return;
                        }

                        if (!(enteredURL.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                            enteredURL.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
                            enteredURL.StartsWith("file:///", StringComparison.OrdinalIgnoreCase)))
                        {
                            enteredURL = "http://" + enteredURL;
                        }

                        // Prompt the user for a tool name
                        string toolName = PromptForToolName(enteredURL);

                        if (!string.IsNullOrEmpty(toolName))
                        {
                            ScriptingTool newTool = new ScriptingTool(toolName, enteredURL, "");
                            AddToolToList(newTool);
                            SaveToolsToConfig();
                            RefreshTools();
                        }
                    }
                    break;
                }






            case "Script":
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog
                    {
                        Title = "Select Script File",
                        Filter = "Script files (*.bat;*.ps1;*.py)|*.bat;*.ps1;*.py",
                        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal)
                    };
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string defaultName = System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                        using (EnterToolName toolNameForm = new EnterToolName(defaultName))
                        {
                            if (toolNameForm.ShowDialog() == DialogResult.OK)
                            {
                                string toolName = toolNameForm.GetToolName();
                                AddToolToList(new ScriptingTool(toolName, openFileDialog.FileName, ""));
                                SaveToolsToConfig();
                                RefreshTools();
                            }
                        }
                    }
                    break;
                }

            case "Executable":
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog
                    {
                        Title = "Select Executable File",
                        Filter = "Executable files (*.exe;*.jar;*.lnk)|*.exe;*.jar;*.lnk",
                        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
                    };

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string defaultName = System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                        using (EnterToolName toolNameForm = new EnterToolName(defaultName))
                        {
                            if (toolNameForm.ShowDialog() == DialogResult.OK)
                            {
                                string toolName = toolNameForm.GetToolName();
                                AddToolToList(new ScriptingTool(toolName, openFileDialog.FileName, ""));
                                SaveToolsToConfig();
                                RefreshTools();
                            }
                        }
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
                        string defaultName = System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                        using (EnterToolName toolNameForm = new EnterToolName(defaultName))
                        {
                            if (toolNameForm.ShowDialog() == DialogResult.OK)
                            {
                                string toolName = toolNameForm.GetToolName();
                                AddToolToList(new ScriptingTool(toolName, openFileDialog.FileName, ""));
                                SaveToolsToConfig();
                                RefreshTools();
                            }
                        }
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
                        string defaultName = new DirectoryInfo(folderPath).Name;

                        using (EnterToolName toolNameForm = new EnterToolName(defaultName))
                        {
                            if (toolNameForm.ShowDialog() == DialogResult.OK)
                            {
                                string toolNameForFolder = toolNameForm.GetToolName();
                                AddToolToList(new ScriptingTool(toolNameForFolder, folderPath, ""));
                                SaveToolsToConfig();
                                RefreshTools();
                            }
                        }
                    }
                    break;
                }

        }
    }




    public void AddToolToList(ScriptingTool tool)
    {
        tools.Add(tool);
        originalOrderTools.Add(tool); // Add the new tool to originalOrderTools as well

        // Record this addition in the undo stack, along with the index at which it was added
        undoStack.Push(new UndoOperation
        {
            Type = OperationType.Add,
            Tool = tool,
            OriginalIndex = tools.Count - 1 // Index where the tool was added
        });

        UpdateListBox();
        SaveToolsToConfig(); // Save the tools to the configuration file immediately after adding a new tool
        RefreshTools();
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

        // Determine which list to use for populating listBox1
        List<ScriptingTool> toolsToDisplay = isSearchActive ? filteredTools : tools;

        foreach (ScriptingTool tool in toolsToDisplay)
        {
            listBox1.Items.Add(tool.Name);
        }
    }



    private void listBox1_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (Control.ModifierKeys == Keys.Control)
        {
            // Suppress the keypress when Control is pressed
            e.Handled = true;
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
            textBox1.Text = string.Empty; // Optionally clear the text
            listBox1.Focus(); // Set focus to listBox1
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

        // Check if a search is active
        isSearchActive = !string.IsNullOrWhiteSpace(searchText);

        // Filter tools based on the entered search text
        filteredTools = tools
            .Where(tool => tool.Name.ToLower().Contains(searchText))
            .ToList();

        // Update the ListBox with the filtered tools for search
        UpdateListBox();
    }







    private bool IsSearchActive()
    {
        // Assuming 'filteredTools' is null or identical to 'tools' when no search is active
        return filteredTools != null && !filteredTools.SequenceEqual(tools);
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
        components = new Container();
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
        notifyIcon = new NotifyIcon(components);
        label6 = new Label();
        SuspendLayout();
        // 
        // button1
        // 
        button1.FlatAppearance.MouseDownBackColor = Color.LimeGreen;
        button1.FlatAppearance.MouseOverBackColor = Color.LimeGreen;
        button1.FlatStyle = FlatStyle.Flat;
        button1.Font = new Font("Calibri", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
        button1.ForeColor = SystemColors.ActiveCaptionText;
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
        button3.BackColor = Color.Transparent;
        button3.FlatAppearance.BorderColor = Color.Black;
        button3.FlatAppearance.MouseDownBackColor = Color.LimeGreen;
        button3.FlatAppearance.MouseOverBackColor = Color.LimeGreen;
        button3.FlatStyle = FlatStyle.Flat;
        button3.ForeColor = SystemColors.ActiveCaptionText;
        button3.Location = new Point(123, 195);
        button3.Name = "button3";
        button3.Size = new Size(49, 24);
        button3.TabIndex = 2;
        button3.Text = "+";
        button3.UseVisualStyleBackColor = false;
        button3.Click += button3_Click;
        // 
        // listBox1
        // 
        listBox1.BackColor = SystemColors.ButtonFace;
        listBox1.BorderStyle = BorderStyle.None;
        listBox1.Font = new Font("Calibri", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
        listBox1.ForeColor = SystemColors.MenuText;
        listBox1.FormattingEnabled = true;
        listBox1.ItemHeight = 14;
        listBox1.Location = new Point(12, 80);
        listBox1.Name = "listBox1";
        listBox1.Size = new Size(215, 98);
        listBox1.TabIndex = 2;
        listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
        // 
        // button5
        // 
        button5.FlatAppearance.MouseDownBackColor = Color.LimeGreen;
        button5.FlatAppearance.MouseOverBackColor = Color.LimeGreen;
        button5.FlatStyle = FlatStyle.Flat;
        button5.Font = new Font("Calibri", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
        button5.ForeColor = SystemColors.ActiveCaptionText;
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
        label1.ForeColor = SystemColors.ActiveCaptionText;
        label1.Location = new Point(12, 62);
        label1.Name = "label1";
        label1.Size = new Size(51, 15);
        label1.TabIndex = 7;
        label1.Text = "Toolbox";
        // 
        // button6
        // 
        button6.FlatAppearance.BorderColor = Color.Black;
        button6.FlatAppearance.MouseDownBackColor = Color.Red;
        button6.FlatAppearance.MouseOverBackColor = Color.LightCoral;
        button6.FlatStyle = FlatStyle.Flat;
        button6.ForeColor = SystemColors.ActiveCaptionText;
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
        linkLabel1.ActiveLinkColor = Color.DodgerBlue;
        linkLabel1.AutoSize = true;
        linkLabel1.Font = new Font("Calibri", 9F);
        linkLabel1.LinkBehavior = LinkBehavior.NeverUnderline;
        linkLabel1.LinkColor = Color.MidnightBlue;
        linkLabel1.Location = new Point(141, 283);
        linkLabel1.Name = "linkLabel1";
        linkLabel1.Size = new Size(31, 14);
        linkLabel1.TabIndex = 9;
        linkLabel1.TabStop = true;
        linkLabel1.Text = "Blog";
        linkLabel1.VisitedLinkColor = Color.Gray;
        linkLabel1.LinkClicked += linkLabel1_LinkClicked;
        // 
        // linkLabel2
        // 
        linkLabel2.ActiveLinkColor = Color.DodgerBlue;
        linkLabel2.AutoSize = true;
        linkLabel2.Font = new Font("Calibri", 9F);
        linkLabel2.LinkBehavior = LinkBehavior.NeverUnderline;
        linkLabel2.LinkColor = Color.MidnightBlue;
        linkLabel2.Location = new Point(182, 283);
        linkLabel2.Name = "linkLabel2";
        linkLabel2.Size = new Size(47, 14);
        linkLabel2.TabIndex = 10;
        linkLabel2.TabStop = true;
        linkLabel2.Text = "Donate";
        linkLabel2.VisitedLinkColor = Color.Gray;
        linkLabel2.LinkClicked += linkLabel2_LinkClicked;
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Font = new Font("Calibri", 15F, FontStyle.Bold);
        label2.ForeColor = SystemColors.ActiveCaptionText;
        label2.Location = new Point(60, 9);
        label2.Name = "label2";
        label2.Size = new Size(118, 24);
        label2.TabIndex = 14;
        label2.Text = "Task Runway";
        label2.TextAlign = ContentAlignment.MiddleCenter;
        label2.Click += label2_Click_1;
        // 
        // label3
        // 
        label3.AutoSize = true;
        label3.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label3.ForeColor = SystemColors.ActiveCaptionText;
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
        label4.Font = new Font("Calibri", 9F, FontStyle.Bold);
        label4.ForeColor = SystemColors.ActiveCaptionText;
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
        textBox1.BackColor = SystemColors.ButtonFace;
        textBox1.BorderStyle = BorderStyle.None;
        textBox1.Font = new Font("Segoe UI", 7.5F);
        textBox1.ForeColor = SystemColors.ActiveCaptionText;
        textBox1.Location = new Point(109, 60);
        textBox1.Name = "textBox1";
        textBox1.PlaceholderText = "Search your toolbox";
        textBox1.Size = new Size(117, 14);
        textBox1.TabIndex = 1;
        textBox1.TextAlign = HorizontalAlignment.Center;
        textBox1.TextChanged += textBox1_TextChanged_1;
        // 
        // notifyIcon
        // 
        notifyIcon.Icon = (Icon)resources.GetObject("notifyIcon.Icon");
        notifyIcon.Text = "Task Runway";
        // 
        // label6
        // 
        label6.AutoSize = true;
        label6.BackColor = SystemColors.ActiveCaptionText;
        label6.Location = new Point(178, 16);
        label6.Name = "label6";
        label6.Size = new Size(37, 15);
        label6.TabIndex = 18;
        label6.Tag = "Timer";
        label6.Text = "Timer";
        label6.Visible = false;
        // 
        // Form1
        // 
        AccessibleRole = AccessibleRole.None;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoSize = true;
        BackColor = SystemColors.ButtonFace;
        ClientSize = new Size(240, 311);
        Controls.Add(label6);
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
        ForeColor = SystemColors.ButtonFace;
        FormBorderStyle = FormBorderStyle.None;
        Icon = (Icon)resources.GetObject("$this.Icon");
        MaximizeBox = false;
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
        if (selectedIndex >= 0)
        {
            // Save the current top index of the ListBox for maintaining scroll position
            int topIndex = listBox1.TopIndex;

            // Call DeleteItem to perform the deletion
            DeleteItem(selectedIndex);

            // Restore the ListBox's scroll position
            listBox1.TopIndex = topIndex;

            // Optionally, update the selected index to the next item in the list
            if (selectedIndex < listBox1.Items.Count)
            {
                listBox1.SelectedIndex = selectedIndex;
            }
            else if (listBox1.Items.Count > 0)
            {
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            }
        }
        else
        {
            MessageBox.Show("No item selected to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }


    private void DeleteItem(int index)
    {
        if (index >= 0 && index < (isSearchActive ? filteredTools : tools).Count)
        {
            var currentList = isSearchActive ? filteredTools : tools;
            ScriptingTool deletedTool = currentList[index];

            // Record the deletion in the undo stack
            undoStack.Push(new UndoOperation
            {
                Type = OperationType.Delete,
                Tool = deletedTool,
                OriginalIndex = tools.IndexOf(deletedTool)
            });

            // Remove the tool from all relevant lists
            tools.Remove(deletedTool);
            originalOrderTools.Remove(deletedTool);
            filteredTools.Remove(deletedTool); // Update if filteredTools is in sync with tools

            // Update UI and save changes
            SaveToolsToConfig();
            LoadToolsFromConfig();

            // Refresh the list based on the search
            if (isSearchActive)
            {
                PerformSearch();
            }
            else
            {
                UpdateListBox();
            }
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

        // Load preferences from the JSON file
        string preferencesFilePath = "preferences.json";
        preferences = Preferences.LoadPreferences(preferencesFilePath);

        searchbarItem = new ToolStripMenuItem("Search Bar");
        searchbarItem.CheckOnClick = true;
        searchbarItem.Checked = preferences.IsSearchbarVisible; // Set initial state based on preferences

        // Event handler for toggling the search bar
        searchbarItem.Click += delegate
        {
            // Toggle the visibility of the search bar
            ToggleSearchbar(searchbarItem);

            // Save the updated preferences
            preferences.SavePreferences(preferencesFilePath);
        };
        contextMenu.Items.Add(searchbarItem);

        alwaysOnTopItem = new ToolStripMenuItem("Always On Top");
        alwaysOnTopItem.CheckOnClick = true;
        alwaysOnTopItem.Checked = preferences.AlwaysOnTop;
        alwaysOnTopItem.Click += delegate
        {
            // Toggle the "Always On Top" state
            ToggleAlwaysOnTop(alwaysOnTopItem);

            // Update the preferences to reflect the new state
            preferences.AlwaysOnTop = alwaysOnTopItem.Checked;

            // Save the updated preferences
            preferences.SavePreferences(preferencesFilePath);
        };
        contextMenu.Items.Add(alwaysOnTopItem);


        minimizeToTrayItem = new ToolStripMenuItem("Minimize To Tray");
        minimizeToTrayItem.CheckOnClick = true;
        minimizeToTrayItem.Checked = preferences.MinimizeToTrayOnClose;
        minimizeToTrayItem.Click += delegate
        {
            preferences.MinimizeToTrayOnClose = !preferences.MinimizeToTrayOnClose;
            minimizeToTrayOnClose = preferences.MinimizeToTrayOnClose; // Update the flag used in FormClosing
            preferences.SavePreferences(preferencesFilePath); // Save the updated preferences
        };
        contextMenu.Items.Add(minimizeToTrayItem);


        // Assuming Preferences class is already loaded
        preferences.DarkMode = preferences.DarkMode ?? false; // Default to false if not set

        // Create the Dark Mode menu item
        ToolStripMenuItem darkModeItem = new ToolStripMenuItem("Dark Mode");
        darkModeItem.CheckOnClick = true;
        darkModeItem.Checked = preferences.DarkMode ?? false; // Default to false if not set

        darkModeItem.Click += delegate
        {
            if (!preferences.SupporterLicensePurchased)
            {
                if (MessageBox.Show("Have you purchased a Supporter's License Key?", "License Check", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    preferences.SupporterLicensePurchased = true; // Set this flag to true for all features
                }
                else
                {
                    MessageBox.Show("This feature is restricted to supporters only. Please purchase a Supporter's License.", "License Required", MessageBoxButtons.OK);
                    darkModeItem.Checked = false; // Reset the check
                    return; // Exit the event handler early
                }
            }
            else
            {
                preferences.DarkMode = darkModeItem.Checked; // Update the preference
            }

            // Toggle between Dark Mode and Light Mode based on the checked state
            if (darkModeItem.Checked)
            {
                ApplyDarkMode();
            }
            else
            {
                ApplyLightMode();
            }

            preferences.SavePreferences(preferencesFilePath); // Save the updated preferences
        };

        contextMenu.Items.Add(darkModeItem);

        /*
        // Create the Pomodoro Timer menu item
        ToolStripMenuItem pomodoroTimerItem = new ToolStripMenuItem("Pomodoro Timer");
        pomodoroTimerItem.CheckOnClick = true;
        pomodoroTimerItem.Checked = preferences.PomodoroTimerEnabled ?? false; // Default to false if not set

        pomodoroTimerItem.Click += delegate
        {
            if (!preferences.SupporterLicensePurchased)
            {
                if (MessageBox.Show("Have you purchased a Supporter's License Key?", "License Check", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    preferences.SupporterLicensePurchased = true;
                }
                else
                {
                    MessageBox.Show("This feature is restricted to supporters only. Please purchase a Supporter's License.", "License Required", MessageBoxButtons.OK);
                    pomodoroTimerItem.Checked = false;
                    return;
                }
            }
            else
            {
                preferences.PomodoroTimerEnabled = pomodoroTimerItem.Checked; // Update the preference
            }

            // Here you can add code to enable/disable the Pomodoro Timer based on the checked state
            TogglePomodoroTimer(pomodoroTimerItem.Checked);

            preferences.SavePreferences(preferencesFilePath); // Save the updated preferences
        };

        contextMenu.Items.Add(pomodoroTimerItem);
        */

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

    private void TogglePomodoroTimer(bool enable)
    {
        if (enable)
        {
            // Code to enable Pomodoro Timer

            // Show label6
            label6.Visible = true;
        }
        else
        {
            // Code to disable Pomodoro Timer

            // Hide label6
            label6.Visible = false;
        }
    }

    // Assuming you have a reference to the Timer form as timerForm
    private void label6_Click(object sender, EventArgs e)
    {
        if (isTimerRunning)
        {
            timer1.Stop(); // Pause the timer
            isTimerRunning = false;
        }
        else
        {
            timer1.Start(); // Start or resume the timer
            isTimerRunning = true;
        }
    }


    private void label6_DoubleClick(object sender, EventArgs e)
    {
        countdownTime = 25 * 60; // Reset the countdown to 25 minutes
        UpdateLabel(); // Update the label to display the new time
    }


    private void timer1_Tick(object sender, EventArgs e)
    {
        if (countdownTime > 0)
        {
            countdownTime--;
            UpdateLabel();
        }
        else
        {
            timer1.Stop(); // Stop the timer when the countdown reaches zero
            isTimerRunning = false;
        }
    }

    private void UpdateLabel()
    {
        int minutes = countdownTime / 60;
        int seconds = countdownTime % 60;
        label6.Text = $"{minutes:D2}:{seconds:D2}";
    }


    private void ApplyDarkMode()
    {
        // Set the form's background color
        this.BackColor = SystemColors.ActiveCaptionText;
        this.ForeColor = SystemColors.ButtonFace;

        SetButtonBorderStyle(button1);
        SetButtonBorderStyle(button3);
        SetButtonBorderStyle(button5);
        SetButtonBorderStyle(button6);

        // Method to recursively apply the dark theme to all controls
        ApplyDarkModeToControls(this.Controls);
    }

    private void SetButtonBorderStyle(Button button)
    {
        button.FlatAppearance.BorderSize = 1;
        button.FlatAppearance.BorderColor = Color.FromArgb(255, 255, 255); // White border, adjust as needed
    }

    private void ApplyDarkModeToControls(Control.ControlCollection controls)
    {
        foreach (Control c in controls)
        {
            if (c is Button)
            {
                // Example for buttons
                c.BackColor = Color.FromArgb(30, 30, 30); // Dark gray
                c.ForeColor = Color.White;
                ((Button)c).FlatStyle = FlatStyle.Flat;
            }
            else if (c is Label || c is LinkLabel)
            {
                // Example for labels and link labels
                c.ForeColor = Color.White;
                if (c is LinkLabel)
                {
                    ((LinkLabel)c).LinkColor = Color.LightBlue; // Or any color that suits your theme
                }
            }
            else if (c is ListBox)
            {
                // Example for ListBoxes
                c.BackColor = Color.Black;
                c.ForeColor = Color.White;
            }
            else if (c is TextBox)
            {
                // Example for TextBoxes
                c.BackColor = Color.FromArgb(50, 50, 50); // Slightly lighter than the button's color
                c.ForeColor = Color.White;
            }
            // ... Add more control types as needed

            // Apply dark mode to any child controls (like panels, group boxes, etc.)
            if (c.Controls.Count > 0)
            {
                ApplyDarkModeToControls(c.Controls);
            }
        }
    }

    private void ApplyLightMode()
    {
        // Button1 properties
        button1.BackColor = Color.Transparent;
        button1.FlatAppearance.BorderColor = Color.Black;
        button1.FlatAppearance.MouseDownBackColor = Color.LimeGreen;
        button1.FlatAppearance.MouseOverBackColor = Color.LimeGreen;
        button1.FlatStyle = FlatStyle.Flat;
        button1.ForeColor = SystemColors.ActiveCaptionText;
        button1.UseVisualStyleBackColor = true;

        // Button3 properties
        button3.BackColor = Color.Transparent;
        button3.FlatAppearance.BorderColor = Color.Black;
        button3.FlatAppearance.MouseDownBackColor = Color.LimeGreen;
        button3.FlatAppearance.MouseOverBackColor = Color.LimeGreen;
        button3.FlatStyle = FlatStyle.Flat;
        button3.ForeColor = SystemColors.ActiveCaptionText;
        button3.UseVisualStyleBackColor = false;

        // ListBox1 properties
        listBox1.BackColor = SystemColors.ButtonFace;
        listBox1.BorderStyle = BorderStyle.None;
        listBox1.Font = new Font("Calibri", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
        listBox1.ForeColor = SystemColors.MenuText;

        // Button5 properties
        button5.BackColor = Color.Transparent;
        button5.FlatAppearance.BorderColor = Color.Black;
        button5.FlatAppearance.MouseDownBackColor = Color.LimeGreen;
        button5.FlatAppearance.MouseOverBackColor = Color.LimeGreen;
        button5.FlatStyle = FlatStyle.Flat;
        button5.ForeColor = SystemColors.ActiveCaptionText;
        button5.UseVisualStyleBackColor = true;

        // Label1 properties
        label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label1.ForeColor = SystemColors.ActiveCaptionText;

        // Button6 properties
        button6.BackColor = Color.Transparent;
        button6.FlatAppearance.BorderColor = Color.Black;
        button6.FlatAppearance.MouseDownBackColor = Color.Red;
        button6.FlatAppearance.MouseOverBackColor = Color.LightCoral;
        button6.FlatStyle = FlatStyle.Flat;
        button6.ForeColor = SystemColors.ActiveCaptionText;
        button6.UseVisualStyleBackColor = true;

        // LinkLabel1 properties
        linkLabel1.ActiveLinkColor = Color.DodgerBlue;
        linkLabel1.Font = new Font("Calibri", 9F);
        linkLabel1.LinkBehavior = LinkBehavior.NeverUnderline;
        linkLabel1.LinkColor = Color.MidnightBlue;
        linkLabel1.VisitedLinkColor = Color.Gray;

        // LinkLabel2 properties
        linkLabel2.ActiveLinkColor = Color.DodgerBlue;
        linkLabel2.Font = new Font("Calibri", 9F);
        linkLabel2.LinkBehavior = LinkBehavior.NeverUnderline;
        linkLabel2.LinkColor = Color.MidnightBlue;
        linkLabel2.VisitedLinkColor = Color.Gray;

        // Label2 properties
        label2.Font = new Font("Calibri", 15F, FontStyle.Bold);
        label2.ForeColor = SystemColors.ActiveCaptionText;
        label2.TextAlign = ContentAlignment.MiddleCenter;

        // Label3 properties
        label3.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
        label3.ForeColor = SystemColors.ActiveCaptionText;
        label3.TextAlign = ContentAlignment.MiddleCenter;

        // Label4 properties
        label4.Font = new Font("Calibri", 9F, FontStyle.Bold);
        label4.ForeColor = SystemColors.ActiveCaptionText;
        label4.TextAlign = ContentAlignment.MiddleCenter;

        // Label5 properties
        label5.Font = new Font("Segoe UI", 14F);
        label5.ForeColor = SystemColors.ActiveCaptionText;

        // TextBox1 properties
        textBox1.BackColor = SystemColors.ButtonFace;
        textBox1.BorderStyle = BorderStyle.None;
        textBox1.Font = new Font("Segoe UI", 7.5F);
        textBox1.ForeColor = SystemColors.ActiveCaptionText;
        textBox1.TextAlign = HorizontalAlignment.Center;

        // Form properties
        this.BackColor = SystemColors.ButtonFace;
        this.ForeColor = SystemColors.ButtonFace;

        // Refresh the form to apply the changes
        this.Refresh();
    }


    private void ToggleAlwaysOnTop(ToolStripMenuItem item)
    {
        this.TopMost = !this.TopMost;
        item.Checked = this.TopMost;

        if (urlForm != null && !urlForm.IsDisposed)
            urlForm.TopMost = this.TopMost;

        if (toolNameForm != null && !toolNameForm.IsDisposed)
            toolNameForm.TopMost = this.TopMost;
    }






    private void ToggleSearchbar(ToolStripMenuItem searchbarItem)
    {
        // Toggle the visibility based on the current state in preferences
        preferences.IsSearchbarVisible = !preferences.IsSearchbarVisible;

        // Apply the updated visibility to the textBox1 (assuming textBox1 is your search bar)
        textBox1.Visible = preferences.IsSearchbarVisible;

        // Update the Checked state of the menu item to reflect the new visibility state
        searchbarItem.Checked = preferences.IsSearchbarVisible;

        // Clear the search text if the search bar is being hidden
        if (!preferences.IsSearchbarVisible)
        {
            textBox1.Text = string.Empty;
        }

        // Consider saving the preferences here if the changes need to be immediately persisted
        // preferences.SavePreferences(preferencesFilePath);
    }






    private string previousCustomFlags = ""; // Declare a variable to store the previous custom flags

    private void CustomFlags()
    {
        int selectedIndex = listBox1.SelectedIndex;
        if (selectedIndex >= 0 && selectedIndex < tools.Count)
        {
            ScriptingTool selectedTool = tools[selectedIndex];
            if (selectedTool.Path.EndsWith(".bat", StringComparison.OrdinalIgnoreCase) || selectedTool.Path.EndsWith(".py", StringComparison.OrdinalIgnoreCase) || selectedTool.Path.EndsWith(".ps1", StringComparison.OrdinalIgnoreCase))
            {
                string currentFlags = selectedTool.CustomFlags;

                // Set the default input to the previous custom flags
                string newFlags = Interaction.InputBox("Enter Custom Flags:", "Edit Custom Flags", previousCustomFlags);

                // Store the previous custom flags
                previousCustomFlags = newFlags;

                // Update the custom flags
                selectedTool.CustomFlags = newFlags;
                SaveToolsToConfig();

                // Create an undo operation and push it to the stack
                undoStack.Push(new UndoOperation
                {
                    Type = OperationType.CustomFlagsChange,
                    Tool = selectedTool,
                    OldCustomFlags = currentFlags
                });
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


    enum OperationType { Add, Delete, Rename, Move, PathChange, CustomFlagsChange }

    class UndoOperation
    {
        public OperationType Type { get; set; }
        public ScriptingTool Tool { get; set; }
        public int OriginalIndex { get; set; }
        public int NewIndex { get; set; } // For move operations
        public string OldName { get; set; } // For rename operations
        public string NewName { get; set; } // For rename operations
        public string OldPath { get; set; } // For path change operations
        public string OldCustomFlags { get; set; }
        public string NewCustomFlags { get; set; }
    }


    private Stack<UndoOperation> undoStack = new Stack<UndoOperation>();

    private void UndoChanges()
    {
        if (undoStack.Count > 0)
        {
            int topIndex = listBox1.TopIndex;

            var operation = undoStack.Pop();
            switch (operation.Type)
            {
                case OperationType.Add:
                    UndoAdd(operation);
                    break;
                case OperationType.Delete:
                    UndoDelete(operation);
                    break;
                case OperationType.Rename:
                    UndoRename(operation);
                    break;
                case OperationType.Move:
                    UndoMove(operation);
                    break;
                case OperationType.PathChange:
                    UndoPathChange(operation);
                    break;
                case OperationType.CustomFlagsChange:
                    UndoCustomFlagsChange(operation);
                    break;
            }

            UpdateListBox(filteredTools);
            SaveToolsToConfig();
            LoadToolsFromConfig();
            listBox1.TopIndex = topIndex;
            RefreshTools();
        }
        else
        {
            MessageBox.Show("No actions to undo.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void UndoCustomFlagsChange(UndoOperation operation)
    {
        if (operation != null && operation.Tool != null)
        {
            // Restore the custom flags to the previous value
            operation.Tool.CustomFlags = operation.OldCustomFlags;
            SaveToolsToConfig();
        }
    }



    private void UndoAdd(UndoOperation operation)
    {
        if (operation != null && operation.OriginalIndex >= 0 && operation.OriginalIndex < tools.Count)
        {
            tools.RemoveAt(operation.OriginalIndex);
            originalOrderTools.RemoveAt(operation.OriginalIndex); // If originalOrderTools should mirror tools

            // Update UI            
            SaveToolsToConfig();
            UpdateListBox(filteredTools);
            LoadToolsFromConfig();
        }
    }




    private void UndoDelete(UndoOperation operation)
    {
        if (operation != null && operation.Tool != null && operation.OriginalIndex >= 0 && operation.OriginalIndex <= tools.Count)
        {
            // Re-insert the tool at its original position
            tools.Insert(operation.OriginalIndex, operation.Tool);
            filteredTools.Insert(operation.OriginalIndex, operation.Tool); // If using
            originalOrderTools.Insert(operation.OriginalIndex, operation.Tool); // If using

            // Update UI
            UpdateListBox(filteredTools);
            SaveToolsToConfig();
            RefreshTools();

        }
    }




    private void UndoRename(UndoOperation operation)
    {
        if (operation != null && operation.Tool != null && !string.IsNullOrEmpty(operation.OldName))
        {
            operation.Tool.Name = operation.OldName;

            // Update UI
            UpdateListBox(filteredTools);
            SaveToolsToConfig();
            LoadToolsFromConfig();
        }
    }



    private void UndoMove(UndoOperation operation)
    {
        if (operation != null && operation.OriginalIndex >= 0 && operation.NewIndex >= 0 &&
            operation.OriginalIndex < tools.Count && operation.NewIndex < tools.Count)
        {
            SwapItemsInList(tools, operation.OriginalIndex, operation.NewIndex);
            SwapItemsInList(originalOrderTools, operation.OriginalIndex, operation.NewIndex);
            SwapItemsInList(filteredTools, operation.OriginalIndex, operation.NewIndex);

            UpdateListBox();
            SaveToolsToConfig();

        }
    }


    private void UndoPathChange(UndoOperation operation)
    {
        // Check if the operation and tool are valid
        if (operation != null && operation.Tool != null)
        {
            // Revert the tool's path to its old value
            operation.Tool.Path = operation.OldPath;

            // Optional: If you need to update a specific tool in a list based on its index
            if (operation.OriginalIndex >= 0 && operation.OriginalIndex < tools.Count)
            {
                tools[operation.OriginalIndex].Path = operation.OldPath;
            }

            // Update the UI with the latest tool data
            UpdateListBox(filteredTools);

            // Save the updated tool data to your configuration
            SaveToolsToConfig();
        }
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

        // Check if an item is selected
        if (selectedIndex >= 0)
        {
            ScriptingTool selectedTool;

            // Determine which list to use based on search state
            if (isSearchActive)
            {
                // Use the filtered list when a search is active
                if (selectedIndex < filteredTools.Count)
                {
                    selectedTool = filteredTools[selectedIndex];
                }
                else
                {
                    MessageBox.Show("Invalid selection.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                // Use the complete list when no search is active
                if (selectedIndex < tools.Count)
                {
                    selectedTool = tools[selectedIndex];
                }
                else
                {
                    MessageBox.Show("Invalid selection.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // Copy the path to the clipboard
            Clipboard.SetText(selectedTool.Path);
        }
        else
        {
            MessageBox.Show("No item selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show("This action is only compatible with scripts, programs, and folders.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }
    }

    private void OpenConfigFile()
    {
        Process notepadProcess = Process.Start("notepad.exe", configPath);

        // Wait for the Notepad process to be created
        notepadProcess.WaitForInputIdle();

        // Make the Notepad window appear on top
        IntPtr notepadWindowHandle = notepadProcess.MainWindowHandle;
        SetWindowPos(notepadWindowHandle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
    }

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOSIZE = 0x0001;
    private void OpenPathForSelectedTool()
    {
        int selectedIndex = listBox1.SelectedIndex;

        if (isSearchActive)
        {
            if (selectedIndex >= 0 && selectedIndex < filteredTools.Count)
            {
                ScriptingTool selectedTool = filteredTools[selectedIndex];
                string folderPath = System.IO.Path.GetDirectoryName(selectedTool.Path);

                if (!string.IsNullOrEmpty(folderPath))
                {
                    if (Directory.Exists(folderPath))
                    {
                        // If it's a folder, open it directly
                        Process.Start("explorer.exe", folderPath);
                    }
                    else
                    {
                        string extension = System.IO.Path.GetExtension(selectedTool.Path).ToLower();

                        // Allowed script file extensions
                        List<string> allowedExtensions = new List<string> { ".bat", ".exe", ".ps1", ".py" };

                        if (allowedExtensions.Contains(extension))
                        {
                            // If it's a script file, open the folder containing it
                            Process.Start("explorer.exe", folderPath);
                        }
                        else if (!IsWebsite(selectedTool.Path))
                        {
                            // Additional check to exclude URLs (websites)
                            MessageBox.Show("This action is only compatible with .bat, .exe, .ps1, .py scripts, folders, documents, and programs.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Unable to determine the folder path for the selected tool.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
            else
            {
                MessageBox.Show("Please select a valid tool to open its path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }
        else
        {
            if (selectedIndex >= 0 && selectedIndex < tools.Count)
            {
                ScriptingTool selectedTool = tools[selectedIndex];
                string folderPath = System.IO.Path.GetDirectoryName(selectedTool.Path);

                if (!string.IsNullOrEmpty(folderPath))
                {
                    if (Directory.Exists(folderPath))
                    {
                        // If it's a folder, open it directly
                        Process.Start("explorer.exe", folderPath);
                    }
                    else
                    {
                        string extension = System.IO.Path.GetExtension(selectedTool.Path).ToLower();

                        // Allowed script file extensions
                        List<string> allowedExtensions = new List<string> { ".bat", ".exe", ".ps1", ".py" };

                        if (allowedExtensions.Contains(extension))
                        {
                            // If it's a script file, open the folder containing it
                            Process.Start("explorer.exe", folderPath);
                        }
                        else if (!IsWebsite(selectedTool.Path))
                        {
                            // Additional check to exclude URLs (websites)
                            MessageBox.Show("This action is only compatible with .bat, .exe, .ps1, .py scripts, folders, documents, and programs.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Unable to determine the folder path for the selected tool.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
            else
            {
                MessageBox.Show("Please select a valid tool to open its path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }
    }

    // Function to check if a path is a website (URL)
    private bool IsWebsite(string path)
    {
        Uri uriResult;
        return Uri.TryCreate(path, UriKind.Absolute, out uriResult) &&
            (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }



    // Custom function to show the smaller input box
    private string ShowSmallInputBox(string prompt, string title)
    {
        TextBox textBox = new TextBox();
        textBox.Size = new Size(200, 20); // Set the size you prefer
        Form form = new Form();
        form.Text = title;
        form.Size = new Size(300, 100); // Adjust the form size as needed
        form.Controls.Add(textBox);

        Label label = new Label();
        label.Text = prompt;
        label.Location = new Point(10, 20);
        form.Controls.Add(label);

        Button buttonOK = new Button();
        buttonOK.Text = "OK";
        buttonOK.DialogResult = DialogResult.OK;
        buttonOK.Location = new Point(10, 50);
        form.Controls.Add(buttonOK);

        form.AcceptButton = buttonOK;
        DialogResult dialogResult = form.ShowDialog();

        if (dialogResult == DialogResult.OK)
        {
            return textBox.Text;
        }
        else
        {
            return string.Empty;
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

    private NotifyIcon notifyIcon;
    private System.Windows.Forms.Timer timer1;
    private TreeView treeView1;
    private ContextMenuStrip contextMenuStrip1;
    private ToolStripMenuItem toolStripMenuItem1;
    private Label label6;
}
