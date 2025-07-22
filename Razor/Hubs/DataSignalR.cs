using Microsoft.AspNetCore.SignalR;

namespace Razor.Hubs
{
    public class DataSignalR:Hub
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

    }
}
