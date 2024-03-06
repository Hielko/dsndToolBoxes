using DSND;
using Utils;

namespace Dsnd.CLI
{
    internal class CliGenerate
    {
        static string[] SplitDir(string? name)
        {
            string[] split = name.Split(Path.DirectorySeparatorChar);
            return split;
        }

        static string FindRootOfSamples(FileInfo fileInfo)
        {
            var dirs = SplitDir(fileInfo.DirectoryName);
            string dirWithOutWavs = null;

            for (int i = dirs.Length; i > 1; i--)
            {
                var p = string.Join(Path.DirectorySeparatorChar, dirs.Take(i));
                var haswavs = Directory.EnumerateFiles(p, "*.wav", new EnumerationOptions { RecurseSubdirectories = false });
                if (!haswavs.Any())
                {
                    dirWithOutWavs = p;
                    break;
                }
            }

            return dirWithOutWavs;
        }

        public static void DoGenerate(string[] args, GetOptions argOptions)
        {
            DirectoryInfo importDirectory = null;
            DirectoryInfo exportDirectory = null;
            if (argOptions.TryGetValue(CliOptions.GenerateTag_g, out var str))
            {
                importDirectory = new DirectoryInfo(str);
            }
            else
            {
                throw new Exception($"Directory not specified after {CliOptions.GenerateTag_g}");
            }
            if (argOptions.TryGetValue(CliOptions.ExportPathTag_p, out var exportPathTag))
            {
                exportDirectory = new DirectoryInfo(exportPathTag);
            }
            else
            {
                exportDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
            }

            var files = Directory.EnumerateFiles(importDirectory.FullName, "*.*", new EnumerationOptions { RecurseSubdirectories = true });
            var fileinfos = files.Select(f => new FileInfo(f)).ToList();
            var lookups = fileinfos.ToLookup(fi => FindRootOfSamples(fi));

            foreach (var lu in lookups)
            {
                var rootDir = lu.Key;
                string dsndName = rootDir.Substring(rootDir.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                var directories = Directory.EnumerateDirectories(rootDir, "*", new EnumerationOptions { ReturnSpecialDirectories = false, RecurseSubdirectories = true });

                if (directories.Any())
                {
                    var dsnd = new NewDsnd(dsndName);
                    foreach (var directory in directories)
                    {
                        var zone = dsnd.AddZone(directory);
                        files = Directory.EnumerateFiles(directory, "*.wav", new EnumerationOptions { RecurseSubdirectories = false });
                        foreach (var f in files)
                        {
                            dsnd.AddLayer(zone, new FileInfo(f));
                        }
                    }

                    string dndDirectory = exportDirectory != null ? exportDirectory.FullName : rootDir;
                    dsnd.Save(new FileInfo($"{dndDirectory}{Path.DirectorySeparatorChar}{dsndName}.dsnd"));
                }

            }
        }
    }
}
