using DoenaSoft.AbstractionLayer.IOServices;
using DoenaSoft.FolderList.Xml;

namespace DoenaSoft.FolderList;

internal static class XmlCreator
{
    internal static RootItem Create(IFolderInfo folder
        , List<FolderData> folderDatas)
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
        var line = folderData.Folder.FullName.Substring(fullNameLength + 1);

        var rootFolder = folderData.Folder.FullName.Substring(0, fullNameLength);

        var lineParts = line.Split('\\');

        ProcessSegment(lineParts, 0, ref rootItem.Item, rootFolder, folderData.LastWriteTime);
    }

    private static void ProcessSegment(string[] cells
        , int cellIndex
        , ref SubItem[] items
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
            ProcessSegment(cells, nextCellIndex, ref item.Item, rootFolder, lastWriteTime);
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

        var subRootParts = new string[cellIndex + 1];
        Array.Copy(cells, 0, subRootParts, 0, cellIndex + 1);

        var allParts = rootParts
            .Concat(subRootParts)
            .ToArray();

        var fullPath = Path.Combine(allParts);

        return fullPath;
    }
}