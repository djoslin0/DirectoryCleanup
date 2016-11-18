using System;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic.FileIO;

namespace DirectoryCleanup
{
    class Program
    {
        static int LifespanDefault = 7;

        private static void CleanupDirectory(int lifespan, string directory, bool startingDirectory)
        {
            // Delete all files in this directory older than lifespan.
            DateTime now = DateTime.Now;
            string[] filepaths = Directory.GetFiles(directory);
            foreach (string filepath in filepaths)
            {
                // Check creation and modification dates
                if (DaysSince(File.GetCreationTime(filepath)) < lifespan) { continue; }
                if (DaysSince(File.GetLastWriteTime(filepath)) < lifespan) { continue; }
                // Recycle file
                FileSystem.DeleteFile(filepath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            }

            // Cleanup all subdirectories.
            string[] directories = Directory.GetDirectories(directory);
            foreach(string subdirectory in directories)
            {
                CleanupDirectory(lifespan, subdirectory, false);
            }

            // If this directory no longer contains any files or subdirectories, delete it.
            if (startingDirectory) { return; } // unless it is the starting directory
            if (Directory.GetFiles(directory).Length > 0) { return; }
            if (Directory.GetDirectories(directory).Length > 0) { return; }

            // Remove readonly flag to ensure deletion.
            DirectoryInfo info = new DirectoryInfo(directory);
            info.Attributes &= ~FileAttributes.ReadOnly;

            // Recycle directory
            FileSystem.DeleteDirectory(directory, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
        }

        private static double DaysSince(DateTime datetime)
        {
            // DaysSince(): a helper function that increases clarity in CleanupFiles()
            return (DateTime.Now - datetime).TotalDays;
        }

        private static void DisplayHelp()
        {
            // default console output, explains how to use the program
            // can also be triggered by /? or -? parameters
            string processName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            Console.WriteLine("Deletes all files that have not been modified after 'd' days in every subdirectory, and deletes empty subdirectories");
            Console.WriteLine(" ");
            Console.WriteLine(processName + " [-d#] [drive:]path");
            Console.WriteLine("\t" + "-d#" + "\t" + "days" + "\t" + "Replace # with an integer amount of days, defaults to " + LifespanDefault);
            Console.WriteLine(" ");
            Console.WriteLine("Example: " + processName + " -d10 \"C:/Users/Owner/Downloads/\"");
            Console.WriteLine("\tDeletes files that have not been modified in 10 days within every");
            Console.WriteLine("\tsubdirectory inside C:/Users/Owner/Downloads/");
            Console.WriteLine("\tAlso removes any empty subdirectories inside C:/Users/Owner/Downloads/");

        }

        static void Main(string[] args)
        {
            // Ensure that the user input parameters
            if (args.Length < 1) { DisplayHelp(); return; }
            
            // set lifespan and directory
            int lifespan = LifespanDefault;
            string directory = args.Last();
            
            // parse parameters
            foreach(string arg in args)
            {
                if (arg == "/?" || arg == "-?" || arg == "help")
                {
                    // display help
                    DisplayHelp();
                    return;
                }
                else if(arg.StartsWith("-d", StringComparison.InvariantCultureIgnoreCase))
                {
                    // user set lifespan
                    if (!int.TryParse(arg.Substring(2), out lifespan))
                    {
                        Console.WriteLine("Failed to parse lifespan parameter: '" + arg + "'");
                        return;
                    }
                }
            }

            // ensure that the user specified directory exists
            if(!Directory.Exists(directory))
            {
                Console.WriteLine("The supplied directory does not exist: '" + directory + "'");
                return;
            }

            // cleanup the directory
            CleanupDirectory(lifespan, directory, true);
        }
    }
}
