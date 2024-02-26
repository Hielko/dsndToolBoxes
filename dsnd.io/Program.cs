using DSND;

Console.WriteLine("Export DSND file(s)");

if (args.Length > 0)
{
    var arg0 = args[0];
    if (arg0.Contains('*') || arg0.Contains('?'))
    {
        foreach (var file in Directory.EnumerateFiles(Directory.GetCurrentDirectory(), arg0, new EnumerationOptions { RecurseSubdirectories = true }))
        {
            var dsndSound = new DSND.RiffParser().ParseDsndFile(new FileInfo(file));
            new Export().ExportSamples(null, dsndSound);
        }
    }
    else
    {
        var fileInfo = new FileInfo(args[0]);

        var dsndSound = new DSND.RiffParser().ParseDsndFile(fileInfo);

        new Export().ExportSamples(null, dsndSound);
    }
} else
{
    Console.WriteLine("Usage: dsnd.io.exe filename or dsnd.io.exe wildcard, *.* for example");
}