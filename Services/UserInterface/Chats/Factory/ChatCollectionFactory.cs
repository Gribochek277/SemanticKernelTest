using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using SemanticKernelTest.Services.UserInterface.Chats.Chat;

namespace SemanticKernelTest.Services.UserInterface.Chats.Factory;

/// <inheritdoc cref="IChatCollectionFactory"/>
public class ChatCollectionFactory: IChatCollectionFactory
{
    private readonly Kernel _kernel;
    private readonly IConfiguration _configuration;

    public ChatCollectionFactory(Kernel kernel,
        IConfiguration configuration)
    {
        _kernel = kernel;
        _configuration = configuration;
    }
    
    
    /// <inheritdoc cref="IChatCollectionFactory.CreateChats"/>
    public List<IChat> CreateChats()
    {
        var chats = new List<IChat>
        {
            new ChatNoMemory(_kernel, _configuration),
            new ChatWIthRag(_kernel, _configuration)
        };

        return chats;
    }
}