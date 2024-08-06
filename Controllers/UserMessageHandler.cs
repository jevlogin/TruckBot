using System;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TruckBot.Data;
using TruckBot.Helper;
using TruckBot.Model.User;
using User = TruckBot.Model.User.User;


internal class UserMessageHandler : IMessageHandler
{
    #region Fields

    private readonly TelegramBotClient _bot;
    private DatabaseService _databaseService;
    private readonly Dictionary<long, User> _adminList;
    private Dictionary<long, User> _userList;
    private Dictionary<string, int> _buttonMsgId = new();

    #endregion


    #region ClassLifeCicles

    public UserMessageHandler(TelegramBotClient bot, DatabaseService databaseService, Dictionary<long, User> adminList, Dictionary<long, User> userList)
    {
        _bot = bot;
        _databaseService = databaseService;
        _adminList = adminList;
        _userList = userList;
    }

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


    public bool IsCanHandle(long userId)
    {
        if (!_adminList.ContainsKey(userId))
        {
            return true;
        }
        return false;
    }

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
                if (update.CallbackQuery is not { } callbackQuery)
                    return;
                await HandleCallBackQuery(callbackQuery);
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

    private async Task HandleCallBackQuery(CallbackQuery callbackQuery)
    {
        if (callbackQuery.Data is not { } data) return;
        Console.WriteLine($"Наша кнопка {data}");

        switch (data)
        {
            case "accept_car":
                if (_buttonMsgId.TryGetValue(data, out var msgId))
                {
                    await _bot.DeleteMessageAsync(callbackQuery.Message?.Chat.Id, msgId);
                }
                await _bot.SendTextMessageAsync(callbackQuery.Message?.Chat.Id, "Введите номер авто в формате x999xx");
                break;
        }
    }

    private async Task HandleTextMsgAsync(Message message, CancellationToken canToken)
    {
        await Console.Out.WriteLineAsync($"{message.Text}");

        var userId = message.From.Id;
        var isRegUser = _userList.ContainsKey(userId);
        if (isRegUser)
        {
            await MsgHasReceived(message, canToken);
        }
        else
        {
            await SendMsgUnknowUser(message, userId, canToken);
        }

        List<string> listPhones = new();
        foreach (var admin in _adminList.Values)
        {
            if (admin.UserId == 0) continue;
            if (!isRegUser)
                await _bot.SendTextMessageAsync(admin.UserId, $"{string.Format(DialogData.USER_HAS_ID, userId)} {DialogData.USER_HAS_NOT_REGISTER} ", cancellationToken: canToken);
            else
                await _bot.SendTextMessageAsync(admin.UserId, $"{string.Format(DialogData.USER_HAS_ID, userId)} {DialogData.USER_HAS_REGISTER} ", cancellationToken: canToken);
            await _bot.ForwardMessageAsync(admin.UserId, message.Chat.Id, message.MessageId, cancellationToken: canToken);

            if (admin.Phone is not { } phone) continue;
            listPhones.Add(phone);
        }

        if (isRegUser)
        {
            var currentDriver = _userList[userId];


            if (currentDriver.IsHasAuto)
            {
                await _bot.SendTextMessageAsync(userId, "Тут будет 2 кнопки, Принять АВТО или Сдать Авто", cancellationToken: canToken);
            }
            else
            {
                var acceptButton = new InlineKeyboardButton("Принять авто")
                {
                    CallbackData = "accept_car"
                };

                var keyboardMarkup = new InlineKeyboardMarkup(acceptButton);

                var msg = await _bot.SendTextMessageAsync(userId,
                    string.Format(DialogData.WELCOME_MESSAGE_DEFAULT, currentDriver.FirstName, currentDriver.SecondName, currentDriver.LastName, listPhones.FirstOrDefault()),
                    replyMarkup: keyboardMarkup,
                   cancellationToken: canToken);

                if (_buttonMsgId.ContainsKey("accept_car"))
                {
                    _buttonMsgId.Remove("accept_car");
                }
                _buttonMsgId.Add("accept_car", msg.MessageId);
            }
        }
    }

    private async Task SendMsgUnknowUser(Message message, long userId, CancellationToken canToken)
    {
        await _bot.SendTextMessageAsync(message.Chat.Id, string.Format(DialogData.WELCOME_MESSAGE_TEMPLATE, message.From.Username, userId));
        await MsgHasReceived(message, canToken);
    }

    private async Task MsgHasReceived(Message message, CancellationToken canToken)
    {
        await _bot.SendTextMessageAsync(message.Chat.Id, DialogData.YOUR_MESSAGE_HAS_BEEN_RECEIVED, cancellationToken: canToken);
    }

    private async Task HandleCommandMsgAsync(Message message, CancellationToken token)
    {
        if (message.Text is { } text)
        {
            await Console.Out.WriteLineAsync($"{text}");
            var commands = text.ToLower().Split(' ');
            var command1 = commands[0];

            switch (command1)
            {
                case "/start":
                    if (_userList.ContainsKey(message.From.Id))
                    {
                        await _bot.SendTextMessageAsync(message.Chat.Id, "Не кипишуй, бронируй авто. Вызывай в меню пункт /my_command и да будет тебе счастье.", cancellationToken: token);
                    }
                    else
                    {
                        await SendMsgUnknowUser(message, message.Chat.Id, token);

                        //await _bot.SendTextMessageAsync(message.Chat.Id, "Хули тыкаешь?! Жди пока зарегают", cancellationToken: token);
                    }
                    break;

                case "/my_command":

                    await _bot.SendTextMessageAsync(message.Chat.Id, "Жми /book чтобы забронировать авто, сдать авто, или посмотреть какое авто у тебя не сдано.", cancellationToken: token);
                    await _bot.SendTextMessageAsync(message.Chat.Id, "Жми /report чтобы сдать отчетность.", cancellationToken: token);

                    Console.WriteLine("Тут надо нарисовать кнопочки бронирования авто.");

                    break;

                case "/help":
                    await _bot.SendTextMessageAsync(message.Chat.Id, "Что тебе надо старче?", cancellationToken: token);
                    await Pause.Wait(1000);
                    await _bot.SendTextMessageAsync(message.Chat.Id, $"Задавай свои вопросы админу. {DialogData.HELP_MSG_DEFAULT}", cancellationToken: token);
                    var admin = _adminList.Values.FirstOrDefault();

                    if (admin != null && !string.IsNullOrEmpty(admin.Phone))
                    {
                        await _bot.SendTextMessageAsync(message.Chat.Id, $"Вот тебе телефон менеджера - {admin?.Phone}", cancellationToken: token);
                    }


                    break;

                case "/contacts":
                    await _bot.SendTextMessageAsync(message.Chat.Id, DialogData.CONTACTS_MSG_DEFAULT, cancellationToken: token);

                    break;

            }
        }

        #endregion



    }
}