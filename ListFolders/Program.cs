using System.Reflection;
using DoenaSoft.AbstractionLayer.IOServices;
using DoenaSoft.ListFolders;

Console.WriteLine($"ListFolders v{Assembly.GetExecutingAssembly().GetName().Version}");

try
{
    if (args?.Length == 3)
    {
        var rootFolder = (new IOServices()).GetFolder(args[0]);

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

static void Scan(IFolderInfo folder, string searchPatterns, string outputFileName)
{
    Console.WriteLine($"Listing '{folder.FullName}' ('{searchPatterns}') to '{outputFileName}'");

    var folderNames = FolderGetter.Get(folder, searchPatterns);

    var rootItem = XmlCreator.Create(folder, folderNames);

    Cleaner.Clean(rootItem);

    Serializer.Serialize(folder, outputFileName, rootItem);
}