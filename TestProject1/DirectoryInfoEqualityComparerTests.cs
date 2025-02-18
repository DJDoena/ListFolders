using DoenaSoft.AbstractionLayer.IOServices;
using DoenaSoft.ListFolders;

namespace TestProject1;

[TestClass]
public sealed class DirectoryInfoEqualityComparerTests
{
    private readonly IIOServices _ioServices;

    private readonly IEqualityComparer<IFolderInfo> _comparer;

    public DirectoryInfoEqualityComparerTests()
    {
        _ioServices = new IOServices();

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
        var equals = _comparer.Equals(null, _ioServices.GetFolder(@"C:\"));

        Assert.IsFalse(equals);
    }

    [TestMethod]
    public void RightNull()
    {
        var equals = _comparer.Equals(_ioServices.GetFolder(@"C:\"), null);

        Assert.IsFalse(equals);
    }

    [TestMethod]
    public void BothEqual()
    {
        var equals = _comparer.Equals(_ioServices.GetFolder(@"C:\"), _ioServices.GetFolder(@"C:\"));

        Assert.IsTrue(equals);
    }
}