using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LuaAdvancedWatcher
{
    static class Program
    {
        public static readonly Process CurrentProcess;
        public static readonly string StartupPath;

        static Program()
        {
            CurrentProcess = Process.GetCurrentProcess();
            StartupPath = Path.GetDirectoryName(CurrentProcess.MainModule.FileName);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            var settingsFilename = "";

            bool createdNew;
            Mutex mutex = new Mutex(true, $"LuaAdvanced_{settingsFilename}", out createdNew);
            if (!createdNew)
            {
                MessageBox.Show("LuaAdvanced is already running in this folder.", "LuaAdvanced", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // TODO: Uncomment below once repository is about to go public.
            // NOTE: Debugged, it works fine (update installation part it is not yet complete). Also, it will throw an exception if you try to run it while repository is still private.
            /*if ((AutoUpdater.CheckForUpdates() == true) && (MessageBox.Show($"You are running out-of-date version of LuaAdvanced.{Environment.NewLine}Update is available and ready for download!{Environment.NewLine}{Environment.NewLine}Do you want to download an update now?", @"LuaAdvanced - Updater", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1) == DialogResult.Yes))
            {
                string fileName = AutoUpdater.DownloadUpdateSync();
                if (MessageBox.Show($"LuaAdvanced has finished downloading an update.{Environment.NewLine}Update is ready to be installed!{Environment.NewLine}{Environment.NewLine}Do you want to install it now?", @"LuaAdvanced - Updater", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    AutoUpdater.InstallUpdate(fileName);
                    return;
                }
            }*/

            try
            {
                FileAssociation.Associate(".lua_advanced", "LUA_ADVANCED", "LuaAdvanced Settings File", "", Application.ExecutablePath);
            }
            catch { }

#if DEBUG
            settingsFilename = "Test/.lua_advanced";
#else
            if (args.Length == 0)
            {
                MessageBox.Show("No file specified. Please open .lua_advanced.json", "LuaAdvanced", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            settingsFilename = args[0];
#endif

            if (!File.Exists(settingsFilename))
            {
                MessageBox.Show($"No such file: {settingsFilename}", "LuaAdvanced", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(settingsFilename));
        }
    }
}
