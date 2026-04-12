# DoenaSoft.FolderList

A .NET library for scanning directories and generating hierarchical XML representations of folder structures based on file search patterns.

## Overview

DoenaSoft.FolderList is a core library that provides functionality to scan file systems, locate files matching specific patterns, and generate structured XML output representing the directory hierarchy. The library works directly with the .NET file system APIs for efficient directory scanning and file pattern matching.

## Features

- **Pattern-based file scanning**: Search for files using multiple comma-separated patterns (e.g., "*.mp4,*.avi")
- **Hierarchical XML generation**: Creates structured XML representation of folder trees
- **Smart folder consolidation**: Automatically groups multi-disc/multi-part folders (cd*, disc*, part*)
- **Timestamp tracking**: Records last write time for folders containing matching files
- **Multi-target support**: Targets both .NET Standard 2.0 and .NET 10.0
- **Comprehensive XML documentation**: All public and internal APIs are fully documented
- **Backup management**: Automatically creates backup copies (.old, .old.old) of previous scans

## Installation

Install via NuGet Package Manager:

```bash
dotnet add package DoenaSoft.FolderList
```

Or via Package Manager Console:

```powershell
Install-Package DoenaSoft.FolderList
```

## Usage

### Basic Example

```csharp
using DoenaSoft.FolderList;

// Get a folder reference
var rootFolder = new DirectoryInfo(@"C:\Videos");

// Scan for files matching patterns and generate XML output
var (oldFileName, outFileName) = Creator.Scan(
    folder: rootFolder,
    searchPatterns: "*.mp4,*.mkv,*.avi",
    outputFileName: @"C:\Output\video-list.xml"
);

// oldFileName contains the previous version path (if it existed)
// outFileName contains the new output file path
```

### Advanced Usage

For more control over the scanning process, you can use the individual components:

```csharp
using DoenaSoft.FolderList;

var rootFolder = new DirectoryInfo(@"C:\Music");

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
```

## API Reference

### Creator

Main entry point for the scanning process. Orchestrates the entire workflow from scanning to serialization.

**Namespace**: `DoenaSoft.FolderList`

```csharp
public static class Creator
{
    public static (string oldFileName, string outFileName) Scan(
        DirectoryInfo folder,
        string searchPatterns,
        string outputFileName);
}
```

**Parameters**:
- `folder`: The root directory to scan recursively
- `searchPatterns`: Comma-separated file patterns (e.g., "*.mp4,*.mkv,*.avi")
- `outputFileName`: The output XML file name (can be relative or absolute path)

**Returns**: A tuple with the backup file path and the new output file path

---

### FolderGetter

Scans directories and collects folder information based on file patterns. Handles the recursive file discovery and filters folders containing matching files.

**Namespace**: `DoenaSoft.FolderList`

```csharp
internal static class FolderGetter
{
    internal static List<FolderData> Get(
        DirectoryInfo folder,
        string searchPatterns);
}
```

**Key Behavior**:
- Splits comma-separated patterns and searches for all matching files
- Identifies unique folders containing matching files
- Automatically consolidates multi-disc folders (cd*, disc*, part*)
- Tracks the newest file timestamp per folder

---

### XmlCreator

Generates hierarchical XML structure from folder data. Converts the flat list of folders into a nested tree structure.

**Namespace**: `DoenaSoft.FolderList`

```csharp
internal static class XmlCreator
{
    internal static RootItem Create(
        DirectoryInfo folder,
        List<FolderData> folderDatas);
}
```

**Key Behavior**:
- Creates a hierarchical tree from folder paths
- Preserves relative folder structure
- Sets LastWriteTime on leaf nodes
- Generates FullPath for each folder item

---

### Cleaner

Optimizes the XML structure by removing redundant entries and cleaning up paths.

**Namespace**: `DoenaSoft.FolderList`

```csharp
internal static class Cleaner
{
    internal static void Clean(RootItem rootItem);
}
```

**Key Behavior**:
- Removes empty folder arrays
- Clears FullPath for parent nodes (only leaves retain full paths)
- Special handling for network paths starting with "N:\" (converts to forward slashes)

---

### Serializer

Handles XML serialization to file system with automatic backup management.

**Namespace**: `DoenaSoft.FolderList`

```csharp
internal static class Serializer
{
    internal static (string oldFileName, string outFileName) Serialize(
        DirectoryInfo folder,
        string outputFileName,
        RootItem rootItem);
}
```

**Key Behavior**:
- Creates backup chain: .old.old ← .old ← current
- Uses XSLT transformation for XML output
- Sets Archive attribute on output file
- Returns paths to both the backup and new file

---

### FolderData

Internal data class representing a folder with timestamp information.

**Namespace**: `DoenaSoft.FolderList`

```csharp
internal sealed class FolderData : IEquatable<FolderData>, IComparable<FolderData>
{
    public DirectoryInfo Folder { get; }
    public DateTime? LastWriteTime { get; }
}
```

**Key Behavior**:
- Stores folder reference and newest file timestamp
- Implements equality based on full path
- Sorts by path, then by newest timestamp (descending)

## Special Folder Handling

The library recognizes and consolidates folders with special naming patterns:

- **cd*** (e.g., cd1, cd2, cdA, cdB)
- **part*** (e.g., part1, part2)
- **disc*** (e.g., disc1, disc2)

These folders are automatically grouped under their parent folder to avoid duplicate entries in the output.

## XML Output Format

The library generates XML files with the following structure:

```xml
<?xml version="1.0" encoding="utf-8"?>
<root>
  <item name="Movies">
    <item name="Action">
      <item name="Movie1" lastWriteTime="2024-01-15T10:30:00Z" />
    </item>
  </item>
</root>
```

**Key Elements**:
- `<root>`: The root element containing all folder items
- `<item>`: Represents a folder in the hierarchy
  - `name`: The folder name (not full path)
  - `lastWriteTime`: UTC timestamp of the newest matching file (only on leaf nodes)
  - `fullPath`: Complete path (only on leaf nodes after cleaning)

**XSLT Transformation**: The XML output is formatted using XSLT for consistent styling and readability.

## Internal Architecture

The library follows a pipeline architecture:

1. **FolderGetter**: Scans file system → Returns `List<FolderData>`
2. **XmlCreator**: Processes folder list → Returns `RootItem` (hierarchical tree)
3. **Cleaner**: Optimizes tree structure → Modifies `RootItem` in place
4. **Serializer**: Writes to XML file → Returns file paths

This separation of concerns makes the code maintainable and testable.

## Testing

The library includes comprehensive unit tests in the `FolderList.Tests` project:

- **DirectoryInfoEqualityComparerTests**: Tests for folder comparison logic
- **FolderInfoTests**: Tests for folder data handling and multi-disc consolidation
- **DocumentTests**: Tests for XML document generation and structure

All tests use MSTest framework and run on .NET 10.0.

## Best Practices

### Error Handling

The library lets exceptions bubble up to the caller. Ensure you handle:
- `DirectoryNotFoundException`: When the root folder doesn't exist
- `UnauthorizedAccessException`: When lacking permissions to access folders
- `IOException`: When file operations fail (e.g., disk full)

### Performance Considerations

- **Large directories**: Scanning is recursive and may take time for large folder structures
- **Network paths**: Performance depends on network speed; consider caching results
- **Pattern complexity**: Multiple patterns are evaluated sequentially; use specific patterns when possible

### File Pattern Examples

```csharp
// Single pattern
Creator.Scan(folder, "*.mp4", "output.xml");

// Multiple patterns
Creator.Scan(folder, "*.mp4,*.mkv,*.avi", "output.xml");

// All files
Creator.Scan(folder, "*.*", "output.xml");

// Specific file types
Creator.Scan(folder, "*.jpg,*.png,*.gif,*.bmp", "images.xml");
```

## Dependencies

- [DoenaSoft.FolderList.Xml](https://www.nuget.org/packages/DoenaSoft.FolderList.Xml/) (>= 1.0.5) - XML schema definitions and data models

The library uses `DoenaSoft.ToolBox.Generics.XsltSerializer<T>` (via FolderList.Xml) for XSLT-based XML serialization.

## Target Frameworks

- .NET Standard 2.0
- .NET 10.0

## Version History

### 1.0.0
- Initial release
- Core scanning and XML generation functionality
- Multi-target support for .NET Standard 2.0 and .NET 10.0
- Comprehensive XML documentation for all APIs
- Smart folder consolidation (cd*, disc*, part*)
- Automatic backup management (.old, .old.old)
- XSLT-based XML serialization

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
