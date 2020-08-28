using BatchMuxer_SubEd_Console.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using static BatchMuxer_SubEd_Console.Methods.Methods;

namespace BatchMuxer_SubEd_Console
{
    class Program
    {
        
        static string MKVmergePath;
        static readonly string[] Extensions = { ".mkv", ".webm", ".mp4" };
        static FileInfo[] fi=null;
        /// <summary>
        /// Merge subtitles with their video files in the selected directory.
        /// </summary>
        /// <param name="folder">The directory name</param>
        static void Main(string[] args)
        {
            DirectoryInfo folder=new DirectoryInfo(args[0]);
            IConfiguration config = new ConfigurationBuilder()
          .AddJsonFile("appsettings.json", true, true)
          .Build();
            var appConfig = config.GetSection("application").Get<Application>(); ;
            MKVmergePath = appConfig.MkvMergePath;

            if (ThisOperatingSystem.IsWindows())
            {
                if (string.IsNullOrEmpty(MKVmergePath) || !File.Exists(MKVmergePath + @"\mkvmerge.exe"))
                {
                    Console.WriteLine("MkvMerge Path is invalid or not set, Please input path to MKVMerge. eg- C:\\MkvToolnix\\");
                    string temp = Console.ReadLine();
                   
                    if (File.Exists(Path.Join(temp, @"mkvmerge.exe")))
                    {
                        WriteToConfig("mkvMergePath",temp);
                        MKVmergePath = temp;
                    }
                    else
                    {
                        Console.WriteLine("MkvMerge Path is invalid\n Program exiting...");
                        return;
                    }
                }
            }
           
            bool ProceedFurther=true;

            folder ??= new DirectoryInfo(".");
            if (folder.Exists)
            {
                try
                {
                     fi = folder.EnumerateFiles()
                        .Where(f => Extensions.Contains(f.Extension.ToLower()))
                        .ToArray();
                    RenameFile(fi);
                }
                catch (UnauthorizedAccessException exception)
                {
                    Console.WriteLine(exception.Message);
                    ProceedFurther=false;
                }
            }
            else
            {
                Console.WriteLine("Directory does not exist!");
                ProceedFurther=false;
            }
            if (ProceedFurther)
            {
                Console.Write("Muxing Subtitles...");
                using (var progress = new ProgressBar())
                {
                    for(int i = 0; i < fi.Length; ++i)
                    {
                        ProcessFile(fi[i], folder.FullName, MKVmergePath);
                        progress.Report((double)i / fi.Length);
                    }
                }
                if (appConfig.AutoCleanUp)
                {
                    DeleteAndMove(fi,folder.FullName);
                }
                Console.WriteLine("\nDone. Press any key to Exit...");
                Console.ReadKey(true);
            }
        }
    }
}
