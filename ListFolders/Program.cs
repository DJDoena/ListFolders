using System.Reflection;
using DoenaSoft.ListFolders;

Console.WriteLine($"ListFolders - v{Assembly.GetExecutingAssembly().GetName().Version}");

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

static void Scan(DirectoryInfo folder, string searchPatterns, string outputFileName)
{
    Console.WriteLine($"Listing '{folder.FullName}' ('{searchPatterns}') to '{outputFileName}'");

    var folderNames = FolderGetter.Get(folder, searchPatterns);

    var rootItem = XmlCreator.Create(folder, folderNames);

    Cleaner.Clean(rootItem);    
    
    Serializer.Serialize(folder, outputFileName, rootItem);
}