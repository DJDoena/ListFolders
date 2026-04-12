namespace DoenaSoft.FolderList;

/// <summary>
/// Defines a contract for transforming file paths during XML cleaning operations.
/// </summary>
public interface IPathTransformer
{
    /// <summary>
    /// Transforms a full file path according to custom logic.
    /// </summary>
    /// <param name="fullPath">The original full path to transform.</param>
    /// <returns>The transformed path, or the original path if no transformation is needed.</returns>
    string Transform(string fullPath);
}