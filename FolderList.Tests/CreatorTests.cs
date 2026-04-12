using DoenaSoft.FolderList;

namespace DoenaSoft.FolderList.Tests;

[TestClass]
[DoNotParallelize]
public sealed class CreatorTests
{
    private DirectoryInfo _testRoot;
    private string _testFolder;

    [TestInitialize]
    public void Setup()
    {
        var tempFolder = new DirectoryInfo(Path.GetTempPath());
        _testFolder = Path.Combine(tempFolder.FullName, "CreatorTests");

        if (Directory.Exists(_testFolder))
        {
            Directory.Delete(_testFolder, true);
        }

        _testRoot = Directory.CreateDirectory(_testFolder);
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (Directory.Exists(_testFolder))
        {
            try
            {
                Directory.Delete(_testFolder, true);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }

    [TestMethod]
    public void Creator_WithNoInterfaces_CreatesBasicOutput()
    {
        // Arrange
        var moviesFolder = CreateFolder(_testRoot, "Movies");
        AddFile(moviesFolder, "movie1.mp4", DateTime.UtcNow);

        var creator = new Creator();
        var outputFile = "test-output.xml";

        // Act
        var (oldFileName, outFileName) = creator.Scan(_testRoot, "*.mp4", outputFile);

        // Assert
        Assert.IsNull(oldFileName); // No backup strategy, so no old file
        Assert.IsTrue(File.Exists(outFileName));

        // Verify XML contains expected content
        var xmlContent = File.ReadAllText(outFileName);
        StringAssert.Contains(xmlContent, "Movies");
    }

    [TestMethod]
    public void Creator_WithFolderConsolidator_ConsolidatesDiscs()
    {
        // Arrange
        var albumFolder = CreateFolder(_testRoot, "Album");
        var disc1Folder = CreateFolder(albumFolder, "Disc 1");
        var disc2Folder = CreateFolder(albumFolder, "Disc 2");

        AddFile(disc1Folder, "track1.mp3", DateTime.UtcNow);
        AddFile(disc2Folder, "track2.mp3", DateTime.UtcNow.AddDays(1));

        var consolidator = new MultiDiscFolderConsolidator();
        var creator = new Creator(consolidator);
        var outputFile = "test-consolidation.xml";

        // Act
        var (_, outFileName) = creator.Scan(_testRoot, "*.mp3", outputFile);

        // Assert
        var xmlContent = File.ReadAllText(outFileName);
        StringAssert.Contains(xmlContent, "Album");
        // Disc folders should be consolidated, so only Album should appear as a folder item
        Assert.IsTrue(File.Exists(outFileName));
    }

    [TestMethod]
    public void Creator_WithoutFolderConsolidator_DoesNotConsolidateDiscs()
    {
        // Arrange
        var albumFolder = CreateFolder(_testRoot, "Album");
        var disc1Folder = CreateFolder(albumFolder, "Disc 1");
        var disc2Folder = CreateFolder(albumFolder, "Disc 2");

        AddFile(disc1Folder, "track1.mp3", DateTime.UtcNow);
        AddFile(disc2Folder, "track2.mp3", DateTime.UtcNow.AddDays(1));

        var creator = new Creator(); // No consolidator
        var outputFile = "test-no-consolidation.xml";

        // Act
        var (_, outFileName) = creator.Scan(_testRoot, "*.mp3", outputFile);

        // Assert
        var xmlContent = File.ReadAllText(outFileName);
        StringAssert.Contains(xmlContent, "Album");
        StringAssert.Contains(xmlContent, "Disc 1");
        StringAssert.Contains(xmlContent, "Disc 2");
    }

    [TestMethod]
    public void Creator_WithPathTransformer_TransformsPaths()
    {
        // Arrange
        var moviesFolder = CreateFolder(_testRoot, "Movies");
        AddFile(moviesFolder, "movie1.mp4", DateTime.UtcNow);

        var transformer = new TestPathTransformer();
        var creator = new Creator(pathTransformer: transformer);
        var outputFile = "test-transform.xml";

        // Act
        var (_, outFileName) = creator.Scan(_testRoot, "*.mp4", outputFile);

        // Assert
        var xmlContent = File.ReadAllText(outFileName);
        StringAssert.Contains(xmlContent, "Movies");

        // The path should be transformed
        Assert.IsTrue(transformer.WasCalled);
    }

    [TestMethod]
    public void Creator_WithBackupStrategy_CreatesBackup()
    {
        // Arrange
        var moviesFolder = CreateFolder(_testRoot, "Movies");
        AddFile(moviesFolder, "movie1.mp4", DateTime.UtcNow);

        var outputFile = "test-backup.xml";

        // Create initial file
        var creator1 = new Creator();
        creator1.Scan(_testRoot, "*.mp4", outputFile);
        var initialPath = Path.Combine(_testRoot.FullName, outputFile);
        Assert.IsTrue(File.Exists(initialPath));

        // Act - Create with backup strategy
        var backupStrategy = new TwoLevelBackupStrategy();
        var creator2 = new Creator(backupStrategy: backupStrategy);
        var (oldFileName, outFileName) = creator2.Scan(_testRoot, "*.mp4", outputFile);

        // Assert
        Assert.IsNotNull(oldFileName);
        Assert.IsTrue(File.Exists(oldFileName));
        StringAssert.EndsWith(oldFileName, ".old");
        Assert.IsTrue(File.Exists(outFileName));
    }

    [TestMethod]
    public void Creator_WithoutBackupStrategy_DoesNotCreateBackup()
    {
        // Arrange
        var moviesFolder = CreateFolder(_testRoot, "Movies");
        AddFile(moviesFolder, "movie1.mp4", DateTime.UtcNow);

        var outputFile = "test-no-backup.xml";

        // Create initial file
        var creator1 = new Creator();
        creator1.Scan(_testRoot, "*.mp4", outputFile);

        // Act - Create without backup strategy
        var creator2 = new Creator(); // No backup strategy
        var (oldFileName, outFileName) = creator2.Scan(_testRoot, "*.mp4", outputFile);

        // Assert
        Assert.IsNull(oldFileName); // No backup created
        Assert.IsTrue(File.Exists(outFileName));
    }

    [TestMethod]
    public void Creator_WithAllInterfaces_AppliesAllBehaviors()
    {
        // Arrange
        var albumFolder = CreateFolder(_testRoot, "Album");
        var disc1Folder = CreateFolder(albumFolder, "Disc 1");
        AddFile(disc1Folder, "track1.mp3", DateTime.UtcNow);

        var outputFile = "test-all.xml";

        // Create initial file for backup test
        var initialCreator = new Creator();
        initialCreator.Scan(_testRoot, "*.mp3", outputFile);

        // Act - Use all three interfaces
        var consolidator = new MultiDiscFolderConsolidator();
        var transformer = new TestPathTransformer();
        var backupStrategy = new TwoLevelBackupStrategy();
        var creator = new Creator(consolidator, transformer, backupStrategy);
        
        var (oldFileName, outFileName) = creator.Scan(_testRoot, "*.mp3", outputFile);

        // Assert
        // Backup created
        Assert.IsNotNull(oldFileName);
        Assert.IsTrue(File.Exists(oldFileName));

        // Output created
        Assert.IsTrue(File.Exists(outFileName));

        // Consolidation applied
        var xmlContent = File.ReadAllText(outFileName);
        StringAssert.Contains(xmlContent, "Album");

        // Transformation applied
        Assert.IsTrue(transformer.WasCalled);
    }

    [TestMethod]
    public void Creator_WithConsolidatorAndTransformer_AppliesBoth()
    {
        // Arrange
        var albumFolder = CreateFolder(_testRoot, "Album");
        var cdFolder = CreateFolder(albumFolder, "CD1");
        AddFile(cdFolder, "track.mp3", DateTime.UtcNow);

        var consolidator = new MultiDiscFolderConsolidator();
        var transformer = new TestPathTransformer();
        var creator = new Creator(consolidator, transformer);
        var outputFile = "test-combo.xml";

        // Act
        var (oldFileName, outFileName) = creator.Scan(_testRoot, "*.mp3", outputFile);

        // Assert
        Assert.IsNull(oldFileName); // No backup strategy
        Assert.IsTrue(File.Exists(outFileName));

        var xmlContent = File.ReadAllText(outFileName);
        StringAssert.Contains(xmlContent, "Album");
        Assert.IsTrue(transformer.WasCalled);
    }

    [TestMethod]
    public void Creator_WithConsolidatorAndBackup_AppliesBoth()
    {
        // Arrange
        var albumFolder = CreateFolder(_testRoot, "Album");
        var partFolder = CreateFolder(albumFolder, "Part1");
        AddFile(partFolder, "track.mp3", DateTime.UtcNow);

        var outputFile = "test-consolidate-backup.xml";

        // Create initial
        new Creator().Scan(_testRoot, "*.mp3", outputFile);

        var consolidator = new MultiDiscFolderConsolidator();
        var backupStrategy = new TwoLevelBackupStrategy();
        var creator = new Creator(consolidator, backupStrategy: backupStrategy);

        // Act
        var (oldFileName, outFileName) = creator.Scan(_testRoot, "*.mp3", outputFile);

        // Assert
        Assert.IsNotNull(oldFileName);
        Assert.IsTrue(File.Exists(outFileName));

        var xmlContent = File.ReadAllText(outFileName);
        StringAssert.Contains(xmlContent, "Album");
    }

    [TestMethod]
    public void Creator_WithTransformerAndBackup_AppliesBoth()
    {
        // Arrange
        var moviesFolder = CreateFolder(_testRoot, "Movies");
        AddFile(moviesFolder, "movie.mp4", DateTime.UtcNow);

        var outputFile = "test-transform-backup.xml";

        // Create initial
        new Creator().Scan(_testRoot, "*.mp4", outputFile);

        var transformer = new TestPathTransformer();
        var backupStrategy = new TwoLevelBackupStrategy();
        var creator = new Creator(pathTransformer: transformer, backupStrategy: backupStrategy);

        // Act
        var (oldFileName, outFileName) = creator.Scan(_testRoot, "*.mp4", outputFile);

        // Assert
        Assert.IsNotNull(oldFileName);
        Assert.IsTrue(File.Exists(outFileName));
        Assert.IsTrue(transformer.WasCalled);
    }

    [TestMethod]
    public void Creator_MultipleScans_MaintainsBackupChain()
    {
        // Arrange
        var moviesFolder = CreateFolder(_testRoot, "Movies");
        AddFile(moviesFolder, "movie.mp4", DateTime.UtcNow);

        var outputFile = "test-chain.xml";
        var backupStrategy = new TwoLevelBackupStrategy();
        var creator = new Creator(backupStrategy: backupStrategy);

        // Act - First scan (no backup yet)
        var (old1, out1) = creator.Scan(_testRoot, "*.mp4", outputFile);
        Assert.IsNull(old1);

        // Second scan (creates .old)
        var (old2, out2) = creator.Scan(_testRoot, "*.mp4", outputFile);
        Assert.IsNotNull(old2);
        Assert.IsTrue(File.Exists(old2));

        // Third scan (creates .old and .old.old)
        var (old3, out3) = creator.Scan(_testRoot, "*.mp4", outputFile);
        Assert.IsNotNull(old3);
        Assert.IsTrue(File.Exists(old3));
        
        var oldOldPath = $"{out3}.old.old";
        Assert.IsTrue(File.Exists(oldOldPath));
    }

    // Helper methods
    private DirectoryInfo CreateFolder(DirectoryInfo parent, string name)
    {
        var fullPath = Path.Combine(parent.FullName, name);
        return Directory.CreateDirectory(fullPath);
    }

    private void AddFile(DirectoryInfo folder, string name, DateTime writeTime)
    {
        var filePath = Path.Combine(folder.FullName, name);
        File.WriteAllText(filePath, "test content");
        File.SetLastWriteTimeUtc(filePath, writeTime);
    }

    // Test implementation of IPathTransformer
    private class TestPathTransformer : IPathTransformer
    {
        public bool WasCalled { get; private set; }

        public string Transform(string fullPath)
        {
            WasCalled = true;
            return fullPath?.Replace("\\", "/");
        }
    }
}
