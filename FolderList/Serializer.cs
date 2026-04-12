using DoenaSoft.AbstractionLayer.IOServices;
using DoenaSoft.FolderList.Xml;
using DoenaSoft.ToolBox.Generics;

namespace DoenaSoft.FolderList;

internal static class Serializer
{
    internal static (string oldFileName, string outFileName) Serialize(IFolderInfo folder
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