# Task Runway Documentation

![explorer_5q74JXyN6j](https://github.com/davidinfosec/task-runway/assets/87215831/106245a9-5313-4a98-8fd0-b60e354d6140)

Task Runway is an **open-source application** designed as a central location to take flight into your favorite tools. It is developed in C#, with alternative versions in Python available.

As efforts to support the development and support of Task Runway, a [$10 donation is strongly encouraged](https://www.davidinfosec.com/donate) if you find value with this application. **With your contribution you will receive an exclusive Supporter's License Key.** Please take your time to evaluate before donating. We want to keep things light-weight and open source.

# Task Runway 1.0.4.4 Patchnotes
- More inuitive experience between switching from the search bar and listbox items. 
- Patched bugs that were present in CTRL + C // CTRL + SHIFT + X hotkeys.
- Added Ctrl + K back for searchbar focus. Escape key to cancel search now works globally.
- New context menu for Edit Path; fixed to appear on top, even if Task Runway is set for Always-On-Top.
- Selection bug for rename/edit path fixed and will now properly deselect other items.

Click [here](https://taskrunway.com/install) to download the latest version!


### Latest Versions Summary:

### Task Runway 1.0.3 Release and Beyond, Featuring: Automatic Updates

Bug changes will be coming soon in v1.0.3 to address some of the issues mentioned at https://taskrunway.com/issues

## Task Runway v1.0.2/v1.0.3 Release: Featuring: Automatic Updates

This update features one major addition, automatic updating. The reason I wanted to get this release out quickly is because it will allow me to patch bugs while understanding that users may not want to have to keep updating their installation. 

Automatic updates are handled with an UpdaterApp.exe, which will periodically download from this repository for updates.

Version 1.0.3 is the automatic update (1.0.2) with some bug fixes. The reason I have separated these into separate versions is to test the auto-update feature more thoroughly.


# About Task Runway

## C# GUI - x64 Executable (Recommended)

The C# version of Task Runway is a very light-weight executable, less than 1MB, offering a swift and enjoyable user experience. The source code is also provided for customization.

Task Runway supports scripts, executables, and websites. Simply add your desired tool to the list, and it will populate in a list box for easy access.

![transparent](https://github.com/davidinfosec/task-runway/assets/87215831/51eb210b-86ae-439c-999e-8629120a82b9)




Task Runway is tiny, but powerful.

## Features

- **Always-On-Top, to let Task Runway stay accessible for your daily workflow.**, let Task Runway fly with you while you tab between your applications.

- **Open Config, Save Config, directly modify and customize Task Runway to work for you.**, share your config.txt file with co-workers and simplify complicated processes with standardized configurations. 

- **Website support, for all your favorite web applications, launch right into your web browser.**, add specific URLs like your Gmail or Calendar, so you can access it quickly throughout your busy week.

- **Batch file / Server support**, imagine you have a Minecraft server, and you want to spin it up for friends every now and again, you can easily launch the batch file when needed.

- **Custom flags**, add custom flags to scripts where you need them, reliable for more complex tasks.
  - For example, my [Domain Name Ninja tool](https://github.com/davidinfosec/Domain-Name-Ninja), which uses custom flags in the python command, I can still specify those parameters very simply like so:

  ![firefox_QSEYsr2Y0Y](https://github.com/davidinfosec/task-runway/assets/87215831/b287dc70-ffd7-4560-a080-a1a7ac11d49a)


  - This is the equivalent if you ran the following command in your terminal:
    ```
    python <script.py> <flags>
    ```

**Download Explorer** (Hand-picked tools ready for you to download), featuring tools like:
- [Sp.py](https://github.com/davidinfosec/sp.py)
- [Domain Name Ninja](https://github.com/davidinfosec/domain-name-ninja)
- [Dropfilter CLI (by Crock)](https://github.com/crock/dropfilter-cli)
- [ReportName.com](https://www.reportname.com)
- [WHOIS Domain Info](https://github.com/davidinfosec/whois-domain-info/)
- [Easy-Template](https://github.com/davidinfosec/easy-template)
- [Geo IP API Tool](https://github.com/davidinfosec/ip-tool)
- [DownloadClip](https://github.com/davidinfosec/downloadclip)



No more memorizing file paths when you're opening your programs. Easily access your tools when you need them. Task Runway helps you take flight so you can stay productive.

## Download:

Easy Download:
1. Download the latest version of Task Runway (x64) here: [Latest Version of Task Runway](https://www.taskrunway.com/install)
2. Run the installer
3. Download the [.NET framework dependency](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-8.0.100-windows-x64-installer) after the install, if necessary.

Note (the installer will do these for you, but good to know):
- keep the config.txt in the same path as the executable

Thank you for using Task Runway. I hope it helps you as much as it helped me. 
If you enjoy using it, consider [donating](https://www.davidinfosec.com/donate) or [checking out my ReportName.com project](https://www.reportname.com).

## Uninstall
1. Backup your config.txt file (with the save config.txt feature)

![explorer_pQTaBSlgQr](https://github.com/davidinfosec/task-runway/assets/87215831/433b9627-ea09-4641-bef6-ceb9028bc8bb)

2. Press the Windows Key, type 'Add or Remove Programs', and search for TaskRunwaySetup.

3. Back up your Downloaded Tools in the program folder, if you used the Download Explorer and want to save anything.

4. Press the Uninstall button


---


## Python CLI

Download: [CLI Release](https://github.com/davidinfosec/task-runway/releases/download/v1.0.0/taskrunway_cli.py)

Run by opening a command line or terminal window, and type:

```bash
python taskrunway_cli.py
```

![firefox_Jwa8E7EOAT](https://github.com/davidinfosec/task-runway/assets/87215831/5a27992b-ae92-47ad-a3c7-b586e0f509e9)


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


---

## Python GUI

Download: [Python GUI Release](https://github.com/davidinfosec/task-runway/releases/download/v1.0.0/taskrunway_gui.py) and click the download button in the top right.

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

Feel free to reach out, report issues here: https://github.com/davidinfosec/task-runway/issues

## License

Task Runway is open-source software released under the [MIT License](https://opensource.org/licenses/MIT). See the `LICENSE` file for more details.

## Support

If you found this useful, consider donating towards my latest projects. [Donate](https://www.davidinfosec.com/donate)

You can also check out my blog here at [DavidInfosec.com](https://davidinfosec.com)

