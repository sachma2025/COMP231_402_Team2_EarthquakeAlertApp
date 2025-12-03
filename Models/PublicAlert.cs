using Amazon.DynamoDBv2.DataModel;

namespace Team2_EarthquakeAlertApp.Models
{
    [DynamoDBTable("PublicAlerts")]
    public class PublicAlert
    {
        [DynamoDBHashKey]
        public string timestamp { get; set; }

        [DynamoDBProperty]
        public string title { get; set; }

        [DynamoDBProperty]
        public string magnitude { get; set; }

        [DynamoDBProperty]
        public string description { get; set; }
    }
}
