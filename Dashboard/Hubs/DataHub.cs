using System.Threading.Tasks;

using Dashboard.Services;

using Microsoft.AspNetCore.SignalR;

using MQTTnet;

namespace Dashboard.Hubs
{
    public class DataHub : Hub<IDataHub>
    {
        public HildebrandStateStore Store { get; }

        public DataHub(HildebrandStateStore store)
        {
            Store = store;
        }

        public override async Task OnConnectedAsync()
        {
            var state = Store.Latest;
            if (state != null)
            {
                await Clients.Caller.Data(state.Topic, state.Json);
            }
            await base.OnConnectedAsync();
        }
    }
}
