
using Microsoft.AspNetCore.SignalR;

namespace SignalR
{
    public class HubServer : Hub
    {
        public void HasNewData()
        {
            Clients.All.SendAsync("ReloadProduct");
        }
    }
}
