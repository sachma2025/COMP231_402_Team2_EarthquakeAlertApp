using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
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
                ["longitude"] = request.Longitude,
                ["latitude"] = request.Latitude,
                ["people"] = request.People,
                ["message"] = request.Message,
                ["problem"] = request.Problem
            };

            try
            {
                await SosAlertTable.PutItemAsync(sosDoc);
                return $"Successfully sent alert: {request.Problem}";
            }
            catch (Exception ex)
            {
                return $"Error sending alert: {ex.Message}";
            }
        }
    }
}
