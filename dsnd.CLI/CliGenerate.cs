using Dsnd.Core.Riff;

namespace Dsnd.CLI
{
    /// <summary>
    ///  Generate .dsnd file(s) from samples
    /// </summary>
    internal class CliGenerate
    {
        private static string[]? SplitDir(string? name)
        {
            var split = name?.Split(Path.DirectorySeparatorChar);
            return split;
        }

        private static string? FindRootOfSamples(FileInfo fileInfo)
        {
            var directoryParts = SplitDir(fileInfo.DirectoryName);
            string? dirWithOutWavs = null;

            if (directoryParts == null || directoryParts.Length == 0)
            {
                return null;
            }

            for (int i = directoryParts.Length; i > 1; i--)
            {
                var p = string.Join(Path.DirectorySeparatorChar, directoryParts.Take(i));
                var hasWaveFiles = Directory.EnumerateFiles(p, "*.wav", new EnumerationOptions { RecurseSubdirectories = false });
                if (!hasWaveFiles.Any())
                {
                    dirWithOutWavs = p;
                    break;
                }
            }

            return dirWithOutWavs;
        }

        private static bool TryParseZoneInfo(string directoryName, out int zoneNr, out string zoneName)
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
                    return false;
                }
                zoneName = directoryName.Substring(p + 1);
                return true;
            }
            return false;
        }

        public static void DoGenerateDsnd(string[] args, GetOptions argOptions)
        {
            DirectoryInfo? importDirectory = null;
            DirectoryInfo? exportDirectory = null;

            if (argOptions.TryGetValue(CliOptions.GenerateTag_g, out var importDirectoryStr) && importDirectoryStr != null)
            {
                importDirectory = new DirectoryInfo(importDirectoryStr);
            }
            else
            {
                throw new Exception($"Directory not specified after {CliOptions.GenerateTag_g}");
            }

            if (argOptions.TryGetValue(CliOptions.ExportPathTag_p, out var exportPathTag) && exportPathTag != null)
            {
                exportDirectory = new DirectoryInfo(exportPathTag);
            }

            var allFiles = Directory.EnumerateFiles(importDirectory.FullName, "*.*", new EnumerationOptions { RecurseSubdirectories = true });
            var fileinfos = allFiles.Select(f => new FileInfo(f)).ToList();
            var lookups = fileinfos.ToLookup(fi => FindRootOfSamples(fi));

            foreach (var lu in lookups)
            {
                var rootDir = lu.Key;
                if (rootDir == null)
                {
                    break;
                }

                var dsndName = rootDir.Substring(rootDir.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                var directories = Directory.EnumerateDirectories(rootDir, "*", new EnumerationOptions { ReturnSpecialDirectories = false, RecurseSubdirectories = true });

                if (directories.Any())
                {
                    var dsndSound = new NewDsnd(dsndName);
                    int zoneCount = 1;

                    foreach (var directory in directories)
                    {
                        var velocityFilename = Path.Combine(directory, Dsnd.Core.Constants.Velocities.VelocitiesMapFilename);
                        var velocityMap = new VelocityMap(velocityFilename);
                        var lastPartDirectory = directory.Substring(directory.LastIndexOf(Path.DirectorySeparatorChar) + 1);

                        if (!TryParseZoneInfo(lastPartDirectory, out zoneCount, out var zoneName))
                        {
                            zoneName = lastPartDirectory;
                            zoneCount++;
                        }
                        else
                        {
                            Console.WriteLine($"Can't read zone number from {lastPartDirectory}");
                        }

                        var zone = dsndSound.AddZone(zoneName, zoneCount);
                        var waveFiles = Directory.EnumerateFiles(directory, "*.wav", new EnumerationOptions { RecurseSubdirectories = false });
                        foreach (var f in waveFiles)
                        {
                            var fi = new FileInfo(f);
                            var velocity = velocityMap.FindVelocity(fi) ?? throw new Exception($"Can't find velocity for {fi.FullName} in {velocityFilename}");
                            velocity += 152; // Add 152 to convert from 0-127 to 152-279 range used in dsnd files
                            dsndSound.AddLayer(zone, fi, velocity);
                        }
                    }

                    var dsndDirectory = exportDirectory != null ? exportDirectory.FullName : rootDir;
                    Directory.CreateDirectory(dsndDirectory);

                    var dsndPathAndFilename = $"{dsndDirectory}{Path.DirectorySeparatorChar}{dsndName}.dsnd";
                    Console.WriteLine($"Writing {dsndPathAndFilename}");
                    dsndSound.Save(new FileInfo(dsndPathAndFilename));
                }
            }
        }
    }
}