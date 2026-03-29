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
    public FileAttributes Attributes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool IsReadOnly { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    long IFileInfo.Length => throw new NotImplementedException();

    public IFileInfo CopyTo(string destFileName)
    {
        throw new NotImplementedException();
    }

    public IFileInfo CopyTo(string destFileName, bool overwrite)
    {
        throw new NotImplementedException();
    }

    public Stream Create()
    {
        throw new NotImplementedException();
    }

    public StreamWriter CreateText()
    {
        throw new NotImplementedException();
    }

    public void Delete()
    {
        throw new NotImplementedException();
    }

    public bool Equals(IFileInfo other)
        => ReferenceEquals(this, other)
            || string.Equals(this.FullName, other?.FullName, StringComparison.OrdinalIgnoreCase);

    public void MoveTo(string targetFileName)
    {
        throw new NotImplementedException();
    }

    public Stream Open(FileMode mode)
    {
        throw new NotImplementedException();
    }

    public Stream Open(FileMode mode, FileAccess access)
    {
        throw new NotImplementedException();
    }

    public Stream Open(FileMode mode, FileAccess access, FileShare share)
    {
        throw new NotImplementedException();
    }

    public Stream OpenRead()
    {
        throw new NotImplementedException();
    }

    public StreamReader OpenText()
    {
        throw new NotImplementedException();
    }

    public Stream OpenWrite()
    {
        throw new NotImplementedException();
    }
}