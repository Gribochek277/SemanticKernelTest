using SemanticKernelTest.Utils;

namespace SemanticKernelTest.Services.DotNetSolutionScrapper;

public class SolutionScrapper: ISolutionScrapper
{
    private readonly string[] _allowedFileExtensions = new[] { ".cs", ".sln" };
    
    /// <inheritdoc cref="ISolutionScrapper"/>
    public Dictionary<string, string> GetSourceFilesFromDirectory(string rootDirectory)
    {
        var filesDictionary = new Dictionary<string, string>();

        List<string> filesNames = new List<string>();
        foreach (var extension in _allowedFileExtensions)
        {
            filesNames.AddRange(Directory.GetFiles(rootDirectory, "*" + extension,
                SearchOption.AllDirectories));
        }

        int indexCounter = 0;
        int filesCount = filesNames.Count;
        
        foreach (var file in filesNames)
        {
            try
            {
                indexCounter++;
                var percentage = PercentageCounter.CountPercentage(filesCount, indexCounter);
                var content = File.ReadAllText(file);
                Console.Clear();
                Console.WriteLine($"{nameof(SolutionScrapper)} scrapping directory {percentage:F2}%");
                filesDictionary[file.Replace(rootDirectory, "").Replace('/','_')] = content;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file {file}: {ex.Message}");
            }
        }

        return filesDictionary;
    }
}