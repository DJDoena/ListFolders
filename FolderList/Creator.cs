using DoenaSoft.AbstractionLayer.IOServices;

namespace DoenaSoft.FolderList;

public static class Creator
{
    public static (string oldFileName, string outFileName) Scan(IFolderInfo folder
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