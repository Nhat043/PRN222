using Microsoft.AspNetCore.SignalR;

namespace Razor.Hubs
{
    public class DataSignalR : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "AdminGroup");
            await base.OnConnectedAsync();
        }

        public async Task NotifyAdminNewOrder()
        {
            await Clients.Group("AdminGroup").SendAsync("NewOrderReceived");
        }

        public async Task NotifyProductQuantityChanged(int productId)
        {
            await Clients.Group("AdminGroup").SendAsync("ProductQuantityChanged", productId);
        }

        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public async Task SendAllLoad()
        {
            await Clients.All.SendAsync("load");
        }

        public async Task SendAllLoadComment()
        {
            await Clients.All.SendAsync("loadComment");
        }
        
    }
}
