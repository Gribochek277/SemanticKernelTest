using System.Diagnostics.CodeAnalysis;

namespace SemanticKernelTest.Services.Rag
{
    /// <summary>
    /// Serves to fill in rag data
    /// </summary>
    public interface IRagImporter
    {
        /// <summary>
        /// Imports memory collection/>
        /// </summary>
        /// <param name="memoryCollectionName">Name for memory collection</param>
        /// <param name="scrappedFiles">Dictionary of files which were extracted</param>
        /// <param name="cancellationToken"></param>
        [Experimental("SKEXP0001")]
        Task ImportMemoryCollection(string memoryCollectionName, Dictionary<string, string> scrappedFiles, CancellationToken cancellationToken);
    }
}