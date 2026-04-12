using DoenaSoft.FolderList.Xml;

namespace DoenaSoft.FolderList;

/// <summary>
/// Provides functionality for cleaning and optimizing XML folder structures.
/// </summary>
internal static class Cleaner
{
    /// <summary>
    /// Cleans the XML structure by removing empty items and optimizing path information.
    /// </summary>
    /// <param name="rootItem">The root item of the XML structure to clean.</param>
    internal static void Clean(RootItem rootItem)
    {
        if (!Clean(ref rootItem.Item))
        {
            rootItem.Item = null;
        }
    }

    private static bool Clean(ref SubItem[] items)
    {
        if (items != null && items.Length == 0)
        {
            items = null;

            return false;
        }
        else
        {
            foreach (var item in items)
            {
                if (Clean(ref item.Item))
                {
                    item.FullPath = null;
                }
                else
                {
                    var path = item.FullPath;

                    if (!string.IsNullOrEmpty(path) && path.StartsWith(@"N:\", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var cleanedPath = path.Substring(2).Replace("\\", "/").TrimEnd('/') + "/";

                        item.FullPath = cleanedPath;
                    }
                }
            }

            return true;
        }
    }
}