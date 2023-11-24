using System.Text;

Console.WriteLine(typeof(Program).Assembly.GetName().Version);

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

    var uniqueFolderNames = files
        .Select(f => f.Directory)
        .Select(GetFolderName)
        .Distinct()
        .OrderBy(f => f)
        .ToList();

    var lines = new List<string[]>();

    var indents = new List<int>();

    foreach (var folderName in uniqueFolderNames)
    {
        var line = folderName.Substring(folder.FullName.Length + 1);

        ProcessLine(lines, indents, line);
    }

    WriteFile(folder, outputFileName, lines, indents);
}

static string GetFolderName(DirectoryInfo folder)
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

static void ProcessLine(List<string[]> lines, List<int> indents, string line)
{
    var lineParts = line.Split('\\');

    lines.Add(lineParts);

    for (var linePartIndex = 0; linePartIndex < lineParts.Length - 1; linePartIndex++)
    {
        var linePart = lineParts[linePartIndex];

        var linePartLength = linePart.Length;

        if (linePartIndex > indents.Count - 1)
        {
            indents.Add(linePartLength);
        }
        else if (linePartLength > indents[linePartIndex])
        {
            indents[linePartIndex] = linePartLength;
        }
    }
}

static void WriteFile(DirectoryInfo folder, string outputFileName, List<string[]> lines, List<int> indents)
{
    var outputFile = new FileInfo(Path.Combine(folder.FullName, outputFileName));

    Backup($"{outputFile.FullName}.old", $"{outputFile.FullName}.old.old");

    Backup(outputFile.FullName, $"{outputFile.FullName}.old");

    using var sw = new StreamWriter(outputFile.FullName, false, Encoding.UTF8);

    foreach (var lineParts in lines)
    {
        var lastIndex = lineParts.Length - 1;

        for (var linePartIndex = 0; linePartIndex < lastIndex; linePartIndex++)
        {
            sw.Write(lineParts[linePartIndex].PadRight(indents[linePartIndex]));
            sw.Write("   ");
        }

        sw.WriteLine(lineParts[lastIndex]);
    }
}

static void Backup(string sourceFileName, string targetFileName)
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