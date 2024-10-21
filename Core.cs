using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using SemanticKernelTest.Services.DotNetSolutionScrapper;
using SemanticKernelTest.Services.Rag;
using SemanticKernelTest.Services.UserInterface.Chats;

namespace SemanticKernelTest;

/// <inheritdoc cref="ICore"/>
public sealed class Core: ICore
{
    private readonly ISolutionScrapper _solutionScrapper;
    private readonly IConfiguration _configuration;
    private readonly IChatsService _chatsService;
    private readonly IRagImporter _ragImporter;
    
    //TODO: remove shortcut
    public const string MemoryCollectionName = "IoCoreSources";

    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="solutionScrapper"></param>
    /// <param name="configuration"></param>
    /// <param name="ragImporter"></param>
    /// <param name="chatsService"></param>
    public Core(
        ISolutionScrapper solutionScrapper,
        IConfiguration configuration, IRagImporter ragImporter, IChatsService chatsService)
    {
        _solutionScrapper = solutionScrapper;
        _configuration = configuration;
        _chatsService = chatsService;
        _ragImporter = ragImporter;
    }

    ///<inheritdoc cref="ICore"/>
    [Experimental("SKEXP0050")]
    public async Task RunAsync(CancellationToken cancellationToken)
    {
       var locations = _configuration.GetSection("Scrapper:Locations")
            .Get<List<string>>();

        Dictionary<string, string> scrappedFiles = _solutionScrapper.GetSourceFilesFromDirectory(locations[0]);
    
        await _ragImporter.ImportMemoryCollection(MemoryCollectionName, scrappedFiles, cancellationToken);
        
        await _chatsService.UseChats(cancellationToken);
    }
}