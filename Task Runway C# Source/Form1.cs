using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO; // Add this for file operations
using System.Runtime.InteropServices;
using System.Windows.Forms;

public class Form1 : Form
{
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


    private List<ScriptingTool> tools = new List<ScriptingTool>(); // Add this list

    private string configPath = "config.txt"; // Add the path for config file

    public Form1()
    {
        InitializeComponent();
        InitializeContextMenu();

        string relativeIconPath = Path.Combine(Application.StartupPath, "app_icon.ico");
        this.Icon = new Icon(relativeIconPath);
        LoadToolsFromConfig(); // Load tools from config on form initialization

        MaximizeBox = false;
        MinimizeBox = false;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        listBox1.KeyDown += listBox1_KeyDown;

    }

    private void Form1_Load(object sender, EventArgs e)
    {
        // ... (existing code)

        // Attach the context menu to the listBox1 control
        listBox1.ContextMenuStrip = listBoxContextMenu;

        // Attach the MouseDown event handler
        listBox1.MouseDown += listBox1_MouseDown;
    }



    private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
    }

    private ContextMenuStrip listBoxContextMenu;
    private void listBox1_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            int index = listBox1.IndexFromPoint(e.Location);

            if (index >= 0)
            {
                // Select the item on right-click
                listBox1.SelectedIndex = index;

                // Show the context menu only if an item is right-clicked
                listBoxContextMenu.Show(listBox1, listBox1.PointToClient(Cursor.Position));
            }
        }
    }






    private void InitializeContextMenu()
    {
        listBoxContextMenu = new ContextMenuStrip();

        // Add menu items and their event handlers
        ToolStripMenuItem refreshItem = new ToolStripMenuItem("Refresh");
        refreshItem.Click += (s, args) => RefreshTools();
        listBoxContextMenu.Items.Add(refreshItem);

        ToolStripMenuItem undoItem = new ToolStripMenuItem("Undo");
        undoItem.Click += (s, args) => UndoChanges();
        listBoxContextMenu.Items.Add(undoItem);

        ToolStripMenuItem customFlagsItem = new ToolStripMenuItem("Custom Flags");
        customFlagsItem.Click += (s, args) => CustomFlags();
        listBoxContextMenu.Items.Add(customFlagsItem);

        // Add a separator
        listBoxContextMenu.Items.Add(new ToolStripSeparator());

        // Add "Rename" menu item
        ToolStripMenuItem renameItem = new ToolStripMenuItem("Rename");
        renameItem.Click += (s, args) => RenameTool();
        listBoxContextMenu.Items.Add(renameItem);

        // Attach the context menu to the listBox1 control
        listBox1.ContextMenuStrip = listBoxContextMenu;

        // Attach the listBox1_MouseDown event handler
        listBox1.MouseDown += listBox1_MouseDown;
    }

    private void RenameTool()
    {
        if (listBox1.SelectedIndex != -1)
        {
            int selectedIndex = listBox1.SelectedIndex;
            string currentName = tools[selectedIndex].Name;

            // Prompt user for a new name
            string newName = InputBox("Enter a new name:", "Rename Tool", currentName);

            // Check if the user provided a new name
            if (!string.IsNullOrEmpty(newName))
            {
                // Save the current name for potential undo
                renamedToolsHistory.Push(new Tuple<int, string>(selectedIndex, currentName));

                // Update the tool name
                tools[selectedIndex].Name = newName;

                // Save the updated tools list to the config file
                SaveToolsToConfig();

                // Update the listBox to reflect the changes
                UpdateListBox();
            }
        }
    }

    public static string InputBox(string prompt, string title, string defaultValue = "")
    {
        Form promptForm = new Form()
        {
            Width = 250,
            Height = 130,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            Text = title,
            StartPosition = FormStartPosition.CenterScreen
        };

        Label textLabel = new Label() { Left = 20, Top = 20, Text = prompt, Width = 200 };
        TextBox textBox = new TextBox() { Left = 20, Top = 50, Width = 150, Text = defaultValue };
        Button confirmation = new Button() { Text = "Ok", Left = 180, Width = 40, Top = 50, DialogResult = DialogResult.OK };
        Button cancellation = new Button() { Text = "Cancel", Left = 130, Width = 40, Top = 50, DialogResult = DialogResult.Cancel };

        confirmation.Click += (sender, e) => { promptForm.Close(); };
        cancellation.Click += (sender, e) => { promptForm.Close(); };
        textBox.KeyDown += (sender, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Prevent system beep
                confirmation.PerformClick(); // Trigger the "Ok" button click
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true; // Prevent system beep
                cancellation.PerformClick(); // Trigger the "Cancel" button click
            }
        };

        promptForm.Controls.Add(textBox);
        promptForm.Controls.Add(confirmation);
        promptForm.Controls.Add(cancellation);
        promptForm.Controls.Add(textLabel);

        return promptForm.ShowDialog() == DialogResult.OK ? textBox.Text : "";
    }



    private void button1_Click(object sender, EventArgs e)
    {
        LaunchSelectedTool();
    }

    private void listBox1_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            LaunchSelectedTool();
            e.Handled = true; // Mark the event as handled to prevent additional processing
        }
    }

    private void LaunchSelectedTool()
    {
        int selectedIndex = listBox1.SelectedIndex;

        if (selectedIndex >= 0 && selectedIndex < tools.Count)
        {
            ScriptingTool selectedTool = tools[selectedIndex];

            try
            {
                // Determine the type of the selected tool and launch accordingly
                if (selectedTool.Path.EndsWith(".bat", StringComparison.OrdinalIgnoreCase))
                {
                    // If it's a batch script, launch using cmd.exe
                    System.Diagnostics.Process.Start(new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/k \"{selectedTool.Path}\" {selectedTool.CustomFlags}",
                        WorkingDirectory = Path.GetDirectoryName(selectedTool.Path),
                        CreateNoWindow = false
                    });
                }
                else if (selectedTool.Path.EndsWith(".py", StringComparison.OrdinalIgnoreCase))
                {
                    // If it's a Python script, launch using cmd.exe to keep the console window open
                    System.Diagnostics.Process.Start(new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/k python \"{selectedTool.Path}\" {selectedTool.CustomFlags}",
                        WorkingDirectory = Path.GetDirectoryName(selectedTool.Path),
                        CreateNoWindow = false
                    });
                }
                else if (selectedTool.Path.EndsWith(".ps1", StringComparison.OrdinalIgnoreCase))
                {
                    // If it's a PowerShell script, launch using powershell.exe
                    System.Diagnostics.Process.Start(new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        Arguments = $"-File \"{selectedTool.Path}\" {selectedTool.CustomFlags}",
                        WorkingDirectory = Path.GetDirectoryName(selectedTool.Path),
                        CreateNoWindow = false
                    });
                }
                else if (selectedTool.Path.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                {
                    // If it's an executable, launch directly
                    System.Diagnostics.Process.Start(new ProcessStartInfo
                    {
                        FileName = selectedTool.Path,
                        WorkingDirectory = Path.GetDirectoryName(selectedTool.Path),
                        CreateNoWindow = false
                    });
                }
                else if (Uri.TryCreate(selectedTool.Path, UriKind.Absolute, out Uri uri) && uri.Scheme.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    // If it's a valid HTTP or HTTPS URL, open in the default browser
                    System.Diagnostics.Process.Start(new ProcessStartInfo
                    {
                        FileName = selectedTool.Path,
                        UseShellExecute = true
                    });
                }
                else
                {
                    MessageBox.Show($"Invalid URL format or unsupported file type: {selectedTool.Path}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }




    // Import the ShellExecute function
    [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr ShellExecute(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);

    // Constants for ShellExecute
    private const int SW_SHOWNORMAL = 1;



    private void button2_Click(object sender, EventArgs e)
    {
    }

    private void button3_Click(object sender, EventArgs e)
    {
        ContextMenuStrip contextMenu = new ContextMenuStrip();
        ToolStripMenuItem addWebsiteToolItem = new ToolStripMenuItem("Add Website (URL)");
        ToolStripMenuItem addScriptToolItem = new ToolStripMenuItem("Add Script (.ps1, .bat, .py)");
        ToolStripMenuItem addExecutableToolItem = new ToolStripMenuItem("Add Program (.exe)");

        addWebsiteToolItem.Click += (s, args) => AddTool("Website");
        addScriptToolItem.Click += (s, args) => AddTool("Script");
        addExecutableToolItem.Click += (s, args) => AddTool("Executable");

        contextMenu.Items.Add(addWebsiteToolItem);
        contextMenu.Items.Add(addScriptToolItem);
        contextMenu.Items.Add(addExecutableToolItem);

        button3.ContextMenuStrip = contextMenu;
        button3.ContextMenuStrip.Show(button3, new System.Drawing.Point(0, button3.Height));
    }

    private void AddTool(string toolType)
    {
        if (toolType == "Website")
        {
            string url = Microsoft.VisualBasic.Interaction.InputBox("Enter the URL (http(s)://whatever.com):", "Enter URL", "");

            if (!string.IsNullOrWhiteSpace(url))
            {
                string toolName = Microsoft.VisualBasic.Interaction.InputBox("Enter a name for the tool (optional):", "Enter Name", url);

                if (string.IsNullOrWhiteSpace(toolName))
                {
                    toolName = new Uri(url).Host;
                }

                AddToolToList(new ScriptingTool(toolName, url, ""));
            }
        }
        else if (toolType == "Script")
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select Script File",
                Filter = "Script files (*.bat;*.ps1;*.py)|*.bat;*.ps1;*.py",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string toolName = Microsoft.VisualBasic.Interaction.InputBox("Enter a name for the tool (optional):", "Enter Name", openFileDialog.SafeFileName);

                if (string.IsNullOrWhiteSpace(toolName))
                {
                    toolName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                }

                AddToolToList(new ScriptingTool(toolName, openFileDialog.FileName, ""));
            }
        }
        else if (toolType == "Executable")
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select Executable File",
                Filter = "Executable files (*.exe)|*.exe",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string toolName = Microsoft.VisualBasic.Interaction.InputBox("Enter a name for the tool (optional):", "Enter Name", openFileDialog.SafeFileName);

                if (string.IsNullOrWhiteSpace(toolName))
                {
                    toolName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                }

                AddToolToList(new ScriptingTool(toolName, openFileDialog.FileName, ""));
            }
        }
    }
    private void AddToolToList(ScriptingTool tool)
    {
        tools.Add(tool);
        SaveToolsToConfig();
        UpdateListBox();
    }

    private void SaveToolsToConfig()
    {
        using (StreamWriter writer = new StreamWriter(configPath))
        {
            foreach (var tool in tools)
            {
                writer.WriteLine($"[{tool.Name}]");
                writer.WriteLine($"path={tool.Path}");
                writer.WriteLine($"custom_flags={tool.CustomFlags}");
                writer.WriteLine();
            }
        }
    }

    private void LoadToolsFromConfig()
    {
        if (File.Exists(configPath))
        {
            tools.Clear();
            using (StreamReader reader = new StreamReader(configPath))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine().Trim();
                    if (line.StartsWith("[") && line.EndsWith("]"))
                    {
                        string toolName = line.Substring(1, line.Length - 2);
                        string pathLine = reader.ReadLine()?.Trim();
                        string customFlagsLine = reader.ReadLine()?.Trim();

                        if (pathLine != null && customFlagsLine != null && pathLine.StartsWith("path=") && customFlagsLine.StartsWith("custom_flags="))
                        {
                            string path = pathLine.Substring("path=".Length);
                            string customFlags = customFlagsLine.Substring("custom_flags=".Length);

                            tools.Add(new ScriptingTool(toolName, path, customFlags));
                        }
                    }
                }
            }
            UpdateListBox(); // Update the list box after loading tools
        }
    }

    private void UpdateListBox()
    {
        listBox1.Items.Clear();
        foreach (var tool in tools)
        {
            listBox1.Items.Add(tool.Name);
        }
    }

    private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        // Open the blog link in the default web browser
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://www.davidinfosec.com",
                UseShellExecute = true
            });
        }
        catch (System.ComponentModel.Win32Exception ex)
        {
            // Handle the exception or log the error message
            MessageBox.Show($"Error opening blog link: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }


    private void textBox1_TextChanged(object sender, EventArgs e)
    {
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
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        AutoSize = true;
        ClientSize = new Size(239, 311);
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
        // Center label2 horizontally and vertically
        label2.Location = new Point((this.ClientSize.Width - label2.Width) / 2, (this.ClientSize.Height - label2.Height) / 2);
        label4.Location = new Point((this.ClientSize.Width - label4.Width) / 2, label4.Top);
    }


    private void button6_Click(object sender, EventArgs e)
    {
        int selectedIndex = listBox1.SelectedIndex;

        if (selectedIndex >= 0 && selectedIndex < tools.Count)
        {
            // Remove the selected tool from the tools list
            ScriptingTool deletedTool = tools[selectedIndex];
            tools.RemoveAt(selectedIndex);

            // Save the deleted tool to the history
            deletedTools.Add(deletedTool);

            // Save the updated tools list to the config file
            SaveToolsToConfig();

            // Update the listBox to reflect the changes
            UpdateListBox();
        }
    }



    // Add the ScriptingTool class
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

    private void button5_Click(object sender, EventArgs e)
    {
        // Create a context menu
        ContextMenuStrip contextMenu = new ContextMenuStrip();

        // Add "Refresh" to the main context menu
        ToolStripMenuItem refreshItem = new ToolStripMenuItem("Refresh");
        refreshItem.Click += (s, args) => RefreshTools();
        contextMenu.Items.Add(refreshItem);

        // Add "Undo" to the main context menu
        ToolStripMenuItem undoItem = new ToolStripMenuItem("Undo");
        undoItem.Click += (s, args) => UndoChanges();
        contextMenu.Items.Add(undoItem);

        // Add "Always On Top" option with a checkmark
        ToolStripMenuItem alwaysOnTopItem = new ToolStripMenuItem("Always On Top");
        alwaysOnTopItem.CheckOnClick = true;
        alwaysOnTopItem.Checked = this.TopMost; // Set initial state based on the current TopMost property
        alwaysOnTopItem.Click += (s, args) => ToggleAlwaysOnTop(alwaysOnTopItem);
        contextMenu.Items.Add(alwaysOnTopItem);

        // Add "Custom Flags" to the main context menu
        ToolStripMenuItem customFlagsItem = new ToolStripMenuItem("Custom Flags");
        customFlagsItem.Click += (s, args) => CustomFlags();
        contextMenu.Items.Add(customFlagsItem);

        // Add a divider to the main context menu
        contextMenu.Items.Add(new ToolStripSeparator());

        // Create "Open" submenu
        ToolStripMenuItem openMenu = new ToolStripMenuItem("Open");

        // Add "Open Config File" to the "Open" submenu
        ToolStripMenuItem openConfigFileItem = new ToolStripMenuItem("Open Config File");
        openConfigFileItem.Click += (s, args) => OpenConfigFile();
        openMenu.DropDownItems.Add(openConfigFileItem);

        // Add "Open Path" to the "Open" submenu
        ToolStripMenuItem openPathItem = new ToolStripMenuItem("Open Path");
        openPathItem.Click += (s, args) => OpenPathForSelectedTool();
        openMenu.DropDownItems.Add(openPathItem);

        // Add "Open" submenu to the main context menu
        contextMenu.Items.Add(openMenu);

        // Create "Save" submenu
        ToolStripMenuItem saveMenu = new ToolStripMenuItem("Save");

        ToolStripMenuItem saveConfigItem = new ToolStripMenuItem("Save Config");
        saveConfigItem.Click += (s, args) => SaveConfig();
        saveMenu.DropDownItems.Add(saveConfigItem);

        ToolStripMenuItem saveFlagsItem = new ToolStripMenuItem("Save Flags");
        saveFlagsItem.Click += (s, args) => SaveFlags();
        saveMenu.DropDownItems.Add(saveFlagsItem);

        // Add "Save" submenu to the main context menu
        contextMenu.Items.Add(saveMenu);

        // Create "Copy" submenu
        ToolStripMenuItem copyMenu = new ToolStripMenuItem("Copy");

        ToolStripMenuItem copyConfigItem = new ToolStripMenuItem("Copy Config");
        copyConfigItem.Click += (s, args) => CopyConfig();
        copyMenu.DropDownItems.Add(copyConfigItem);

        ToolStripMenuItem copyPathItem = new ToolStripMenuItem("Copy Path");
        copyPathItem.Click += (s, args) => CopyPath();
        copyMenu.DropDownItems.Add(copyPathItem);

        ToolStripMenuItem copyFlagsItem = new ToolStripMenuItem("Copy Flags");
        copyFlagsItem.Click += (s, args) => CopyFlags();
        copyMenu.DropDownItems.Add(copyFlagsItem);

        // Add "Copy" submenu to the main context menu
        contextMenu.Items.Add(copyMenu);

        // Show the context menu at the button's location
        contextMenu.Show(button5, new System.Drawing.Point(0, button5.Height));
    }

    private void ToggleAlwaysOnTop(ToolStripMenuItem item)
    {
        // Toggle the TopMost property of the form
        this.TopMost = !this.TopMost;

        // Update the checkmark state
        item.Checked = this.TopMost;
    }

    private void CustomFlags()
    {
        int selectedIndex = listBox1.SelectedIndex;

        if (selectedIndex >= 0 && selectedIndex < tools.Count)
        {
            ScriptingTool selectedTool = tools[selectedIndex];

            if (selectedTool.Path.EndsWith(".bat", StringComparison.OrdinalIgnoreCase)
                || selectedTool.Path.EndsWith(".py", StringComparison.OrdinalIgnoreCase)
                || selectedTool.Path.EndsWith(".ps1", StringComparison.OrdinalIgnoreCase))
            {
                // Show a dialog to edit custom flags
                string currentFlags = selectedTool.CustomFlags;
                string newFlags = Microsoft.VisualBasic.Interaction.InputBox("Enter Custom Flags:", "Edit Custom Flags", currentFlags);

                // Update the selected tool with the new custom flags
                selectedTool.CustomFlags = newFlags;

                // Save the updated tools list to the config file
                SaveToolsToConfig();
            }
            else
            {
                MessageBox.Show("This action is only compatible with a script.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        else
        {
            MessageBox.Show("Please select a valid tool.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }


    private void RefreshTools()
    {
        // Reload tools from the config file
        LoadToolsFromConfig();
        // Ensure the ListBox reflects the latest changes after the refresh
        UpdateListBox();
    }

    private List<ScriptingTool> deletedTools = new List<ScriptingTool>(); // Change to a list
    private Stack<Tuple<int, string>> renamedToolsHistory = new Stack<Tuple<int, string>>();
    private void UndoChanges()
    {
        if (deletedTools.Count > 0)
        {
            // Restore the last deleted tool
            ScriptingTool restoredTool = deletedTools[deletedTools.Count - 1];
            deletedTools.RemoveAt(deletedTools.Count - 1);

            // Add the restored tool back to the tools list
            tools.Add(restoredTool);

            // Save the updated tools list to the config file
            SaveToolsToConfig();

            // Update the listBox to reflect the changes
            UpdateListBox();
        }
        else if (renamedToolsHistory.Count > 0)
        {
            // Undo the last renaming operation
            Tuple<int, string> lastRename = renamedToolsHistory.Pop();
            int index = lastRename.Item1;
            string oldName = lastRename.Item2;

            // Restore the original name
            tools[index].Name = oldName;

            // Save the updated tools list to the config file
            SaveToolsToConfig();

            // Update the listBox to reflect the changes
            UpdateListBox();
        }
        else
        {
            MessageBox.Show("No changes to undo.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }


    private void SaveConfig()
    {
        // Create a SaveFileDialog
        SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            Title = "Save Config File",
            Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
            DefaultExt = "txt",
            FileName = "config.txt"
        };

        // Show the dialog and get the selected file path
        DialogResult result = saveFileDialog.ShowDialog();

        if (result == DialogResult.OK)
        {
            string selectedFilePath = saveFileDialog.FileName;

            // Save the config file to the selected path
            SaveToolsToConfig(selectedFilePath);

            MessageBox.Show($"Config saved to {selectedFilePath}", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }


    private void SaveToolsToConfig(string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (var tool in tools)
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

            if (selectedTool.Path.EndsWith(".bat", StringComparison.OrdinalIgnoreCase)
                || selectedTool.Path.EndsWith(".py", StringComparison.OrdinalIgnoreCase)
                || selectedTool.Path.EndsWith(".ps1", StringComparison.OrdinalIgnoreCase))
            {
                // Implement the logic to save flags
            }
            else
            {
                MessageBox.Show("This action is only compatible with a script.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void CopyConfig()
    {
        // Copy the content of the config file to the clipboard
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

            if (selectedTool.Path.EndsWith(".bat", StringComparison.OrdinalIgnoreCase)
                || selectedTool.Path.EndsWith(".py", StringComparison.OrdinalIgnoreCase)
                || selectedTool.Path.EndsWith(".ps1", StringComparison.OrdinalIgnoreCase))
            {
                // Copy the path to the clipboard
                Clipboard.SetText(selectedTool.Path);
            }
            else
            {
                MessageBox.Show("This action is only compatible with a script.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void CopyFlags()
    {
        int selectedIndex = listBox1.SelectedIndex;

        if (selectedIndex >= 0 && selectedIndex < tools.Count)
        {
            ScriptingTool selectedTool = tools[selectedIndex];

            if (selectedTool.Path.EndsWith(".bat", StringComparison.OrdinalIgnoreCase)
                || selectedTool.Path.EndsWith(".py", StringComparison.OrdinalIgnoreCase)
                || selectedTool.Path.EndsWith(".ps1", StringComparison.OrdinalIgnoreCase))
            {
                // Implement the logic to copy flags
            }
            else
            {
                MessageBox.Show("This action is only compatible with a script.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }


    private void OpenConfigFile()
    {
        // Open the config file using default system behavior
        System.Diagnostics.Process.Start("notepad.exe", configPath);
    }

    private void OpenPathForSelectedTool()
    {
        int selectedIndex = listBox1.SelectedIndex;

        if (selectedIndex >= 0 && selectedIndex < tools.Count)
        {
            ScriptingTool selectedTool = tools[selectedIndex];

            if (selectedTool.Path.EndsWith(".bat", StringComparison.OrdinalIgnoreCase)
                || selectedTool.Path.EndsWith(".py", StringComparison.OrdinalIgnoreCase)
                || selectedTool.Path.EndsWith(".ps1", StringComparison.OrdinalIgnoreCase))
            {
                // Open the path using default system behavior
                System.Diagnostics.Process.Start("explorer.exe", $"/select, \"{selectedTool.Path}\"");
            }
            else
            {
                MessageBox.Show("This action is only compatible with a script.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }


    //begin link labels
    private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        // Open the donate link in the default web browser
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://www.poof.io/tip/@davidinfosec",
                UseShellExecute = true
            });
        }
        catch (System.ComponentModel.Win32Exception ex)
        {
            // Handle the exception or log the error message
            MessageBox.Show($"Error opening donate link: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void label4_Click(object sender, EventArgs e)
    {

    }

    private Label label2;
    private Button button2;

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

    private Label label4;
}