using Telegram.Bot;
using TruckBot.Data;
using Telegram.Bot.Polling;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var databaseService = services.GetRequiredService<DatabaseService>();

            await databaseService.MigrateAsync();

            var adminList = await databaseService.LoadAdminListAsync();
            var userList = await databaseService.LoadUserListAsync();

            var updateDispatcher = new UpdateDispatcher();

            var bot = services.GetRequiredService<TelegramBotClient>();
            var me = await bot.GetMeAsync();

            ReceiverOptions receiverOptions = new ReceiverOptions();
            using var cts = new CancellationTokenSource();

            bot.StartReceiving(
                updateHandler: updateDispatcher,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );


            IMessageHandler userMessageHandler = new UserMessageHandler(bot, databaseService, adminList, userList);
            IMessageHandler adminMessageHandler = new AdminMessageHandler(bot, databaseService, adminList, userList);

            updateDispatcher.AddHandler(userMessageHandler);
            updateDispatcher.AddHandler(adminMessageHandler);

            await Console.Out.WriteLineAsync($"Начало работы бота {me.Username}");
            await Console.In.ReadLineAsync();

        }
        await host.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                var configuration = hostContext.Configuration;
                var appConfig = new AppConfig();
                configuration.Bind(appConfig);
                services.AddSingleton(appConfig);
                

                if (appConfig?.ConnectionStrings == null)
                {
                    throw new InvalidOperationException("ConnectionStrings is not configured.");
                }

                var connectionDefault = appConfig.ConnectionStrings["ConnectionDefault"];

                if (string.IsNullOrEmpty(connectionDefault))
                {
                    throw new InvalidOperationException("ConnectionDefault is not set.");

                }
                Console.WriteLine(appConfig);

                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlite(connectionDefault));

                services.AddTransient((provider) =>
                {
                    var appConfig = provider.GetRequiredService<AppConfig>();
                    var dbContext = provider.GetRequiredService<ApplicationDbContext>();

                    return new DatabaseService(appConfig, dbContext);
                });

                services.AddSingleton<TelegramBotClient>((provider) =>
                {
                    var appConfig = provider.GetRequiredService<AppConfig>();
                    return new TelegramBotClient(appConfig.BotKeyRelease);
                });
            });
    }
}