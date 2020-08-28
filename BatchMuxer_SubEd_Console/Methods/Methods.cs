using BatchMuxer_SubEd_Console.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace BatchMuxer_SubEd_Console.Methods
{
    public static class Methods
    {
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
        public static void ProcessFile(FileInfo fl, string path,string mkvmergePath)
        {
            string subtitle = $@"""{fl.FullName.Replace(fl.Extension, ".srt")}""";
            string fileName =$@"""{fl.FullName}""";
            if (fl.Extension != ".mkv")
            {
                File.Move(fl.FullName, fl.FullName.Replace(fl.Extension, ".mkv"));//caution!
            }

            if (File.Exists(subtitle.Replace("\"","")) &&
                !File.Exists(path + @"\muxed\" + fl.Name))
            {
                string output =@$"""{path}\muxed\{fl.Name}""";
                string mkvPath=ThisOperatingSystem.IsWindows()?Path.Join(mkvmergePath,"mkvmerge.exe"):"mkvmerge";
                ProcessStartInfo startInfo = new ProcessStartInfo(mkvPath)
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    //WorkingDirectory = mkvmergePath,
                    Arguments = $@"-o {output} --default-track 0:yes {subtitle} {fileName}",
                    CreateNoWindow = true,
                };
                try
                {
                    var oProcess = new Process
                    {
                        StartInfo = startInfo
                    };
                    oProcess.Start();
                    oProcess.WaitForExit();
                    oProcess.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(@"*** Make sure ""MKVMerge"" is installed and set in PATH");
                }
                
            }
        }
        public static void WriteToConfig(string key,string value)
        {
            string json = File.ReadAllText("appsettings.json");
            var application=new Application();
            JsonConvert.PopulateObject(json, application);
            dynamic jsonObj = JsonConvert.DeserializeObject(json);
            jsonObj["application"][key] = value;
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            File.WriteAllText("appsettings.json", output);
        }
        public static void DeleteAndMove(FileInfo[] files,string path)
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
        public static bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }
    }
}
