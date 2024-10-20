using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Xml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Plugins.Memory;
using SemanticKernelTest.Services.DotNetSolutionScrapper;
using SemanticKernelTest.Utils;

#pragma warning disable CS8604 // Possible null reference argument.

namespace SemanticKernelTest;

class Program
{
    [Experimental("SKEXP0070")]
    static async Task Main(string[] args)
    {
        var customHttpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:11434/"),
            Timeout = TimeSpan.FromSeconds(5000)
        };
       
       HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

       builder.Services.AddTransient<ISolutionScrapper, SolutionScrapper>();
       //builder.Services.AddOllamaChatCompletion(builder.Configuration.GetValue<string>("Models:TextModel"), customHttpClient);
       //builder.Services.AddOllamaTextEmbeddingGeneration(builder.Configuration.GetValue<string>("Models:EmbeddingModel"), customHttpClient);
       
       //builder.Services.AddKeyedTransient<Kernel>("SemanticKernelTests", (serviceProvider, key) =>
       //{
       //    KernelPluginCollection kernelPluginCollection = [];
       //    
       //    return new Kernel(serviceProvider, kernelPluginCollection);
       //});

       var host = builder.Build();
       
       Kernel kernel = Kernel.CreateBuilder()
           .AddOllamaChatCompletion(builder.Configuration.GetValue<string>("Models:TextModel"), customHttpClient)
           .AddOllamaTextEmbeddingGeneration(builder.Configuration.GetValue<string>("Models:EmbeddingModel"), customHttpClient)
           .Build();


        SolutionScrapper scrapper = new SolutionScrapper();

        var locations = builder.Configuration.GetSection("Scrapper:Locations").Get<List<string>>();

        var scrappedFiles = scrapper.GetSourceFilesFromDirectory(locations[0]);
        
        //**** MEMORY INIT*****************
        var embeddingGenerator = kernel.Services.GetRequiredService<ITextEmbeddingGenerationService>();
        var memoryStore = new VolatileMemoryStore();
        var memory = new SemanticTextMemory(memoryStore, embeddingGenerator);
        
        
        const string memoryCollectionName = "IoCoreSources";
        

        int fileCount = scrappedFiles.Count;
        int currentIndex = 0;


        foreach (var scrappedFile in scrappedFiles)
        {
            var cacheFileName = Path.Combine(Directory.GetCurrentDirectory(), "memoryCache",
                memoryCollectionName + scrappedFile.Key);
            
            Console.WriteLine("Loaded " + Path.GetFileName(scrappedFile.Key));
            currentIndex++;
            var percentComplete = PercentageCounter.CountPercentage(fileCount, currentIndex);
            
            Console.WriteLine($"Processed: {percentComplete:F2}%");
            if(!File.Exists(cacheFileName))
            {
                var s = await memory.SaveInformationAsync(memoryCollectionName, id: scrappedFile.Key, text: scrappedFile.Value);
                
                var memoryRecord = await memoryStore.GetAsync(memoryCollectionName, scrappedFile.Key, true);
                
                var json = JsonSerializer.Serialize(memoryRecord);
                await File.WriteAllTextAsync(
                    Path.Combine(Directory.GetCurrentDirectory(),"memoryCache", memoryCollectionName + scrappedFile.Key),
                json);
            }
            else
            {
                var fileContent = File.ReadAllText(cacheFileName);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                if (!await memoryStore.DoesCollectionExistAsync(memoryCollectionName))
                {
                    await memoryStore.CreateCollectionAsync(memoryCollectionName);
                }
                var memoryRecord = JsonSerializer.Deserialize<MemoryRecord>(fileContent, options);
                await memoryStore.UpsertAsync(memoryCollectionName, memoryRecord);
            }
        }
        
        TextMemoryPlugin memoryPlugin = new(memory);

        //kernel = host.Services.GetKeyedService<Kernel>("SemanticKernelTests");
        // Import the text memory plugin into the Kernel.
        kernel.ImportPluginFromObject(memoryPlugin);
        
        OpenAIPromptExecutionSettings settings = new()
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
        };
        //********************************************
        
        while(true)
        {
        var question = Console.ReadLine();
        IAsyncEnumerable<StreamingKernelContent> response = null;
        //Console.WriteLine($"\n{ollamaModelName} response (no memory).");
        //var response = kernel.InvokePromptStreamingAsync(question);
        //await foreach (var result in response)
        //{
        //    Console.Write(result);
        //    
        //}
        
       
        Console.WriteLine("");
        Console.WriteLine("************************************************");
        
        var prompt = @"
                        Question: {{$input}}
                        Answer the question using the memory content: {{Recall}}";
        
        var arguments = new KernelArguments(settings)
        {
            { "input", question },
            { "collection", memoryCollectionName }
        };
        
        
        Console.WriteLine($"{builder.Configuration.GetValue<string>("Models:TextModel")} response (using semantic memory).");
        
        response = kernel.InvokePromptStreamingAsync(prompt, arguments);
        await foreach (var result in response)
        {
            Console.Write(result);
        }
        }
    }
}