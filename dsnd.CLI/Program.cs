using Dsnd;
using Dsnd.CLI;
using Dsnd.CLI.Utils;

var argOptions = new GetOptions(args);
DsndOptions.OverwriteExistingFiles = argOptions.TagExist(CliOptions.OverwriteTag_o);

if (argOptions.TagExist(CliOptions.ExportTag_e))
{
    Console.WriteLine("Export DSND file(s)");
    CliExport.DoExport(args, argOptions);
}

if (argOptions.TagExist(CliOptions.GenerateTag_g))
{
    Console.WriteLine("Create DSND file(s) from wav files");
    CliGenerate.DoGenerate(args, argOptions);
}



if (args.Length == 0)
{
    Console.WriteLine($"Usage: {AppDomain.CurrentDomain.FriendlyName} filename or {AppDomain.CurrentDomain.FriendlyName} wildcard, *.* for example");
    Console.WriteLine($"Options:  {CliOptions.ExportTag_e} does the export, or ");
    Console.WriteLine($"          {CliOptions.GenerateTag_g} generates dsnd file(s)");

    Console.WriteLine($"          {CliOptions.TasksTag_t} <n> where n is number of max threads, minimal 2 when present");
    Console.WriteLine($"          {CliOptions.RecursiveTag_r} recursive");
    Console.WriteLine($"          {CliOptions.OverwriteTag_o} overwrite existing files");
    Console.WriteLine($"          {CliOptions.ExportPathTag_p} \"export directory\"");
    return;
}