namespace SemanticKernelTest.Services.DotNetSolutionScrapper;

public interface ISolutionScrapper
{
    /// <summary>
    /// Recursively exctracts all files from provided directory
    /// </summary>
    /// <returns>Dictionary of filepath as a key and filecontent as a value</returns>
    Dictionary<string, string> GetSourceFilesFromDirectory(string rootDirectory);
}