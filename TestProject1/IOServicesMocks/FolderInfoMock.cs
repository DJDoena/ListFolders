using System.Diagnostics;
using DoenaSoft.AbstractionLayer.IOServices;

namespace TestProject1.IOServicesMocks;

[DebuggerDisplay("{FullName}")]
internal sealed class FolderInfoMock : IFolderInfo
{
    public IIOServices IOServices { get; set; }

    public string Name { get; set; }

    public IFolderInfo Parent { get; set; }

    public IFolderInfo Root { get; set; }

    public IDriveInfo Drive { get; set; }

    public bool Exists { get; set; }

    public string FullName { get; set; }

    public DateTime LastWriteTime { get; set; }
    public DateTime LastWriteTimeUtc { get; set; }
    public DateTime CreationTime { get; set; }
    public DateTime CreationTimeUtc { get; set; }
    public DateTime LastAccessTime { get; set; }
    public DateTime LastAccessTimeUtc { get; set; }

    public IEnumerable<IFileInfo> Files { get; set; }

    public IEnumerable<IFolderInfo> Folders { get; set; }

    public void Create()
    {
        throw new NotImplementedException();
    }

    public bool Equals(IFolderInfo other)
        => ReferenceEquals(this, other)
            || string.Equals(this.FullName, other?.FullName, StringComparison.OrdinalIgnoreCase);

    public IEnumerable<IFileInfo> GetFiles(string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        var files = this.Files ?? [];

        if (searchOption != SearchOption.TopDirectoryOnly)
        {
            var subFolders = this.GetFolders("*.*", SearchOption.AllDirectories);

            files =
            [
                .. files,
                .. subFolders
                    .SelectMany(f =>
                        {
                            var subFiles = f.GetFiles(searchPattern, SearchOption.TopDirectoryOnly);

                            return subFiles;
                        })
            ];

            return files;
        }

        return files;
    }

    public IEnumerable<IFolderInfo> GetFolders(string searchPattern, SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        var folders = this.Folders ?? [];

        if (searchOption != SearchOption.TopDirectoryOnly)
        {
            folders =
            [
                .. folders,
                .. folders
                    .SelectMany(f =>
                    {
                        var subFolders = f.GetFolders(searchPattern, searchOption);

                        return subFolders;
                    }),
            ];
        }

        return folders;
    }
}