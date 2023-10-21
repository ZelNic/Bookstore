using Minotaur.DataAccess.Repository.IRepository;
using Minotaur.Models;
using Minotaur.Models.Models;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Minotaur.TelegramBot
{
    public class TelegramBot
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IUnitOfWork _unitOfWork;

        public TelegramBot(ITelegramBotClient telegramBotClient, IUnitOfWork unitOfWork)
        {
            _botClient = telegramBotClient;
            _unitOfWork = unitOfWork;
        }

        private static void Main() { }

        public async Task StartReceiving<TUpdateHandler>(ReceiverOptions? options, CancellationToken cancellationToken) where TUpdateHandler : Telegram.Bot.Polling.IUpdateHandler
        {
            _botClient.StartReceiving(Update, Error);
        }

        private Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            return Task.CompletedTask;
        }

        private Task Update(ITelegramBotClient client, Update update, CancellationToken token)
        {
            Bot_OnMessage(update.Message);
            return Task.CompletedTask;
        }


        private async Task Bot_OnMessage(Message message)
        {
            if (message.Text == null) { return; }

            var isContin = NeedCheckForAction(message);
            if (isContin == false) { return; }


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
                    await _botClient.SendTextMessageAsync(message.Chat.Id, "Введите номер заказа:");
                    NeedCheckForAction(message);

                    await _unitOfWork.TelegramRequestsRepository.AddAsync(new()
                    {
                        ChatId = message.Chat.Id,
                        Operation = message.Text
                    });
                    break;

                default: await _botClient.SendTextMessageAsync(message.Chat.Id, "Извините, команда не распознана! Для вызова оператора отправьте /call"); break;
            }
        }


        private bool NeedCheckForAction(Message message)
        {
            var product = _unitOfWork.Products.GetAll(u => u.ProductId == 1);
            RequestTelegram? request = _unitOfWork.TelegramRequestsRepository.GetAll(r => r.ChatId == message.Chat.Id).FirstOrDefault();
            if (request != null)
            {
                switch (request.Operation)
                {
                    case "/orderStatus":
                        HandleOrderStatusInput(message, request);
                        return false;
                    default: break;
                }
            }
            return true;
        }



        private async Task HandleOrderStatusInput(Message message, RequestTelegram request)
        {
            Order? statusOrder = await _unitOfWork.Orders.GetAsync(o => o.OrderId == Guid.Parse(message.Text));
            await _botClient.SendTextMessageAsync(message.Chat.Id, $"Статус заказа {statusOrder.OrderStatus}.");
            _unitOfWork.TelegramRequestsRepository.Remove(request);
        }

    }
}
