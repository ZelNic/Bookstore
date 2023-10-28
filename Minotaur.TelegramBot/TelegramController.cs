using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models.Models;
using Minotaur.Models.SD;
using Minotaur.Utility;
using System;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Minotaur.TelegramController
{
    public class TelegramController
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IUnitOfWork _unitOfWork;
        //private const string BOT_TOKEN = "6504892449:AAEDmHDwgkFG_Wg6Gywn-5ivRHcePsySn-4";

        public TelegramController(ITelegramBotClient botClient, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            _botClient = botClient;
        }

        private static void Main() { }


        public async Task StartReceiving<TUpdateHandler>(ReceiverOptions? options, CancellationToken cancellationToken) where TUpdateHandler : Telegram.Bot.Polling.IUpdateHandler
        {
            _botClient.StartReceiving(Update, Error);

            Notification notifAboutStartTelegramBot = new()
            {
                RecipientId = Guid.Parse("604c075d-c691-49d6-9d6f-877cfa866e59"),
                EmailSender = "TelegramBot",
                Text = $"Запуск TelegramBot",
                SendingTime = MoscowTime.GetTime(),
                TypeNotification = NotificationSD.SimpleNotification,
            };

            await _unitOfWork.Notifications.AddAsync(notifAboutStartTelegramBot);
            await _unitOfWork.Save();
        }

        private async Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            Notification notificationAboutError = new()
            {
                RecipientId = Guid.Parse("604c075d-c691-49d6-9d6f-877cfa866e59"),
                EmailSender = "TelegramBot",
                Text = $"{exception}.",
                SendingTime = MoscowTime.GetTime(),
                TypeNotification = NotificationSD.ErrorNotification,
            };
            await _unitOfWork.Notifications.AddAsync(notificationAboutError);
            await _unitOfWork.Save();
        }

        private async Task Update(ITelegramBotClient client, Update update, CancellationToken token)
        {
            await Bot_OnMessage(update.Message);
        }


        private async Task Bot_OnMessage(Message message)
        {
            RequestTelegram? request = await _unitOfWork.TelegramRequestsRepository.GetAsync(r => r.ChatId == message.Chat.Id, null, false);

            if (request != null)
            {
                _unitOfWork.TelegramRequestsRepository.StopTracking(request);

                switch (request.Operation)
                {
                    case "/orderstatus":
                        _unitOfWork.TelegramRequestsRepository.Remove(request);
                        await _unitOfWork.Save();
                        await HandleOrderStatusInput(message.Chat.Id, message.Text);
                        return;

                    default:
                        _unitOfWork.TelegramRequestsRepository.Remove(request);
                        await _unitOfWork.Save();
                        break;
                }
            }

            switch (message.Text)
            {
                case "/start":
                    var commands = "Здравствуйте! Добро пожаловать в Минотавр.\n" +
                   "Возможности бота:\n" +
                   "1. Позвать оператора: /call\n" +
                   "2. Узнать статус заказа: /orderstatus\n" +
                   "3. ...";

                    var replyMarkup = new ReplyKeyboardMarkup(new[]
                        {
                        new[]
                        {
                            new KeyboardButton("/call"),
                            new KeyboardButton("/orderstatus")
                        }
                    })
                    {
                        ResizeKeyboard = true
                    };

                    await _botClient.SendTextMessageAsync(message.Chat.Id, commands, replyMarkup: replyMarkup);
                    break;
                case "/call":
                    await _botClient.SendTextMessageAsync(message.Chat.Id, "Вызван оператор! Ожидайте.");
                    break;
                case "/orderstatus":

                    RequestTelegram newTask = new()
                    {
                        ChatId = message.Chat.Id,
                        Operation = message.Text
                    };

                    await _unitOfWork.TelegramRequestsRepository.AddAsync(newTask);                   
                    await _unitOfWork.Save();
                    _unitOfWork.TelegramRequestsRepository.StopTracking(newTask);

                    await _botClient.SendTextMessageAsync(message.Chat.Id, "Введите номер заказа:");
                    break;

                default: await _botClient.SendTextMessageAsync(message.Chat.Id, "Извините, команда не распознана! Для вызова оператора отправьте /call"); break;
            }
        }

        private async Task HandleOrderStatusInput(long chatId, string messageText)
        {
            Guid? guid = null;
            if (Guid.TryParse(messageText, out Guid result))
            {
                guid = result;
            }
            else
            {
                await _botClient.SendTextMessageAsync(chatId, $"Неверный код заказа.");
                return;
            }

            Order? order = await _unitOfWork.Orders.GetAsync(o => o.OrderId == guid);

            if (order != null)
            {
                await _botClient.SendTextMessageAsync(chatId, $"Статус заказа - {order.OrderStatus}.");
            }
            else
            {
                await _botClient.SendTextMessageAsync(chatId, $"Заказ не найден.");
            }
        }
    }
}


//private async Task<bool> NeedCheckForAction(Message message)
//{
//    RequestTelegram? request = await _unitOfWork.TelegramRequestsRepository.GetAsync(r => r.ChatId == message.Chat.Id);
//    if (request == null) { return true; }

//    switch (request.Operation)
//    {
//        case "/orderstatus":
//            _unitOfWork.TelegramRequestsRepository.Remove(request);
//            _unitOfWork.Save();
//            await HandleOrderStatusInput(message);
//            return false;
//        default:
//            _unitOfWork.TelegramRequestsRepository.Remove(request);
//            _unitOfWork.Save();
//            return true;
//    }

//}