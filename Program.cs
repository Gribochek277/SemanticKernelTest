using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using SemanticKernelTest.Services.DotNetSolutionScrapper;
using SemanticKernelTest.Services.Rag;
using SemanticKernelTest.Services.UserInterface.Chats;
using SemanticKernelTest.Services.UserInterface.Chats.Factory;

namespace SemanticKernelTest;

static class Program
{
    private static IServiceProvider _serviceProvider;
    
    [Experimental("SKEXP0070")]
    static async Task Main(string[] args)
    { 
       RegisterServices(args);
       var core = _serviceProvider.GetService<ICore>();
       await core.RunAsync(CancellationToken.None);
    }

    [Experimental("SKEXP0070")]
    private static void RegisterServices(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();
        var textModel = configuration.GetValue<string>("Models:TextModel") ?? throw new InvalidOperationException();
        var embeddingModel = configuration.GetValue<string>("Models:EmbeddingModel") ?? throw new InvalidOperationException();
        
        var customHttpClient = new HttpClient
        {
            BaseAddress = new Uri(configuration.GetValue<string>("OllamaEndpoint") ?? throw new InvalidOperationException()),
            Timeout = TimeSpan.FromSeconds(5000)
        };
        
        var kernel = Kernel.CreateBuilder()
            .AddOllamaChatCompletion(textModel, customHttpClient)
            .AddOllamaTextEmbeddingGeneration(embeddingModel, customHttpClient)
            .Build();
        
        var collection = new ServiceCollection();

        collection.AddSingleton(_ => kernel);
        collection.AddScoped(_ => configuration);
        collection.AddTransient<IChatsService, ChatsService>();
        collection.AddTransient<ISolutionScrapper, SolutionScrapper>();
        collection.AddTransient<IChatCollectionFactory, ChatCollectionFactory>();
        collection.AddTransient<IRagImporter, RagImporter>();
        collection.AddTransient<ICore, Core>();
        
        _serviceProvider = collection.BuildServiceProvider();
    }
}