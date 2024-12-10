using System.CommandLine;

class Program
{
    static void Main(string[] args)
    {
        //create options
        var outputOption = new Option<FileInfo>("--output", "File path and name of the bundle");
        outputOption.AddAlias("-o");

        var languageOption = new Option<string>("--language", "Languages of code to include (comma separated or 'all')")
        {
            IsRequired = true,
        }.FromAmong("csharp", "c", "cpp", "java", "js", "html", "css", "scss", "ts", "sql", "python", "all");
        languageOption.AddAlias("-l");


        var noteOption = new Option<bool>("--note", "Add source file comment to bundle");
        noteOption.AddAlias("-n");

        var sortOption = new Option<string>("--sort", "Sort files by name or type (default by name)").FromAmong("abc", "type");
        sortOption.SetDefaultValue("abc");
        sortOption.AddAlias("-s");

        var removeEmptyLinesOption = new Option<bool>("--remove-empty-lines", "Remove empty lines from the code");
        removeEmptyLinesOption.AddAlias("-e");

        var authorOption = new Option<string>("--author", "Name of the file author");
        authorOption.AddAlias("-a");

        //create commands
        var bundleCommand = new Command("bundle", "Bundle code files into a single file")
        {
            outputOption,
            languageOption,
            noteOption,
            sortOption,
            removeEmptyLinesOption,
            authorOption
        };
        
        bundleCommand.SetHandler((FileInfo output, string language, bool note, string sort, bool removeEmptyLines, string author) =>
        {
            Dictionary<string, string> supportedLanguages = new()
            {
            { "csharp",".cs" },
            { "c", ".c" },
            { "cpp", ".cpp" },
            { "java", ".java" },
            { "js",".js" },
            { "html",".html" },
            { "css",".css" },
            { "scss",".scss" },
            { "ts",".ts" },
            { "sql", ".sql" },
            { "python",".python" },
            { "all","all" }
        };
            string[] validLanguages = Array.Empty<string>(); 
            List<FileInfo> foundFiles = new();

            // Validation of languages
            try
            {
                if (language.ToLower() != "all" && !language.Split(',').All(lang => supportedLanguages.ContainsKey(lang.ToLower())))
                {
                    Console.WriteLine("Error: One or more languages are invalid. Supported languages are: csharp, c, cpp, java, js, html, css, scss, ts, sql, python.");
                    return;
                }

                validLanguages = language.ToLower() == "all" ? supportedLanguages.Keys.ToArray() : language.Split(',').Select(l => l.Trim().ToLower()).ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error validating language: {ex.Message}");
                return;
            }

            // Searching for files
            try
            {
                var directoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
                foundFiles = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories)
                    .Where(file => validLanguages.Any(lang => file.Extension.ToLower().EndsWith(lang)))
                    .Where(file => !file.FullName.Contains("bin", StringComparison.OrdinalIgnoreCase) &&
                                   !file.FullName.Contains("debug", StringComparison.OrdinalIgnoreCase) &&
                                   !file.FullName.Contains("obj", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(file => sort == "type" ? file.Extension : file.Name)
                    .ToList();

                if (!foundFiles.Any())
                {
                    Console.WriteLine("Error: No files found matching the specified language.");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error finding files: {ex.Message}");
                return;
            }
            // Creating the bundle file
            try
            {
                if (output?.DirectoryName == null || !Directory.Exists(output.DirectoryName))
                {
                    Console.WriteLine("Error: The directory path for the output file does not exist.");
                    return;
                }

                using (var streamWriter = new StreamWriter(output.FullName))
                {
                    // Adding source file comments if required
                    if (note)
                    {
                        foreach (var file in foundFiles)
                        {
                            streamWriter.WriteLine($"// Source file: {file.Name}, Path: {file.FullName}");
                        }
                    }

                    // Adding author if provided
                    if (!string.IsNullOrEmpty(author))
                    {
                        streamWriter.WriteLine($"// Author: {author}");
                    }

                    // Copying the content of each file
                    foreach (var file in foundFiles)
                    {
                        var code = File.ReadAllText(file.FullName);

                        // Removing empty lines if required
                        if (removeEmptyLines)
                        {
                            code = string.Join(Environment.NewLine, code.Split(Environment.NewLine).Where(line => !string.IsNullOrWhiteSpace(line)));
                        }

                        streamWriter.WriteLine(code);
                    }
                }

                Console.WriteLine($"Files bundled successfully into {output.FullName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating bundle: {ex.Message}");
            }
        }, outputOption, languageOption, noteOption, sortOption, removeEmptyLinesOption, authorOption);

        // create-rsp command to generate response file
        var createRspCommand = new Command("create-rsp", "Generate response file for bundle command");

        createRspCommand.SetHandler(() =>
        {
            Dictionary<string, string> supportedLanguages = new()
            {
            { "csharp",".cs" },
            { "c", ".c" },
            { "cpp", ".cpp" },
            { "java", ".java" },
            { "js",".js" },
            { "html",".html" },
            { "css",".css" },
            { "scss",".scss" },
            { "ts",".ts" },
            { "sql", ".sql" },
            { "python",".python" },
            { "all","all" }
        };
            // Prompting user for each option
            string[] validLanguages = Array.Empty<string>();
            do
            {
                Console.WriteLine("Enter language (comma separated or 'all'):");
                supportedLanguages.Keys.ToList().ForEach(Console.WriteLine);
                var languageResponse = Console.ReadLine();
                try
                {
                    if (languageResponse.ToLower() != "all" && !languageResponse.Split(',').All(lang => supportedLanguages.ContainsKey(lang.ToLower())))
                    {
                        Console.WriteLine("Error: One or more languages are invalid. Supported languages are: csharp, c, cpp, java, js, html, css, scss, ts, sql, python.");
                        return;
                    }

                    validLanguages = languageResponse.ToLower() == "all" ? supportedLanguages.Keys.ToArray() : languageResponse.Split(',').Select(l => l.Trim().ToLower()).ToArray();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error validating language: {ex.Message}");
                    return;
                }
            }
            while (!validLanguages.Any());

            Console.WriteLine("Enter output file path:");
            var outputResponse = Console.ReadLine();

            Console.WriteLine("Add source comment? (yes/no):");
            var noteResponse = Console.ReadLine()?.ToLower() == "yes";

            Console.WriteLine("Sort files by name or type?(abc/type)");
            var sortResponse = Console.ReadLine()?.ToLower();

            Console.WriteLine("Remove empty lines? (yes/no):");
            var removeEmptyLinesResponse = Console.ReadLine()?.ToLower() == "yes";

            Console.WriteLine("Enter author's name (optional):");
            var authorResponse = Console.ReadLine();

            // Creating the response file
            var rspFileName = "response.rsp";
            if (File.Exists(rspFileName))
            {
                Console.WriteLine("Warning: The response file already exists. It will be overwritten.");
            }

            using (var writer = new StreamWriter(rspFileName))
            {
                if (!string.IsNullOrEmpty(string.Join(",", validLanguages)))
                {
                    writer.WriteLine($"--language {string.Join(",", validLanguages)}");
                }
                if (!string.IsNullOrEmpty(outputResponse))
                {
                    writer.WriteLine($"--output {outputResponse}");
                }
                writer.WriteLine($"--note {noteResponse.ToString().ToLower()}");
                if (!string.IsNullOrEmpty(sortResponse))
                {
                    writer.WriteLine($"--sort {sortResponse}");
                }
                writer.WriteLine($"--remove-empty-lines {removeEmptyLinesResponse.ToString().ToLower()}");
                if (!string.IsNullOrEmpty(authorResponse))
                {
                    writer.WriteLine($"--author {authorResponse}");
                }
            }

            Console.WriteLine($"Response file created: {rspFileName}");
        });

        // Adding commands to the root
        var rootCommand = new RootCommand
        {
            bundleCommand,
            createRspCommand
        };

        // Running the commands
        rootCommand.InvokeAsync(args).Wait();
    }
}
