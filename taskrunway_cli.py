import os
import subprocess
import configparser

class ScriptingTool:
    def __init__(self, name: str, path: str):
        self.name = name
        self.path = path

class TaskRunway:
    def __init__(self, config_path: str):
        self.config_path = config_path
        self.tools = self.load_tools_from_config()

    def load_tools_from_config(self):
        config = configparser.ConfigParser()
        if os.path.exists(self.config_path):
            config.read(self.config_path)
            tools = []
            for section in config.sections():
                name = section
                path = config.get(section, 'path')
                tools.append(ScriptingTool(name, path))
            return tools
        else:
            return []

    def save_tools_to_config(self):
        config = configparser.ConfigParser()
        for tool in self.tools:
            config[tool.name] = {'path': tool.path}

        with open(self.config_path, 'w') as config_file:
            config.write(config_file)

    def launch_tool(self, tool_index: int):
        if 1 <= tool_index <= len(self.tools):
            tool = self.tools[tool_index - 1]

            # Set the working directory to the directory of the script
            script_directory = os.path.dirname(tool.path)
            os.chdir(script_directory)

            command = f'start cmd /k python "{tool.path}"'
            subprocess.run(command, shell=True)

            # Restore the original working directory
            os.chdir(os.path.dirname(os.path.abspath(__file__)))
        else:
            print("Invalid tool index. Please select a valid tool.")

    def add_tool(self, name: str, path: str):
        tool = ScriptingTool(name, path)
        self.tools.append(tool)
        self.save_tools_to_config()

    def remove_tool(self, name: str):
        tool_to_remove = next((tool for tool in self.tools if tool.name == name), None)
        if tool_to_remove:
            self.tools.remove(tool_to_remove)
            self.save_tools_to_config()
        else:
            print(f"Tool '{name}' not found.")

    def open_settings(self):
        print("Settings:")
        print("1. Change tool path")
        option = input("Enter the number corresponding to the setting or 'back' to return to the main menu: ")

        if option == '1':
            self.change_tool_path()
        elif option.lower() == 'back':
            pass
        else:
            print("Invalid option. Please enter a valid setting number.")

    def change_tool_path(self):
        print("Current Tools:")
        for i, tool in enumerate(self.tools, start=1):
            print(f"{i}. {tool.name}")

        tool_index = input("Enter the number corresponding to the tool to change its path or 'back' to return: ")

        if tool_index.lower() == 'back':
            return

        tool_index = int(tool_index)
        if 1 <= tool_index <= len(self.tools):
            tool = self.tools[tool_index - 1]
            new_path = input(f"Enter the new path for {tool.name} or 'back' to return: ")

            if new_path.lower() == 'back':
                return

            tool.path = new_path
            self.save_tools_to_config()
            print(f"Tool path for {tool.name} changed successfully.")
        else:
            print("Invalid tool index. Please select a valid tool.")

    def display_program_description(self):
        print(r'''
        



___       __           __                          
 |   /\  /__` |__/    |__) |  | |\ | |  |  /\  \ / 
 |  /~~\ .__/ |  \    |  \ \__/ | \| |/\| /~~\  |  
                                                   


                                     

                ''')
        print("Task Runway - Your runway for takeoff into all your favorite command-line tools.")
        print("Type 'help' for help.")
        print()

    def display_help(self):
        print("Available commands:")
        print("  - 'add': Add a new tool")
        print("  - 'remove': Remove a tool")
        print("  - 'settings': Open settings")
        print("  - 'cls': Clear the screen")
        print("  - 'exit': Exit the program")
        print("  - 'help': Display help information")

def clear_screen():
    subprocess.run('cls' if os.name == 'nt' else 'clear', shell=True)

# Example usage:

# Specify the path for the configuration file
config_path = 'taskrunway_config.ini'

# Create an instance of TaskRunway
taskrunway = TaskRunway(config_path)

# Display the program description and help information
taskrunway.display_program_description()
taskrunway.display_help()

while True:
    # Clear the screen and display the program description
    clear_screen()
    taskrunway.display_program_description()

    # Display numbered list of tools
    print("Favorite Tools:")
    for i, tool in enumerate(taskrunway.tools, start=1):
        print(f"{i}. {tool.name}")

    # Get user input for tool selection or action
    action = input("Enter the number corresponding to the tool, 'add', 'remove', 'settings', 'cls', 'exit', or 'help': ")

    if action == 'exit':
        break
    elif action == 'cls':
        clear_screen()
    elif action == 'help':
        taskrunway.display_help()
        input("Press Enter to continue...")
    elif action.isdigit():
        tool_index = int(action)
        taskrunway.launch_tool(tool_index)
    elif action.lower() == 'add':
        name = input("Enter the tool name: ")
        path = input("Enter the tool path: ")
        taskrunway.add_tool(name, path)
    elif action.lower() == 'remove':
        name = input("Enter the tool name to remove: ")
        taskrunway.remove_tool(name)
    elif action.lower() == 'settings':
        taskrunway.open_settings()
    else:
        print("Invalid input. Please enter a valid tool number, action, or 'cls'.")
