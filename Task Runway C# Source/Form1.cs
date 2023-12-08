// Task_Runway_x64, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// Form1
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using TaskRunway;

public class Form1 : Form
{
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

    private Button button2;

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
        InitializeContextMenu();
        string relativeIconPath = Path.Combine(Application.StartupPath, "app_icon.ico");
        base.Icon = new Icon(relativeIconPath);
        LoadToolsFromConfig();
        base.MaximizeBox = false;
        base.MinimizeBox = false;
        base.FormBorderStyle = FormBorderStyle.FixedSingle;
        listBox1.KeyDown += listBox1_KeyDown;
    }

    public void RemoveToolFromList(ScriptingTool tool)
    {
        if (tools.Contains(tool))
        {
            tools.Remove(tool);
        }
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        listBox1.ContextMenuStrip = listBoxContextMenu;
        listBox1.MouseDown += listBox1_MouseDown;
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
        ToolStripMenuItem refreshItem = new ToolStripMenuItem("Refresh");
        refreshItem.Click += delegate
        {
            RefreshTools();
        };
        listBoxContextMenu.Items.Add(refreshItem);
        ToolStripMenuItem undoItem = new ToolStripMenuItem("Undo");
        undoItem.Click += delegate
        {
            UndoChanges();
        };
        listBoxContextMenu.Items.Add(undoItem);
        ToolStripMenuItem customFlagsItem = new ToolStripMenuItem("Custom Flags");
        customFlagsItem.Click += delegate
        {
            CustomFlags();
        };
        listBoxContextMenu.Items.Add(customFlagsItem);
        listBoxContextMenu.Items.Add(new ToolStripSeparator());
        ToolStripMenuItem renameItem = new ToolStripMenuItem("Rename");
        renameItem.Click += delegate
        {
            RenameTool();
        };
        listBoxContextMenu.Items.Add(renameItem);
        listBox1.ContextMenuStrip = listBoxContextMenu;
        listBox1.MouseDown += listBox1_MouseDown;
    }

    private void RenameTool()
    {
        if (listBox1.SelectedIndex != -1)
        {
            int selectedIndex = listBox1.SelectedIndex;
            string currentName = tools[selectedIndex].Name;
            string newName = InputBox("Enter a new name:", "Rename Tool", currentName);
            if (!string.IsNullOrEmpty(newName))
            {
                renamedToolsHistory.Push(new Tuple<int, string>(selectedIndex, currentName));
                tools[selectedIndex].Name = newName;
                SaveToolsToConfig();
                UpdateListBox();
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
    }

    private void LaunchSelectedTool()
    {
        int selectedIndex = listBox1.SelectedIndex;
        if (selectedIndex < 0 || selectedIndex >= tools.Count)
        {
            return;
        }
        ScriptingTool selectedTool = tools[selectedIndex];
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
            else if (Uri.TryCreate(selectedTool.Path, UriKind.Absolute, out uri) && uri.Scheme.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = selectedTool.Path,
                    UseShellExecute = true
                });
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
        ToolStripMenuItem addExecutableToolItem = new ToolStripMenuItem("Add Program (.exe)");
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
        contextMenu.Items.Add(addWebsiteToolItem);
        contextMenu.Items.Add(addScriptToolItem);
        contextMenu.Items.Add(addExecutableToolItem);
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
                        Filter = "Executable files (*.exe)|*.exe",
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
        }
    }

    public void AddToolToList(ScriptingTool tool)
    {
        tools.Add(tool);
        SaveToolsToConfig();
        UpdateListBox();
    }

    private void SaveToolsToConfig()
    {
        using StreamWriter writer = new StreamWriter(configPath);
        foreach (ScriptingTool tool in tools)
        {
            writer.WriteLine("[" + tool.Name + "]");
            writer.WriteLine("path=" + tool.Path);
            writer.WriteLine("custom_flags=" + tool.CustomFlags);
            writer.WriteLine();
        }
    }

    private void LoadToolsFromConfig()
    {
        if (!File.Exists(configPath))
        {
            return;
        }
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
        UpdateListBox();
    }

    private void UpdateListBox()
    {
        listBox1.Items.Clear();
        foreach (ScriptingTool tool in tools)
        {
            listBox1.Items.Add(tool.Name);
        }
    }

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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
        this.button1 = new System.Windows.Forms.Button();
        this.button3 = new System.Windows.Forms.Button();
        this.listBox1 = new System.Windows.Forms.ListBox();
        this.button5 = new System.Windows.Forms.Button();
        this.label1 = new System.Windows.Forms.Label();
        this.button6 = new System.Windows.Forms.Button();
        this.linkLabel1 = new System.Windows.Forms.LinkLabel();
        this.linkLabel2 = new System.Windows.Forms.LinkLabel();
        this.label2 = new System.Windows.Forms.Label();
        this.label3 = new System.Windows.Forms.Label();
        this.label4 = new System.Windows.Forms.Label();
        base.SuspendLayout();
        this.button1.Location = new System.Drawing.Point(12, 241);
        this.button1.Name = "button1";
        this.button1.Size = new System.Drawing.Size(215, 28);
        this.button1.TabIndex = 0;
        this.button1.Text = "Launch Tool";
        this.button1.UseVisualStyleBackColor = true;
        this.button1.Click += new System.EventHandler(button1_Click);
        this.button3.Location = new System.Drawing.Point(123, 195);
        this.button3.Name = "button3";
        this.button3.Size = new System.Drawing.Size(49, 24);
        this.button3.TabIndex = 2;
        this.button3.Text = "+";
        this.button3.UseVisualStyleBackColor = true;
        this.button3.Click += new System.EventHandler(button3_Click);
        this.listBox1.FormattingEnabled = true;
        this.listBox1.ItemHeight = 15;
        this.listBox1.Location = new System.Drawing.Point(12, 80);
        this.listBox1.Name = "listBox1";
        this.listBox1.Size = new System.Drawing.Size(215, 109);
        this.listBox1.TabIndex = 4;
        this.listBox1.SelectedIndexChanged += new System.EventHandler(listBox1_SelectedIndexChanged);
        this.button5.Location = new System.Drawing.Point(12, 195);
        this.button5.Name = "button5";
        this.button5.Size = new System.Drawing.Size(79, 23);
        this.button5.TabIndex = 6;
        this.button5.Text = "Settings";
        this.button5.UseVisualStyleBackColor = true;
        this.button5.Click += new System.EventHandler(button5_Click);
        this.label1.AutoSize = true;
        this.label1.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
        this.label1.Location = new System.Drawing.Point(12, 62);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(51, 15);
        this.label1.TabIndex = 7;
        this.label1.Text = "Toolbox";
        this.button6.Location = new System.Drawing.Point(178, 195);
        this.button6.Name = "button6";
        this.button6.Size = new System.Drawing.Size(49, 24);
        this.button6.TabIndex = 8;
        this.button6.Text = "-";
        this.button6.UseVisualStyleBackColor = true;
        this.button6.Click += new System.EventHandler(button6_Click);
        this.linkLabel1.AutoSize = true;
        this.linkLabel1.Location = new System.Drawing.Point(141, 283);
        this.linkLabel1.Name = "linkLabel1";
        this.linkLabel1.Size = new System.Drawing.Size(31, 15);
        this.linkLabel1.TabIndex = 9;
        this.linkLabel1.TabStop = true;
        this.linkLabel1.Text = "Blog";
        this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel1_LinkClicked);
        this.linkLabel2.AutoSize = true;
        this.linkLabel2.Location = new System.Drawing.Point(182, 283);
        this.linkLabel2.Name = "linkLabel2";
        this.linkLabel2.Size = new System.Drawing.Size(45, 15);
        this.linkLabel2.TabIndex = 10;
        this.linkLabel2.TabStop = true;
        this.linkLabel2.Text = "Donate";
        this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel2_LinkClicked);
        this.label2.AutoSize = true;
        this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15f, System.Drawing.FontStyle.Bold);
        this.label2.Location = new System.Drawing.Point(48, 9);
        this.label2.Name = "label2";
        this.label2.Size = new System.Drawing.Size(142, 25);
        this.label2.TabIndex = 14;
        this.label2.Text = "Task Runway";
        this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.label2.Click += new System.EventHandler(label2_Click_1);
        this.label3.AutoSize = true;
        this.label3.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
        this.label3.Location = new System.Drawing.Point(12, 283);
        this.label3.Name = "label3";
        this.label3.Size = new System.Drawing.Size(96, 15);
        this.label3.TabIndex = 15;
        this.label3.Text = "by DavidInfosec";
        this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.label3.Click += new System.EventHandler(label3_Click_1);
        this.label4.Font = new System.Drawing.Font("Microsoft JhengHei", 7f, System.Drawing.FontStyle.Bold);
        this.label4.Location = new System.Drawing.Point(12, 34);
        this.label4.Margin = new System.Windows.Forms.Padding(10, 0, 3, 0);
        this.label4.Name = "label4";
        this.label4.Size = new System.Drawing.Size(215, 18);
        this.label4.TabIndex = 16;
        this.label4.Text = "Take flight into your favorite tools.";
        this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.label4.Click += new System.EventHandler(label4_Click_1);
        base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 15f);
        base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.AutoSize = true;
        base.ClientSize = new System.Drawing.Size(239, 311);
        base.Controls.Add(this.label4);
        base.Controls.Add(this.label3);
        base.Controls.Add(this.label2);
        base.Controls.Add(this.linkLabel2);
        base.Controls.Add(this.linkLabel1);
        base.Controls.Add(this.button6);
        base.Controls.Add(this.label1);
        base.Controls.Add(this.button5);
        base.Controls.Add(this.listBox1);
        base.Controls.Add(this.button3);
        base.Controls.Add(this.button1);
        base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
        base.Name = "Form1";
        this.Text = "Task Runway";
        base.Load += new System.EventHandler(Form1_Load);
        base.ResumeLayout(false);
        base.PerformLayout();
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
        contextMenu.Show(button5, new Point(0, button5.Height));
    }


    private void ToggleAlwaysOnTop(ToolStripMenuItem item)
    {
        base.TopMost = !base.TopMost;
        item.Checked = base.TopMost;
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
            SaveToolsToConfig();
            UpdateListBox();
        }
        else if (renamedToolsHistory.Count > 0)
        {
            Tuple<int, string> lastRename = renamedToolsHistory.Pop();
            int index = lastRename.Item1;
            string oldName = lastRename.Item2;
            tools[index].Name = oldName;
            SaveToolsToConfig();
            UpdateListBox();
        }
        else
        {
            MessageBox.Show("No changes to undo.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
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
        using StreamWriter writer = new StreamWriter(filePath);
        foreach (ScriptingTool tool in tools)
        {
            writer.WriteLine("[" + tool.Name + "]");
            writer.WriteLine("path=" + tool.Path);
            writer.WriteLine("custom_flags=" + tool.CustomFlags);
            writer.WriteLine();
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
}
