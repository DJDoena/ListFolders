using System.Diagnostics;

namespace DoenaSoft.FolderList;

/// <summary>
/// Represents data for a folder containing files that match search patterns, including timestamp information.
/// </summary>
[DebuggerDisplay("{Folder.FullName}")]
internal sealed class FolderData : IEquatable<FolderData>, IComparable<FolderData>
{
    /// <summary>
    /// Gets the directory information for this folder.
    /// </summary>
    public DirectoryInfo Folder { get; }

    /// <summary>
    /// Gets the last write time of the newest file in this folder, or null if no files were found.
    /// </summary>
    public DateTime? LastWriteTime { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FolderData"/> class.
    /// </summary>
    /// <param name="folder">The directory containing the files.</param>
    /// <param name="files">The files in the folder to analyze for timestamp information.</param>
    public FolderData(DirectoryInfo folder
        , IEnumerable<FileInfo> files)
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