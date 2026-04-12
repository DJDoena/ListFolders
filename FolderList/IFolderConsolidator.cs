namespace DoenaSoft.FolderList;

/// <summary>
/// Defines a contract for determining whether a folder should be consolidated with its parent.
/// </summary>
public interface IFolderConsolidator
{
    /// <summary>
    /// Determines whether a folder should be consolidated with its parent folder.
    /// </summary>
    /// <param name="folder">The folder to check for consolidation.</param>
    /// <returns>True if the folder should be consolidated with its parent; otherwise, false.</returns>
    /// <remarks>
    /// Common use cases include consolidating multi-disc folders (cd1, cd2), multi-part folders (part1, part2),
    /// or other organizational structures where child folders should be grouped under their parent.
    /// </remarks>
    bool ShouldConsolidate(DirectoryInfo folder);
}