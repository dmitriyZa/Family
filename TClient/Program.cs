using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

class Program
{
    private static ITelegramBotClient? _botClient;

    static async Task Main()
    {
        var token = "8273248222:AAFvNsMk8ZORnv1Jdbs1r20WgJDPEK3vT9U"; // Ваш токен
        _botClient = new TelegramBotClient(token);

        using var cts = new CancellationTokenSource();

        // Начинаем получение обновлений
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { } // Получаем все виды обновлений
        };

        _botClient.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken: cts.Token
        );

        var me = await _botClient.GetMe();
        Console.WriteLine($"@{me.Username} запущен... Нажмите Enter для завершения");
        Console.ReadLine();
        cts.Cancel(); // останавливаем бота
    }

    // Обработка входящих сообщений
    private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message && update.Message.Text != null)
        {
            var message = update.Message;
            switch (message.Text.Trim().ToLowerInvariant())
            {
                case "/start":
                    var startReply = "Добро пожаловать! Выберите действие:";
                    var keyboard = new ReplyKeyboardMarkup(new[]
                    {
                    new KeyboardButton[] { new KeyboardButton("Добавить члена семьи👨‍👩‍👧‍👦"), new KeyboardButton("Информация о члене семьиℹ️") },
                    new KeyboardButton[] { new KeyboardButton("Отношения в семье💬") },
                })
                    {
                        ResizeKeyboard = true
                    };
                    await botClient.SendMessage(message.Chat.Id, startReply, replyMarkup: keyboard, cancellationToken: cancellationToken);
                    break;
                case "добавить члена семьи👨‍👩‍👧‍👦":
                    Console.WriteLine("Выбрана первая кнопка");
                    await botClient.SendMessage(message.Chat.Id, $"Выбрана кнопка: {message.Text}", cancellationToken: cancellationToken);
                    break;
                case "информация о члене семьиℹ️":
                    Console.WriteLine("Выбрана первая кнопка");
                    await botClient.SendMessage(message.Chat.Id, $"Выбрана кнопка: {message.Text}", cancellationToken: cancellationToken);
                    break;
                case "отношения в семье💬":
                    Console.WriteLine("Выбрана первая кнопка");
                    await botClient.SendMessage(message.Chat.Id, $"Выбрана кнопка: {message.Text}", cancellationToken: cancellationToken);
                    break;
                default:
                    await botClient.SendMessage(message.Chat.Id, "Команда не распознана. Используйте /start, чтобы начать.", cancellationToken: cancellationToken);
                    break;
            }
        }
    }

    // Обработка ошибок
    private static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        // Позволим отложить выполнение, чтобы избежать частых ошибок
        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
    }
}
