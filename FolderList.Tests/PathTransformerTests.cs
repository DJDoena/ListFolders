namespace DoenaSoft.FolderList.Tests;

[TestClass]
public class PathTransformerTests
{
    [TestMethod]
    public void NetworkPathTransformer_TransformsNetworkPath()
    {
        // Arrange
        var transformer = new NetworkPathTransformer();
        var inputPath = @"N:\Movies\Action";

        // Act
        var result = transformer.Transform(inputPath);

        // Assert
        Assert.AreEqual("Movies/Action/", result);
    }

    [TestMethod]
    public void NetworkPathTransformer_DoesNotTransformRegularPath()
    {
        // Arrange
        var transformer = new NetworkPathTransformer();
        var inputPath = @"C:\Movies\Action";

        // Act
        var result = transformer.Transform(inputPath);

        // Assert
        Assert.AreEqual(@"C:\Movies\Action", result);
    }

    [TestMethod]
    public void NetworkPathTransformer_HandlesNullPath()
    {
        // Arrange
        var transformer = new NetworkPathTransformer();

        // Act
        var result = transformer.Transform(null);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void NetworkPathTransformer_HandlesEmptyPath()
    {
        // Arrange
        var transformer = new NetworkPathTransformer();

        // Act
        var result = transformer.Transform(string.Empty);

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void NetworkPathTransformer_IsCaseInsensitive()
    {
        // Arrange
        var transformer = new NetworkPathTransformer();
        var inputPath = @"n:\Movies\Action";

        // Act
        var result = transformer.Transform(inputPath);

        // Assert
        Assert.AreEqual("Movies/Action/", result);
    }
}
