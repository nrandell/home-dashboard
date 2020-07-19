using System.Threading.Tasks;

namespace Dashboard.Hubs
{
    public interface IDataHub
    {
        Task Data(string topic, string json);
    }
}
