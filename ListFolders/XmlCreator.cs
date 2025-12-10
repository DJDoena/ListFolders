using DoenaSoft.AbstractionLayer.IOServices;
using DoenaSoft.FolderList.Xml;

namespace DoenaSoft.ListFolders;

internal static class XmlCreator
{
    internal static RootItem Create(IFolderInfo folder, List<FolderData> folderDatas)
    {
        var rootItem = new RootItem()
        {
            Item = [],
        };

        foreach (var folderData in folderDatas)
        {
            ProcessRoot(rootItem, folderData, folder.FullName.Length);
        }

        return rootItem;
    }

    private static void ProcessRoot(RootItem rootItem
        , FolderData folderData
        , int fullNameLength)
    {
        var line = folderData.Folder.FullName[(fullNameLength + 1)..];

        var rootFolder = folderData.Folder.FullName[..fullNameLength];

        var lineParts = line.Split('\\');

        ProcessSegment(lineParts, 0, rootItem.Item, rootFolder, folderData.LastWriteTime);
    }

    private static void ProcessSegment(string[] cells
        , int cellIndex
        , SubItem[] items
        , string rootFolder
        , DateTime? lastWriteTime)
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

            items = [.. items, item];
        }

        var nextCellIndex = cellIndex + 1;

        if (cells.Length > nextCellIndex)
        {
            ProcessSegment(cells, nextCellIndex, item.Item, rootFolder, lastWriteTime);
        }
        else if (lastWriteTime.HasValue)
        {
            item.LastWriteTime = lastWriteTime.Value;
            item.LastWriteTimeSpecified = true;
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