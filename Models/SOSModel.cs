using Amazon.DynamoDBv2.DataModel;

namespace Team2_EarthquakeAlertApp.Models
{
    [DynamoDBTable("SOSalerts")]
    public class SosRequest
    {
        [DynamoDBHashKey]
        public string timestamp { get; set; }

        [DynamoDBProperty]
        public double latitude { get; set; }

        [DynamoDBProperty]
        public double longitude { get; set; }

        [DynamoDBProperty]
        public string people { get; set; }

        [DynamoDBProperty]
        public string problem { get; set; }

        [DynamoDBProperty]
        public string message { get; set; }

        [DynamoDBProperty]
        public bool? accepted { get; set; }

        [DynamoDBProperty]
        public string name { get; set; }

        [DynamoDBProperty]
        public string injuryLevel { get; set; }

        [DynamoDBProperty]
        public string photoUrl { get; set; }

    }
}
