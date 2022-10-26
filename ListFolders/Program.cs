using System;
using System.IO;
using System.Linq;
using System.Text;

namespace ListFolders
{
    internal static class Program
    {
        internal static void Main(string[] args)
        {
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
        }

        private static void Scan(DirectoryInfo folder, string searchPatterns, string outputFileName)
        {
            var searchPatternList = searchPatterns.Split(',');

            var files = searchPatternList.SelectMany(p => folder.GetFiles(p, SearchOption.AllDirectories));

            var uniqueFolders = files.Select(f => f.Directory).Select(GetFolderName).Distinct().OrderBy(f => f);

            var outputFile = new FileInfo(Path.Combine(folder.FullName, outputFileName));

            var oldFile = new FileInfo(outputFile.FullName + ".old");

            if (oldFile.Exists)
            {
                oldFile.Delete();
            }

            if (outputFile.Exists)
            {
                outputFile.MoveTo(outputFile.FullName + ".old");

                outputFile = new FileInfo(Path.Combine(folder.FullName, outputFileName));
            }

            using (var sw = new StreamWriter(outputFile.FullName, false, Encoding.UTF8))
            {
                foreach (var uniqueFolder in uniqueFolders)
                {
                    var line = uniqueFolder.Substring(folder.FullName.Length + 1);

                    sw.WriteLine(line);
                }
            }
        }

        private static string GetFolderName(DirectoryInfo folder)
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
    }
}
