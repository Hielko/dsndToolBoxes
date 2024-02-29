using DSND;

Console.WriteLine("Export DSND file(s)");


if (args.Length > 0)
{
    var currentDirectory = Directory.GetCurrentDirectory();
    var argOptions = new GetOpt(args);
    var recurseSubdirectories = argOptions.TagExist("-r");
    DSNDOptions.OverwriteExistingFiles = argOptions.TagExist("-o");
    int tasks = 1;
    if (argOptions.TryGetValue("-t", out var tasksStr))
    {
        if (int.TryParse(tasksStr, out tasks))
        {
            if (tasks < 2)
                throw new Exception("tasks must be absent or > 1");
            if (tasks > 16)
                throw new Exception("tasks cant be more than 16");
        }
    }

    DirectoryInfo exportDirectory = null;
    if (argOptions.TryGetValue("-e", out var exportDirectoryStr))
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

        if (!(new DirectoryInfo(searchDirectory).Exists)) {
            searchDirectory = currentDirectory;
        }

        Console.WriteLine($"searchDirectory {searchDirectory}, pattern {pattern}");
        files = Directory.EnumerateFiles(searchDirectory, pattern, new EnumerationOptions { RecurseSubdirectories = recurseSubdirectories });
    }
    else
    {
        files = new List<string>() { arg0 };
    }

    Console.WriteLine($"Files: {files.Count()}");

    Parallel.ForEach(files, new ParallelOptions { MaxDegreeOfParallelism = tasks }, filename =>
    {
        var fi = new FileInfo(filename);

        if (!fi.Exists)
        {
            Console.WriteLine($"{filename} does not exists.");
        } else
        if (fi.Extension.ToLower() == ".dsnd")
        {
            // Strip drive
            var tmp = fi.DirectoryName;
            while (tmp[0] != Path.DirectorySeparatorChar)
            {
                tmp = tmp.Remove(0, 1);
            }

            var dsndSound = new RiffParser().ParseDsndFile(fi);
            Console.WriteLine($"Exporting {filename}");

            var pathstr = fi.DirectoryName;
            var p = pathstr.LastIndexOf(Path.DirectorySeparatorChar);
            pathstr = p > 0 ? pathstr.Substring(p + 1) : pathstr;
            var path = new DirectoryInfo(exportDirectory.FullName + tmp + Path.DirectorySeparatorChar + pathstr + "_Export");

            Console.WriteLine($"Exporting {filename} to {path}");

            new Export().ExportSamples(path, dsndSound);
        }
    });

}

else
{
    Console.WriteLine("Usage: dsnd.io.exe filename or dsnd.io.exe wildcard, *.* for example");
    Console.WriteLine("Options:  -t <n> where n is number of max threads, minimal 2 when present");
    Console.WriteLine("          -r recursive");
    Console.WriteLine("          -o overwrite existing files");
    Console.WriteLine("          -e export directory");
}