namespace DoenaSoft.FolderList;

internal sealed class FolderInfoEqualityComparer : IEqualityComparer<DirectoryInfo>
{
    public bool Equals(DirectoryInfo left, DirectoryInfo right)
        => ReferenceEquals(left, right)
            || left?.FullName.Equals(right?.FullName) == true;

    public int GetHashCode(DirectoryInfo obj)
        => obj?.FullName.GetHashCode() ?? -1;
}