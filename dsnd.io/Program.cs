using DSND;
using DsndCLI;
using Utils;

var argOptions = new GetOpt(args);
DsndOptions.OverwriteExistingFiles = argOptions.TagExist(CliOptions.OverwriteTag);

if (argOptions.TagExist(CliOptions.ExportTag))
{
    Console.WriteLine("Export DSND file(s)");
    new CliExport().DoExport(args, argOptions);
}

if (argOptions.TagExist(CliOptions.GenerateTag))
{
    Console.WriteLine("Create DSND file(s) from wav files");
    new CliGenerate().DoGenerate(args, argOptions);
}




if (args.Length == 0)
{
    Console.WriteLine($"Usage: {AppDomain.CurrentDomain.FriendlyName} filename or {AppDomain.CurrentDomain.FriendlyName} wildcard, *.* for example");
    Console.WriteLine($"Options:  {CliOptions.ExportTag} does the export, or ");
    Console.WriteLine($"          {CliOptions.GenerateTag} generates dsnd file(s)");

    Console.WriteLine($"          {CliOptions.TaksTag} <n> where n is number of max threads, minimal 2 when present");
    Console.WriteLine($"          {CliOptions.RecurseTag} recursive");
    Console.WriteLine($"          {CliOptions.OverwriteTag} overwrite existing files");
    Console.WriteLine($"          {CliOptions.ExportPathTag} export directory");
    return;
}