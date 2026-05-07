// DevelopmentChecker.cs
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;

public static class DevelopmentChecker
{
    public class DevCheckResult
    {
        public string ToolName { get; set; }
        public string Version { get; set; }
        public bool IsInstalled { get; set; }
        public string Status { get; set; }
        public string Recommendation { get; set; }
        public string Category { get; set; }
    }

    public class ProjectTemplate
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }
        public List<string> Commands { get; set; }
        public List<string> Files { get; set; }
    }

    // Выполнить все проверки разработки
    public static List<DevCheckResult> RunAllDevChecks()
    {
        Logger.Logger.Info("=== Development Environment Checks / Проверки Среды Разработки ===");
        
        var results = new List<DevCheckResult>();
        
        // Проверка языков программирования
        results.AddRange(CheckProgrammingLanguages());
        
        // Проверка инструментов разработки
        results.AddRange(CheckDevelopmentTools());
        
        // Проверка баз данных
        results.AddRange(CheckDatabases());
        
        // Проверка контейнеризации
        results.AddRange(CheckContainerization());
        
        // Проверка веб-разработки
        results.AddRange(CheckWebDevelopment());
        
        // Проверка мобильной разработки
        results.AddRange(CheckMobileDevelopment());
        
        // Проверка системных инструментов
        results.AddRange(CheckSystemTools());
        
        // Проверка IDE и редакторов
        results.AddRange(CheckIDEs());
        
        // Проверка систем сборки
        results.AddRange(CheckBuildSystems());
        
        // Проверка тестирования
        results.AddRange(CheckTestingTools());
        
        // Проверка документации
        results.AddRange(CheckDocumentationTools());
        
        DisplayDevResults(results);
        return results;
    }
    
    private static List<DevCheckResult> CheckProgrammingLanguages()
    {
        var results = new List<DevCheckResult>();
        
        // Python
        results.Add(CheckTool("python3", "python3 --version", "Python", "Programming Language"));
        results.Add(CheckTool("pip3", "pip3 --version", "Pip", "Package Manager"));
        
        // Node.js
        results.Add(CheckTool("node", "node --version", "Node.js", "Runtime"));
        results.Add(CheckTool("npm", "npm --version", "NPM", "Package Manager"));
        results.Add(CheckTool("yarn", "yarn --version", "Yarn", "Package Manager"));
        
        // Java
        results.Add(CheckTool("java", "java -version", "Java", "Runtime"));
        results.Add(CheckTool("javac", "javac -version", "Java Compiler", "Compiler"));
        results.Add(CheckTool("mvn", "mvn --version", "Maven", "Build Tool"));
        results.Add(CheckTool("gradle", "gradle --version", "Gradle", "Build Tool"));
        
        // C/C++
        results.Add(CheckTool("gcc", "gcc --version", "GCC", "Compiler"));
        results.Add(CheckTool("g++", "g++ --version", "G++", "Compiler"));
        results.Add(CheckTool("make", "make --version", "Make", "Build Tool"));
        results.Add(CheckTool("cmake", "cmake --version", "CMake", "Build System"));
        
        // C#
        results.Add(CheckTool("dotnet", "dotnet --version", ".NET", "Framework"));
        
        // Go
        results.Add(CheckTool("go", "go version", "Go", "Language"));
        
        // Rust
        results.Add(CheckTool("rustc", "rustc --version", "Rust", "Language"));
        results.Add(CheckTool("cargo", "cargo --version", "Cargo", "Package Manager"));
        
        // PHP
        results.Add(CheckTool("php", "php --version", "PHP", "Language"));
        results.Add(CheckTool("composer", "composer --version", "Composer", "Package Manager"));
        
        // Ruby
        results.Add(CheckTool("ruby", "ruby --version", "Ruby", "Language"));
        results.Add(CheckTool("gem", "gem --version", "Gem", "Package Manager"));
        
        return results;
    }
    
    private static List<DevCheckResult> CheckDevelopmentTools()
    {
        var results = new List<DevCheckResult>();
        
        // Git
        results.Add(CheckTool("git", "git --version", "Git", "Version Control"));
        
        // Docker
        results.Add(CheckTool("docker", "docker --version", "Docker", "Containerization"));
        results.Add(CheckTool("docker-compose", "docker-compose --version", "Docker Compose", "Container Orchestration"));
        
        // Kubernetes
        results.Add(CheckTool("kubectl", "kubectl version --client", "Kubectl", "Kubernetes CLI"));
        results.Add(CheckTool("helm", "helm version", "Helm", "Kubernetes Package Manager"));
        
        // CI/CD
        results.Add(CheckTool("jenkins", "jenkins --version", "Jenkins", "CI/CD"));
        results.Add(CheckTool("gitlab-runner", "gitlab-runner --version", "GitLab Runner", "CI/CD"));
        
        // Monitoring
        results.Add(CheckTool("prometheus", "prometheus --version", "Prometheus", "Monitoring"));
        results.Add(CheckTool("grafana", "grafana --version", "Grafana", "Visualization"));
        
        return results;
    }
    
    private static List<DevCheckResult> CheckDatabases()
    {
        var results = new List<DevCheckResult>();
        
        // PostgreSQL
        results.Add(CheckTool("psql", "psql --version", "PostgreSQL", "Database"));
        
        // MySQL
        results.Add(CheckTool("mysql", "mysql --version", "MySQL", "Database"));
        
        // MongoDB
        results.Add(CheckTool("mongod", "mongod --version", "MongoDB", "Database"));
        
        // Redis
        results.Add(CheckTool("redis-cli", "redis-cli --version", "Redis", "Cache/Database"));
        
        // SQLite
        results.Add(CheckTool("sqlite3", "sqlite3 --version", "SQLite", "Database"));
        
        return results;
    }
    
    private static List<DevCheckResult> CheckContainerization()
    {
        var results = new List<DevCheckResult>();
        
        results.Add(CheckTool("docker", "docker --version", "Docker", "Container Platform"));
        results.Add(CheckTool("podman", "podman --version", "Podman", "Container Engine"));
        results.Add(CheckTool("buildah", "buildah --version", "Buildah", "Container Builder"));
        
        return results;
    }
    
    private static List<DevCheckResult> CheckWebDevelopment()
    {
        var results = new List<DevCheckResult>();
        
        // Frontend tools
        results.Add(CheckTool("npm", "npm --version", "NPM", "Frontend Package Manager"));
        results.Add(CheckTool("yarn", "yarn --version", "Yarn", "Frontend Package Manager"));
        results.Add(CheckTool("pnpm", "pnpm --version", "PNPM", "Frontend Package Manager"));
        
        // Build tools
        results.Add(CheckTool("webpack", "webpack --version", "Webpack", "Module Bundler"));
        results.Add(CheckTool("vite", "vite --version", "Vite", "Build Tool"));
        results.Add(CheckTool("parcel", "parcel --version", "Parcel", "Build Tool"));
        
        // Frameworks
        results.Add(CheckTool("ng", "ng version", "Angular CLI", "Framework CLI"));
        results.Add(CheckTool("vue", "vue --version", "Vue CLI", "Framework CLI"));
        results.Add(CheckTool("npx", "npx create-react-app --version", "React CLI", "Framework CLI"));
        
        return results;
    }
    
    private static List<DevCheckResult> CheckMobileDevelopment()
    {
        var results = new List<DevCheckResult>();
        
        // React Native
        results.Add(CheckTool("npx", "npx react-native --version", "React Native", "Mobile Framework"));
        
        // Flutter
        results.Add(CheckTool("flutter", "flutter --version", "Flutter", "Mobile Framework"));
        
        // Cordova
        results.Add(CheckTool("cordova", "cordova --version", "Cordova", "Mobile Framework"));
        
        // Android
        results.Add(CheckTool("adb", "adb version", "Android Debug Bridge", "Mobile Development"));
        
        return results;
    }
    
    private static List<DevCheckResult> CheckSystemTools()
    {
        var results = new List<DevCheckResult>();
        
        // Shell tools
        results.Add(CheckTool("bash", "bash --version", "Bash", "Shell"));
        results.Add(CheckTool("zsh", "zsh --version", "Zsh", "Shell"));
        results.Add(CheckTool("fish", "fish --version", "Fish", "Shell"));
        
        // Text editors
        results.Add(CheckTool("vim", "vim --version", "Vim", "Text Editor"));
        results.Add(CheckTool("nano", "nano --version", "Nano", "Text Editor"));
        results.Add(CheckTool("emacs", "emacs --version", "Emacs", "Text Editor"));
        
        // System utilities
        results.Add(CheckTool("curl", "curl --version", "Curl", "HTTP Client"));
        results.Add(CheckTool("wget", "wget --version", "Wget", "HTTP Client"));
        results.Add(CheckTool("jq", "jq --version", "JQ", "JSON Processor"));
        results.Add(CheckTool("tree", "tree --version", "Tree", "Directory Viewer"));
        
        return results;
    }
    
    private static List<DevCheckResult> CheckIDEs()
    {
        var results = new List<DevCheckResult>();
        
        // VS Code
        results.Add(CheckTool("code", "code --version", "VS Code", "IDE"));
        
        // JetBrains IDEs
        results.Add(CheckTool("code-insiders", "code-insiders --version", "VS Code Insiders", "IDE"));
        results.Add(CheckTool("idea", "idea --version", "IntelliJ IDEA", "IDE"));
        results.Add(CheckTool("webstorm", "webstorm --version", "WebStorm", "IDE"));
        results.Add(CheckTool("pycharm", "pycharm --version", "PyCharm", "IDE"));
        
        // Other editors
        results.Add(CheckTool("subl", "subl --version", "Sublime Text", "Editor"));
        results.Add(CheckTool("atom", "atom --version", "Atom", "Editor"));
        
        return results;
    }
    
    private static List<DevCheckResult> CheckBuildSystems()
    {
        var results = new List<DevCheckResult>();
        
        // General build tools
        results.Add(CheckTool("make", "make --version", "Make", "Build System"));
        results.Add(CheckTool("cmake", "cmake --version", "CMake", "Build System"));
        results.Add(CheckTool("ninja", "ninja --version", "Ninja", "Build System"));
        
        // Language-specific
        results.Add(CheckTool("mvn", "mvn --version", "Maven", "Build System"));
        results.Add(CheckTool("gradle", "gradle --version", "Gradle", "Build System"));
        results.Add(CheckTool("sbt", "sbt --version", "SBT", "Build System"));
        results.Add(CheckTool("leiningen", "lein version", "Leiningen", "Build System"));
        
        return results;
    }
    
    private static List<DevCheckResult> CheckTestingTools()
    {
        var results = new List<DevCheckResult>();
        
        // General testing
        results.Add(CheckTool("pytest", "pytest --version", "Pytest", "Testing Framework"));
        results.Add(CheckTool("jest", "jest --version", "Jest", "Testing Framework"));
        results.Add(CheckTool("mocha", "mocha --version", "Mocha", "Testing Framework"));
        results.Add(CheckTool("jasmine", "jasmine --version", "Jasmine", "Testing Framework"));
        
        // Code coverage
        results.Add(CheckTool("coverage", "coverage --version", "Coverage.py", "Code Coverage"));
        results.Add(CheckTool("nyc", "nyc --version", "NYC", "Code Coverage"));
        
        // Performance testing
        results.Add(CheckTool("ab", "ab -V", "Apache Bench", "Performance Testing"));
        results.Add(CheckTool("wrk", "wrk --version", "WRK", "Performance Testing"));
        
        return results;
    }
    
    private static List<DevCheckResult> CheckDocumentationTools()
    {
        var results = new List<DevCheckResult>();
        
        // Documentation generators
        results.Add(CheckTool("doxygen", "doxygen --version", "Doxygen", "Documentation Generator"));
        results.Add(CheckTool("sphinx", "sphinx-build --version", "Sphinx", "Documentation Generator"));
        results.Add(CheckTool("mkdocs", "mkdocs --version", "MkDocs", "Documentation Generator"));
        results.Add(CheckTool("hugo", "hugo version", "Hugo", "Static Site Generator"));
        results.Add(CheckTool("jekyll", "jekyll --version", "Jekyll", "Static Site Generator"));
        
        // API documentation
        results.Add(CheckTool("swagger-codegen", "swagger-codegen --version", "Swagger Codegen", "API Documentation"));
        results.Add(CheckTool("openapi-generator", "openapi-generator --version", "OpenAPI Generator", "API Documentation"));
        
        return results;
    }
    
    private static DevCheckResult CheckTool(string command, string versionCommand, string name, string category)
    {
        var result = new DevCheckResult
        {
            ToolName = name,
            Category = category,
            IsInstalled = false
        };
        
        try
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/usr/bin/which",
                    Arguments = command,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            proc.WaitForExit();
            
            if (proc.ExitCode == 0)
            {
                result.IsInstalled = true;
                
                // Get version
                try
                {
                    var versionProc = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "/bin/bash",
                            Arguments = $"-c \"{versionCommand}\"",
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };
                    versionProc.Start();
                    string versionOutput = versionProc.StandardOutput.ReadToEnd();
                    string errorOutput = versionProc.StandardError.ReadToEnd();
                    versionProc.WaitForExit();
                    
                    if (!string.IsNullOrWhiteSpace(versionOutput))
                    {
                        result.Version = ExtractVersion(versionOutput);
                    }
                    else if (!string.IsNullOrWhiteSpace(errorOutput))
                    {
                        result.Version = ExtractVersion(errorOutput);
                    }
                    else
                    {
                        result.Version = "Unknown";
                    }
                }
                catch
                {
                    result.Version = "Unknown";
                }
                
                result.Status = "Installed / Установлен";
                result.Recommendation = "Tool is ready for use / Инструмент готов к использованию";
            }
            else
            {
                result.Status = "Not installed / Не установлен";
                result.Recommendation = GetInstallationRecommendation(name, category);
            }
        }
        catch
        {
            result.Status = "Not installed / Не установлен";
            result.Recommendation = GetInstallationRecommendation(name, category);
        }
        
        return result;
    }
    
    private static string ExtractVersion(string output)
    {
        var lines = output.Split('\n');
        foreach (var line in lines)
        {
            var versionMatch = System.Text.RegularExpressions.Regex.Match(line, @"(\d+\.\d+(\.\d+)?)");
            if (versionMatch.Success)
            {
                return versionMatch.Value;
            }
        }
        return "Unknown";
    }
    
    private static string GetInstallationRecommendation(string toolName, string category)
    {
        var recommendations = new Dictionary<string, string>
        {
            // Languages
            ["Python"] = "sudo pacman -S python python-pip",
            ["Node.js"] = "sudo pacman -S nodejs npm",
            ["Java"] = "sudo pacman -S jdk-openjdk",
            ["GCC"] = "sudo pacman -S gcc",
            [".NET"] = "sudo pacman -S dotnet-sdk",
            ["Go"] = "sudo pacman -S go",
            ["Rust"] = "curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh",
            ["PHP"] = "sudo pacman -S php php-composer",
            ["Ruby"] = "sudo pacman -S ruby",
            
            // Tools
            ["Git"] = "sudo pacman -S git",
            ["Docker"] = "sudo pacman -S docker docker-compose",
            ["PostgreSQL"] = "sudo pacman -S postgresql",
            ["MySQL"] = "sudo pacman -S mysql",
            ["Redis"] = "sudo pacman -S redis",
            ["MongoDB"] = "Install from AUR or manual setup",
            
            // Editors
            ["VS Code"] = "Install from AUR or download from website",
            ["Vim"] = "sudo pacman -S vim",
            ["Nano"] = "sudo pacman -S nano",
            
            // Build tools
            ["Make"] = "sudo pacman -S make",
            ["CMake"] = "sudo pacman -S cmake",
            ["Maven"] = "sudo pacman -S maven",
            ["Gradle"] = "sudo pacman -S gradle",
            
            // Testing
            ["Pytest"] = "pip3 install pytest",
            ["Jest"] = "npm install -g jest",
            
            // Default
            ["default"] = $"Install {toolName} using package manager or from official website"
        };
        
        return recommendations.GetValueOrDefault(toolName, recommendations["default"]);
    }
    
    private static void DisplayDevResults(List<DevCheckResult> results)
    {
        Logger.Logger.Info("=== Development Environment Results / Результаты Среды Разработки ===");
        
        var categories = results.GroupBy(r => r.Category).OrderBy(g => g.Key);
        
        foreach (var category in categories)
        {
            Console.WriteLine($"\n📁 {category.Key} / {category.Key}:");
            
            var toolsInCategory = category.OrderBy(r => r.ToolName);
            
            foreach (var tool in toolsInCategory)
            {
                var icon = tool.IsInstalled ? "✅" : "❌";
                var version = tool.IsInstalled && tool.Version != "Unknown" ? $" (v{tool.Version})" : "";
                Console.WriteLine($"   {icon} {tool.ToolName}{version}");
                
                if (!tool.IsInstalled)
                {
                    Console.WriteLine($"      💡 {tool.Recommendation}");
                }
            }
        }
        
        // Summary
        var totalTools = results.Count;
        var installedTools = results.Count(r => r.IsInstalled);
        var installRate = (double)installedTools / totalTools * 100;
        
        Console.WriteLine($"\n📊 Summary / Сводка:");
        Console.WriteLine($"   Total tools: {totalTools}");
        Console.WriteLine($"   Installed: {installedTools}");
        Console.WriteLine($"   Installation rate: {installRate:F1}%");
        
        // Recommendations
        Console.WriteLine($"\n💡 Recommendations / Рекомендации:");
        
        var missingCategories = results.Where(r => !r.IsInstalled)
            .GroupBy(r => r.Category)
            .Where(g => g.Count() > 0)
            .OrderByDescending(g => g.Count())
            .Take(3);
        
        foreach (var category in missingCategories)
        {
            Console.WriteLine($"   • {category.Key}: {category.Count()} tools missing");
        }
        
        if (installRate >= 80)
        {
            Logger.Logger.Success("Development environment is well configured! / Среда разработки хорошо настроена!");
        }
        else if (installRate >= 50)
        {
            Logger.Logger.Warning("Development environment needs some improvements / Среде разработки требуются улучшения");
        }
        else
        {
            Logger.Logger.Error("Development environment needs significant setup / Среде разработки требуется значительная настройка");
        }
    }
    
    // Создание шаблонов проектов
    public static void CreateProjectTemplate(string projectName, string templateType)
    {
        var templates = GetProjectTemplates();
        var template = templates.FirstOrDefault(t => t.Name.Equals(templateType, StringComparison.OrdinalIgnoreCase));
        
        if (template == null)
        {
            Logger.Logger.Error($"Template '{templateType}' not found");
            return;
        }
        
        Logger.Logger.Info($"Creating project '{projectName}' using {template.Name} template...");
        
        try
        {
            var projectPath = Path.Combine(Directory.GetCurrentDirectory(), projectName);
            Directory.CreateDirectory(projectPath);
            
            // Create basic files
            foreach (var file in template.Files)
            {
                var filePath = Path.Combine(projectPath, file);
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                File.WriteAllText(filePath, GetFileContent(file, template.Language));
            }
            
            // Run setup commands
            foreach (var command in template.Commands)
            {
                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "/bin/bash",
                        Arguments = $"-c \"{command.Replace("{project}", projectName)}\"",
                        WorkingDirectory = projectPath,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                proc.Start();
                proc.WaitForExit();
            }
            
            Logger.Logger.Success($"Project '{projectName}' created successfully!");
            Logger.Logger.Info($"Project location: {projectPath}");
        }
        catch (Exception ex)
        {
            Logger.Logger.Error($"Failed to create project: {ex.Message}");
        }
    }
    
    private static List<ProjectTemplate> GetProjectTemplates()
    {
        return new List<ProjectTemplate>
        {
            new ProjectTemplate
            {
                Name = "python-basic",
                Description = "Basic Python project",
                Language = "Python",
                Commands = new List<string> { "python3 -m venv venv", "source venv/bin/activate && pip install --upgrade pip" },
                Files = new List<string> { "main.py", "requirements.txt", "README.md", ".gitignore" }
            },
            new ProjectTemplate
            {
                Name = "node-basic",
                Description = "Basic Node.js project",
                Language = "JavaScript",
                Commands = new List<string> { "npm init -y", "npm install --save-dev jest" },
                Files = new List<string> { "index.js", "package.json", "README.md", ".gitignore" }
            },
            new ProjectTemplate
            {
                Name = "dotnet-basic",
                Description = "Basic .NET project",
                Language = "C#",
                Commands = new List<string> { "dotnet new console -n {project}", "dotnet restore" },
                Files = new List<string> { "Program.cs", "{project}.csproj", "README.md", ".gitignore" }
            },
            new ProjectTemplate
            {
                Name = "go-basic",
                Description = "Basic Go project",
                Language = "Go",
                Commands = new List<string> { "go mod init {project}" },
                Files = new List<string> { "main.go", "go.mod", "README.md", ".gitignore" }
            },
            new ProjectTemplate
            {
                Name = "rust-basic",
                Description = "Basic Rust project",
                Language = "Rust",
                Commands = new List<string> { "cargo init --name {project}" },
                Files = new List<string> { "src/main.rs", "Cargo.toml", "README.md", ".gitignore" }
            }
        };
    }
    
    private static string GetFileContent(string fileName, string language)
    {
        var contents = new Dictionary<string, Dictionary<string, string>>
        {
            ["Python"] = new Dictionary<string, string>
            {
                ["main.py"] = "#!/usr/bin/env python3\n\ndef main():\n    print(\"Hello, World!\")\n\nif __name__ == \"__main__\":\n    main()",
                ["requirements.txt"] = "# Add your dependencies here\nrequests>=2.25.0",
                ["README.md"] = "# Project Name\n\nDescription of the project.\n\n## Installation\n\n```bash\npip install -r requirements.txt\n```",
                [".gitignore"] = "__pycache__/\n*.pyc\nvenv/\n.env\n*.log"
            },
            ["JavaScript"] = new Dictionary<string, string>
            {
                ["index.js"] = "console.log('Hello, World!');",
                ["package.json"] = "{\n  \"name\": \"project-name\",\n  \"version\": \"1.0.0\",\n  \"description\": \"\",\n  \"main\": \"index.js\",\n  \"scripts\": {\n    \"test\": \"jest\"\n  }\n}",
                ["README.md"] = "# Project Name\n\nDescription of the project.\n\n## Installation\n\n```bash\nnpm install\n```",
                [".gitignore"] = "node_modules/\n.env\n*.log\ndist/"
            },
            ["C#"] = new Dictionary<string, string>
            {
                ["Program.cs"] = "using System;\n\nclass Program\n{\n    static void Main()\n    {\n        Console.WriteLine(\"Hello, World!\");\n    }\n}",
                ["{project}.csproj"] = "<Project Sdk=\"Microsoft.NET.Sdk\">\n  <PropertyGroup>\n    <OutputType>Exe</OutputType>\n    <TargetFramework>net6.0</TargetFramework>\n  </PropertyGroup>\n</Project>",
                ["README.md"] = "# Project Name\n\nDescription of the project.\n\n## Installation\n\n```bash\ndotnet restore\ndotnet run\n```",
                [".gitignore"] = "bin/\nobj/\n*.user\n*.suo"
            },
            ["Go"] = new Dictionary<string, string>
            {
                ["main.go"] = "package main\n\nimport \"fmt\"\n\nfunc main() {\n    fmt.Println(\"Hello, World!\")\n}",
                ["go.mod"] = "module project-name\n\ngo 1.21",
                ["README.md"] = "# Project Name\n\nDescription of the project.\n\n## Installation\n\n```bash\ngo mod tidy\ngo run main.go\n```",
                [".gitignore"] = "*.exe\n*.exe~\n*.dll\n*.so\n*.dylib\ntest*.out"
            },
            ["Rust"] = new Dictionary<string, string>
            {
                ["src/main.rs"] = "fn main() {\n    println!(\"Hello, world!\");\n}",
                ["Cargo.toml"] = "[package]\nname = \"project-name\"\nversion = \"0.1.0\"\nedition = \"2021\"",
                ["README.md"] = "# Project Name\n\nDescription of the project.\n\n## Installation\n\n```bash\ncargo run\n```",
                [".gitignore"] = "/target\nCargo.lock"
            }
        };
        
        if (contents.TryGetValue(language, out var langContents) && 
            langContents.TryGetValue(fileName, out var content))
        {
            return content;
        }
        
        return $"# {fileName}\n# Generated file for {language} project";
    }
    
    // Проверка зависимостей проекта
    public static void CheckProjectDependencies(string projectPath)
    {
        Logger.Logger.Info($"Checking dependencies for project: {projectPath}");
        
        if (!Directory.Exists(projectPath))
        {
            Logger.Logger.Error($"Project path not found: {projectPath}");
            return;
        }
        
        var dependencyFiles = new[]
        {
            "requirements.txt", "Pipfile", "pyproject.toml", // Python
            "package.json", "yarn.lock", "package-lock.json", // Node.js
            "pom.xml", "build.gradle", "build.sbt", // Java
            "Cargo.toml", "Cargo.lock", // Rust
            "go.mod", "go.sum", // Go
            "*.csproj", "packages.config", // C#
            "composer.json", "composer.lock", // PHP
            "Gemfile", "Gemfile.lock" // Ruby
        };
        
        var foundFiles = new List<string>();
        
        foreach (var pattern in dependencyFiles)
        {
            var files = Directory.GetFiles(projectPath, pattern, SearchOption.AllDirectories);
            foundFiles.AddRange(files);
        }
        
        if (foundFiles.Count == 0)
        {
            Logger.Logger.Warning("No dependency files found in the project");
            return;
        }
        
        Console.WriteLine($"\n📦 Found dependency files:");
        foreach (var file in foundFiles)
        {
            Console.WriteLine($"   📄 {Path.GetFileName(file)}");
        }
        
        // Analyze each dependency file
        foreach (var file in foundFiles)
        {
            AnalyzeDependencyFile(file);
        }
    }
    
    private static void AnalyzeDependencyFile(string filePath)
    {
        try
        {
            var fileName = Path.GetFileName(filePath);
            var content = File.ReadAllText(filePath);
            
            Console.WriteLine($"\n🔍 Analyzing {fileName}:");
            
            switch (fileName.ToLower())
            {
                case "requirements.txt":
                    AnalyzePythonRequirements(content);
                    break;
                case "package.json":
                    AnalyzeNodePackage(content);
                    break;
                case "cargo.toml":
                    AnalyzeRustCargo(content);
                    break;
                case "go.mod":
                    AnalyzeGoModule(content);
                    break;
                default:
                    Console.WriteLine($"   No specific analysis available for {fileName}");
                    break;
            }
        }
        catch (Exception ex)
        {
            Logger.Logger.Error($"Failed to analyze {filePath}: {ex.Message}");
        }
    }
    
    private static void AnalyzePythonRequirements(string content)
    {
        var lines = content.Split('\n');
        var dependencies = lines.Where(line => !string.IsNullOrWhiteSpace(line) && !line.StartsWith("#")).ToList();
        
        Console.WriteLine($"   📦 Python dependencies: {dependencies.Count}");
        foreach (var dep in dependencies.Take(5))
        {
            Console.WriteLine($"      • {dep}");
        }
        if (dependencies.Count > 5)
        {
            Console.WriteLine($"      ... and {dependencies.Count - 5} more");
        }
    }
    
    private static void AnalyzeNodePackage(string content)
    {
        try
        {
            var packageJson = JsonSerializer.Deserialize<JsonElement>(content);
            
            if (packageJson.TryGetProperty("dependencies", out var deps))
            {
                var depCount = 0;
                foreach (var dep in deps.EnumerateObject())
                {
                    depCount++;
                    if (depCount <= 5)
                    {
                        Console.WriteLine($"      • {dep.Name}: {dep.Value}");
                    }
                }
                Console.WriteLine($"   📦 Dependencies: {depCount}");
            }
            
            if (packageJson.TryGetProperty("devDependencies", out var devDeps))
            {
                var devDepCount = 0;
                foreach (var dep in devDeps.EnumerateObject())
                {
                    devDepCount++;
                }
                Console.WriteLine($"   🔧 Dev dependencies: {devDepCount}");
            }
        }
        catch
        {
            Console.WriteLine("   Could not parse package.json");
        }
    }
    
    private static void AnalyzeRustCargo(string content)
    {
        var lines = content.Split('\n');
        var inDependencies = false;
        var depCount = 0;
        
        foreach (var line in lines)
        {
            if (line.Trim().StartsWith("[dependencies]"))
            {
                inDependencies = true;
                continue;
            }
            if (line.Trim().StartsWith("[") && inDependencies)
            {
                inDependencies = false;
                continue;
            }
            if (inDependencies && !string.IsNullOrWhiteSpace(line) && !line.StartsWith("#"))
            {
                depCount++;
                if (depCount <= 5)
                {
                    Console.WriteLine($"      • {line.Trim()}");
                }
            }
        }
        
        Console.WriteLine($"   📦 Rust dependencies: {depCount}");
    }
    
    private static void AnalyzeGoModule(string content)
    {
        var lines = content.Split('\n');
        var inRequire = false;
        var depCount = 0;
        
        foreach (var line in lines)
        {
            if (line.Trim() == "require")
            {
                inRequire = true;
                continue;
            }
            if (line.Trim() == ")" && inRequire)
            {
                inRequire = false;
                continue;
            }
            if (inRequire && !string.IsNullOrWhiteSpace(line) && !line.StartsWith("#"))
            {
                depCount++;
                if (depCount <= 5)
                {
                    Console.WriteLine($"      • {line.Trim()}");
                }
            }
        }
        
        Console.WriteLine($"   📦 Go dependencies: {depCount}");
    }
}
