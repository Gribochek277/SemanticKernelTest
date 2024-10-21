using SemanticKernelTest.Services.UserInterface.Chats.Chat;

namespace SemanticKernelTest.Services.UserInterface.Chats;

/// <summary>
/// Provides console chat interface for user interaction with model
/// </summary>
public interface IChatsService
{
    /// <summary>
    /// Collection of different chats to operate
    /// </summary>
    List<IChat> Chats { get; }
    /// <summary>
    /// Run chat service in console
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task UseChats(CancellationToken cancellationToken);
}