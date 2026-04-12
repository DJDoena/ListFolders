using DoenaSoft.FolderList.Xml;

namespace DoenaSoft.FolderList;

/// <summary>
/// Provides functionality for cleaning and optimizing XML folder structures.
/// </summary>
internal sealed class Cleaner
{
    private readonly IPathTransformer _pathTransformer;

    /// <summary>
    /// Initializes a new instance of the <see cref="Cleaner"/> class.
    /// </summary>
    /// <param name="pathTransformer">Optional path transformer to apply custom path transformations. If null, no transformation is applied.</param>
    internal Cleaner(IPathTransformer pathTransformer = null)
    {
        _pathTransformer = pathTransformer;
    }

    /// <summary>
    /// Cleans the XML structure by removing empty items and optimizing path information.
    /// </summary>
    /// <param name="rootItem">The root item of the XML structure to clean.</param>
    internal void Clean(RootItem rootItem)
    {
        if (!Clean(ref rootItem.Item))
        {
            rootItem.Item = null;
        }
    }

    private bool Clean(ref SubItem[] items)
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
                else if (_pathTransformer != null && !string.IsNullOrEmpty(item.FullPath))
                {
                    item.FullPath = _pathTransformer.Transform(item.FullPath);
                }
            }

            return true;
        }
    }
}