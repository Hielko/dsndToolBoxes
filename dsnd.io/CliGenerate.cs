using DSND;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace DsndCLI
{
    internal class CliGenerate
    {
        static string[] SplitDir(string name)
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

        public void DoGenerate(string[] args, GetOpt argOptions)
        {
            DirectoryInfo importDirectory = null;
            if (argOptions.TryGetValue(CliOptions.GenerateTag, out var str))
            {
                importDirectory = new DirectoryInfo(str);
            }
            else
            {
                throw new Exception($"Directory not specified after {CliOptions.GenerateTag}");
            }


            IEnumerable<string> files;
            files = Directory.EnumerateFiles(importDirectory.FullName, "*.*", new EnumerationOptions { RecurseSubdirectories = true });

            var fileinfos = files.Select(f => new FileInfo(f)).ToList();

            var gr = fileinfos.DistinctBy(fi => fi.DirectoryName);
            //    findRootOfSamples(gr, "");
            var gr2 = fileinfos.DistinctBy(fi => FindRootOfSamples(fi));

            var lookups = fileinfos.ToLookup(fi => FindRootOfSamples(fi));

            foreach (var lu in lookups)
            {
                var rootDir = lu.Key;

                var dirs = Directory.EnumerateDirectories(rootDir, "*", new EnumerationOptions { ReturnSpecialDirectories = false, RecurseSubdirectories = true });
                if (dirs.Any())
                {
                    var Dsnd = new NewDsnd("ff");
                    foreach (var d in dirs)
                    {
                        var zone = Dsnd.AddZone(d);
                        files = Directory.EnumerateFiles(d, "*.*", new EnumerationOptions { RecurseSubdirectories = false });
                        foreach (var f in files)
                        {
                            Dsnd.AddLayer(zone, new FileInfo(f));
                        }
                        Dsnd.Save(new FileInfo($"{rootDir}{Path.DirectorySeparatorChar}.blabla.dsnd"));
                            
                         
                    }
                }


                //  g.

            }



            foreach (var fi in fileinfos)
            {
                var root = FindRootOfSamples(fi);
                Console.WriteLine($"{fi.Name} - {root} ");

                //var fi = new FileInfo(fi);
                //if (fi.Extension.ToLower().Equals(".map")) {
                //}


            }



        }
    }
}
