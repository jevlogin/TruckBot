using Telegram.Bot.Types;

internal interface IMessageHandler
{
    public Task HandlePollingErrorAsync(Exception exception, CancellationToken cancellationToken);

    public Task HandleUpdateAsync(Update update, CancellationToken cancellationToken);

    public bool IsCanHandle(long userId);
}