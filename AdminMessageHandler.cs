using Newtonsoft.Json.Linq;
using System;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TruckBot.Data;
using TruckBot.Helper;
using User = TruckBot.Model.User.User;


internal class AdminMessageHandler : IMessageHandler
{
    #region Fields

    private readonly TelegramBotClient _bot;
    private readonly DatabaseService _databaseService;
    private Dictionary<long, User> _adminList;
    private Dictionary<long, User> _userList;

    #endregion


    #region ClassLifeCicles

    public AdminMessageHandler(TelegramBotClient bot, DatabaseService databaseService, Dictionary<long, User> adminList, Dictionary<long, User> userList)
    {
        _bot = bot;
        _databaseService = databaseService;
        _adminList = adminList;
        _userList = userList;
    }

    #endregion


    #region HandleUpdateAsync

    public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        switch (update.Type)
        {
            case UpdateType.Unknown:
                Console.WriteLine("Пришли не известные данные от пользователя");
                break;
            case UpdateType.Message:
                if (update.Message is not { } message)
                    return;
                switch (message.Type)
                {
                    case MessageType.Unknown:
                        Console.WriteLine("Пришли не известные данные в сообщении от пользователя");
                        break;
                    case MessageType.Text:
                        if (message.Text is not { } text)
                            return;
                        if (text.StartsWith('/'))
                        {
                            await HandleCommandMsgAsync(message, cancellationToken);
                        }
                        else
                        {
                            await HandleTextMsgAsync(message, cancellationToken);
                        }
                        break;
                    case MessageType.Photo:
                        break;
                    case MessageType.Audio:
                        break;
                    case MessageType.Video:
                        break;
                    case MessageType.Voice:
                        break;
                    case MessageType.Document:
                        break;
                    case MessageType.Sticker:
                        break;
                    case MessageType.Location:
                        break;
                    case MessageType.Contact:
                        break;
                    case MessageType.Venue:
                        break;
                    case MessageType.Game:
                        break;
                    case MessageType.VideoNote:
                        break;
                    case MessageType.Invoice:
                        break;
                    case MessageType.SuccessfulPayment:
                        break;
                    case MessageType.WebsiteConnected:
                        break;
                    case MessageType.ChatMembersAdded:
                        break;
                    case MessageType.ChatMemberLeft:
                        break;
                    case MessageType.ChatTitleChanged:
                        break;
                    case MessageType.ChatPhotoChanged:
                        break;
                    case MessageType.MessagePinned:
                        break;
                    case MessageType.ChatPhotoDeleted:
                        break;
                    case MessageType.GroupCreated:
                        break;
                    case MessageType.SupergroupCreated:
                        break;
                    case MessageType.ChannelCreated:
                        break;
                    case MessageType.MigratedToSupergroup:
                        break;
                    case MessageType.MigratedFromGroup:
                        break;
                    case MessageType.Poll:
                        break;
                    case MessageType.Dice:
                        break;
                    case MessageType.MessageAutoDeleteTimerChanged:
                        break;
                    case MessageType.ProximityAlertTriggered:
                        break;
                    case MessageType.WebAppData:
                        if (message.WebAppData is not { } webAppData)
                            return;
                        await _bot.SendTextMessageAsync(message.Chat.Id, $"Данные получены! ❤ 👌 ✔", replyMarkup: new ReplyKeyboardRemove());
                        await Pause.Wait(500);
                        await ParseWebAppData(message, webAppData, cancellationToken);
                        break;
                    case MessageType.VideoChatScheduled:
                        break;
                    case MessageType.VideoChatStarted:
                        break;
                    case MessageType.VideoChatEnded:
                        break;
                    case MessageType.VideoChatParticipantsInvited:
                        break;
                    case MessageType.Animation:
                        break;
                    case MessageType.ForumTopicCreated:
                        break;
                    case MessageType.ForumTopicClosed:
                        break;
                    case MessageType.ForumTopicReopened:
                        break;
                    case MessageType.ForumTopicEdited:
                        break;
                    case MessageType.GeneralForumTopicHidden:
                        break;
                    case MessageType.GeneralForumTopicUnhidden:
                        break;
                    case MessageType.WriteAccessAllowed:
                        break;
                    case MessageType.UserShared:
                        break;
                    case MessageType.ChatShared:
                        break;
                }
                break;
            case UpdateType.InlineQuery:
                break;
            case UpdateType.ChosenInlineResult:
                break;
            case UpdateType.CallbackQuery:
                break;
            case UpdateType.EditedMessage:
                break;
            case UpdateType.ChannelPost:
                break;
            case UpdateType.EditedChannelPost:
                break;
            case UpdateType.ShippingQuery:
                break;
            case UpdateType.PreCheckoutQuery:
                break;
            case UpdateType.Poll:
                break;
            case UpdateType.PollAnswer:
                break;
            case UpdateType.MyChatMember:
                break;
            case UpdateType.ChatMember:
                break;
            case UpdateType.ChatJoinRequest:
                break;
        }
    }

    private async Task ParseWebAppData(Message message, WebAppData webAppData, CancellationToken token)
    {
        var parseArray = JArray.Parse(webAppData.Data);

        JObject callbackType = (JObject)parseArray[0];
        CallbackType type;
        try
        {
            type = callbackType.GetValue("callBackMethod").ToObject<CallbackType>();
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync($"Возникло исключение WebAppData при обработке callBackMethod:\n\n{ex}");
            return;
        }

        switch (type)
        {
            case CallbackType.UserRegister:
                await _bot.SendTextMessageAsync(message.Chat.Id, "Мы получили ваши данные. Обрабатываем");
                JObject userResponse = (JObject)parseArray[1];
                
                User newUser;
                try
                {
                    newUser = userResponse.ToObject<User>();
                }
                catch (Exception ex)
                {
                    await Console.Out.WriteLineAsync($"Возникло исключение WebAppData при обработке CallbackType.UserRegister:\n\n{ex}");
                    return;
                }
                Console.WriteLine(newUser.ToString());

                await _databaseService.AddUserAsync(newUser);

                if (newUser.IsAdmin)
                {
                    _adminList.Add(newUser.UserId, newUser);
                }
                else
                {
                    _userList.Add(newUser.UserId, newUser);
                }

                break;
        }
    }

    private async Task HandleCommandMsgAsync(Message message, CancellationToken token)
    {
        if (message.Text is not { } text) return;

        await Console.Out.WriteLineAsync($"{text}");

        var command = text.Split(' ')[0].ToLower();
        var args = text.Split(' ').Skip(1).ToArray();

        switch (command)
        {
            case "/start":
                await _bot.SendTextMessageAsync(message.Chat.Id, "Привет Админ! Ты управляешь этим чатом.");
                break;
            //case "/addadmin":
            //    await HandleAddAdminCommandAsync(message, args, token);
            //    break;
            case "/adduser":
                await HandleAddUserCommandAsync(message, args, token);
                break;

            case "/my_command":
                await _bot.SendTextMessageAsync(message.Chat.Id, "Жми /adduser чтобы зарегистрировать нового водителя.", cancellationToken: token);
                await _bot.SendTextMessageAsync(message.Chat.Id, "Жми /report чтобы сдать отчетность.", cancellationToken: token);

                break;
            case "/help":
                await _bot.SendTextMessageAsync(message.Chat.Id, "Что тебе надо старче?", cancellationToken: token);
                await Pause.Wait(1000);
                await _bot.SendTextMessageAsync(message.Chat.Id, $"Задавай свои вопросы админу.", cancellationToken: token);
                await _bot.SendTextMessageAsync(message.Chat.Id, $"Ах да, это же ты 😉😂", cancellationToken: token);

                break;
            case "/contacts":
                await _bot.SendTextMessageAsync(message.Chat.Id, DialogData.CONTACTS_MSG_DEFAULT, cancellationToken: token);
                break;

            default:
                await _bot.SendTextMessageAsync(message.Chat.Id, "Простите, но я не понимаю данной команды.", cancellationToken: token);
                break;
        }
    }

    private async Task HandleAddUserCommandAsync(Message message, string[] args, CancellationToken token)
    {
        Console.WriteLine("Надо научиться добавлять пользователей.");

        var webAppinfo = new WebAppInfo();
        webAppinfo.Url = @"https://jevlogin.github.io/TruckBot/www/addUser.html";

        var button = new KeyboardButton("👽 Зарегать пользователя");
        button.WebApp = webAppinfo;

        var replyKeyboard = new ReplyKeyboardMarkup(button) { ResizeKeyboard = true };

        await _bot.SendTextMessageAsync(message.Chat.Id, "Чтобы зарегать пользователя, жмякай на кнопку", replyMarkup: replyKeyboard);
    }

    #region remove_later
    private async Task HandleAddAdminCommandAsync(Message message, string[] args, CancellationToken token)
    {
        if (message.From?.Id is { } id)
        {
            if (IsCanHandle(id))
            {
                if (args.Length > 0)
                {
                    long userId = long.Parse(args[0]);

                    if (_adminList.TryGetValue(userId, out var admin))
                    {
                        var msgInfo = $"Такой администратор уже есть";
                        await Console.Out.WriteLineAsync(msgInfo);
                        await _bot.SendTextMessageAsync(message.Chat.Id, msgInfo, cancellationToken: token);
                        return;
                    }

                    _adminList[userId] = new User
                    {
                        UserId = userId,
                        IsAdmin = true,
                        FirstName = $"Admin_{userId}",

                    };

                    await _databaseService.AddUserAsync(_adminList[userId]);
                }
                else
                {
                    await _bot.SendTextMessageAsync(message.Chat.Id, "Пожалуйста введите корректные данные", cancellationToken: token);
                }
            }
        }
    } 
    #endregion

    private async Task HandleTextMsgAsync(Message message, CancellationToken cancellationToken)
    {
        switch (message.Type)
        {
            case MessageType.Text:
                if (message.ReplyToMessage is { } replyToMessage)
                {
                    if (replyToMessage.ForwardFrom is { } forwardFrom && forwardFrom.Id != _bot.BotId)
                    {
                        var userId = forwardFrom.Id;
                        var userName = forwardFrom.FirstName;

                        if (message.Text is { } text)
                        {
                            var msgText = $"<b>{message.From} {message.From.FirstName}</b>: Уважаемый {userName}, <i>{text}</i>";

                            try
                            {
                                await _bot.SendTextMessageAsync(userId, msgText,
                                                                        parseMode: ParseMode.Html, replyToMessageId: replyToMessage.MessageId,
                                                                        cancellationToken: cancellationToken);
                            }
                            catch (ApiRequestException ex)
                            {
                                await Console.Out.WriteLineAsync($"По не известной причине, блок метода - 'HandleTextAsync', не отработал.\n{ex.Message}");

                                await _bot.SendTextMessageAsync(userId, msgText, parseMode: ParseMode.Html,
                                                                        cancellationToken: cancellationToken);
                            }
                        }
                    }
                }

                await _bot.SendTextMessageAsync(message.Chat.Id, $"Вы отправили - {message.Text}");
                break;
        }
    }

    #endregion


    #region IsCanHandle

    public bool IsCanHandle(long userId)
    {
        return _adminList.ContainsKey(userId);
    }

    #endregion


    #region HandlePollingErrorAsync

    public async Task HandlePollingErrorAsync(Exception exception, CancellationToken cancellationToken)
    {
        await Console.Out.WriteLineAsync($"An error occurred during handling user message: {exception}");

        if (exception is ApiRequestException apiException)
        {
            await Console.Out.WriteLineAsync($"API error occurred: {apiException.ErrorCode} - {apiException.Message}");
        }
        else
        {
            await Console.Out.WriteLineAsync("An unknown error occurred.");
        }
        await Task.CompletedTask;
    }

    #endregion
}