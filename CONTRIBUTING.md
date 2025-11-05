# Contributing to InvisiLaunch

Thank you for your interest in contributing to InvisiLaunch! This document provides guidelines and instructions for contributing to the project.

## Code of Conduct

By participating in this project, you agree to maintain a respectful and inclusive environment for all contributors.

## Getting Started

### Prerequisites

- Windows operating system
- .NET 6.0 SDK or later
- Visual Studio 2022 (recommended) or Visual Studio Code
- Git for version control
- Basic knowledge of C# and PowerShell

### Setting Up Your Development Environment

1. **Fork the repository** on GitHub
2. **Clone your fork** locally:
   ```bash
   git clone https://github.com/YOUR-USERNAME/invisi-launch.git
   cd invisi-launch
   ```
3. **Add the upstream remote**:
   ```bash
   git remote add upstream https://github.com/deepblue523/invisi-launch.git
   ```
4. **Build the project**:
   ```bash
   dotnet build
   ```

## Development Workflow

### Branch Strategy

- `main` - Protected branch containing stable, production-ready code
- Feature branches - Use descriptive names like `feature/add-batch-support` or `fix/powershell-argument-escaping`

### Creating a Feature Branch

1. **Update your local main branch**:
   ```bash
   git checkout main
   git pull upstream main
   ```

2. **Create a new branch**:
   ```bash
   git checkout -b feature/your-feature-name
   ```

### Making Changes

1. **Write clean, readable code** following C# conventions
2. **Test your changes** thoroughly on Windows
3. **Commit your changes** with clear, descriptive messages:
   ```bash
   git add .
   git commit -m "Add support for batch file execution"
   ```

### Commit Message Guidelines

Write clear and descriptive commit messages:

- Use the imperative mood ("Add feature" not "Added feature")
- Keep the first line under 50 characters
- Provide additional details in the body if needed
- Reference issues when applicable (e.g., "Fixes #123")

**Good examples:**
```
Add support for .bat and .cmd file execution
Fix argument escaping for PowerShell scripts
Update README with batch file examples
```

**Bad examples:**
```
Fixed stuff
Updated code
Changes
```

## Testing

Before submitting a pull request:

1. **Build the project** without errors:
   ```bash
   dotnet build
   ```

2. **Test manually** with various scenarios:
   - Regular executables (e.g., `notepad.exe`)
   - PowerShell scripts (`.ps1` files)
   - Scripts with arguments
   - Scripts with spaces in paths
   - Error conditions

3. **Test in both Debug and Release configurations**:
   ```bash
   dotnet build -c Debug
   dotnet build -c Release
   ```

## Submitting a Pull Request

1. **Push your branch** to your fork:
   ```bash
   git push origin feature/your-feature-name
   ```

2. **Create a pull request** on GitHub:
   - Go to the original repository
   - Click "New Pull Request"
   - Select your fork and branch
   - Fill out the PR template with details about your changes

3. **PR Requirements**:
   - Clear description of what the PR does
   - Reference any related issues
   - Ensure all builds pass
   - Respond to review feedback promptly

### Pull Request Template

When creating a PR, include:

```
## Description
Brief description of the changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Documentation update
- [ ] Code refactoring

## Testing
Describe how you tested your changes

## Related Issues
Fixes #(issue number)
```

## Code Style Guidelines

### C# Conventions

- Follow standard C# naming conventions (PascalCase for classes/methods, camelCase for variables)
- Use meaningful variable and method names
- Add comments for complex logic
- Keep methods focused and concise
- Use `var` for local variables when the type is obvious
- Add XML documentation comments for public APIs

### Example:

```csharp
/// <summary>
/// Launches an application or script with the specified arguments.
/// </summary>
/// <param name="args">Command-line arguments including the program path.</param>
private static void LaunchApplication(string[] args)
{
    string programPath = args[0];
    string arguments = args.Length > 1 ? string.Join(" ", args, 1, args.Length - 1) : "";
    
    // Implementation details...
}
```

## What to Contribute

### Areas for Contribution

We welcome contributions in these areas:

- **Bug fixes** - Fix issues reported in the issue tracker
- **New features** - Add support for additional file types or execution modes
- **Documentation** - Improve README, code comments, or examples
- **Performance** - Optimize execution or resource usage
- **Testing** - Add test scenarios or improve test coverage

### Feature Ideas

- Support for additional script types (`.cmd`, `.bat`, `.vbs`, `.js`)
- Configuration file support for default options
- Logging capabilities
- Command-line flags for different execution modes
- Enhanced error reporting

## Reporting Issues

When reporting bugs or requesting features:

1. **Check existing issues** first to avoid duplicates
2. **Use issue templates** when available
3. **Provide clear descriptions** with steps to reproduce
4. **Include system information** (Windows version, .NET version)
5. **Attach relevant logs or error messages**

## Questions?

If you have questions about contributing:

- Open a discussion on GitHub
- Review existing issues and pull requests
- Check the README.md for general project information

## License

By contributing to InvisiLaunch, you agree that your contributions will be licensed under the GPL-3.0 License.

---

Thank you for contributing to InvisiLaunch! Your efforts help make this tool better for everyone.
