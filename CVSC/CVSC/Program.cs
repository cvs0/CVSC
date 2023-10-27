using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Usage: cvsc.exe <project_directory>");
            return;
        }

        string projectDirectory = args[0];

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
        File.WriteAllText(mainCvsPath, "console.log('Hello, CVSCode!');");

        Console.WriteLine("CVSCode project created in: " + projectDirectory);
    }

    static string MakeProjectId(string projectName)
    {
        // Remove spaces, convert to lowercase, and replace spaces with hyphens
        return projectName.Replace(" ", "-").ToLower();
    }
}
