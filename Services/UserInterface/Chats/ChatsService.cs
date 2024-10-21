using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using SemanticKernelTest.Services.UserInterface.Chats.Chat;
using SemanticKernelTest.Services.UserInterface.Chats.Factory;

namespace SemanticKernelTest.Services.UserInterface.Chats;

public class ChatsService: IChatsService
{
    private readonly IConfiguration _configuration;
    private readonly Kernel _kernel;
    
    public List<IChat> Chats { get; private set; }

    public ChatsService(Kernel kernel, IChatCollectionFactory chatCollectionFactory, IConfiguration configuration)
    {
        _kernel = kernel;
        _configuration = configuration;
        Chats = chatCollectionFactory.CreateChats();
    }

    public async Task UseChats(CancellationToken cancellationToken)
    {
        while (true)
        {
            var question = Console.ReadLine();

            if (question == "exit")
            {
                break;
            }
            
            foreach (var chat in Chats)
            {
                await chat.SendMessage(question, cancellationToken);
            
                Console.WriteLine('\n');
                Console.WriteLine("************************************************");
            }
        }
    }
}