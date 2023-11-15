using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using Microsoft.Win32;

class Program
{
    static void Main(string[] args)
    {
        CheckAndCreateContextMenuRegistryEntries();

        if (args.Length > 0)
        {
            foreach (var arg in args)
            {
                if (File.Exists(arg))
                {
                    ShredFile(arg);
                }
                else if (Directory.Exists(arg))
                {
                    ShredFolder(arg);
                }
                else
                {
                    Console.WriteLine($"Path '{arg}' does not exist.");
                }
            }
        }
        else
        {
            Console.WriteLine("Please provide file or folder paths as command-line arguments.");
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    static void CheckAndCreateContextMenuRegistryEntries()
    {
        string appName = "ShredX";
        string appPath = Process.GetCurrentProcess().MainModule.FileName;

        using (RegistryKey key = Registry.ClassesRoot.OpenSubKey("*\\shell\\" + appName))
        {
            if (key == null)
            {
                using (RegistryKey newKey = Registry.ClassesRoot.CreateSubKey("*\\shell\\" + appName))
                {
                    newKey.SetValue("", "Shred with " + appName);
                    using (RegistryKey commandKey = newKey.CreateSubKey("command"))
                    {
                        commandKey.SetValue("", $"\"{appPath}\" \"%1\"");
                    }
                }
            }
            else
            {
                if (key.GetValue("").ToString() != "Shred with " + appName)
                {
                    key.SetValue("", "Shred with " + appName);
                }

                using (RegistryKey commandKey = key.OpenSubKey("command", true))
                {
                    if (commandKey.GetValue("").ToString() != $"\"{appPath}\" \"%1\"")
                    {
                        commandKey.SetValue("", $"\"{appPath}\" \"%1\"");
                    }
                }
            }
        }

        using (RegistryKey key = Registry.ClassesRoot.OpenSubKey("Directory\\shell\\" + appName))
        {
            if (key == null)
            {
                using (RegistryKey newKey = Registry.ClassesRoot.CreateSubKey("Directory\\shell\\" + appName))
                {
                    newKey.SetValue("", "Shred with " + appName);
                    using (RegistryKey commandKey = newKey.CreateSubKey("command"))
                    {
                        commandKey.SetValue("", $"\"{appPath}\" \"%1\"");
                    }
                }
            }
            else
            {
                if (key.GetValue("").ToString() != "Shred with " + appName)
                {
                    key.SetValue("", "Shred with " + appName);
                }

                using (RegistryKey commandKey = key.OpenSubKey("command", true))
                {
                    if (commandKey.GetValue("").ToString() != $"\"{appPath}\" \"%1\"")
                    {
                        commandKey.SetValue("", $"\"{appPath}\" \"%1\"");
                    }
                }
            }
        }
    }

    static void ShredFile(string filePath)
    {
        try
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Write))
            {
                ShredPass(fs);
                ShredPass(fs);
                ShredPass(fs);
            }

            File.Delete(filePath);

            Console.WriteLine($"File {filePath} shredded successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error shredding file: {ex.Message}");
        }
    }

    static void ShredFolder(string folderPath)
    {
        try
        {
            var files = Directory.GetFiles(folderPath);
            foreach (var file in files)
            {
                ShredFile(file);
            }

            var subdirectories = Directory.GetDirectories(folderPath);
            foreach (var subdirectory in subdirectories)
            {
                ShredFolder(subdirectory);
            }

            Console.WriteLine($"Files in folder {folderPath} shredded successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error shredding folder: {ex.Message}");
        }
    }

    static void ShredPass(FileStream fs)
    {
        using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
        {
            byte[] randomData = new byte[fs.Length];
            rng.GetBytes(randomData);
            fs.Seek(0, SeekOrigin.Begin);
            fs.Write(randomData, 0, randomData.Length);
            fs.Flush();
        }
    }
}