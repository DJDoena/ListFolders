using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using ListFolders;

Console.WriteLine($"v{Assembly.GetExecutingAssembly().GetName().Version}");

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
        .Where(f => f != folder.FullName)
        .Distinct()
        .OrderBy(f => f)
        .ToList();

    var rootItem = new RootItem()
    {
        Item = new()
    };

    foreach (var folderName in uniqueFolderNames)
    {
        var line = folderName[(folder.FullName.Length + 1)..];

        ProcessLine(rootItem, line);
    }

    CleanUp(ref rootItem.Item);

    WriteFile(folder, outputFileName, rootItem);

    //var lines = new List<string[]>();

    //var indents = new List<int>();

    //foreach (var folderName in uniqueFolderNames)
    //{
    //    var line = folderName[(folder.FullName.Length + 1)..];

    //    ProcessLine(lines, indents, line);
    //}

    //WriteFile(folder, outputFileName, lines, indents);
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

static void ProcessLine(RootItem rootItem, string line)
{
    var lineParts = line.Split('\\');

    ProcessSegment(lineParts, 0, rootItem.Item);
}

static void ProcessSegment(string[] cells, int cellIndex, List<SubItem> items)
{
    var cell = cells[cellIndex];

    var item = items.FirstOrDefault(i => i.Name == cell);

    if (item == null)
    {
        item = new SubItem()
        {
            Name = cell,
            Item = new()
        };

        items.Add(item);
    }

    var nextCellIndex = cellIndex + 1;

    if (cells.Length > nextCellIndex)
    {
        ProcessSegment(cells, nextCellIndex, item.Item);
    }
}

static void CleanUp(ref List<SubItem> items)
{
    if (items != null && items.Count == 0)
    {
        items = null;
    }
    else
    {
        foreach (var item in items)
        {
            CleanUp(ref item.Item);
        }
    }
}

void WriteFile(DirectoryInfo folder, string outputFileName, RootItem rootItem)
{
    var outputFile = new FileInfo(Path.Combine(folder.FullName, outputFileName));

    Backup($"{outputFile.FullName}.old", $"{outputFile.FullName}.old.old");

    Backup(outputFile.FullName, $"{outputFile.FullName}.old");

    var xml = Serialize(rootItem);

    xml = "\t" + xml.Replace(Environment.NewLine, Environment.NewLine + "\t");

    using (var sw = new StreamWriter(outputFile.FullName, false, Encoding.UTF8))
    {
        sw.WriteLine(GetPrefix());
        sw.WriteLine(xml);
        sw.Write(GetSuffix());
    }

    File.SetAttributes(outputFile.FullName, FileAttributes.Archive);
}

static string Serialize(RootItem instance)
{
    using var ms = new MemoryStream();

    var encoding = Encoding.UTF8;

    var settings = new XmlWriterSettings()
    {
        Encoding = encoding,
        Indent = true,
        OmitXmlDeclaration = true,
        IndentChars = "\t",
    };

    using var writer = XmlWriter.Create(ms, settings);

    var serializer = new XmlSerializer(typeof(RootItem));

    serializer.Serialize(writer, instance, new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty }));

    var xml = encoding.GetString(ms.ToArray());

    return xml;
}

static string GetPrefix()
=> "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<?xml-stylesheet type=\"text/xml\" href=\"#stylesheet\"?>\r\n<!DOCTYPE doc [\r\n<!ATTLIST xsl:stylesheet\r\n    id    ID    #REQUIRED>\r\n]>\r\n<doc>";

static string GetSuffix()
=> "\t<xsl:stylesheet id=\"stylesheet\" version=\"1.0\" xmlns:xsl=\"http://www.w3.org/1999/XSL/Transform\">\r\n\t\t<xsl:template name=\"PrintItem\">\r\n\t\t\t<xsl:param name=\"item\" />\r\n\t\t\t\t<li>\r\n\t\t\t\t\t<xsl:value-of select=\"@Name\"/>\r\n\t\t\t\t\t<xsl:call-template name=\"PrintItems\">\r\n\t\t\t\t\t\t<xsl:with-param name=\"items\" select=\"$item/Item\" />\r\n\t\t\t\t\t</xsl:call-template>\r\n\t\t\t\t</li>\r\n\t\t</xsl:template>\r\n\t\t\t\t\r\n\t\t<xsl:template name=\"PrintItems\">\r\n\t\t\t<xsl:param name=\"items\" />\r\n\t\t\t<xsl:if test=\"count($items) > 0\">\r\n\t\t\t\t<ul>\r\n\t\t\t\t\t<xsl:for-each select=\"$items\">\r\n\t\t\t\t\t\t<xsl:call-template name=\"PrintItem\">\r\n\t\t\t\t\t\t\t<xsl:with-param name=\"item\" select=\"current()\" />\r\n\t\t\t\t\t\t</xsl:call-template>\t\r\n\t\t\t\t\t</xsl:for-each>\r\n\t\t\t\t</ul>\r\n\t\t\t</xsl:if>\r\n\t\t</xsl:template>\r\n\r\n\t\t<xsl:template match=\"/\">\r\n\t\t\t<html>\r\n\t\t\t\t<body>\r\n\t\t\t\t\t<xsl:call-template name=\"PrintItems\">\r\n\t\t\t\t\t\t<xsl:with-param name=\"items\" select=\"//doc/RootItem/Item\" />\r\n\t\t\t\t\t</xsl:call-template>\r\n\t\t\t\t</body>\r\n\t\t\t</html>\r\n\t\t</xsl:template>\r\n\t</xsl:stylesheet>\r\n</doc>";



//static void ProcessLine(List<string[]> lines, List<int> indents, string line)
//{
//    var lineParts = line.Split('\\');

//    lines.Add(lineParts);

//    for (var linePartIndex = 0; linePartIndex < lineParts.Length; linePartIndex++)
//    {
//        var linePart = lineParts[linePartIndex];

//        var linePartLength = linePart.Length;

//        if (linePartIndex > indents.Count - 1)
//        {
//            indents.Add(linePartLength);
//        }
//        else if (linePartLength > indents[linePartIndex])
//        {
//            indents[linePartIndex] = linePartLength;
//        }
//    }
//}

//static void WriteFile(DirectoryInfo folder, string outputFileName, List<string[]> lines, List<int> indents)
//{
//    var outputFile = new FileInfo(Path.Combine(folder.FullName, outputFileName));

//    Backup($"{outputFile.FullName}.old", $"{outputFile.FullName}.old.old");

//    Backup(outputFile.FullName, $"{outputFile.FullName}.old");

//    using var sw = new StreamWriter(outputFile.FullName, false, Encoding.UTF8);

//    for (var lineIndex = 0; lineIndex < lines.Count; lineIndex++)
//    {
//        var currentRow = lines[lineIndex];

//        var lastColumnIndex = currentRow.Length - 1;

//        for (var columnIndex = 0; columnIndex < lastColumnIndex; columnIndex++)
//        {
//            if (lineIndex == 0)
//            {
//                WriteCell(indents, sw, columnIndex, currentRow[columnIndex]);
//            }
//            else
//            {
//                var previousRow = lines[lineIndex - 1];

//                if (previousRow.Length <= columnIndex)
//                {
//                    WriteCell(indents, sw, columnIndex, currentRow[columnIndex]);
//                }
//                else
//                {
//                    var currentRowCell = currentRow[columnIndex];

//                    var previousRowCell = previousRow[columnIndex];

//                    if (currentRowCell != previousRowCell)
//                    {
//                        WriteCell(indents, sw, columnIndex, currentRowCell);
//                    }
//                    else
//                    {
//                        WriteCell(indents, sw, columnIndex, string.Empty);
//                    }
//                }
//            }
//        }

//        sw.WriteLine(currentRow[lastColumnIndex]);
//    }
//}

//static void WriteCell(List<int> indents, StreamWriter sw, int columnIndex, string cell)
//{
//    sw.Write(cell.PadRight(indents[columnIndex]));
//    sw.Write("   ");
//}

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