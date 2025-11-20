using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using System.Threading.Tasks;
using Team2_EarthquakeAlertApp.Models;

namespace Team2_EarthquakeAlertApp.Services
{
    public class DynamoDbService
    {
        private readonly DynamoDBContext _context;

        public DynamoDbService()
        {
            var client = new AmazonDynamoDBClient();
            _context = new DynamoDBContext(client);
        }

        public async Task SaveSignalAsync(SosRequest signal)
        {
            await _context.SaveAsync(signal);
        }

        public async Task<List<SosRequest>> GetAllSignalsAsync()
        {
            var conditions = new List<ScanCondition>();
            return await _context.ScanAsync<SosRequest>(conditions).GetRemainingAsync();
        }
    }
}
