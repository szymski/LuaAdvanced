using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LuaAdvancedWatcher
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            var settingsFilename = "";
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

            bool createdNew;
            Mutex mutex = new Mutex(true, $"LuaAdvanced_{settingsFilename}", out createdNew);
            if (!createdNew)
            {
                MessageBox.Show("LuaAdvanced is already running in this folder.", "LuaAdvanced", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                FileAssociation.Associate(".lua_advanced", "LUA_ADVANCED", "LuaAdvanced Settings File", "", Application.ExecutablePath);
            }
            catch { }

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
