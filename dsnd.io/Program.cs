using DSND;

Console.WriteLine("Export DSND file(s)");

if (args.Length > 0)
{
    var arg0 = args[0];
    if (arg0.Contains('*') || arg0.Contains('?'))
    {
        var files = Directory.EnumerateFiles(Directory.GetCurrentDirectory(), arg0, new EnumerationOptions { RecurseSubdirectories = true });

        foreach (var filename in files)
        {
            var fi  = new FileInfo(filename);
            var dsndSound = new RiffParser().ParseDsndFile(fi);
            Console.WriteLine($"Exporting {filename}");

            var pathstr = fi.DirectoryName;
            var p = pathstr.LastIndexOf(Path.DirectorySeparatorChar);
            pathstr = p>0? pathstr.Substring(p+1): pathstr; 
            var path = new DirectoryInfo(pathstr+"_Export");
            new Export().ExportSamples(path, dsndSound);
        }


        // Parallel.ForEach(files, new ParallelOptions { MaxDegreeOfParallelism = 2 }, filename =>
        //{
        //    var dsndSound = new RiffParser().ParseDsndFile(new FileInfo(filename));
        //    Console.WriteLine($"Exporting {filename}");
        //    new Export().ExportSamples(null, dsndSound);
        //});
    }
    else
    {
        var fileInfo = new FileInfo(args[0]);
        var dsndSound = new RiffParser().ParseDsndFile(fileInfo);
        Console.WriteLine($"Exporting {fileInfo.Name}");
        new Export().ExportSamples(null, dsndSound);
    }
}
else
{
    Console.WriteLine("Usage: dsnd.io.exe filename or dsnd.io.exe wildcard, *.* for example");
}