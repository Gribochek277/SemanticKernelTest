using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Plugins.Memory;
using SemanticKernelTest.Utils;

namespace SemanticKernelTest.Services.Rag;

public class RagImporter: IRagImporter
{
    private readonly Kernel _kernel;

    public RagImporter(Kernel kernel)
    {
        _kernel = kernel;
    }
    [Experimental("SKEXP0001")]
    public async Task ImportMemoryCollection(string memoryCollectionName, Dictionary<string, string> scrappedFiles, CancellationToken cancellationToken)
    {
        var embeddingGenerator = _kernel.Services.GetRequiredService<ITextEmbeddingGenerationService>();
        var memoryStore = new VolatileMemoryStore();
        var memory = new SemanticTextMemory(memoryStore,
            embeddingGenerator);

        var fileCount = scrappedFiles.Count;
        var currentIndex = 0;


        foreach (var scrappedFile in scrappedFiles)
        {
            var cacheFileName = Path.Combine(Directory.GetCurrentDirectory(),
                "memoryCache",
                memoryCollectionName + scrappedFile.Key);

            Console.WriteLine("Loaded " + Path.GetFileName(scrappedFile.Key));
            currentIndex++;
            var percentComplete = PercentageCounter.CountPercentage(fileCount,
                currentIndex);

            Console.WriteLine($"Processed: {percentComplete:F2}%");
            if (!File.Exists(cacheFileName))
            {
                var s = await memory.SaveInformationAsync(memoryCollectionName,
                    id: scrappedFile.Key ?? throw new InvalidOperationException(),
                    text: scrappedFile.Value, cancellationToken: cancellationToken);

                var memoryRecord = await memoryStore.GetAsync(memoryCollectionName,
                    scrappedFile.Key,
                    true, cancellationToken);

                var json = JsonSerializer.Serialize(memoryRecord);
                await File.WriteAllTextAsync(
                    Path.Combine(Directory.GetCurrentDirectory(),
                        "memoryCache",
                        memoryCollectionName + scrappedFile.Key),
                    json, cancellationToken);
            }
            else
            {
                var fileContent = File.ReadAllText(cacheFileName);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                if (!await memoryStore.DoesCollectionExistAsync(memoryCollectionName, cancellationToken))
                {
                    await memoryStore.CreateCollectionAsync(memoryCollectionName, cancellationToken);
                }

                var memoryRecord = JsonSerializer.Deserialize<MemoryRecord>(fileContent,
                    options);
                await memoryStore.UpsertAsync(memoryCollectionName,
                    memoryRecord, cancellationToken);
            }
        }

        TextMemoryPlugin memoryPlugin = new(memory);

        // Import the text memory plugin into the Kernel.
        _kernel.ImportPluginFromObject(memoryPlugin);

    }
}