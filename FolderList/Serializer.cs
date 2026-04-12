using DoenaSoft.FolderList.Xml;
using DoenaSoft.ToolBox.Generics;

namespace DoenaSoft.FolderList;

/// <summary>
/// Provides functionality for serializing folder structures to XML files with backup management.
/// </summary>
internal sealed class Serializer
{
    private readonly IBackupStrategy _backupStrategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="Serializer"/> class.
    /// </summary>
    /// <param name="backupStrategy">Optional backup strategy for managing backup files. If null, no backups are created.</param>
    internal Serializer(IBackupStrategy backupStrategy = null)
    {
        _backupStrategy = backupStrategy;
    }

    /// <summary>
    /// Serializes the folder structure to an XML file and manages backup files.
    /// </summary>
    /// <param name="folder">The root directory where the output file will be created.</param>
    /// <param name="outputFileName">The name of the output XML file.</param>
    /// <param name="rootItem">The root item containing the folder structure to serialize.</param>
    /// <returns>A tuple containing the path to the backup file (if any) and the new output file.</returns>
    internal (string oldFileName, string outFileName) Serialize(DirectoryInfo folder
        , string outputFileName
        , RootItem rootItem)
    {
        var outputFile = new FileInfo(Path.Combine(folder.FullName, outputFileName));

        var outFileName = outputFile.FullName;

        var oldFileName = _backupStrategy?.CreateBackups(outFileName);

        (new XsltSerializer<RootItem>(new RootItemXsltSerializerDataProvider())).Serialize(outputFile.FullName, rootItem);

        File.SetAttributes(outputFile.FullName, FileAttributes.Archive);

        return (oldFileName, outFileName);
    }
}