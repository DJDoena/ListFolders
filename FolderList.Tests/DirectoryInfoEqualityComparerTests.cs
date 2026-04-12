using DoenaSoft.FolderList;

namespace TestProject1;

[TestClass]
public sealed class DirectoryInfoEqualityComparerTests
{

    private readonly IEqualityComparer<DirectoryInfo> _comparer;

    public DirectoryInfoEqualityComparerTests()
    {
        _comparer = new FolderInfoEqualityComparer();
    }

    [TestMethod]
    public void BothNull()
    {
        var equals = _comparer.Equals(null, null);

        Assert.IsTrue(equals);
    }

    [TestMethod]
    public void LeftNull()
    {
        var equals = _comparer.Equals(null, new DirectoryInfo(@"C:\"));

        Assert.IsFalse(equals);
    }

    [TestMethod]
    public void RightNull()
    {
        var equals = _comparer.Equals(new DirectoryInfo(@"C:\"), null);

        Assert.IsFalse(equals);
    }

    [TestMethod]
    public void BothEqual()
    {
        var equals = _comparer.Equals(new DirectoryInfo(@"C:\"), new DirectoryInfo(@"C:\"));

        Assert.IsTrue(equals);
    }
}