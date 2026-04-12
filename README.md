# ListFolders

A .NET console application that scans directories and generates an XML representation of folder structures based on file search patterns.

## Overview

ListFolders is a command-line tool that recursively searches for files matching specified patterns within a directory tree and creates a hierarchical XML output representing the folder structure. The tool can automatically compare the new output with previous results using Beyond Compare (if installed).

## Features

- **Pattern-based file search**: Search for files using multiple comma-separated patterns
- **Hierarchical XML output**: Generates a structured XML representation of folders containing matching files
- **Automatic comparison**: Integrates with Beyond Compare to show differences between current and previous scans
- **Smart folder grouping**: Automatically consolidates multi-disc/multi-part folders (cd*, disc*, part*)
- **Timestamp tracking**: Records last write time for folders

## Requirements

- .NET 10.0 Runtime
- Optional: Beyond Compare 5 (for automatic comparison of results - checks `C:\Program Files\Beyond Compare 5\BCompare.exe`)

## Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/DJDoena/ListFolders.git
   ```

2. Build the project:
   ```bash
   dotnet build
   ```

## Usage

```bash
ListFolders <root-folder> <search-patterns> <output-file>
```

### Parameters

- `<root-folder>`: The root directory to scan
- `<search-patterns>`: Comma-separated list of file patterns to search for (e.g., "*.mp4,*.avi")
- `<output-file>`: Path to the output XML file

### Example

```bash
ListFolders "C:\Videos" "*.mp4,*.mkv" "C:\Output\video-list.xml"
```

This will:
1. Scan all subdirectories under `C:\Videos`
2. Find all folders containing `.mp4` or `.mkv` files
3. Generate an XML file at `C:\Output\video-list.xml`
4. If a previous version exists, automatically open Beyond Compare to show differences

## Project Structure

- **ListFolders**: Console application entry point (.NET 10.0)
- **FolderList**: Core library containing the scanning and XML generation logic (.NET Standard 2.0 / .NET 10.0)
- **FolderList.Xml**: NuGet package for XML schema handling (.NET Standard 2.0 / .NET 10.0)
- **FolderList.Tests**: MSTest-based unit tests (.NET 10.0)

## Dependencies

### NuGet Packages
- [DoenaSoft.FolderList.Xml](https://www.nuget.org/packages/DoenaSoft.FolderList.Xml/) (v1.0.5): XML schema library for folder list serialization
- [DoenaSoft.ToolBox](https://www.nuget.org/packages/DoenaSoft.ToolBox/) (v3.0.4): XML serialization utilities (used by FolderList.Xml)
- [MSTest.Sdk](https://www.nuget.org/packages/MSTest.Sdk/) (v4.0.2): Testing framework
- [Microsoft.NET.Test.Sdk](https://www.nuget.org/packages/Microsoft.NET.Test.Sdk/) (v18.4.0): .NET test SDK
- [MSTest.TestAdapter](https://www.nuget.org/packages/MSTest.TestAdapter/) (v4.2.1): MSTest adapter
- [MSTest.TestFramework](https://www.nuget.org/packages/MSTest.TestFramework/) (v4.2.1): MSTest framework

## How It Works

1. **Scanning** (`Creator.Scan`): The main entry point orchestrates the scanning process
2. **File Discovery** (`FolderGetter`): Recursively searches for files matching the specified patterns
3. **Folder Collection**: Identifies all unique folders containing matching files
4. **XML Generation** (`XmlCreator`): Creates a hierarchical XML structure representing the folder tree
5. **Cleanup** (`Cleaner`): Removes redundant entries and optimizes the output
6. **Serialization** (`Serializer`): Saves the XML to the specified output file
7. **Comparison**: If a previous version exists, launches Beyond Compare for side-by-side comparison (if Beyond Compare 5 is installed)

### Special Folder Handling

The tool recognizes and consolidates folders starting with:
- `cd*` (e.g., cd1, cd2)
- `part*` (e.g., part1, part2)
- `disc*` (e.g., disc1, disc2)

These folders are grouped together under their parent folder to avoid duplicate entries.

## Building

### Debug Configuration

```bash
dotnet build -c Debug
```

The assembly version is automatically generated based on the current timestamp in the format `yyyy.MM.dd.HHmm`.

### Running Tests

The project uses MSTest for unit testing. To run tests:

```bash
dotnet test
```

Tests include:
- `DirectoryInfoEqualityComparerTests`: Tests for folder comparison logic
- `FolderInfoTests`: Tests for folder information handling
- `DocumentTests`: Tests for XML document generation

Mock implementations of the IO abstraction layer are provided for testing purposes.

## License

MIT License

## Author

DJ Doena / Doena Soft.

## Links

- [GitHub Repository](https://github.com/DJDoena/ListFolders)
- [Project Homepage](https://github.com/DJDoena/ListFolders)

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.
