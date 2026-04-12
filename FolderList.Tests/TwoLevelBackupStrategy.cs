using DoenaSoft.FolderList;

namespace DoenaSoft.FolderList.Tests;

/// <summary>
/// Implements a two-level backup strategy using .old and .old.old file extensions.
/// </summary>
/// <remarks>
/// This strategy maintains two generations of backups:
/// - current file → .old (when new file is written)
/// - .old → .old.old (when current is backed up to .old)
/// </remarks>
internal sealed class TwoLevelBackupStrategy : IBackupStrategy
{
    /// <summary>
    /// Creates backups by moving existing files through a two-level chain.
    /// </summary>
    /// <param name="outputFilePath">The full path to the output file that will be created.</param>
    /// <returns>The path to the .old backup file, or null if the output file didn't exist.</returns>
    public string CreateBackups(string outputFilePath)
    {
        if (string.IsNullOrEmpty(outputFilePath))
        {
            return null;
        }

        var oldOldFileName = $"{outputFilePath}.old.old";
        var oldFileName = $"{outputFilePath}.old";

        // Move .old to .old.old
        MoveFile(oldFileName, oldOldFileName);

        // Move current to .old
        MoveFile(outputFilePath, oldFileName);

        // Return the .old file path if it exists
        return File.Exists(oldFileName) ? oldFileName : null;
    }

    private static void MoveFile(string sourceFileName, string targetFileName)
    {
        if (File.Exists(sourceFileName))
        {
            if (File.Exists(targetFileName))
            {
                File.Delete(targetFileName);
            }

            File.Move(sourceFileName, targetFileName);
        }
    }
}
