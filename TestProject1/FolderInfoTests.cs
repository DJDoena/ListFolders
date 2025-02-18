using DoenaSoft.AbstractionLayer.IOServices;
using DoenaSoft.ListFolders;
using TestProject1.IOServicesMocks;

namespace TestProject1;

[TestClass]
public sealed class FolderInfoTests
{
    [TestMethod]
    public void TwoParts()
    {
        var albumFolders = new List<IFolderInfo>();

        var rootFolder = new FolderInfoMock()
        {
            FullName = @"C:\",
            Name = @"C:\",
            Folders = albumFolders,
        };

        var discFolders = new List<IFolderInfo>();

        var albumFolder = new FolderInfoMock()
        {
            Parent = rootFolder,
            FullName = @"C:\Album",
            Name = "Album",
            Folders = discFolders,
        };

        albumFolders.Add(albumFolder);

        var disc1Files = new List<IFileInfo>();

        var disc1Folder = new FolderInfoMock()
        {
            Parent = albumFolder,
            FullName = @"C:\Album\Disc 1",
            Name = "Disc 1",
            Files = disc1Files,
        };

        discFolders.Add(disc1Folder);

        var titleAFile = new FileInfoMock()
        {
            Folder = disc1Folder,
            FullName = @"C:\Album\ Disc 1\Title A.mp3",
            Name = "Title A.mp3",
            LastWriteTimeUtc = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        };

        disc1Files.Add(titleAFile);

        var disc2Files = new List<IFileInfo>();

        var disc2Folder = new FolderInfoMock()
        {
            Parent = albumFolder,
            FullName = @"C:\Album\ Disc 2",
            Name = "Disc 2",
            Files = disc2Files,
        };

        discFolders.Add(disc2Folder);

        var titleBFile = new FileInfoMock()
        {
            Folder = disc2Folder,
            FullName = @"C:\Album\ Disc 2\Title B.mp3",
            Name = "Title B.mp3",
            LastWriteTimeUtc = new DateTime(2025, 1, 2, 0, 0, 0, DateTimeKind.Utc),
        };

        disc2Files.Add(titleBFile);

        FolderGetter.Get(rootFolder, "*.mp3");
    }
}