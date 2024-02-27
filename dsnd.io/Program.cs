using DSND;

Console.WriteLine("Export DSND file(s)");

if (args.Length > 0)
{
    var argOptions = new GetOpt(args);

    var recurseSubdirectories = argOptions.ExistValue("-r");

    var overwriteExistingFiles = argOptions.ExistValue("-o");

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

    var arg0 = args[0];
    IEnumerable<string> files;
    if (arg0.Contains('*') || arg0.Contains('?'))
    {
        files = Directory.EnumerateFiles(Directory.GetCurrentDirectory(), arg0, new EnumerationOptions { RecurseSubdirectories = recurseSubdirectories });
    }
    else
    {
        files = new List<string>() { arg0 };
    }

    Parallel.ForEach(files, new ParallelOptions { MaxDegreeOfParallelism = tasks }, filename =>
    {
        var fi = new FileInfo(filename);
        if (!fi.Exists)
        {
            Console.WriteLine($"{filename} does not exists.");
        }
        if (fi.Extension.ToLower() == ".dsnd")
        {
            var dsndSound = new RiffParser().ParseDsndFile(fi);
            Console.WriteLine($"Exporting {filename}");

            var pathstr = fi.DirectoryName;
            var p = pathstr.LastIndexOf(Path.DirectorySeparatorChar);
            pathstr = p > 0 ? pathstr.Substring(p + 1) : pathstr;
            var path = new DirectoryInfo(pathstr + "_Export");
            new Export().ExportSamples(path, dsndSound);
        }
    });

}

else
{
    Console.WriteLine("Usage: dsnd.io.exe filename or dsnd.io.exe wildcard, *.* for example");
    Console.WriteLine("Options:  -t <n> where n is number of max threads, minimal 2 when present");
    Console.WriteLine("          -r recursive");
}