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
            var message = Store.Latest;
            if (message != null)
            {
                var json = message.ConvertPayloadToString();
                await Clients.Caller.Data(message.Topic, json);
            }
            await base.OnConnectedAsync();
        }
    }
}
