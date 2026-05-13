using Dsnd.CLI;

var argOptions = new GetOptions(args);
Dsnd.Core.DsndOptions.OverwriteExistingFiles = argOptions.TagExist(CliOptions.OverwriteTag_o);

// Export dsnd to seperate wav's:
// "s:\\dd_SNRE_571_Gaynor.dsnd" -e  -p "s:\\export_directory"
// "s:\\*.*" -e -p "s:\\export1" -o -r

// Generate dsnd:
// -g "s:\\export1"


if (argOptions.TagExist(CliOptions.ExportTag_e))
{
    Console.WriteLine("Export DSND file(s) to seperate wav files");
    CliExport.DoExportToWavs(args, argOptions);
}
else
if (argOptions.TagExist(CliOptions.GenerateTag_g))
{
    Console.WriteLine("Create DSND file(s) from wav files");
    CliGenerate.DoGenerateDsnd(args, argOptions);
}

if (args.Length == 0)
{
    Console.WriteLine($"Usage: {AppDomain.CurrentDomain.FriendlyName} filename or {AppDomain.CurrentDomain.FriendlyName} wildcard, *.* for example");
    Console.WriteLine($"Main usage:  {CliOptions.ExportTag_e} does the export, or ");
    Console.WriteLine($"             {CliOptions.GenerateTag_g} generates dsnd file(s)");

    Console.WriteLine($"Options:  {CliOptions.TasksTag_t} <n> where n is number of max threads, minimal 2 when present");
    Console.WriteLine($"          {CliOptions.RecursiveTag_r} recursive");
    Console.WriteLine($"          {CliOptions.OverwriteTag_o} overwrite existing files");
    Console.WriteLine($"          {CliOptions.ExportPathTag_p} \"export root directory\"");
    return;
}