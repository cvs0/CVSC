using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

class Program
{
    // Define valid licenses
    static string[] validLicenses = {"zlib", "unlicense", "ncsa", "ofl-1.1", "postgresql", "osl-3.0", "mpl-2.0",
                                     "mit", "ms-pl", "lppl-1.3c", "isc", "lgpl-3.0", "lgpl-2.1", "lgpl", "gpl-3.0",
                                     "gpl-2.0", "gpl", "agpl-3.0", "eupl-1.1", "epl-2.0", "epl-1.0", "ecl-2.0",
                                     "wtfpl", "cc-by-sa-4.0", "cc-by-4.0", "cc0-1.0", "cc", "0bsd", "bsd-4-clause",
                                     "bsd-3-clause-clear", "bsd-3-clause", "bsd-2-clause", "bsl-1.0", "artistic-2.0",
                                     "apache-2.0", "afl-3.0"};

    static void Main(string[] args)
    {
        try
        {
            // Check command-line arguments
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: cvsc.exe create <project_directory> OR cvsc.exe run <file>");
                return;
            }

            // Get the mode (create or run)
            string mode = args[0].ToLower();

            // Validate arguments based on the mode
            if (mode == "create" && args.Length != 2)
            {
                Console.WriteLine("Usage: cvsc.exe create <project_directory>");
                return;
            }
            else if (mode == "run" && args.Length != 2)
            {
                Console.WriteLine("Usage: cvsc.exe run <file>");
                return;
            }

            // Execute the appropriate mode
            if (mode == "create")
            {
                string projectDirectory = args[1];
                CreateProject(projectDirectory);
            }
            else if (mode == "run")
            {
                string filePath = args[1];
                RunProject(filePath);
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine($"IOException: {ex.Message}");
        }
        catch (OutOfMemoryException ex)
        {
            Console.WriteLine($"OutOfMemoryException: {ex.Message}");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine($"ArgumentOutOfRangeException: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    // Function to create a CVSCode project
    static void CreateProject(string projectDirectory)
    {
        try
        {
            // Default values
            string projectName = "MyCVSCodeProject";
            string projectVersion = "1.0.0";
            string projectDescription = "Example CVSCode project";
            string projectAuthor = "Your Name";
            string githubRepository = "https://github.com/cvs0/cvscode";
            string projectLicense = "MIT";
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

            Console.Write("Enter the license type (default: MIT): ");
            input = Console.ReadLine().Trim();
            if (!string.IsNullOrEmpty(input) && validLicenses.Contains(input.ToLower()))
            {
                projectLicense = input;
            }
            else
            {
                Console.WriteLine("Invalid license type. Using default: MIT");
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
                )),
                new JProperty("license", projectLicense)
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
        catch (IOException ex)
        {
            Console.WriteLine($"IOException: {ex.Message}");
        }
        catch (OutOfMemoryException ex)
        {
            Console.WriteLine($"OutOfMemoryException: {ex.Message}");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine($"ArgumentOutOfRangeException: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    // Function to run a CVSCode project
    static void RunProject(string filePath)
    {
        string cvsCodeLocalDirectory = @"C:\Program Files\CVSCode\Local";
        string denoCommand = $"deno run -A main.ts --run \"{filePath}\"";
        Console.Clear();
        ExecuteCommand(denoCommand, cvsCodeLocalDirectory);
    }


    // Helper function to execute a command in the current directory
    static void ExecuteCommand(string command, string workingDirectory)
    {
        try
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
        catch (Exception ex)
        {
            Console.WriteLine($"Error executing command: {ex.Message}");
        }
    }

    // Function to create a project ID
    static string MakeProjectId(string projectName)
    {
        // Remove spaces, convert to lowercase, and replace spaces with hyphens
        return projectName.Replace(" ", "-").ToLower();
    }
}