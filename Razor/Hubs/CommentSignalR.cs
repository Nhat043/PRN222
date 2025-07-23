using Microsoft.AspNetCore.SignalR;

namespace Razor.Hubs
{
    public class CommentSignalR : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
        public async Task SendAllLoad()
        {
            await Clients.All.SendAsync("load");
        }

    }
}
