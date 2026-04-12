# DoenaSoft.FolderList

A .NET library for scanning directories and generating hierarchical XML representations of folder structures based on file search patterns.

## Overview

DoenaSoft.FolderList is a core library that provides functionality to scan file systems, locate files matching specific patterns, and generate structured XML output representing the directory hierarchy. The library works directly with the .NET file system APIs for efficient directory scanning and file pattern matching.

## Features

- **Pattern-based file scanning**: Search for files using multiple comma-separated patterns (e.g., "*.mp4,*.avi")
- **Hierarchical XML generation**: Creates structured XML representation of folder trees
- **Flexible folder consolidation**: Inject custom logic via `IFolderConsolidator` to group multi-disc/multi-part folders
- **Timestamp tracking**: Records last write time for folders containing matching files
- **Multi-target support**: Targets both .NET Standard 2.0 and .NET 10.0
- **Comprehensive XML documentation**: All public and internal APIs are fully documented
- **Customizable backup management**: Inject custom backup strategies via `IBackupStrategy` interface
- **Extensible path transformation**: Inject custom path transformation logic via `IPathTransformer` interface
- **Constructor injection**: Clean dependency injection pattern with sealed instance classes

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

// Create scanner with default settings (no consolidation or transformation)
var creator = new Creator();

// Scan for files matching patterns and generate XML output
var (oldFileName, outFileName) = creator.Scan(
    folder: rootFolder,
    searchPatterns: "*.mp4,*.mkv,*.avi",
    outputFileName: @"C:\Output\video-list.xml"
);

// oldFileName contains the previous version path (if it existed)
// outFileName contains the new output file path
```

### Custom Path Transformation

You can provide custom path transformation logic for special use cases:

```csharp
using DoenaSoft.FolderList;

// Create a custom transformer in your application
public class NetworkPathTransformer : IPathTransformer
{
    public string Transform(string fullPath)
    {
        if (string.IsNullOrEmpty(fullPath))
            return fullPath;

        if (fullPath.StartsWith(@"N:\", StringComparison.InvariantCultureIgnoreCase))
        {
            return fullPath.Substring(3).Replace("\\", "/").TrimEnd('/') + "/";
        }

        return fullPath;
    }
}

// Use it when scanning
var pathTransformer = new NetworkPathTransformer();
var creator = new Creator(pathTransformer: pathTransformer);

var rootFolder = new DirectoryInfo(@"N:\Videos");

var (oldFileName, outFileName) = creator.Scan(
    folder: rootFolder,
    searchPatterns: "*.mp4,*.mkv,*.avi",
    outputFileName: @"C:\Output\video-list.xml"
);
```

### Custom Folder Consolidation

You can implement custom logic to consolidate multi-disc or multi-part folders:

```csharp
using DoenaSoft.FolderList;

// Create a custom folder consolidator
public class MultiDiscFolderConsolidator : IFolderConsolidator
{
    public bool ShouldConsolidate(DirectoryInfo folder)
    {
        if (folder == null)
            return false;

        var name = folder.Name;
        return name.StartsWith("cd", StringComparison.OrdinalIgnoreCase)
            || name.StartsWith("disc", StringComparison.OrdinalIgnoreCase)
            || name.StartsWith("part", StringComparison.OrdinalIgnoreCase);
    }
}

// Use both consolidator and transformer
var folderConsolidator = new MultiDiscFolderConsolidator();
var pathTransformer = new NetworkPathTransformer();
var creator = new Creator(folderConsolidator, pathTransformer);

var (oldFileName, outFileName) = creator.Scan(
    folder: new DirectoryInfo(@"C:\Music"),
    searchPatterns: "*.mp3,*.flac",
    outputFileName: "music-list.xml"
);
```

### Custom Backup Strategy

You can implement custom backup file management strategies:

```csharp
using DoenaSoft.FolderList;

// Create a custom backup strategy
public class TwoLevelBackupStrategy : IBackupStrategy
{
    public string CreateBackups(string outputFilePath)
    {
        if (string.IsNullOrEmpty(outputFilePath))
            return null;

        var oldOldFileName = $"{outputFilePath}.old.old";
        var oldFileName = $"{outputFilePath}.old";

        // Move .old to .old.old
        if (File.Exists(oldFileName))
        {
            if (File.Exists(oldOldFileName))
                File.Delete(oldOldFileName);
            File.Move(oldFileName, oldOldFileName);
        }

        // Move current to .old
        if (File.Exists(outputFilePath))
        {
            File.Move(outputFilePath, oldFileName);
            return oldFileName;
        }

        return null;
    }
}

// Use all three customizations
var folderConsolidator = new MultiDiscFolderConsolidator();
var pathTransformer = new NetworkPathTransformer();
var backupStrategy = new TwoLevelBackupStrategy();
var creator = new Creator(folderConsolidator, pathTransformer, backupStrategy);

var (oldFileName, outFileName) = creator.Scan(
    folder: new DirectoryInfo(@"C:\Videos"),
    searchPatterns: "*.mp4,*.mkv",
    outputFileName: "video-list.xml"
);

// Or create other custom transformers for different scenarios
public class UncPathTransformer : IPathTransformer
{
    public string Transform(string fullPath)
    {
        // Your custom transformation logic
        return fullPath.Replace(@"\\server\share", "/share");
    }
}
```

### Advanced Usage

For more control over the scanning process, you can use the individual components:

```csharp
using DoenaSoft.FolderList;

var rootFolder = new DirectoryInfo(@"C:\Music");

// Create dependencies
var folderConsolidator = new MultiDiscFolderConsolidator();
var pathTransformer = new NetworkPathTransformer();
var backupStrategy = new TwoLevelBackupStrategy();

// Step 1: Get all folders containing matching files
var folderGetter = new FolderGetter(folderConsolidator);
var folderData = folderGetter.Get(rootFolder, "*.mp3,*.flac");

// Step 2: Create XML structure
var xmlCreator = new XmlCreator();
var rootItem = xmlCreator.Create(rootFolder, folderData);

// Step 3: Clean up the structure (remove redundant entries)
var cleaner = new Cleaner(pathTransformer);
cleaner.Clean(rootItem);

// Step 4: Serialize to XML file
var serializer = new Serializer(backupStrategy);
var (oldFile, newFile) = serializer.Serialize(
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
public sealed class Creator
{
    public Creator(
        IFolderConsolidator folderConsolidator = null,
        IPathTransformer pathTransformer = null,
        IBackupStrategy backupStrategy = null);

    public (string oldFileName, string outFileName) Scan(
        DirectoryInfo folder,
        string searchPatterns,
        string outputFileName);
}
```

**Constructor Parameters**:
- `folderConsolidator`: (Optional) Custom folder consolidator for grouping multi-disc/multi-part folders
- `pathTransformer`: (Optional) Custom path transformer for specialized path handling
- `backupStrategy`: (Optional) Custom backup strategy for managing backup files

**Scan Method Parameters**:
- `folder`: The root directory to scan recursively
- `searchPatterns`: Comma-separated file patterns (e.g., "*.mp4,*.mkv,*.avi")
- `outputFileName`: The output XML file name (can be relative or absolute path)

**Returns**: A tuple with the backup file path and the new output file path

---

### FolderGetter

Scans directories and collects folder information based on file patterns. Handles the recursive file discovery and filters folders containing matching files.

**Namespace**: `DoenaSoft.FolderList`

```csharp
internal sealed class FolderGetter
{
    internal FolderGetter(IFolderConsolidator folderConsolidator = null);

    internal List<FolderData> Get(
        DirectoryInfo folder,
        string searchPatterns);
}
```

**Constructor Parameters**:
- `folderConsolidator`: (Optional) Determines which folders should be consolidated with their parent

**Key Behavior**:
- Splits comma-separated patterns and searches for all matching files
- Identifies unique folders containing matching files
- Applies folder consolidation logic via injected `IFolderConsolidator`
- Tracks the newest file timestamp per folder

---

### XmlCreator

Generates hierarchical XML structure from folder data. Converts the flat list of folders into a nested tree structure.

**Namespace**: `DoenaSoft.FolderList`

```csharp
internal sealed class XmlCreator
{
    internal RootItem Create(
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

Optimizes the XML structure by removing redundant entries and applying optional path transformations.

**Namespace**: `DoenaSoft.FolderList`

```csharp
internal sealed class Cleaner
{
    internal Cleaner(IPathTransformer pathTransformer = null);

    internal void Clean(RootItem rootItem);
}
```

**Constructor Parameters**:
- `pathTransformer`: (Optional) Custom path transformer for specialized path handling

**Key Behavior**:
- Removes empty folder arrays
- Clears FullPath for parent nodes (only leaves retain full paths)
- Applies custom path transformations via `IPathTransformer` if provided

---

### Serializer

Handles XML serialization to file system with automatic backup management.

**Namespace**: `DoenaSoft.FolderList`

```csharp
internal sealed class Serializer
{
    internal Serializer(IBackupStrategy backupStrategy = null);

    internal (string oldFileName, string outFileName) Serialize(
        DirectoryInfo folder,
        string outputFileName,
        RootItem rootItem);
}
```

**Constructor Parameters**:
- `backupStrategy`: (Optional) Custom backup strategy for managing backup files

**Key Behavior**:
- Applies backup strategy via injected `IBackupStrategy` if provided
- Uses XSLT transformation for XML output
- Sets Archive attribute on output file
- Returns paths to both the backup (if any) and new file

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

---

### IPathTransformer

Public interface for implementing custom path transformation logic.

**Namespace**: `DoenaSoft.FolderList`

```csharp
public interface IPathTransformer
{
    string Transform(string fullPath);
}
```

**Purpose**: Allows callers to inject custom path transformation logic during the cleaning phase, enabling specialized handling of paths (e.g., network drives, UNC paths, cloud storage paths).

**Implementation Example**:

```csharp
// Example: Network path transformer for N:\ drives
public sealed class NetworkPathTransformer : IPathTransformer
{
    public string Transform(string fullPath)
    {
        if (string.IsNullOrEmpty(fullPath))
        {
            return fullPath;
        }

        if (fullPath.StartsWith(@"N:\", StringComparison.InvariantCultureIgnoreCase))
        {
            var cleanedPath = fullPath.Substring(3).Replace("\\", "/").TrimEnd('/') + "/";
            return cleanedPath;
        }

        return fullPath;
    }
}
```

This is an example implementation - create your own in your application based on your specific needs.

---

### IFolderConsolidator

Public interface for implementing custom folder consolidation logic.

**Namespace**: `DoenaSoft.FolderList`

```csharp
public interface IFolderConsolidator
{
    bool ShouldConsolidate(DirectoryInfo folder);
}
```

**Purpose**: Allows callers to inject custom logic to determine which folders should be consolidated with their parent folders, enabling specialized handling of multi-disc, multi-part, or other organizational structures.

**Implementation Example**:

```csharp
// Example: Multi-disc folder consolidator
public sealed class MultiDiscFolderConsolidator : IFolderConsolidator
{
    public bool ShouldConsolidate(DirectoryInfo folder)
    {
        if (folder == null)
            return false;

        var name = folder.Name;
        return name.StartsWith("cd", StringComparison.OrdinalIgnoreCase)
            || name.StartsWith("disc", StringComparison.OrdinalIgnoreCase)
            || name.StartsWith("part", StringComparison.OrdinalIgnoreCase);
    }
}
```

This is an example implementation - create your own in your application based on your specific organizational needs.

---

### IBackupStrategy

Public interface for implementing custom backup file management strategies.

**Namespace**: `DoenaSoft.FolderList`

```csharp
public interface IBackupStrategy
{
    string CreateBackups(string outputFilePath);
}
```

**Purpose**: Allows callers to inject custom backup file management logic before writing new XML output files. Enables flexible backup strategies such as versioned backups, timestamped backups, or multi-level backup chains.

**Implementation Example**:

```csharp
// Example: Two-level backup strategy (.old and .old.old)
public sealed class TwoLevelBackupStrategy : IBackupStrategy
{
    public string CreateBackups(string outputFilePath)
    {
        if (string.IsNullOrEmpty(outputFilePath))
            return null;

        var oldOldFileName = $"{outputFilePath}.old.old";
        var oldFileName = $"{outputFilePath}.old";

        // Move .old to .old.old
        if (File.Exists(oldFileName))
        {
            if (File.Exists(oldOldFileName))
                File.Delete(oldOldFileName);
            File.Move(oldFileName, oldOldFileName);
        }

        // Move current to .old
        if (File.Exists(outputFilePath))
        {
            File.Move(outputFilePath, oldFileName);
            return oldFileName;
        }

        return null;
    }
}
```

This is an example implementation - create your own based on your backup requirements (e.g., timestamped backups, unlimited versions, cloud storage backups).

## Special Folder Handling

The library provides flexible folder consolidation through the `IFolderConsolidator` interface. Common use cases include:

- **cd*** (e.g., cd1, cd2, cdA, cdB)
- **part*** (e.g., part1, part2)
- **disc*** (e.g., disc1, disc2)

By implementing `IFolderConsolidator`, you can define which folders should be grouped under their parent folder to avoid duplicate entries in the output. If no consolidator is provided, all folders are treated independently.

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

### Path Transformation

Use `IPathTransformer` when you need to customize how paths appear in the XML output:

```csharp
// Example: Network path transformer (implement in your application)
public class NetworkPathTransformer : IPathTransformer
{
    public string Transform(string fullPath)
    {
        if (string.IsNullOrEmpty(fullPath))
            return fullPath;

        if (fullPath.StartsWith(@"N:\", StringComparison.InvariantCultureIgnoreCase))
        {
            return fullPath.Substring(3).Replace("\\", "/").TrimEnd('/') + "/";
        }

        return fullPath;
    }
}

var transformer = new NetworkPathTransformer();
Creator.Scan(folder, "*.mp4", "output.xml", transformer);

// Custom transformer for UNC paths
public class UncPathTransformer : IPathTransformer
{
    public string Transform(string fullPath)
    {
        if (fullPath.StartsWith(@"\\server\share\"))
        {
            return fullPath.Replace(@"\\server\share\", "").Replace("\\", "/");
        }
        return fullPath;
    }
}

// Custom transformer for multiple scenarios
public class MultiPathTransformer : IPathTransformer
{
    public string Transform(string fullPath)
    {
        // Remove sensitive server names
        fullPath = fullPath.Replace(@"\\internal-server\", "/");

        // Normalize cloud storage paths
        if (fullPath.Contains("OneDrive"))
        {
            fullPath = fullPath.Replace("OneDrive - Company", "OneDrive");
        }

        return fullPath;
    }
}
```

**When to use path transformers**:
- Converting absolute paths to relative paths
- Normalizing network or UNC paths for portability
- Removing sensitive information from paths
- Converting paths for cross-platform compatibility
- Standardizing cloud storage path formats
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
