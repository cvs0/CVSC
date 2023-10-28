using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: cvsc.exe create <project_directory> OR cvsc.exe run <file>");
            return;
        }

        string mode = args[0].ToLower();

        if (mode == "create" && args.Length != 3)
        {
            Console.WriteLine("Usage: cvsc.exe create <project_directory>");
            return;
        }
        else if (mode == "run" && args.Length != 2)
        {
            Console.WriteLine("Usage: cvsc.exe run <file>");
            return;
        }

        if (mode == "create")
        {
            string projectDirectory = args[2];
            CreateProject(projectDirectory);
        }
        else if (mode == "run")
        {
            string filePath = args[1];
            RunProject(filePath);
        }
    }

    static void CreateProject(string projectDirectory)
    {
        // Default values
        string projectName = "MyCVSCodeProject";
        string projectVersion = "1.0.0";
        string projectDescription = "Example CVSCode project";
        string projectAuthor = "Your Name";
        string githubRepository = "https://github.com/cvs0/cvscode";
        string projectId = "";

        // Prompt the user for project details
        Console.Write("Enter project name (default: MyCVSCodeProject): ");
        string input = Console.ReadLine().Trim();
        if (!string.IsNullOrEmpty(input))
        {
            projectName = input;
        }

        Console.Write("Enter project version (default: 1.0.0): ");
        input = Console.ReadLine().Trim();
        if (!string.IsNullOrEmpty(input))
        {
            projectVersion = input;
        }

        Console.Write("Enter project description (default: Example CVSCode project): ");
        input = Console.ReadLine().Trim();
        if (!string.IsNullOrEmpty(input))
        {
            projectDescription = input;
        }

        Console.Write("Enter project author (default: Your Name): ");
        input = Console.ReadLine().Trim();
        if (!string.IsNullOrEmpty(input))
        {
            projectAuthor = input;
        }

        Console.Write("Enter GitHub repository URL: ");
        input = Console.ReadLine().Trim();
        if (!string.IsNullOrEmpty(input))
        {
            githubRepository = input;
        }

        // Create the project directory if it doesn't exist
        if (!Directory.Exists(projectDirectory))
        {
            Directory.CreateDirectory(projectDirectory);
        }

        projectId = MakeProjectId(projectName);

        // Create the package.json file
        string packageJsonPath = Path.Combine(projectDirectory, "package.json");
        JObject packageJson = new JObject(
            new JProperty("name", projectName),
            new JProperty("id", projectId),
            new JProperty("version", projectVersion),
            new JProperty("description", projectDescription),
            new JProperty("author", projectAuthor),
            new JProperty("repository", new JObject(
                new JProperty("type", "git"),
                new JProperty("url", githubRepository)
            ))
        );

        File.WriteAllText(packageJsonPath, packageJson.ToString());

        // Create the src directory and main.cvs file
        string srcDirectory = Path.Combine(projectDirectory, "src");
        if (!Directory.Exists(srcDirectory))
        {
            Directory.CreateDirectory(srcDirectory);
        }

        string mainCvsPath = Path.Combine(srcDirectory, "main.cvs");
        File.WriteAllText(mainCvsPath, "let x = 45;");

        Console.WriteLine("CVSCode project created in: " + projectDirectory);
    }

    static void RunProject(string filePath)
    {
        string cvsCodeLocalDirectory = @"C:\Program Files\CVSCode\Local";
        string rundenoScriptPath = Path.Combine(cvsCodeLocalDirectory, "rundeno.bat");
        string denoCommand = $"\"{rundenoScriptPath}\" \"{filePath}\"";
        ExecuteCommand(denoCommand, cvsCodeLocalDirectory);
    }

    // Helper function to execute a command in the current directory
    static void ExecuteCommand(string command, string workingDirectory)
    {
        ProcessStartInfo processStartInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            RedirectStandardInput = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = false
        };

        Process process = new Process
        {
            StartInfo = processStartInfo,
            EnableRaisingEvents = true,
        };

        // Set the working directory for the process
        process.StartInfo.WorkingDirectory = workingDirectory;

        process.Start();

        // Send the command to the command prompt
        process.StandardInput.WriteLine(command);
        process.StandardInput.Flush();
        process.StandardInput.Close();

        // Display the command prompt output
        string output = process.StandardOutput.ReadToEnd();
        Console.WriteLine(output);

        process.WaitForExit();
        process.Close();
    }





    static string MakeProjectId(string projectName)
    {
        // Remove spaces, convert to lowercase, and replace spaces with hyphens
        return projectName.Replace(" ", "-").ToLower();
    }
}
