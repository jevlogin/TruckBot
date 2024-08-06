namespace TruckBot.Model.User
{
    public class User
    {
        public long UserId { get; set; }
        public bool IsAdmin { get; set; } = false;
        public string? TelegramUsername { get; set; }
        public string? FirstName { get; set; }
        public string? SecondName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? DriversLicense { get; set; }
        public bool IsHasAuto { get; set; } = false;


        #region override
        public override string ToString()
        {
            return $"User ID: {UserId}\n" +
                   $"Is Admin: {IsAdmin}\n" +
                   $"Telegram Username: {TelegramUsername}\n" +
                   $"First Name: {FirstName}\n" +
                   $"Second Name: {SecondName}\n" +
                   $"Last Name: {LastName}\n" +
                   $"Email: {Email}\n" +
                   $"Phone: {Phone}\n" +
                   $"Drivers License: {DriversLicense}\n" +
                   $"Has Auto: {IsHasAuto}";
        } 
        #endregion
    }
}
