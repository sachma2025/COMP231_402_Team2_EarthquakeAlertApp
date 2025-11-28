namespace Team2_EarthquakeAlertApp.Models
{
    public class VictimReport
    {
        public string timestamp { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string Name { get; internal set; }   
        public string injuryLevel { get; set; }
        public string description { get; set; }
        public string PhotoPath { get; set; }
      

    }


}
