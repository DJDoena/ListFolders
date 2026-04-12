namespace DoenaSoft.FolderList;

/// <summary>
/// Provides equality comparison for <see cref="DirectoryInfo"/> instances based on their full path.
/// </summary>
internal sealed class FolderInfoEqualityComparer : IEqualityComparer<DirectoryInfo>
{
    /// <summary>
    /// Determines whether two <see cref="DirectoryInfo"/> instances are equal based on their full path.
    /// </summary>
    /// <param name="left">The first directory to compare.</param>
    /// <param name="right">The second directory to compare.</param>
    /// <returns>True if the directories have the same full path; otherwise, false.</returns>
    public bool Equals(DirectoryInfo left, DirectoryInfo right)
        => ReferenceEquals(left, right)
            || left?.FullName.Equals(right?.FullName) == true;

    /// <summary>
    /// Gets the hash code for a <see cref="DirectoryInfo"/> based on its full path.
    /// </summary>
    /// <param name="obj">The directory for which to get the hash code.</param>
    /// <returns>The hash code of the directory's full path, or -1 if the directory is null.</returns>
    public int GetHashCode(DirectoryInfo obj)
        => obj?.FullName.GetHashCode() ?? -1;
}