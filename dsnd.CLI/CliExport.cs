using Dsnd.Core.Riff;

namespace Dsnd.CLI
{
    internal class CliExport
    {
        public static void DoExportToWavs(string[] args, GetOptions argOptions)
        {
            DirectoryInfo exportRootDirectory;
            if (argOptions.TryGetValue(CliOptions.ExportPathTag_p, out var str) && str != null)
            {
                exportRootDirectory = new DirectoryInfo(str);
            }
            else
            {
                throw new ArgumentException($"Export path is not set, use {CliOptions.ExportPathTag_p} to set.");
            }

            var recurseSubdirectories = argOptions.TagExist(CliOptions.RecursiveTag_r);
            int tasks = CliOptions.GetTasks(argOptions);
            var directory = args[0];

            Console.WriteLine($"Directory: {directory}");

            IEnumerable<string> files;
            if (directory.Contains('*') || directory.Contains('?'))
            {
                var p = directory.LastIndexOf(Path.DirectorySeparatorChar);
                var searchDirectory = p != -1 ? directory.Substring(0, p) : directory;
                var pattern = p != -1 ? directory.Substring(p + 1) : directory;

                if (!(new DirectoryInfo(searchDirectory).Exists))
                {
                    Console.WriteLine($"Directory {searchDirectory} does not exist");
                    searchDirectory = Directory.GetCurrentDirectory();
                }

                Console.WriteLine($"Searching directory {searchDirectory}, pattern {pattern}");
                files = Directory.EnumerateFiles(searchDirectory, pattern, new EnumerationOptions { RecurseSubdirectories = recurseSubdirectories });
            }
            else
            {
                files = [directory];
            }

            Console.WriteLine($"Number of files: {files.Count()}");

            Parallel.ForEach(files, new ParallelOptions { MaxDegreeOfParallelism = tasks }, filename =>
            {
                var fi = new FileInfo(filename);

                if (fi.Exists)
                {
                    if (fi?.Extension.ToLower() == ".dsnd")
                    {
                        Console.WriteLine($"Exporting {filename}");

                        var dsndSound = new ParseRiff().ParseDsndFile(fi);
                        var exportSamplesDirectory = new DirectoryInfo($"{exportRootDirectory}{Path.DirectorySeparatorChar}{dsndSound.Name}");

                        Console.WriteLine($"Exporting {filename} to {exportSamplesDirectory}");
                        new ExportRiff().ExportSamples(exportSamplesDirectory, dsndSound);
                    }
                }
                else
                {
                    Console.WriteLine($"{filename} does not exists.");
                }

            });
        }
    }
}