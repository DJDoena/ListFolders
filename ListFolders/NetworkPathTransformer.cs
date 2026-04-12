using DoenaSoft.FolderList;

namespace DoenaSoft.ListFolders;

/// <summary>
/// Transforms network paths starting with "N:\" to use forward slashes and relative format.
/// </summary>
/// <remarks>
/// This transformer converts paths like "N:\Folder\File" to "Folder/File/".
/// </remarks>
internal sealed class NetworkPathTransformer : IPathTransformer
{
    /// <summary>
    /// Transforms paths starting with "N:\" by removing the drive letter and converting backslashes to forward slashes.
    /// </summary>
    /// <param name="fullPath">The original full path.</param>
    /// <returns>The transformed path if it starts with "N:\", otherwise the original path.</returns>
    public string Transform(string fullPath)
    {
        if (string.IsNullOrEmpty(fullPath))
        {
            return fullPath;
        }

        if (fullPath.StartsWith(@"N:\", StringComparison.InvariantCultureIgnoreCase))
        {
            var cleanedPath = fullPath.Substring(3).Replace("\\", "/").TrimEnd('/') + "/";

            return cleanedPath;
        }

        return fullPath;
    }
}