using DoenaSoft.AbstractionLayer.IOServices;

namespace DoenaSoft.FolderList;

internal static class FolderGetter
{
    internal static List<FolderData> Get(IFolderInfo folder
        , string searchPatterns)
    {
        var searchPatternList = searchPatterns.Split(',');

        var files = searchPatternList
            .SelectMany(p => folder.GetFiles(p, SearchOption.AllDirectories))
            .ToList();

        var folderInfos = Get(folder, files);

        return folderInfos;
    }

    private static List<FolderData> Get(IFolderInfo folder
        , IEnumerable<IFileInfo> files)
    {
        var folders = files
            .Select(f => f.Folder)
            .Where(f => f.FullName != folder.FullName)
            .Distinct(new FolderInfoEqualityComparer());

        var folderInfos = folders
            .Select(f => GetFolderInfo(f, files))
            .OrderBy(f => f)
            .Distinct()
            .ToList();

        return folderInfos;
    }

    private static FolderData GetFolderInfo(IFolderInfo folder
        , IEnumerable<IFileInfo> allFiles)
    {
        var folderFiles = GetFolderFiles(folder, allFiles);

        if (folder.Name.StartsWith("cd", StringComparison.OrdinalIgnoreCase))
        {
            return new FolderData(folder.Parent, folderFiles);
        }
        else if (folder.Name.StartsWith("part", StringComparison.OrdinalIgnoreCase))
        {
            return new FolderData(folder.Parent, folderFiles);
        }
        else if (folder.Name.StartsWith("disc", StringComparison.OrdinalIgnoreCase))
        {
            return new FolderData(folder.Parent, folderFiles);
        }
        else
        {
            return new FolderData(folder, folderFiles);
        }
    }

    private static IEnumerable<IFileInfo> GetFolderFiles(IFolderInfo folder
        , IEnumerable<IFileInfo> files)
        => files.Where(f => f.Folder.FullName == folder.FullName);
}