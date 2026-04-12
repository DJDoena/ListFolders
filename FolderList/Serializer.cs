using DoenaSoft.FolderList.Xml;
using DoenaSoft.ToolBox.Generics;

namespace DoenaSoft.FolderList;

/// <summary>
/// Provides functionality for serializing folder structures to XML files with backup management.
/// </summary>
internal static class Serializer
{
    /// <summary>
    /// Serializes the folder structure to an XML file and manages backup files.
    /// </summary>
    /// <param name="folder">The root directory where the output file will be created.</param>
    /// <param name="outputFileName">The name of the output XML file.</param>
    /// <param name="rootItem">The root item containing the folder structure to serialize.</param>
    /// <returns>A tuple containing the path to the backup file and the new output file.</returns>
    internal static (string oldFileName, string outFileName) Serialize(DirectoryInfo folder
        , string outputFileName
        , RootItem rootItem)
    {
        var outputFile = new FileInfo(Path.Combine(folder.FullName, outputFileName));

        var oldOldFileName = $"{outputFile.FullName}.old.old";

        var oldFileName = $"{outputFile.FullName}.old";

        Backup(oldFileName, oldOldFileName);

        var outFileName = outputFile.FullName;

        Backup(outFileName, oldFileName);

        (new XsltSerializer<RootItem>(new RootItemXsltSerializerDataProvider())).Serialize(outputFile.FullName, rootItem);

        File.SetAttributes(outputFile.FullName, FileAttributes.Archive);

        return (oldFileName, outFileName);
    }

    private static void Backup(string sourceFileName
        , string targetFileName)
    {
        if (File.Exists(sourceFileName))
        {
            if (File.Exists(targetFileName))
            {
                File.Delete(targetFileName);
            }

            File.Move(sourceFileName, targetFileName);
        }
    }
}