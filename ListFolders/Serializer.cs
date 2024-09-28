using DoenaSoft.ListFolders.Xml;
using DoenaSoft.ToolBox.Generics;

namespace DoenaSoft.ListFolders;

internal static class Serializer
{
    internal static void Serialize(DirectoryInfo folder, string outputFileName, RootItem rootItem)
    {
        var outputFile = new FileInfo(Path.Combine(folder.FullName, outputFileName));

        Backup($"{outputFile.FullName}.old", $"{outputFile.FullName}.old.old");

        Backup(outputFile.FullName, $"{outputFile.FullName}.old");

        (new XsltSerializer<RootItem>(new RootItemXsltSerializerDataProvider())).Serialize(outputFile.FullName, rootItem);

        File.SetAttributes(outputFile.FullName, FileAttributes.Archive);
    }

    private static void Backup(string sourceFileName, string targetFileName)
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