using Humanizer;
using System;
using System.IO;

namespace CBackup
{
    class Program
    {
        static string BackupFolder = "Backups";

        static void Main(string[] args)
        {
            try
            {
                var filename = args[0];
                TimeSpan backupTimeout = args[1].Contains('h') ? TimeSpan.FromHours(double.Parse(args[1].Replace("h", ""))) :
                    args[1].Contains('m') ? TimeSpan.FromMinutes(double.Parse(args[1].Replace("m", ""))) :
                    args[1].Contains('s') ? TimeSpan.FromSeconds(double.Parse(args[1].Replace("s", ""))) : TimeSpan.FromMinutes(10);

                if (args.Length >= 3) BackupFolder = args[2];

                Console.WriteLine($"Filename: {filename}\nSave Delay: {backupTimeout.Humanize()}\n" +
                    $"Backup Folder: {(BackupFolder == "Backups" ? Directory.GetCurrentDirectory() + "\\" + BackupFolder : BackupFolder)}" +
                    $"\nStarting backup");

                if (!Directory.Exists(BackupFolder)) Directory.CreateDirectory(BackupFolder);

                //Main Loop
                while (true)
                {
                    try
                    {
                        string contents = "";

                        string backupDirectory = BackupFolder + "/" + DateTime.Now.ToString("dd-MM-yyyy");
                        if (!Directory.Exists(backupDirectory)) Directory.CreateDirectory(backupDirectory);

                        string backupLocation = backupDirectory + "/" + DateTime.Now.ToString("HH∶mm∶ss") + Path.GetExtension(filename);

                        using (FileStream stream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                while (!reader.EndOfStream)
                                {
                                    contents = reader.ReadToEnd();
                                }
                            }
                        }

                        File.WriteAllText(backupLocation, contents);
                    }
#if DEBUG
                    catch (Exception e) { Console.WriteLine(e); }
#else
                    catch { }
#endif

                    System.Threading.Thread.Sleep(backupTimeout);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occured (Make sure the parameters are properly passed in the format <filename/file location> <save delay (1h/60m/150s)> <save folder?>):\n" + e);
            }
        }
    }
}
