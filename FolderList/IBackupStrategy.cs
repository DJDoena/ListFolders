namespace DoenaSoft.FolderList;

/// <summary>
/// Defines a contract for managing backup files during XML serialization.
/// </summary>
public interface IBackupStrategy
{
    /// <summary>
    /// Creates backups of the output file before writing a new version.
    /// </summary>
    /// <param name="outputFilePath">The full path to the output file that will be created.</param>
    /// <returns>The path to the most recent backup file (if any), or null if no backup was created.</returns>
    /// <remarks>
    /// This method is called before the new output file is written. Implementations should
    /// handle moving/copying existing files to backup locations according to their backup strategy.
    /// Common strategies include: .old/.old.old chains, timestamped backups, or versioned backups.
    /// </remarks>
    string CreateBackups(string outputFilePath);
}
