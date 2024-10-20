using System.Diagnostics.CodeAnalysis;
using Microsoft.SemanticKernel.Memory;

namespace SemanticKernelTest.Services;

/// <summary>
/// Serves to fill in rag data
/// </summary>
public interface IRagImporter
{
    /// <summary>
    /// Imports memory collection into <see cref="ISemanticTextMemory"/>
    /// </summary>
    /// <param name="memoryCollectionName">Name for memory collection</param>
    /// <param name="semanticTextMemory">Semantic text memory, basically a container for memories</param>
    [Experimental("SKEXP0001")]
    void ImportMemoryCollection(string memoryCollectionName, ISemanticTextMemory semanticTextMemory);
}