using DoenaSoft.AbstractionLayer.IOServices;

namespace DoenaSoft.ListFolders;

internal sealed class FolderInfoEqualityComparer : IEqualityComparer<IFolderInfo>
{
    public bool Equals(IFolderInfo left, IFolderInfo right)
        => ReferenceEquals(left, right)
            || left?.FullName.Equals(right?.FullName) == true;

    public int GetHashCode(IFolderInfo obj)
        => obj?.FullName.GetHashCode() ?? -1;
}