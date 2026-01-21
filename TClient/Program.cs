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
    private static ITelegramBotClient _botClient;

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
            switch (message.Text.Split(' ')[0])
            {
                case "/start":
                    var startReply = "Добро пожаловать! Выберите действие:";
                    var keyboard = new ReplyKeyboardMarkup(new[]
                    {
                        new KeyboardButton[] { "Добавить члена семьи👨‍👩‍👧‍👦", "Информация о члене семьиℹ️" },
                        new KeyboardButton[] { "Отношения в семье💬" },
                    })
                    {
                        ResizeKeyboard = true
                    };
                    await botClient.SendMessage(message.Chat, startReply, replyMarkup: keyboard);
                    break;
                // Обработчики для других команд
                default:
                    await botClient.SendMessage(message.Chat, "Команда не распознана. Используйте /start, чтобы начать.");
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
