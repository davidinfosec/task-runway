# Task Runway Documentation

![mspaint_MPd4EWVVOG](https://github.com/davidinfosec/task-runway/assets/87215831/1d7fe43d-8547-44e6-a838-a6d8c5b3f8d0)

Task Runway is an open-source tool designed as a central location to streamline access to your preferred scripting tools. It offers both a Python CLI/GUI, and a more preferred C# version, providing flexibility for different user preferences.

## C# GUI - x64 Executable (Recommended)

The C# version of Task Runway is very light-weight executable, only 250KB, offering a swift and enjoyable user experience. The source code is also provided for customization.

Task Runway supports scripts, executables, and websites. Simply add your desired tool to the list, and it will populate in a list box for easy access.

![firefox_B8n9zzLJbY](https://github.com/davidinfosec/task-runway/assets/87215831/493e1f10-6eaa-45e6-a2e8-7546acc50df6)


Task Runway is tiny, but powerful, allowing you to add custom flags to scripts where you need them. For example, my [Domain Name Ninja tool](https://github.com/davidinfosec/Domain-Name-Ninja), which uses custom flags in the python command, I can still specify those parameters very simply like so:

![firefox_QSEYsr2Y0Y](https://github.com/davidinfosec/task-runway/assets/87215831/b287dc70-ffd7-4560-a080-a1a7ac11d49a)


This essentially will look like:
```
python <script.py> <flags>
```

For housekeeping, you can also:
- open the path to any tool you select
- save any flag to its own file
- copy the config.txt to clipboard
- Always-on-top so you can let Task Runway float with you all day
- ..and much more.

No more memorizing file paths when you're opening your programs. Easily access your tools when you need them. Task Runway helps you take flight so you can stay productive.

#### Get Started:

You will need to install .NET framework version 8.0.100, available here: [.NET framework dependency](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-8.0.100-windows-x64-installer).

3-Step Download:
1. Download the latest version of Task Runway (x64) here: [Latest Version of Task Runway](https://github.com/davidinfosec/task-runway/raw/main/Task_Runway_x64/Task_Runway_x64.zip)
2. Extract the folder to any directory.
3. Run the executable

Note:
- keep the app_icon.ico in the same path as the executable
- keep the config.txt in the same path as the executable

Thank you for using Task Runway. I hope it helps you as much as it helped me. 
If you enjoy using it, consider [donating](https://www.poof.io/tip/@davidinfosec) or [checking out my ReportName.com project](https://www.reportname.com).

---


## Python CLI

Download: [CLI Release](https://github.com/davidinfosec/task-runway/blob/main/taskrunway_cli.py) and click the download button in the top right.

### Adding a Tool

1. Type `add` when prompted for action.
2. Enter the tool name.
3. Enter the tool path.

### Removing a Tool

1. Type `remove` when prompted for action.
2. Enter the name of the tool to remove.

### Changing Tool Path

1. Type `settings` when prompted for action.
2. Choose the tool by entering its number.
3. Enter the new path for the selected tool.

### Launching a Tool

1. Enter the number corresponding to the tool.
2. The screen will be cleared, and the selected tool will be launched in a new command prompt window.

### Clearing the Screen

Type `cls` when prompted for action.

### Exiting Task Runway

Type `exit` when prompted for action.

### Displaying Help

1. Type `help` when prompted for action.
2. The available commands and their descriptions will be shown.

## Example Usage

```bash
python taskrunway_cli.py
```



---

## Python GUI

Download: [Python GUI Release](https://github.com/davidinfosec/task-runway/blob/main/taskrunway_gui.py) and click the download button in the top right.

Install libraries:
```
pip install tkinter
```

```
pip install ttkthemes
```

Run the program:
```bash
python taskrunway_gui.py
```



Note: The C# version is less buggy, and is lighter-weight, but this is here for those who wish to have it, or even improve it!

Feel free to reach out, report issues, or contribute on [GitHub](https://github.com/davidinfosec/task-runway).

## License

Task Runway is open-source software released under the [MIT License](https://opensource.org/licenses/MIT). See the `LICENSE` file for more details.

## Support

If you found this useful, consider donating towards my latest projects. [Donate](https://www.poof.io/tip/@davidinfosec)

You can also check out my blog here at [DavidInfosec.com](https://davidinfosec.com)

