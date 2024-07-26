using DoenaSoft.ListFolders.Xml;

namespace DoenaSoft.ListFolders;

internal static class XmlCreator
{
    internal static RootItem Create(DirectoryInfo folder, List<string> folderNames)
    {
        var rootItem = new RootItem()
        {
            Item = [],
        };

        foreach (var folderName in folderNames)
        {
            ProcessRoot(rootItem, folderName, folder.FullName.Length);
        }

        return rootItem;
    }

    private static void ProcessRoot(RootItem rootItem, string folderName, int fullNameLength)
    {
        var line = folderName[(fullNameLength + 1)..];

        var rootFolder = folderName[..fullNameLength];

        var lineParts = line.Split('\\');

        ProcessSegment(lineParts, 0, rootItem.Item, rootFolder);
    }

    private static void ProcessSegment(string[] cells, int cellIndex, List<SubItem> items, string rootFolder)
    {
        var cell = cells[cellIndex];

        var item = items.FirstOrDefault(i => i.Name == cell);

        if (item == null)
        {
            item = new SubItem()
            {
                Name = cell,
                Item = [],
                FullPath = GetFullPath(rootFolder, cells, cellIndex),
            };

            items.Add(item);
        }

        var nextCellIndex = cellIndex + 1;

        if (cells.Length > nextCellIndex)
        {
            ProcessSegment(cells, nextCellIndex, item.Item, rootFolder);
        }
    }

    private static string GetFullPath(string rootFolder, string[] cells, int cellIndex)
    {
        var rootParts = rootFolder.Split('\\');

        var subRootParts = cells[0..(cellIndex + 1)];

        var allParts = rootParts
            .Concat(subRootParts)
            .ToArray();

        var fullPath = Path.Combine(allParts);

        return fullPath;
    }
}