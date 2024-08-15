# Project Versioning Tool

The Project Versioning Tool automates the process of managing and applying version information during the build process. Integrated with MSBuild as a pre-build step, this tool ensures that the project's assembly version, file version, informational version, and product version are consistently set based on the latest Git tag on the `main` branch that matches the pattern `v{major}.{minor}.{patch}`. Tags matching this pattern on other branches are ignored.

To update the project to a new version, simply create a new version tag on `main`, and the tool will handle the rest, including validation to ensure versioning rules are followed, and logging for easy monitoring.

**Example Usage:** Consider a scenario where the latest three tags on `main`, in chronological order from most recent to oldest, are `some-tag`, `v1.2.3`, and `v1.2.2`. Running the tool will result in the version `1.2.3` being applied to the assembly version, file version, informational version, and product version of the project's build artifacts. Additionally, a `VersionInfo` class will be generated, containing this version information along with metadata about the current branch and commit. If you build a feature branch that was based on `main` before the `v1.2.3` tag was added, the tool will use version `1.2.2` for that build.

## Dependencies

To run the versioning tool, ensure you have the following dependencies installed:

- **Python 3.x**: Required to execute the versioning script.
- **Git**: The script relies on Git to retrieve version tags and commit information.
- **MSBuild**: This tool is designed to work with .NET projects using MSBuild for the build process.

## Setup and Usage

### Folder Structure

The correct folder structure inside the solution is as follows:

```
build/
├── versioning/
│   ├── __init__.py
│   ├── versioning.py
│   ├── git_utils.py
│   ├── version_info.py
│   ├── csproj_utils.py
│   ├── logging_config.py
│   └── VersionInfoTemplate.cs
│
├── project1_versioning.log
├── project2_versioning.log
└── ...

project1/
project2/
...
```

### MSBuild Integration

To integrate the tool with your .NET project using MSBuild, add the following pre-build step to your `.csproj` file:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0-windows</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <UseWPF>true</UseWPF>
    <ApplicationIcon>Resources\app-icon.ico</ApplicationIcon>
    
    <!-- These values can be initially set to placeholders -->
    <AssemblyVersion>0.0.0.0</AssemblyVersion>
    <FileVersion>0.0.0.0</FileVersion>
    <InformationalVersion>0.0.0.0</InformationalVersion>
    <ProductVersion>0.0.0.0</ProductVersion>
  </PropertyGroup>

  <!-- Include the tool as a pre-build step -->
  <Target Name="PreBuild" BeforeTargets="BeforeBuild">
    <Exec Command="python $(ProjectDir)..\build\versioning\versioning.py" />
  </Target>
</Project>
```

### Terminal Usage

You can also run the tool manually via the terminal for testing or standalone usage:

1. Navigate to the `build/versioning/` directory:

   ```bash
   cd build/versioning
   ```

2. Run the versioning script:

   ```bash
   python versioning.py path/to/project
   ```

Alternatively, you can also call the script directly from the project directory without arguments.

## Validation

### Validation Rules

The tool validates the following versioning rules based on Git tags:

- **Version Progression**: Ensures that each new version tag is greater than the previous one.
- **Version Bumps**:
  - **Major Version Bump**: If the major version increases, minor and patch versions must reset to `0`.
  - **Minor Version Bump**: If the minor version increases, the patch version must reset to `0`, and the major version must remain the same.
  - **Patch Version Bump**: If the patch version increases, the major and minor versions must remain the same.

### Failure Handling

If any of the validation rules fail, the script will:

- Log the specific validation failure.
- Halt further processing.
- Exit with an error code, causing the MSBuild process to fail, which ensures that invalid versions do not propagate.

## Versioning

### Versioning Steps

1. **Retrieve Latest Git Tag**: The script fetches the latest tag matching the pattern `v{major}.{minor}.{patch}` from the Git repository.
2. **Validate Versioning**: The tool checks if the new version follows the specified versioning rules.
3. **Update `.csproj` File**: If the new version differs from the current version, the script updates the `.csproj` file with the new version information.
4. **Create `VersionInfo.cs`**: A `VersionInfo.cs` file is generated containing the version, commit SHA, build date, and branch name.

### Output

- **Updated `.csproj` File**: The version properties in the `.csproj` file are updated with the new version information.
- **`VersionInfo.cs` File**: This file is created/updated in the `Utilities` folder within the project. It contains the static class `VersionInfo` in the namespace `{project name}.Utilities` exposing the version information via static properties and also offers a method `AsPrettyString()` returning a string with each property on a new line.

### How to Verify

- **Check the `.csproj` File**: Open the `.csproj` file and verify the version properties (`AssemblyVersion`, `FileVersion`, `InformationalVersion`, `ProductVersion`).
- **Check the Build Artifacts**: Open the `bin` directory inside the project directory and check the file properties of the relevant build artifacts (.dll, .exe) to verify the file version, informational version, and product version.
- **Check `VersionInfo.cs`**: Verify that the generated `VersionInfo.cs` file contains the correct version and metadata.

## Logging

### What is Logged

The tool logs detailed information at every step of the process, including:

- Git commands executed.
- Validation steps and results.
- Changes made to the `.csproj` file.
- Errors encountered during the process.

### Where is the Output

- **Log File**: The logs are written to a file named `versioning_<ProjectName>.log` located in the `build/` directory.
- **Console Output**: Logs are also streamed to the console during the build process, providing real-time feedback.

## Tool Components

The tool consists of the following components:

- **`versioning.py`**: The main entry point that orchestrates the entire process. It sets up logging, handles input, and coordinates the retrieval and application of version information.

- **`git_utils.py`**: Manages interactions with Git, including retrieving the latest version tag, parsing and validating version information, and collecting Git metadata like commit SHA and branch name.

- **`version_info.py`**: Generates the `VersionInfo.cs` class by populating a template with the version and Git metadata, and then writes it to the `Utilities` folder in the project.

- **`csproj_utils.py`**: Updates the `.csproj` file with the correct version properties. It only modifies the file if the version has changed to avoid unnecessary commits.

- **`logging_config.py`**: Configures logging for the tool, ensuring that logs are written to both a project-specific log file in the `build/` directory and the console for real-time feedback.