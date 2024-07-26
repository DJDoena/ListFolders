using DoenaSoft.ListFolders.Xml;

namespace DoenaSoft.ListFolders;

internal static class Cleaner
{
    internal static void Clean(RootItem rootItem)
    {
        if (!Clean(ref rootItem.Item))
        {
            rootItem.Item = null;
        }
    }

    private static bool Clean(ref List<SubItem> items)
    {
        if (items != null && items.Count == 0)
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