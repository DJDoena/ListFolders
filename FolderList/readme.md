# DoenaSoft.FolderList

A .NET library for scanning directories and generating hierarchical XML representations of folder structures based on file search patterns.

## Overview

DoenaSoft.FolderList is a core library that provides functionality to scan file systems, locate files matching specific patterns, and generate structured XML output representing the directory hierarchy. The library uses abstraction layers for file system operations, making it testable and platform-independent.

## Features

- **Pattern-based file scanning**: Search for files using multiple comma-separated patterns (e.g., "*.mp4,*.avi")
- **Hierarchical XML generation**: Creates structured XML representation of folder trees
- **Smart folder consolidation**: Automatically groups multi-disc/multi-part folders (cd*, disc*, part*)
- **Timestamp tracking**: Records last write time for folders containing matching files
- **Abstraction layer support**: Uses `DoenaSoft.AbstractionLayer.IO` for testable file system operations
- **Multi-target support**: Targets both .NET Standard 2.0 and .NET 10.0

## Installation

Install via NuGet Package Manager:

``bash
dotnet add package DoenaSoft.FolderList
``

Or via Package Manager Console:

``powershell
Install-Package DoenaSoft.FolderList
``

## Usage

### Basic Example

``csharp
using DoenaSoft.AbstractionLayer.IOServices;
using DoenaSoft.FolderList;

// Get a folder reference using the IO abstraction layer
var ioServices = new IOServices();
var rootFolder = ioServices.GetFolder(@"C:\Videos");

// Scan for files matching patterns and generate XML output
var (oldFileName, outFileName) = Creator.Scan(
    folder: rootFolder,
    searchPatterns: "*.mp4,*.mkv,*.avi",
    outputFileName: @"C:\Output\video-list.xml"
);

// oldFileName contains the previous version path (if it existed)
// outFileName contains the new output file path
``

### Advanced Usage

For more control over the scanning process, you can use the individual components:

``csharp
using DoenaSoft.AbstractionLayer.IOServices;
using DoenaSoft.FolderList;

var ioServices = new IOServices();
var rootFolder = ioServices.GetFolder(@"C:\Music");

// Step 1: Get all folders containing matching files
var folderData = FolderGetter.Get(rootFolder, "*.mp3,*.flac");

// Step 2: Create XML structure
var rootItem = XmlCreator.Create(rootFolder, folderData);

// Step 3: Clean up the structure (remove redundant entries)
Cleaner.Clean(rootItem);

// Step 4: Serialize to XML file
var (oldFile, newFile) = Serializer.Serialize(
    rootFolder, 
    "music-list.xml", 
    rootItem
);
``

## API Reference

### Creator

Main entry point for the scanning process.

``csharp
public static class Creator
{
    public static (string oldFileName, string outFileName) Scan(
        IFolderInfo folder,
        string searchPatterns,
        string outputFileName);
}
``

### FolderGetter

Scans directories and collects folder information based on file patterns.

``csharp
internal static class FolderGetter
{
    internal static List<FolderData> Get(
        IFolderInfo folder,
        string searchPatterns);
}
``

### XmlCreator

Generates hierarchical XML structure from folder data.

``csharp
internal static class XmlCreator
{
    internal static RootItem Create(
        IFolderInfo folder,
        List<FolderData> folderDatas);
}
``

### Cleaner

Optimizes the XML structure by removing redundant entries.

``csharp
internal static class Cleaner
{
    internal static void Clean(RootItem rootItem);
}
``

### Serializer

Handles XML serialization to file system.

``csharp
internal static class Serializer
{
    internal static (string oldFileName, string outFileName) Serialize(
        IFolderInfo folder,
        string outputFileName,
        RootItem rootItem);
}
``

## Special Folder Handling

The library recognizes and consolidates folders with special naming patterns:

- **cd*** (e.g., cd1, cd2, cdA, cdB)
- **part*** (e.g., part1, part2)
- **disc*** (e.g., disc1, disc2)

These folders are automatically grouped under their parent folder to avoid duplicate entries in the output.

## Dependencies

- [DoenaSoft.AbstractionLayer.IO](https://www.nuget.org/packages/DoenaSoft.AbstractionLayer.IO/) (>= 6.0.4)
- [DoenaSoft.FolderList.Xml](https://www.nuget.org/packages/DoenaSoft.FolderList.Xml/) (>= 1.0.4)

## Target Frameworks

- .NET Standard 2.0
- .NET 10.0

## Version History

### 1.0.0
- Initial release
- Core scanning and XML generation functionality
- Multi-target support for .NET Standard 2.0 and .NET 10.0

## License

This library is licensed under the MIT License.

## Author

DJ Doena / Doena Soft.

## Links

- [GitHub Repository](https://github.com/DJDoena/ListFolders)
- [NuGet Package](https://www.nuget.org/packages/DoenaSoft.FolderList/)
- [Project Homepage](https://github.com/DJDoena/ListFolders)

## Support

For issues, questions, or contributions, please visit the [GitHub Issues](https://github.com/DJDoena/ListFolders/issues) page.
