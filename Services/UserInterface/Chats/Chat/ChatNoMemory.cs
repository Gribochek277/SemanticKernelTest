using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;

namespace SemanticKernelTest.Services.UserInterface.Chats.Chat;

/// <summary>
/// Chat to model without using any context
/// </summary>
public class ChatNoMemory: IChat
{
    private readonly Kernel _kernel;
    private readonly string _modelName; 

    public ChatNoMemory(Kernel kernel, IConfiguration configuration)
    {
        _kernel = kernel;
        _modelName = configuration.GetValue<string>("Models:TextModel");
    }
    
    /// <inheritdoc cref="IChat.SendMessage"/>
    public async Task SendMessage(string message, CancellationToken cancellationToken)
    {
        IAsyncEnumerable<StreamingKernelContent> response = _kernel.InvokePromptStreamingAsync(message ?? string.Empty, cancellationToken: cancellationToken);;
        Console.WriteLine($"\n{_modelName} response (no memory).");
     
        await foreach (var result in response)
        {
            Console.Write(result);
        }
    }
}