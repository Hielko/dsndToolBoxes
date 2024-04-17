using Dsnd.CLI.Utils;
using Dsnd.Core.Riff;

namespace Dsnd.CLI
{
    internal class CliExport
    {
        public static void DoExport(string[] args, GetOptions argOptions)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var recurseSubdirectories = argOptions.TagExist(CliOptions.RecursiveTag_r);
            int tasks = CliOptions.GetTasks(argOptions);

            DirectoryInfo exportDirectory;
            if (argOptions.TryGetValue(CliOptions.ExportPathTag_p, out var exportDirectoryStr))
            {
                exportDirectory = new DirectoryInfo(exportDirectoryStr);
            }
            else
            {
                exportDirectory = new DirectoryInfo("Export"); // + Path.DirectorySeparatorChar + "Export");
            }

            var arg0 = args[0];
            Console.WriteLine($"Directory: {arg0}");

            IEnumerable<string> files;
            if (arg0.Contains('*') || arg0.Contains('?'))
            {
                var p = arg0.LastIndexOf(Path.DirectorySeparatorChar);
                var searchDirectory = p != -1 ? arg0.Substring(0, p) : arg0;
                var pattern = p != -1 ? arg0.Substring(p + 1) : arg0;

                if (!(new DirectoryInfo(searchDirectory).Exists))
                {
                    searchDirectory = currentDirectory;
                }

                Console.WriteLine($"searchDirectory {searchDirectory}, pattern {pattern}");
                files = Directory.EnumerateFiles(searchDirectory, pattern, new EnumerationOptions { RecurseSubdirectories = recurseSubdirectories });
            }
            else
            {
                files = new List<string>() { arg0 };
            }

            Console.WriteLine($"Number of files: {files.Count()}");

            Parallel.ForEach(files, new ParallelOptions { MaxDegreeOfParallelism = tasks }, filename =>
            {
                var fi = new FileInfo(filename);

                if (!fi.Exists)
                {
                    Console.WriteLine($"{filename} does not exists.");
                }
                else
                if (fi.Extension.ToLower() == ".dsnd")
                {
                    // Strip drive
                    var tmp = fi.DirectoryName;
                    while (tmp[0] != Path.DirectorySeparatorChar)
                    {
                        tmp = tmp.Remove(0, 1);
                    }


                    var dsndSound = new ParseRiff().ParseDsndFile(fi);

                    Console.WriteLine($"Exporting {filename}");

                    var pathstr = fi.DirectoryName;
                    var p = pathstr.LastIndexOf(Path.DirectorySeparatorChar);
                    pathstr = p > 0 ? pathstr.Substring(p + 1) : pathstr;
                    //var path = new DirectoryInfo(exportDirectory.FullName + tmp + Path.DirectorySeparatorChar + pathstr + "_Export");

                    var path = new DirectoryInfo(Path.Combine(exportDirectory.FullName + tmp, pathstr + "_Export"));

                    Console.WriteLine($"Exporting {filename} to {path}");

                    new ExportRiff().ExportSamples(path, dsndSound);
                }
            });

        }

    }
}
