namespace DoenaSoft.FolderList;

internal static class FolderGetter
{
    internal static List<FolderData> Get(DirectoryInfo folder
        , string searchPatterns)
    {
        var searchPatternList = searchPatterns.Split(',');

        var files = searchPatternList
            .SelectMany(p => folder.GetFiles(p, SearchOption.AllDirectories))
            .ToList();

        var folderInfos = Get(folder, files);

        return folderInfos;
    }

    private static List<FolderData> Get(DirectoryInfo folder
        , IEnumerable<FileInfo> files)
    {
        var folders = files
            .Select(f => f.Directory)
            .Where(f => f.FullName != folder.FullName)
            .Distinct(new FolderInfoEqualityComparer());

        var folderInfos = folders
            .Select(f => GetFolderInfo(f, files))
            .OrderBy(f => f)
            .Distinct()
            .ToList();

        return folderInfos;
    }

    private static FolderData GetFolderInfo(DirectoryInfo folder
        , IEnumerable<FileInfo> allFiles)
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

    private static IEnumerable<FileInfo> GetFolderFiles(DirectoryInfo folder
        , IEnumerable<FileInfo> files)
        => files.Where(f => f.Directory.FullName == folder.FullName);
}