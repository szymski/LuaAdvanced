﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LuaAdvanced.Compiler;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;

namespace LuaAdvancedWatcher
{
    public partial class Form1 : Form
    {
        JObject settings;
        string outputFullPath, inputFullPath;
        string comment = "--[[\n\tCompiled using LuaAdvanced\n\tThis file should not be modified\n]]\n";
        bool include_luaa_lib = true;
        Dictionary<string, string> globalDirectives = new Dictionary<string, string>(); 

        public Form1(string settingsFilename)
        {
            InitializeComponent();
            settings = JObject.Parse(File.ReadAllText(settingsFilename));
            Directory.SetCurrentDirectory(new FileInfo(settingsFilename).DirectoryName);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var context = new ContextMenu();
            context.MenuItems.Add("Compile all", (o, args) => CompileAll());
            context.MenuItems.Add("Exit", (o, args) => Close());

            trayIcon.Icon = Icon;
            trayIcon.ContextMenu = context;
            trayIcon.Visible = true;

            CheckConfig();
            CreateWatcher();

            trayIcon.ShowBalloonTip(5000, "LuaAdvanced is running", "Now you can code in LuaAdvanced and every change will be compiled and put into the output folder.", ToolTipIcon.Info);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            Hide();
        }

        void CheckConfig()
        {
            if (settings.GetValue("input_dir") == null)
            {
                MessageBox.Show($"input_dir not set!", "LuaAdvanced", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
            if (settings.GetValue("output_dir") == null)
            {
                MessageBox.Show($"output_dir not set!", "LuaAdvanced", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }

            if (settings.GetValue("comment") != null)
            {
                StringBuilder builder = new StringBuilder();
                foreach (var line in settings["comment"].Select(a => (string)a))
                    builder.AppendLine(line);
                comment = builder.ToString();
            }

            if (settings.GetValue("include_luaa_lib") != null)
                include_luaa_lib = settings.Value<bool>("include_luaa_lib");

            if(settings.GetValue("directives") != null)
                foreach(var dir in (JObject)settings["directives"])
                    globalDirectives.Add(dir.Key, (string)dir.Value);

            if (include_luaa_lib)
                comment += "\ndofile(\"luaa/luaa.lua\")\n";

                 inputFullPath = new DirectoryInfo(settings.Value<string>("input_dir")).FullName;
            outputFullPath = new DirectoryInfo(settings.Value<string>("output_dir")).FullName;
        }

        string PrepareComment(string comment, string filename)
        {
            return comment.Replace("%filename%", filename)
                .Replace("%date%", DateTime.Now.ToShortDateString())
                .Replace("%time%", DateTime.Now.ToLongTimeString())
                .Replace("%year%", DateTime.Now.Year.ToString());
        }

        void CreateWatcher()
        {
            FileSystemWatcher watcher = new FileSystemWatcher(settings.Value<string>("input_dir"));
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = "*.lua*"; // .luaa and .lua files
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

            DateTime lastCompile = DateTime.Now;

            watcher.Changed += (sender, args) =>
            {
                if (DateTime.Now.Subtract(lastCompile).Seconds < 1) // For some reason, without this, file compiles twice after every change
                    return;

                lastCompile = DateTime.Now;

                var outputFilename = Path.Combine(outputFullPath, args.FullPath.Replace(Directory.GetCurrentDirectory(), "").Replace(".luaa", ".lua"));

                if (new FileInfo(args.FullPath).Extension == ".lua")
                {
                    if (!Directory.Exists(Path.GetDirectoryName(outputFilename)))
                        Directory.CreateDirectory(Path.GetDirectoryName(outputFilename));

                    File.WriteAllText(outputFilename, File.ReadAllText(args.FullPath));

                    return;
                }

                try
                {
                    Thread.Sleep(100);
                    Compiler compiler = new Compiler();

                    foreach(var d in globalDirectives)
                        compiler.Directives.Add(d.Key, d.Value);

                    compiler.Directives.Add("__FILE__", $"\"{new FileInfo(args.FullPath).Name}\"");
                    compiler.Directives.Add("_LONG_FILE_", $"\"{args.FullPath.Replace(Directory.GetCurrentDirectory(), "")}\"");

                    compiler.Comment = PrepareComment(comment, new FileInfo(args.FullPath).Name);

                    if (!Directory.Exists(Path.GetDirectoryName(outputFilename)))
                        Directory.CreateDirectory(Path.GetDirectoryName(outputFilename));

                    File.WriteAllText(outputFilename, compiler.Compile(File.ReadAllText(args.FullPath).Replace("\r", "")));
                }
                catch (CompilerException e)
                {
                    trayIcon.ShowBalloonTip(2000, $"Error in {args.FullPath.Replace(Directory.GetCurrentDirectory(), "")}", $" {e.Line}:{e.Position} : {e.Message}", ToolTipIcon.Error);
                }
            };
        }

        void CompileAll(string dir = null)
        {
            if (dir == null)
                dir = inputFullPath;

            foreach (var file in new DirectoryInfo(dir).EnumerateFiles().Where(f => f.Extension == ".luaa" || f.Extension == ".lua"))
            {
                var outputFilename = outputFullPath + "\\" + file.FullName.Replace(Directory.GetCurrentDirectory(), "").Replace(".luaa", ".lua");

                if (file.Extension == ".lua")
                {
                    if (!Directory.Exists(Path.GetDirectoryName(outputFilename)))
                        Directory.CreateDirectory(Path.GetDirectoryName(outputFilename));

                    File.WriteAllText(outputFilename, File.ReadAllText(file.FullName));

                    return;
                }

                try
                {
                    Compiler compiler = new Compiler();

                    foreach (var d in globalDirectives)
                        compiler.Directives.Add(d.Key, d.Value);

                    compiler.Directives.Add("__FILE__", $"\"{new FileInfo(file.FullName).Name}\"");
                    compiler.Directives.Add("_LONG_FILE_", $"\"{file.FullName.Replace(Directory.GetCurrentDirectory(), "")}\"");

                    compiler.Comment = PrepareComment(comment, new FileInfo(file.FullName).Name);

                    if (!Directory.Exists(Path.GetDirectoryName(outputFilename)))
                        Directory.CreateDirectory(Path.GetDirectoryName(outputFilename));

                    File.WriteAllText(outputFilename, compiler.Compile(File.ReadAllText(file.FullName).Replace("\r", "")));
                }
                catch (CompilerException e)
                {
                    trayIcon.ShowBalloonTip(2000, $"Error in {file.FullName.Replace(Directory.GetCurrentDirectory(), "")}", $" {e.Line}:{e.Position} : {e.Message}", ToolTipIcon.Error);
                }
            }

            foreach (var d in new DirectoryInfo(dir).EnumerateDirectories())
                CompileAll(d.FullName);
        }
    }
}
