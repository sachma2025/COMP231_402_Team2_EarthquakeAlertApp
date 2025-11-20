using Amazon.DynamoDBv2.DataModel;

namespace Team2_EarthquakeAlertApp.Models
{
    [DynamoDBTable("DistressSignals")]
    public class SosRequest
    {
        [DynamoDBHashKey]
        public string Timestamp { get; set; }   // use unix timestamp as a SOS id as well

        [DynamoDBProperty]
        public double Latitude { get; set; }

        [DynamoDBProperty]
        public double Longitude { get; set; }

        [DynamoDBProperty]
        public string People { get; set; }

        [DynamoDBProperty]
        public string Problem { get; set; }

        [DynamoDBProperty]
        public string Message { get; set; }

        [DynamoDBProperty]
        public string Status { get; set; } = "Active";
    }
}
