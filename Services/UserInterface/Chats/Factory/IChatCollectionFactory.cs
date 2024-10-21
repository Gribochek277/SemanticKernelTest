using SemanticKernelTest.Services.UserInterface.Chats.Chat;

namespace SemanticKernelTest.Services.UserInterface.Chats.Factory;

/// <summary>
/// Creates collection of chats to operate with
/// </summary>
public interface IChatCollectionFactory
{
    /// <summary>
    /// Create
    /// </summary>
    /// <returns></returns>
    List<IChat> CreateChats();
}