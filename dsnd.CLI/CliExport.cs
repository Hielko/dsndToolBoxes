using Dsnd.Core.Riff;

namespace Dsnd.CLI
{
    internal class CliExport
    {
        public static void DoExportToWavs(string[] args, GetOptions argOptions)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var recurseSubdirectories = argOptions.TagExist(CliOptions.RecursiveTag_r);
            int tasks = CliOptions.GetTasks(argOptions);

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
                {
                    if (fi?.Extension.ToLower() == ".dsnd")
                    {
                        var dsndSound = new ParseRiff().ParseDsndFile(fi);

                        Console.WriteLine($"Exporting {filename}");

                        DirectoryInfo exportRootDirectory;

                        if (argOptions.TryGetValue(CliOptions.ExportPathTag_p, out var exportDirectoryStr))
                        {
                            exportRootDirectory = new DirectoryInfo(exportDirectoryStr);
                        }
                        else
                        {
                            exportRootDirectory = new DirectoryInfo(fi.FullName + Path.DirectorySeparatorChar+"Export"); // + Path.DirectorySeparatorChar + "Export");
                        }

                        Console.WriteLine($"Exporting {filename} to {exportRootDirectory}");

                        new ExportRiff().ExportSamples(exportRootDirectory, dsndSound);
                    }
                }
            });

        }

    }
}
