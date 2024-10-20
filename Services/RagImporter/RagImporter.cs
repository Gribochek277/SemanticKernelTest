using System.Diagnostics.CodeAnalysis;
using Microsoft.SemanticKernel.Memory;

namespace SemanticKernelTest.Services;

public class RagImporter: IRagImporter
{
    [Experimental("SKEXP0001")]
    public void ImportMemoryCollection(string memoryCollectionName, ISemanticTextMemory semanticTextMemory)
    {
        throw new NotImplementedException();
    }
}