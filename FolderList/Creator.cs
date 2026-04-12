namespace DoenaSoft.FolderList;

/// <summary>
/// Provides functionality for scanning directories and generating XML representations of folder structures.
/// </summary>
public sealed class Creator
{
    private readonly IFolderConsolidator _folderConsolidator;

    private readonly IPathTransformer _pathTransformer;

    private readonly IBackupStrategy _backupStrategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="Creator"/> class.
    /// </summary>
    /// <param name="folderConsolidator">Optional folder consolidator to determine which folders should be grouped with their parent. If null, no consolidation is applied.</param>
    /// <param name="pathTransformer">Optional path transformer to apply custom path transformations during cleaning. If null, no transformation is applied.</param>
    /// <param name="backupStrategy">Optional backup strategy for managing backup files. If null, no backups are created.</param>
    public Creator(IFolderConsolidator folderConsolidator = null
        , IPathTransformer pathTransformer = null
        , IBackupStrategy backupStrategy = null)
    {
        _folderConsolidator = folderConsolidator;
        _pathTransformer = pathTransformer;
        _backupStrategy = backupStrategy;
    }

    /// <summary>
    /// Scans a directory for files matching specified patterns and generates an XML output file representing the folder structure.
    /// </summary>
    /// <param name="folder">The root directory to scan.</param>
    /// <param name="searchPatterns">Comma-separated list of file patterns to search for (e.g., "*.mp4,*.avi").</param>
    /// <param name="outputFileName">The path where the XML output file will be saved.</param>
    /// <returns>A tuple containing the path to the old file (if it existed) and the new output file path.</returns>
    public (string oldFileName, string outFileName) Scan(DirectoryInfo folder
        , string searchPatterns
        , string outputFileName)
    {
        var folderNames = (new FolderGetter(_folderConsolidator)).Get(folder, searchPatterns);

        var rootItem = (new XmlCreator()).Create(folder, folderNames);

        (new Cleaner(_pathTransformer)).Clean(rootItem);

        var (oldFileName, outFileName) = (new Serializer(_backupStrategy)).Serialize(folder, outputFileName, rootItem);

        return (oldFileName, outFileName);
    }
}