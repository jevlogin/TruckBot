using System.ComponentModel.DataAnnotations;


namespace TruckBot.Model.User
{
    internal class User
    {
        public long UserId { get; set; }
        public bool IsAdmin { get; set; } = false;
        public string? TelegramUsername { get; set; }
        public string? FirstName { get; set; }
        public string? SecondName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool IsHasAuto { get; set; } = false;
    }
}
