using dsnd.Models;
using Utils;
using static Dsnd.Constants;

namespace Dsnd.CLI
{
    /// <summary>
    ///  Generate .dsnd file(s) from samples
    /// </summary>
    internal class CliGenerate
    {
        static string[] SplitDir(string? name)
        {
            string[] split = name.Split(Path.DirectorySeparatorChar);
            return split;
        }

        static string FindRootOfSamples(FileInfo fileInfo)
        {
            var directoryParts = SplitDir(fileInfo.DirectoryName);
            string dirWithOutWavs = null;

            for (int i = directoryParts.Length; i > 1; i--)
            {
                var p = string.Join(Path.DirectorySeparatorChar, directoryParts.Take(i));
                var haswavs = Directory.EnumerateFiles(p, "*.wav", new EnumerationOptions { RecurseSubdirectories = false });
                if (!haswavs.Any())
                {
                    dirWithOutWavs = p;
                    break;
                }
            }

            return dirWithOutWavs;
        }

        static bool TryParseZoneInfo(string directoryName, out int zoneNr, out string zoneName)
        {
            zoneNr = -1;
            zoneName = string.Empty;

            var p = directoryName.IndexOf("_");
            if (p >= 0)
            {
                var i = p - 1;
                while (Char.IsDigit(directoryName[i]) && i > 0) i--;
                i++;
                string num = directoryName.Substring(i, p - i);
                if (!int.TryParse(num, out zoneNr))
                {
                    Console.WriteLine($"Can't read zone number from {directoryName}");
                    return false;
                }
                zoneName = directoryName.Substring(p + 1);
                return true;
            }
            return false;
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
                    var dsndSound = new NewDsnd(dsndName);
                    int zoneNr = 1;

                    foreach (var directory in directories)
                    {
                        string velocityFilename = Path.Combine(directory, Velocities.VelocitiesMapFilename);
                        VelocityMap velocityMap = new(velocityFilename);

                        string lastPartDirectory = directory.Substring(directory.LastIndexOf(Path.DirectorySeparatorChar) + 1);

                        if (!TryParseZoneInfo(lastPartDirectory, out zoneNr, out var zoneName))
                        {
                            zoneName = lastPartDirectory;
                            zoneNr++;
                        }

                        var zone = dsndSound.AddZone(zoneName, zoneNr);
                        files = Directory.EnumerateFiles(directory, "*.wav", new EnumerationOptions { RecurseSubdirectories = false });
                        foreach (var f in files)
                        {
                            var fi = new FileInfo(f);
                            dsndSound.AddLayer(zone, fi, velocityMap.FindVelocity(fi));
                        }

                    }

                    string dndDirectory = exportDirectory != null ? exportDirectory.FullName : rootDir;
                    Directory.CreateDirectory(dndDirectory);

                    var dsndFilename = $"{dndDirectory}{Path.DirectorySeparatorChar}{dsndName}.dsnd";
                    Console.WriteLine($"Writing {dsndFilename}");
                    dsndSound.Save(new FileInfo(dsndFilename));
                }

            }
        }
    }
}
