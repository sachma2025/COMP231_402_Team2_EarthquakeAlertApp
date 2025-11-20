using Amazon.DynamoDBv2.DataModel;

namespace Team2_EarthquakeAlertApp.Models
{
    [DynamoDBTable("DistressSignals")]
    public class SosRequest
    {
        [DynamoDBHashKey]
        public string SignalId { get; set; }

        [DynamoDBProperty]
        public string VictimName { get; set; }

        [DynamoDBProperty]
        public double Latitude { get; set; }

        [DynamoDBProperty]
        public double Longitude { get; set; }

        [DynamoDBProperty]
        public string Description { get; set; }

        [DynamoDBProperty]
        public string Timestamp { get; set; }

        [DynamoDBProperty]
        public string Status { get; set; } = "Active";
    }
}
