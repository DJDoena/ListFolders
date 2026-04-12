using DoenaSoft.FolderList;

namespace DoenaSoft.ListFolders;

/// <summary>
/// Consolidates multi-disc and multi-part folders by grouping them under their parent folder.
/// </summary>
/// <remarks>
/// This consolidator recognizes folders starting with "cd", "disc", or "part" (case-insensitive)
/// and treats them as sub-divisions that should be consolidated with their parent folder.
/// </remarks>
internal sealed class MultiDiscFolderConsolidator : IFolderConsolidator
{
    /// <summary>
    /// Determines whether a folder should be consolidated with its parent folder.
    /// </summary>
    /// <param name="folder">The folder to check for consolidation.</param>
    /// <returns>True if the folder name starts with "cd", "disc", or "part" (case-insensitive); otherwise, false.</returns>
    public bool ShouldConsolidate(DirectoryInfo folder)
    {
        if (folder == null)
        {
            return false;
        }

        var name = folder.Name;

        return name.StartsWith("cd", StringComparison.OrdinalIgnoreCase)
            || name.StartsWith("disc", StringComparison.OrdinalIgnoreCase)
            || name.StartsWith("part", StringComparison.OrdinalIgnoreCase);
    }
}