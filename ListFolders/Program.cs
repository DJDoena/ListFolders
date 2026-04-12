using DoenaSoft.FolderList;
using DoenaSoft.ListFolders;
using System.Diagnostics;
using System.Reflection;

Console.WriteLine($"ListFolders v{Assembly.GetExecutingAssembly().GetName().Version}");

try
{
    if (args?.Length == 3)
    {
        var rootFolder = (new DirectoryInfo(args[0]));

        Scan(rootFolder, args[1], args[2]);
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

static void Scan(DirectoryInfo folder, string searchPatterns, string outputFileName)
{
    Console.WriteLine($"Listing '{folder.FullName}' ('{searchPatterns}') to '{outputFileName}'");

    var creator = new Creator(new MultiDiscFolderConsolidator()
        , new NetworkPathTransformer()
        , new TwoLevelBackupStrategy());

    var (oldFileName, outFileName) = creator.Scan(folder, searchPatterns, outputFileName);

    if (File.Exists(oldFileName))
    {
        StartBeyondCompare(oldFileName, outFileName);
    }
}

static void StartBeyondCompare(string oldFileName, string outFileName)
{
    var bc5 = @"C:\Program Files\Beyond Compare 5\BCompare.exe";

    if (File.Exists(bc5))
    {
        var psi = new ProcessStartInfo(bc5);

        psi.ArgumentList.Add(outFileName);
        psi.ArgumentList.Add(oldFileName);

        var process = new Process()
        {
            StartInfo = psi,
        };

        process.Start();
    }
}