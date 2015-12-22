using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Net;

namespace LuaAdvancedWatcher
{
    static class AutoUpdater
    {
        static readonly Regex AssemblyVersionRegex = new Regex(@"^\s*\[\s*assembly\s*:\s*AssemblyVersion\s*\(\s*""(\d+)\.(\d+)\.(\d+)\.(\d+)""\s*\)\s*\]", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Multiline);
        static readonly Version LocalVersion = Assembly.GetEntryAssembly().GetName().Version;
        static readonly WebClient WebClient = new WebClient();
        static Version _serverVersion;

        /// <summary>Checks if there are any updates available.</summary>
        /// <returns>Returns true if there are any updates available; otherwise, false.</returns>
        /// <remarks>This function will return null if it fails to obtain the most recent version number.</remarks>
        public static bool? CheckForUpdates()
        {
            try
            {
                string assemblyInfo = WebClient.DownloadString("https://raw.githubusercontent.com/szymski/LuaAdvanced/master/LuaAdvancedWatcher/Properties/AssemblyInfo.cs?token=AJVejmmQoaGa7yYXWDNRuxuwN4M2UW6Xks5Wge6WwA%3D%3D");
                MatchCollection matches = AssemblyVersionRegex.Matches(assemblyInfo);
                if ((matches.Count != 1) || (matches[0].Groups.Count != 5))
                {
                    return null;
                }
                _serverVersion = new Version(Convert.ToInt32(matches[0].Groups[1].Value), Convert.ToInt32(matches[0].Groups[2].Value), Convert.ToInt32(matches[0].Groups[3].Value), Convert.ToInt32(matches[0].Groups[4].Value));
                return LocalVersion < _serverVersion;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>Initiates a download for the newest version.<para/>NOTE: Be sure to call <seealso cref="CheckForUpdates"/> function previously.</summary>
        /// <returns>Returns a filename (including extension) of file downloaded.</returns>
        public static string DownloadUpdateSync()
        {
            if ((_serverVersion == null) || (LocalVersion >= _serverVersion))
            {
                return null;
            }
            string fileName = $"Release-v{_serverVersion.Major}.{_serverVersion.Minor}.{_serverVersion.Build}.{_serverVersion.Revision}.zip";
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            WebClient.DownloadFile($"https://github.com/szymski/LuaAdvanced/releases/download/v{_serverVersion.Major}.{_serverVersion.Minor}.{_serverVersion.Build}.{_serverVersion.Revision}/Release.zip", fileName);
            return fileName;
        }

        /// <summary>Installs an update.<para/>NOTE: Be sure to call <seealso cref="DownloadUpdateSync"/> function previously.</summary>
        /// <param name="zip">Absolute path to newest LuaAdvanced ZIP file.</param>
        public static void InstallUpdate(string zip)
        {
            if (!File.Exists(zip))
            {
                return;
            }
            //
            // Extract contents from an archive ZIP file in a temporary folder (which is next to LuaAdvanced executable file).
            //
            string tmpPath = Path.Combine(Program.StartupPath, ".tmp");
            if (Directory.Exists(tmpPath))
            {
                Directory.Delete(tmpPath, true);
            }
            Directory.CreateDirectory(tmpPath);
            using (ZipArchive archive = ZipFile.OpenRead(zip))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    string fullPath = Path.Combine(tmpPath, entry.FullName);
                    string[] split = entry.FullName.Split('/');
                    if (split.Length > 1)
                    {
                        // Drop out last string from split array (it is a filename).
                        string fileName = split[split.Length - 1];
                        split[split.Length - 1] = null;
                        // Create a directory for a file.
                        string path = Path.Combine(tmpPath, split.Aggregate("", (current, directoryName) => current + directoryName));
                        Directory.CreateDirectory(path);
                        fullPath = Path.Combine(path, fileName);
                    }
                    // Extract a file to disk.
                    entry.ExtractToFile(fullPath);
                }
            }
            //
            // TODO: Rename currently running process filename, close all open handles (DLLs used by this process, e.g. NewtonSoft.Json), move files from temporary folder to startup path of this process and then start LuaAdvanced (that is new).
            // NOTE: No need to force exit at the end, it will terminate this instance once it returns to entry point.
            //
        }
    }
}