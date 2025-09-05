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
- **Error Handling**: Provides user-friendly error messages for failed launches

## Usage

### Basic Syntax
```
InvisiLaunch.exe <program> [arguments...]
```

### Examples

**Launch a regular executable:**
```
InvisiLaunch.exe notepad.exe C:\temp\file.txt
```

**Launch a PowerShell script:**
```
InvisiLaunch.exe C:\scripts\myscript.ps1 -param1 value1
```

**Launch a PowerShell script with multiple arguments:**
```
InvisiLaunch.exe C:\scripts\backup.ps1 -source C:\data -destination D:\backup
```

**Launch a batch file:**
```
InvisiLaunch.exe C:\scripts\cleanup.bat
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

If a launch fails, InvisiLaunch will display a user-friendly error message in a message box, making it easy to identify and resolve issues.

## License

This project is open source and available under the MIT License.
