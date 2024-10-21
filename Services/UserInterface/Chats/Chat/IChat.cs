using OpenAI.Chat;

namespace SemanticKernelTest.Services.UserInterface.Chats.Chat;
/// <summary>
/// Chat to operate with the model
/// </summary>
public interface IChat
{
    /// <summary>
    /// Send message
    /// </summary>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SendMessage(string message, CancellationToken cancellationToken);
}