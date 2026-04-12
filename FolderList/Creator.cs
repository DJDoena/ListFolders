namespace DoenaSoft.FolderList;

/// <summary>
/// Provides functionality for scanning directories and generating XML representations of folder structures.
/// </summary>
public static class Creator
{
    /// <summary>
    /// Scans a directory for files matching specified patterns and generates an XML output file representing the folder structure.
    /// </summary>
    /// <param name="folder">The root directory to scan.</param>
    /// <param name="searchPatterns">Comma-separated list of file patterns to search for (e.g., "*.mp4,*.avi").</param>
    /// <param name="outputFileName">The path where the XML output file will be saved.</param>
    /// <returns>A tuple containing the path to the old file (if it existed) and the new output file path.</returns>
    public static (string oldFileName, string outFileName) Scan(DirectoryInfo folder
        , string searchPatterns
        , string outputFileName)
    {
        var folderNames = FolderGetter.Get(folder, searchPatterns);

        var rootItem = XmlCreator.Create(folder, folderNames);

        Cleaner.Clean(rootItem);

        var (oldFileName, outFileName) = Serializer.Serialize(folder, outputFileName, rootItem);

        return (oldFileName, outFileName);
    }
}