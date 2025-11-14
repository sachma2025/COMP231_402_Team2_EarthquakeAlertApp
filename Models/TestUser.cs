namespace Team2_EarthquakeAlertApp.Models
{
    public class TestUser
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }

    public static class UserData
    {
        // Created one hardcoded test First Responder
        public static readonly List<TestUser> Users = new()
        {
            new TestUser
            {
                Token = "FR-2847",
                Email = "firstresponder@test.com",
                Password = "fr123",
                Role = "FirstResponder"
            }
        };
    }
}
