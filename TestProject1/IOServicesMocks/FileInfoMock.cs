using System.Diagnostics;
using DoenaSoft.AbstractionLayer.IOServices;

namespace TestProject1.IOServicesMocks;

[DebuggerDisplay("{FullName}")]
internal sealed class FileInfoMock : IFileInfo
{
    public IIOServices IOServices { get; set; }

    public string Name { get; set; }

    public string Extension { get; set; }

    public string FullName { get; set; }

    public IFolderInfo Folder { get; set; }

    public string FolderName { get; set; }

    public string DirectoryName { get; set; }

    public string NameWithoutExtension { get; set; }

    public bool Exists { get; set; }

    public ulong Length { get; set; }

    public DateTime LastWriteTime { get; set; }

    public DateTime LastWriteTimeUtc { get; set; }

    public DateTime CreationTime { get; set; }

    public DateTime CreationTimeUtc { get; set; }

    public DateTime LastAccessTime { get; set; }

    public DateTime LastAccessTimeUtc { get; set; }

    public bool Equals(IFileInfo other)
        => ReferenceEquals(this, other)
            || string.Equals(this.FullName, other?.FullName, StringComparison.OrdinalIgnoreCase);

    public void MoveTo(string targetFileName)
    {
        throw new NotImplementedException();
    }
}