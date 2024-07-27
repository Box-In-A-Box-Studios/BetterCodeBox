// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using BetterCodeBox.Lib.Analyzers;
using BetterCodeBox.Lib.Interfaces;
using BetterCodeBox.Lib;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

string solutionPath = args.Length > 0 ? args[0] : throw new ArgumentException("Path to the solution is required");
ILogger logger = new BIABLogger();
var configurationBuilder = new ConfigurationBuilder();
if (File.Exists("appsettings.json"))
{
    configurationBuilder.AddJsonFile("appsettings.json");
}

var configuration = configurationBuilder.Build();
List<ICodeAnalyzer> analyzers = await DirectoryScanner.ScanDirectory(solutionPath, logger, configuration);

string resultDirectory = configuration["ResultDirectory"] ?? "results";

if (!Directory.Exists(resultDirectory))
{
    Directory.CreateDirectory(resultDirectory);
}

string json = JSONHelper.SerializeJson(analyzers);
await File.WriteAllTextAsync(Path.Combine(resultDirectory, "results.json"), json);

// Too Tightly Coupled folders/Classes
// Naming Conventions
// Lacking Comments
// Use of Var or Explicit Type
// Use of dynamic
// Variable Naming and Method Naming Spellcheck
// Base Case Smells and Recursion check
// Unreachable Code
// Unused Variables
// Unused Methods
// Unused Parameters
// Unused Imports
// Bad use of Common Imports
    // EFCore
    // Linq
    // System
    // System.Text.Json
// Unused Interfaces/Classes
// Check for Empty Files or Empty Classes
// Check for Empty Methods that are not virtual
// Public Static Variables
// Missing Default Case in Switch Statements
// Folder should Match Namespace
// Ensure Classes are in the correct folder/namespace
// Potential Switch Replacement for If Statements
// Reduce Nesting
// User input validation/cleaning
// Check for Magic Numbers
// Check for Hardcoded Strings
// Check for Hardcoded Paths
// Check for Hardcoded URLs
// Check for Hardcoded Ports
// Check for Hardcoded IP Addresses
// Suggest IConfiguration for AppSettings that are hardcoded
// Check for Hardcoded Connection Strings
// Check for Hardcoded API Keys
// Check for Hardcoded Secrets
// Check for Hardcoded Tokens
// Check for Hardcoded Passwords
// Use of Thread.Sleep
// Use of Console.WriteLine
// Use of Console.ReadLine
// Use of Console.Write
// Use of Console.Clear
// Use of Console.ReadKey
// Use of Console.Error
// Suggestions for Logging
// Suggestions for Error Handling
// Dont Use out Parameters
// Dont Use ref Parameters
// Dont Use Excessive Tuple Returns
// Use Async for I/O Bound Operations or API Calls
// Use Task.Run for CPU Bound Operations
// Dont Use Thread
// Use of Task.Delay
// Nested Loops
// Nested Try Catch
// Altering List when Enumerating
// Dont Use Newtonsoft.Json
// Avoid Memory Allocation in Loops
// Use of String Interpolation
// Dont Use Async Void
// Potential Razor Integration
// Potential Blazor Integration
// Potential Unity Integration
// Potential Xamarin Integration
// Potential WPF Integration
// Potential WinForms Integration
// Potential Godot Integration
// HttpClient Usage Suggestions
// Nullablility Suggestions
// Null Checks
// .Net Version Suggestions
// White Space Suggestions
// Loss of Precision Suggestions
// Check for Foreign Symbols (Other Languages)
// Check for Non-English Comments
// File Exists before Read or Write
// Await inside an Async Method
// If is a git repo
    // It should have a readme
    // Should have a license
    // Should have a .gitignore (Include .vs, bin, obj, .idea, .vscode, .git)
// Dockerfile Generator
// Docker Compose Generator
// Gitingore Generator
// License Picker
// Readme Generator (Maybe)
// Event Unsubscribe if there are events subscribed