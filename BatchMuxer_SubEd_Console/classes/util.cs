using BatchMuxer_SubEd_Console.Model;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BatchMuxer_SubEd_Console.Classes
{/// <summary>
/// utilility helper class
/// </summary>
    public static class util
    {
        /// <summary>
        /// Deletes original Media files and move files from "muxed" to original folder.
        /// </summary>
        /// <param name="files">Files to move</param>
        /// <param name="path">Full folder path</param>
        public static void DeleteAndMove(FileInfo[] files, string path)
        {
            foreach (var fl in files)
            {
                var subtitle = fl.Name.Replace(fl.Extension, ".srt");
                var subtitlePath = fl.FullName.Replace(fl.Extension, ".srt");
                if (File.Exists(path + @"\" + subtitle) && File.Exists(path + @"\muxed\" + fl.Name))
                {
                    File.Delete(fl.FullName);
                    File.Delete(subtitlePath);
                    File.Move(path + @"\muxed\" + fl.Name, fl.FullName);
                }
            }
            if (IsDirectoryEmpty(path + @"\muxed"))
                Directory.Delete(path + @"\muxed");
        }

        /// <summary>
        /// Checks if a diretory is empty by enumerating through path
        /// </summary>
        /// <param name="path">Directory to check</param>
        /// <returns></returns>
        public static bool IsDirectoryEmpty(string path) => !Directory.EnumerateFileSystemEntries(path).Any();

        /// <summary>
        /// Muxes file using MKVMerge
        /// </summary>
        /// <param name="fl"> media file</param>
        /// <param name="path">Path of the media and subtitle</param>
        /// <param name="mkvmergePath">path to MKVmerge</param>
        public static void ProcessFile(FileInfo fl, string path, string mkvmergePath)
        {
            string subtitle = $@"""{fl.FullName.Replace(fl.Extension, ".srt")}""";
            string fileName = $@"""{fl.FullName}""";
            if (fl.Extension != ".mkv")
            {
                File.Move(fl.FullName, fl.FullName.Replace(fl.Extension, ".mkv"));//caution!
            }
            if (File.Exists(subtitle.Replace("\"", "")) &&
                !File.Exists(Path.Combine(path + "muxed" + fl.Name)))
            {
                string output = @$"""{Path.Combine(path, "muxed", fl.Name)}""";
                string mkvPath = ThisOperatingSystem.IsWindows() ? Path.Combine(mkvmergePath, "mkvmerge.exe") : "mkvmerge";
                ProcessStartInfo startInfo = new ProcessStartInfo(mkvPath)
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    //WorkingDirectory = mkvmergePath,
                    Arguments = $@"-o {output} --default-track 0:yes {subtitle} {fileName}",
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                };
                try
                {
                    var oProcess = new Process
                    {
                        StartInfo = startInfo
                    };
                    oProcess.Start();
                    oProcess.WaitForExit();
                    var r = oProcess.StandardOutput;
                    oProcess.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(@"*** Make sure ""MKVMerge"" is installed and set in PATH");
                }
            }
        }

        /// <summary>
        /// Renames other extension to MKV
        /// </summary>
        /// <param name="fi">File info array containing files to rename</param>
        /// <returns></returns>
        public static bool RenameFile(FileInfo[] fi)
        {
            bool hasRenamed = false;
            foreach (var fl in fi)
            {
                if (fl.Extension != ".mkv")
                {
                    File.Move(fl.FullName, fl.FullName.Replace(fl.Extension, ".mkv")); //caution!
                    hasRenamed = true;
                }
            }

            return hasRenamed;
        }

        /// <summary>
        /// Update appsetting.json
        /// </summary>
        /// <param name="key">key to update</param>
        /// <param name="value">new value</param>
        public static void WriteToConfig(string key, string value)
        {
            string appSettingPath = 
                Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "appsettings.json");
            string json = File.ReadAllText(appSettingPath);
            var application = new Application();
            JsonConvert.PopulateObject(json, application);
            dynamic jsonObj = JsonConvert.DeserializeObject(json);
            jsonObj["application"][key] = value;
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            File.WriteAllText(appSettingPath, output);
        }
    }
}