namespace DoenaSoft.ListFolders;

internal static class FolderGetter
{
    internal static List<string> Get(DirectoryInfo folder, string searchPatterns)
    {
        var searchPatternList = searchPatterns.Split(',');

        var files = searchPatternList
            .SelectMany(p => folder.GetFiles(p, SearchOption.AllDirectories));

        var folderNames = files
            .Select(f => f.Directory)
            .Select(GetFolderName)
            .Where(f => f != folder.FullName)
            .Distinct()
            .OrderBy(f => f)
            .ToList();

        return folderNames;
    }



    private static string GetFolderName(DirectoryInfo folder)
    {
        if (folder.Name.StartsWith("cd", StringComparison.OrdinalIgnoreCase))
        {
            return folder.Parent.FullName;
        }
        else if (folder.Name.StartsWith("part", StringComparison.OrdinalIgnoreCase))
        {
            return folder.Parent.FullName;
        }
        else if (folder.Name.StartsWith("disc", StringComparison.OrdinalIgnoreCase))
        {
            return folder.Parent.FullName;
        }
        else
        {
            return folder.FullName;
        }
    }
}
