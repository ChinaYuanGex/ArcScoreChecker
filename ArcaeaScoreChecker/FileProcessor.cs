using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using Microsoft.JSInterop;

namespace ArcaeaScoreChecker
{
    public class FileProcessor
    {
        public static int TryCreateDefaultFileIfFirst() {
            string basedir = FileSystem.AppDataDirectory;
            if (!File.Exists($"{basedir}/ptt.json")) {
                File.WriteAllBytes($"{basedir}/ptt.json", Properties.fileResource.ptt);
            }
            if (!ExtractDefaultSongFile()) return -2;
            return 0;
        }
        public static int TryReadGameSongFile() {
            if (!ExtractSongFiles()) return -2;
            return 0;
        }
        public static string? GetCurrentAssetAPK() {
            string[] execResult = ShellInterface.RootExecuteWithReturn("pm", "path moe.low.arc").Split('\n');
            
            string[] target = execResult.Where((x) => { return x.Contains("base.apk") || x.Contains("split_arcassets.apk"); }).ToArray();

            if (target.Length < 1)
            {
                return null;
            }

            string[] playstores = target.Where((x) => { return x.Contains("split_arcassets.apk"); }).ToArray();

            if (playstores.Length > 0)
            {
                return playstores[0].Split(':')[1];
            }
            else
            {
                return target[0].Split(':')[1];
            }

        }
        public static bool ExtractDefaultSongFile() {
            try
            {
                if (!Directory.Exists(FileSystem.AppDataDirectory + "/songs/"))
                {
                    MemoryStream stream = new MemoryStream();
                    stream.Write(Properties.fileResource.songs);
                    stream.Flush();
                    ZipArchive songzip = new ZipArchive(stream);
                    songzip.ExtractToDirectory(FileSystem.AppDataDirectory + "/songs/");
                    return true;
                }
                else return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool ExtractSongFiles()
        {
            string FilePath = GetCurrentAssetAPK();

            string baseSongPath = Path.Combine(FileSystem.AppDataDirectory, "songs");

            if (FilePath == null) {
                return false;
            }

            try
            {
                ZipArchive file = ZipFile.OpenRead(FilePath);
                ZipArchiveEntry[] allMatched = file.Entries.Where((x) =>
                {
                    return x.FullName.Contains("assets/songs/") 
                    && (!x.FullName.Contains(".ogg"))
                    && (!x.FullName.Contains(".aff"));
                }).ToArray();

                if (!Directory.Exists(baseSongPath))
                {
                    Directory.CreateDirectory(baseSongPath);
                }

                foreach(ZipArchiveEntry x in allMatched)
                {
                    string shortPath = x.FullName.Replace("assets/songs/", "");
                    string targetPath = Path.Combine(baseSongPath, shortPath);
                    string targetRootDir = targetPath.Substring(0, targetPath.LastIndexOf("/"));
                    if (!Directory.Exists(targetRootDir))
                    {
                        Directory.CreateDirectory(targetRootDir);
                    }
                    x.ExtractToFile(targetPath, true);
                }
                return true;
            }
            catch(Exception ex) {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public static bool CopySt3Database()
        {
            try
            {
                ShellInterface.RootExecute("cp", $"/data/data/moe.low.arc/files/st3 {Path.Combine(FileSystem.AppDataDirectory, "st3")}");
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool SaveFile(string path,byte[] data) {
            try
            {
                File.WriteAllBytes(path, data);
                return true;
            }
            catch { return false; }
        }
        public static int ExtractAPK(Stream zipstream)
        {
            try
            {
                ZipArchive arch = new ZipArchive(zipstream);

                List<ZipArchiveEntry> targetExtract = new List<ZipArchiveEntry>();

                foreach (ZipArchiveEntry en in arch.Entries)
                {
                    if (
                        en.FullName.Contains("assets/songs/") &&
                        (
                            en.FullName.Contains("songlist") ||
                            en.FullName.Contains(".jpg")
                        )
                        ) {
                        targetExtract.Add(en);
                    }
                }

                string dirRoot = FileSystem.AppDataDirectory + "/songs";

                foreach (ZipArchiveEntry en in targetExtract.ToArray())
                {

                    string filename = dirRoot + "/" + en.FullName.Replace("assets/songs/", "");
                    string dir = filename.Substring(0, filename.LastIndexOf('/'));
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                    en.ExtractToFile(filename, true);
                }

                return targetExtract.Count;
            }
            catch
            {
                return -1;
            }
        }
    }
}
