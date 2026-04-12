using DoenaSoft.AbstractionLayer.IOServices;
using System.Diagnostics;

namespace DoenaSoft.FolderList;

[DebuggerDisplay("{Folder.FullName}")]
internal sealed class FolderData : IEquatable<FolderData>, IComparable<FolderData>
{
    public IFolderInfo Folder { get; }

    public DateTime? LastWriteTime { get; }

    public FolderData(IFolderInfo folder, IEnumerable<IFileInfo> files)
    {
        this.Folder = folder;

        var newestFile = files
            .OrderByDescending(f => f.LastWriteTime)
            .FirstOrDefault();

        this.LastWriteTime = newestFile?.LastWriteTimeUtc;
    }

    public override int GetHashCode()
        => this.Folder.FullName.GetHashCode();

    public override bool Equals(object obj)
        => this.Equals(obj as FolderData);

    public bool Equals(FolderData other)
        => this.Folder.FullName.Equals(other?.Folder.FullName);

    public int CompareTo(FolderData other)
    {
        var compare = this.Folder.FullName.CompareTo(other?.Folder.FullName);

        if (compare == 0
            && (other.LastWriteTime.HasValue || this.LastWriteTime.HasValue))
        {
            if (!other.LastWriteTime.HasValue)
            {
                compare = -1;
            }
            else if (!this.LastWriteTime.HasValue)
            {
                compare = 1;
            }
            else
            {
                compare = other.LastWriteTime.Value.CompareTo(this.LastWriteTime.Value); //reverse, newest first
            }
        }

        return compare;
    }
}