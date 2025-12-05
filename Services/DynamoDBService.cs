using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using System.Threading.Tasks;
using Team2_EarthquakeAlertApp.Models;

namespace Team2_EarthquakeAlertApp.Services
{
    public class DynamoDbService
    {
        private readonly AmazonDynamoDBClient _client;
        private readonly DynamoDBContext _context;
        private const string TableName = "SOSalerts";

        public DynamoDbService(IConfiguration config)
        {
            // Read AWS credentials from environment variables or appsettings.json
            string accessKey = config["AWS:AccessKey"];
            string secretKey = config["AWS:SecretKey"];
            string region = config["AWS:Region"];

            var credentials = new BasicAWSCredentials(accessKey, secretKey);
            var clientRegion = RegionEndpoint.GetBySystemName(region);

            _client = new AmazonDynamoDBClient(credentials, clientRegion);
            _context = new DynamoDBContext(_client);
        }

        // POST a new SOS alert
        public async Task<string> SendAlert(SosRequest request)
        {
            var table = Table.LoadTable(_client, TableName);

            var sosDoc = new Document
            {
                ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ["accepted"] = false,
                ["name"] = request.name,
                ["longitude"] = request.longitude,
                ["latitude"] = request.latitude,
                ["people"] = request.people,
                ["message"] = request.message,
                ["problem"] = request.problem,
                ["photoUrl"] = request.photoUrl
            };

            try
            {
                await table.PutItemAsync(sosDoc);
                return $"Successfully sent alert: {request.problem}";
            }
            catch (Exception ex)
            {
                return $"Error sending alert: {ex.Message}";
            }
        }

        public async Task<List<SosRequest>> GetActiveAlerts()
        {
            var conditions = new List<ScanCondition>
            {
                new ScanCondition("accepted", ScanOperator.Equal, false)
            };

            var search = _context.ScanAsync<SosRequest>(conditions);
            return await search.GetRemainingAsync();
        }

        public async Task SaveVictimReport(VictimReport report)
        {
            report.timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            await _context.SaveAsync(report);
        }

        public async Task<string> UpdateAlertStatus(string timestamp, string newStatus)
        {
            if (string.IsNullOrEmpty(timestamp))
                return "Error: Timestamp cannot be empty.";

            try
            {
                bool acceptedValue = newStatus.Equals("Accepted", StringComparison.OrdinalIgnoreCase);

                var key = new Dictionary<string, AttributeValue>
                {
                    { "timestamp", new AttributeValue { S = timestamp } }
                };

                var updates = new Dictionary<string, AttributeValueUpdate>
                {
                    {
                        "accepted",
                        new AttributeValueUpdate
                        {
                            Action = AttributeAction.PUT,
                            Value = new AttributeValue { BOOL = acceptedValue }
                        }
                    }
                };

                var request = new UpdateItemRequest
                {
                    TableName = TableName,
                    Key = key,
                    AttributeUpdates = updates,
                    ReturnValues = "NONE"
                };

                await _client.UpdateItemAsync(request);

                return $"Successfully updated alert {timestamp} to accepted-status: {acceptedValue}";
            }
            catch (Exception ex)
            {
                return $"Error updating alert status for {timestamp}: {ex.Message}";
            }
        }

        public async Task<List<VictimReport>> GetVictimReports()
        {
            var search = _context.ScanAsync<VictimReport>(new List<ScanCondition>());
            return await search.GetRemainingAsync();
        }

        public async Task SavePublicAlert(PublicAlert alert)
        {
            await _context.SaveAsync(alert);
        }
    }
}
