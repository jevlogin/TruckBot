namespace TruckBot.Data
{
    internal class AppConfig
    {
        public string? BotKeyRelease { get; set; }
        public string? BotKeyDebug { get; set; }
        public long FirstAdminId { get; set; }
        public Dictionary<string, string>? ConnectionStrings { get; set; }
    }
}
