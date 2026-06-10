using System.Threading.Tasks;

namespace chetv_dz
{
    public interface ILogger
    {
        Task LogAsync(string type, params (string key, object value)[] fields);
        Task<string[]> GetLastLinesAsync(int count);
        Task<HistoryCounters> GetHistoryCountersAsync();
        void Shutdown();
    }
}