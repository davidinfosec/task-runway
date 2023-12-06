import os
import subprocess
import configparser
import tkinter as tk
from tkinter import simpledialog, messagebox, filedialog
from ttkthemes import ThemedStyle


class ScriptingTool:
    def __init__(self, name: str, path: str, custom_flags: str = ""):
        self.name = name
        self.path = path
        self.custom_flags = custom_flags


class TaskRunwayGUI:
    BUTTON_HEIGHT = 1
    BUTTON_WIDTH = 12

    def __init__(self, config_path: str):
        self.config_path = config_path
        self.tools = self.load_tools_from_config()
        self.selected_tool_index = None
        self.history = []

        self.root = tk.Tk()
        self.root.title("Task Runway GUI")

        # Set the window size explicitly
        window_width = 400
        window_height = 300
        self.root.geometry(f"{window_width}x{window_height}")

        # Set the minimum size to prevent the window from being resized to very small dimensions
        self.root.minsize(window_width, window_height)

        self.root.resizable(False, False)  # Make the window non-resizable

        # Set the window icon with an absolute path
        icon_path = 'E:/task-runway/app_icon.ico'
        self.root.iconbitmap(default=icon_path.replace('\\', '/'))

        style = ThemedStyle(self.root)
        style.set_theme("plastik")

        self.create_widgets()

    def create_widgets(self):
        self.create_header_label()
        self.create_subheading_label()
        self.create_listbox()
        self.create_button_frame()
        self.create_donate_and_blog_links()
        self.update_listbox()

    def create_header_label(self):
        header_label = tk.Label(self.root, text="Task Runway GUI", font=("Helvetica", 12))
        header_label.pack(pady=5)

    def create_subheading_label(self):
        subheading_label = tk.Label(self.root, text="Your runway for takeoff into your favorite tools.",
                                    font=("Helvetica", 10, "italic"))
        subheading_label.pack(pady=5)

    def create_listbox(self):
        self.listbox = tk.Listbox(self.root, selectmode=tk.SINGLE, font=("Helvetica", 8))
        self.listbox.bind("<<ListboxSelect>>", self.on_listbox_select)
        self.listbox.pack(side=tk.LEFT, padx=5, pady=5)

    def create_button_frame(self):
        button_frame = tk.Frame(self.root)
        button_frame.pack(side=tk.RIGHT, padx=5)

        tk.Button(button_frame, text="Launch", command=self.launch_selected_tool,
                  state=tk.DISABLED, height=self.BUTTON_HEIGHT, width=self.BUTTON_WIDTH).pack(pady=5)
        tk.Button(button_frame, text="Add", command=self.add_tool,
                  height=self.BUTTON_HEIGHT, width=self.BUTTON_WIDTH).pack(pady=5)
        tk.Button(button_frame, text="Remove", command=self.remove_tool,
                  height=self.BUTTON_HEIGHT, width=self.BUTTON_WIDTH).pack(pady=5)
        tk.Button(button_frame, text="Undo", command=self.undo_last_action,
                  height=self.BUTTON_HEIGHT, width=self.BUTTON_WIDTH).pack(pady=5)
        tk.Button(button_frame, text="Settings", command=self.open_settings,
                  height=self.BUTTON_HEIGHT, width=self.BUTTON_WIDTH).pack(pady=5)

    def create_donate_and_blog_links(self):
        donate_label = tk.Label(self.root, text="Donate", fg="blue", cursor="hand2", font=("Helvetica", 8))
        donate_label.pack(side=tk.BOTTOM, pady=5)
        donate_label.bind("<Button-1>", lambda e: self.open_url("https://www.poof.io/tip/@davidinfosec"))

        blog_label = tk.Label(self.root, text="Blog", fg="blue", cursor="hand2", font=("Helvetica", 8))
        blog_label.pack(side=tk.BOTTOM, pady=5)
        blog_label.bind("<Button-1>", lambda e: self.open_url("https://www.davidinfosec.com"))

    def load_tools_from_config(self):
        config = configparser.ConfigParser()
        if os.path.exists(self.config_path):
            config.read(self.config_path)
            tools = []
            for section in config.sections():
                name = section
                path = config.get(section, 'path')
                custom_flags = config.get(section, 'custom_flags', fallback="")
                tools.append(ScriptingTool(name, path, custom_flags))
            return tools
        else:
            return []

    def save_tools_to_config(self):
        config = configparser.ConfigParser()
        for tool in self.tools:
            config[tool.name] = {'path': tool.path, 'custom_flags': tool.custom_flags}

        with open(self.config_path, 'w') as config_file:
            config.write(config_file)

    def update_listbox(self):
        self.listbox.delete(0, tk.END)
        for tool in self.tools:
            self.listbox.insert(tk.END, tool.name)

    def on_listbox_select(self, event):
        selected_index = self.listbox.curselection()
        self.selected_tool_index = selected_index[0] if selected_index else None
        self.update_launch_button_state()

    def update_launch_button_state(self):
        launch_button = self.root.nametowidget(".!frame.!button")
        launch_button["state"] = tk.NORMAL if self.selected_tool_index is not None else tk.DISABLED

    def launch_selected_tool(self):
        if self.selected_tool_index is not None:
            tool = self.tools[self.selected_tool_index]
            script_directory = os.path.dirname(tool.path)
            os.chdir(script_directory)
            command = f'start cmd /k python "{tool.path}" {tool.custom_flags}'
            subprocess.run(command, shell=True)
            os.chdir(os.path.dirname(os.path.abspath(__file__)))
        else:
            messagebox.showinfo("Error", "Please select a tool to launch.")

    def add_tool(self):
        name = simpledialog.askstring("Add Tool", "Enter the tool name:")
        path = filedialog.askopenfilename(title="Select Tool Path", filetypes=[("Python files", "*.py")])
        if name and path:
            custom_flags = ""
            tool = ScriptingTool(name, path, custom_flags)
            self.tools.append(tool)
            self.selected_tool_index = None
            self.history.append(("add", tool))
            self.save_tools_to_config()
            self.update_listbox()

    def remove_tool(self):
        selected_index = self.listbox.curselection()
        if selected_index:
            tool = self.tools[selected_index[0]]
            self.history.append(("remove", tool))
            self.tools.remove(tool)
            self.selected_tool_index = None
            self.save_tools_to_config()
            self.update_listbox()
        else:
            messagebox.showinfo("Error", "Please select a tool to remove.")

    def undo_last_action(self):
        if self.history:
            action, tool = self.history.pop()
            if action == "add":
                self.tools.remove(tool)
            elif action == "remove":
                self.tools.append(tool)
            self.save_tools_to_config()
            self.update_listbox()

    def open_settings(self):
        if self.selected_tool_index is not None:
            tool = self.tools[self.selected_tool_index]
            submenu = tk.Menu(self.root, tearoff=0)
            submenu.add_command(label="Edit Path", command=lambda: self.edit_path(tool))
            submenu.add_command(label="Edit Custom Flags", command=lambda: self.edit_flags(tool))
            submenu.post(self.root.winfo_x() + self.root.winfo_width() - 80, self.root.winfo_y() + 150)

    def edit_path(self, tool):
        new_path = filedialog.askopenfilename(title=f"Select New Path for {tool.name}",
                                              initialdir=os.path.dirname(tool.path),
                                              filetypes=[("Python files", "*.py")])
        if new_path:
            tool.path = new_path
            self.save_tools_to_config()
            self.update_listbox()
            self.selected_tool_index = None

    def edit_flags(self, tool):
        custom_flags_editor = CustomFlagsEditor(self.root, tool.custom_flags)
        self.root.wait_window(custom_flags_editor.top)
        if custom_flags_editor.result:
            tool.custom_flags = custom_flags_editor.result
            self.save_tools_to_config()

    def open_url(self, url):
        import webbrowser
        webbrowser.open(url)


class CustomFlagsEditor:
    def __init__(self, parent, initial_flags):
        self.top = tk.Toplevel(parent)
        self.top.title("Edit Custom Flags")
        self.result = None
        self.custom_flags_entry = tk.Entry(self.top, width=30, font=("Helvetica", 8))
        self.custom_flags_entry.insert(tk.END, initial_flags)
        self.custom_flags_entry.pack(pady=5)
        self.ok_button = tk.Button(self.top, text="OK", command=self.ok, height=1, width=8)
        self.ok_button.pack(pady=5)

    def ok(self):
        self.result = self.custom_flags_entry.get()
        self.top.destroy()


# Specify the path for the configuration file
config_path = 'taskrunway_config.ini'

# Create an instance of TaskRunwayGUI and run the GUI
taskrunway_gui = TaskRunwayGUI(config_path)
taskrunway_gui.root.mainloop()
