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
        private const string TableName = "SOSalerts";
        public DynamoDbService() {}
        
        private static readonly AmazonDynamoDBClient DynamoDBClient = InitializeClient();
        private static readonly ITable SosAlertTable = Table.LoadTable(DynamoDBClient, TableName);

        // I did a bad thing here... oh la la 
        // Will fix later (move hardcoded db credentials AWAY)
        private static AmazonDynamoDBClient InitializeClient()
        {
            string awsAccessKey = "AKIAWCOLNPESLCIBCUXR";
            string awsSecretKey = "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";

            AWSCredentials credentials = new BasicAWSCredentials(awsAccessKey, awsSecretKey);
            return new AmazonDynamoDBClient(credentials, RegionEndpoint.CACentral1);
        }

        // POST a new SOS alert
        public async Task<string> SendAlert(SosRequest request)
        {
            var sosDoc = new Document
            {
                ["timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ["accepted"] = false,
                ["longitude"] = request.longitude,
                ["latitude"] = request.latitude,
                ["people"] = request.people,
                ["message"] = request.message,
                ["problem"] = request.problem
            };

            try
            {
                await SosAlertTable.PutItemAsync(sosDoc);
                return $"Successfully sent alert: {request.problem}";
            }
            catch (Exception ex)
            {
                return $"Error sending alert: {ex.Message}";
            }
        }

        public async Task<List<SosRequest>> GetActiveAlerts()
        {
            using (var context = new DynamoDBContext(DynamoDBClient))
            {
                var conditions = new List<ScanCondition>
                {
                    new ScanCondition("accepted", ScanOperator.Equal, false)
                };

                var search = context.ScanAsync<SosRequest>(conditions);

                return await search.GetRemainingAsync();
            }
        }
        public async Task<string> UpdateAlertStatus(string timestamp, string newStatus)
        {
            if (string.IsNullOrEmpty(timestamp))
            {
                return "Error: Timestamp cannot be empty.";
            }

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

                await DynamoDBClient.UpdateItemAsync(request);

                return $"Successfully updated alert {timestamp} to accepted-status: {acceptedValue}";
            }
            catch (Exception ex)
            {
                return $"Error updating alert status for {timestamp}: {ex.Message}";
            }
        }
    }
}
