using System.Text;

try
{
    if (args?.Length == 3)
    {
        Scan(new DirectoryInfo(args[0]), args[1], args[2]);
    }
    else
    {
        Console.WriteLine("Missing arguments.");
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    Console.ReadLine();
}
finally
{
    //Console.WriteLine("Press <Enter> to exit.");
}

void Scan(DirectoryInfo folder, string searchPatterns, string outputFileName)
{
    var searchPatternList = searchPatterns.Split(',');

    var files = searchPatternList
        .SelectMany(p => folder.GetFiles(p, SearchOption.AllDirectories));

    var uniqueFolders = files
        .Select(f => f.Directory)
        .Select(GetFolderName)
        .Distinct()
        .OrderBy(f => f);

    var outputFile = new FileInfo(Path.Combine(folder.FullName, outputFileName));

    Backup($"{outputFile.FullName}.old", $"{outputFile.FullName}.old.old");
    Backup(outputFile.FullName, $"{outputFile.FullName}.old");

    using var sw = new StreamWriter(outputFile.FullName, false, Encoding.UTF8);

    foreach (var uniqueFolder in uniqueFolders)
    {
        var line = uniqueFolder.Substring(folder.FullName.Length + 1);

        sw.WriteLine(line);
    }
}

void Backup(string sourceFileName, string targetFileName)
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

string GetFolderName(DirectoryInfo folder)
{
    if (folder.Name.StartsWith("cd", StringComparison.OrdinalIgnoreCase))
    {
        return folder.Parent.FullName;
    }
    else if (folder.Name.StartsWith("part", StringComparison.OrdinalIgnoreCase))
    {
        return folder.Parent.FullName;
    }
    else if (folder.Name.StartsWith("disc", StringComparison.OrdinalIgnoreCase))
    {
        return folder.Parent.FullName;
    }
    else
    {
        return folder.FullName;
    }
}