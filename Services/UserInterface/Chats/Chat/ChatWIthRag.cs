using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace SemanticKernelTest.Services.UserInterface.Chats.Chat;

/// <summary>
/// Chat implementation which take provided RAG
/// int context.
/// </summary>
public class ChatWIthRag: IChat
{
    private readonly Kernel _kernel;
    private readonly string _modelName; 
    
    public ChatWIthRag(Kernel kernel, IConfiguration configuration)
    {
        _kernel = kernel;
        _modelName = configuration.GetValue<string>("Models:TextModel");
    }
    /// <inheritdoc cref="IChat.SendMessage"/>
    public async Task SendMessage(string message, CancellationToken cancellationToken)
    {
        var prompt = @"
                        Question: {{$input}}
                        Answer the question using the memory content: {{Recall}}";
            
        OpenAIPromptExecutionSettings settings = new()
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
        };

        var arguments = new KernelArguments(settings)
        {
            {
                "input", message
            },
            {
                "collection", Core.MemoryCollectionName
            }
        };


        Console.WriteLine(
            $"{_modelName} response (using semantic memory).");

        var response = _kernel.InvokePromptStreamingAsync(prompt,
            arguments, cancellationToken: cancellationToken);
        await foreach (var result in response)
        {
            Console.Write(result);
        }
    }
}