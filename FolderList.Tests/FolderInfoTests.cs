using DoenaSoft.FolderList;

namespace TestProject1;

[TestClass]
public sealed class FolderInfoTests
{
    [TestMethod]
    public void TwoParts()
    {
        var rootFolder = CreateFolder(null, null, @"C:\", out var _, out var albumFolders);

        var albumFolder = CreateFolder(rootFolder, albumFolders, "Album", out var _, out var discFolders);

        var disc1Folder = CreateFolder(albumFolder, discFolders, "Disc 1", out var disc1Files, out _);

        AddFile(disc1Folder, disc1Files, "Title A.mp3", new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc));

        var disc2Folder = CreateFolder(albumFolder, discFolders, "Disc 2", out var disc2Files, out _);

        AddFile(disc2Folder, disc2Files, "Title B.mp3", new DateTime(2025, 1, 2, 0, 0, 0, DateTimeKind.Utc));

        var folderInfos = FolderGetter.Get(rootFolder, "*.mp3");

        Assert.IsNotNull(folderInfos);
        Assert.AreEqual(1, folderInfos.Count);

        var folderInfo = folderInfos[0];

        Assert.IsNotNull(folderInfo);
        Assert.IsTrue(folderInfo.LastWriteTime.HasValue);
        Assert.AreEqual(new DateTime(2025, 1, 2, 0, 0, 0, DateTimeKind.Utc), folderInfo.LastWriteTime.Value);
    }

    private static DirectoryInfo CreateFolder(DirectoryInfo parent
        , List<DirectoryInfo> siblings
        , string name
        , out List<FileInfo> files
        , out List<DirectoryInfo> subFolders)
    {
        var fullName = parent != null
            ? @$"{parent.FullName}\{name}"
            : name;

        files = [];

        subFolders = [];

        var folder = new DirectoryInfo(fullName);

        siblings?.Add(folder);

        return folder;
    }

    private static void AddFile(DirectoryInfo folder
        , List<FileInfo> files
        , string name
        , DateTime writeTime)
    {
        var titleBFile = new FileInfo(@$"{folder.FullName}\{name}");

        files.Add(titleBFile);
    }
}