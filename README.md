# InvisiLaunch

A Windows utility for launching applications and PowerShell scripts invisibly in the background.

## Description

InvisiLaunch is a simple command-line tool that allows you to launch applications and PowerShell scripts without showing a console window or displaying output. It's particularly useful for:

- Running background tasks and scripts
- Launching applications from batch files or other automation tools
- Executing PowerShell scripts without console windows
- Creating silent launchers for various applications

## Features

- **Silent Execution**: Launches applications without showing console windows
- **PowerShell Support**: Automatically detects and executes `.ps1` files
- **Execution Policy Bypass**: Runs PowerShell scripts regardless of execution policy settings
- **Argument Passing**: Supports passing command-line arguments to launched applications
- **Process Priority Control**: Set the OS process priority (ABOVE_NORMAL, NORMAL, BELOW_NORMAL, LOW) for launched programs
- **Bypass Mode**: `/bypass` parameter allows launching apps directly with shell commands, bypassing all custom logic
- **Error Handling Control**: Configure error behavior with `/onError` parameter (message or ignore)
- **Exit Codes**: Returns exit code 0 for success, 8 for errors
- **Help System**: Built-in help command (`/help`) for quick reference

## Usage

### Basic Syntax
```
InvisiLaunch.exe [/onError:<action>] [/priority:<value>] <program> [arguments...]
InvisiLaunch.exe [/onError:<action>] /bypass <command>
```

### Command-Line Parameters

Parameters must be specified before the program name:

- **`/onError:<action>`** - Control error handling behavior
  - Values: `message`, `ignore`
  - Default: `message`
  - `message`: Display an error message box (default behavior)
  - `ignore`: Silently ignore errors and return exit code 8
  - Example: `/onError:ignore`

- **`/priority:<value>`** - Set the process priority for the launched program
  - Values: `ABOVE_NORMAL`, `NORMAL`, `BELOW_NORMAL`, `LOW`
  - Default: `NORMAL`
  - Example: `/priority:ABOVE_NORMAL`

- **`/bypass`** - Bypass all logic and launch the specified app with a shell command
  - Executes the command as-is without any processing (no PowerShell detection, no priority setting, etc.)
  - Example: `/bypass notepad.exe C:\temp\file.txt`

- **`/help`** - Display help information
  - Example: `InvisiLaunch.exe /help`

### Examples

**Launch a regular executable:**
```
InvisiLaunch.exe notepad.exe C:\temp\file.txt
```

**Launch with high priority:**
```
InvisiLaunch.exe /priority:ABOVE_NORMAL notepad.exe C:\temp\file.txt
```

**Launch a PowerShell script:**
```
InvisiLaunch.exe C:\scripts\myscript.ps1 -param1 value1
```

**Launch a PowerShell script with low priority:**
```
InvisiLaunch.exe /priority:BELOW_NORMAL C:\scripts\backup.ps1 -source C:\data -destination D:\backup
```

**Launch a batch file with low priority:**
```
InvisiLaunch.exe /priority:LOW C:\scripts\cleanup.bat
```

**Display help:**
```
InvisiLaunch.exe /help
```

**Bypass all logic and launch with shell command:**
```
InvisiLaunch.exe /bypass notepad.exe C:\temp\file.txt
```

**Bypass mode with shell commands:**
```
InvisiLaunch.exe /bypass dir C:\temp
```

**Silent error handling (no message box):**
```
InvisiLaunch.exe /onError:ignore notepad.exe C:\temp\file.txt
```

**Combine parameters:**
```
InvisiLaunch.exe /onError:ignore /priority:ABOVE_NORMAL C:\scripts\backup.ps1
```

## Requirements

- Windows operating system
- .NET 6.0 Runtime (for running the compiled executable)
- PowerShell (for executing `.ps1` files)

## Building

This project uses .NET 6.0 and can be built using:

```bash
dotnet build
```

Or for release:

```bash
dotnet build -c Release
```

## How It Works

1. **Application Detection**: InvisiLaunch checks the file extension of the target program
2. **PowerShell Scripts**: If the file ends with `.ps1`, it launches `powershell.exe` with the `-ExecutionPolicy Bypass -File` parameters
3. **Regular Applications**: For other files, it launches them directly using `Process.Start()`
4. **Silent Execution**: All output is redirected and discarded, and no console window is shown

## Error Handling

Error handling behavior can be controlled using the `/onError:<action>` parameter:

- **`message`** (default): Displays a user-friendly error message in a message box
- **`ignore`**: Silently ignores errors without displaying a message box

### Exit Codes

- **0**: Success - the application was launched successfully
- **8**: Error - an exception occurred or validation failed

The exit code is always 8 on errors, regardless of the `/onError` setting. Use `/onError:ignore` to suppress error message boxes in automated scenarios.

## License

This project is open source and available under the MIT License.
