using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
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
    private static readonly HttpClient httpClient = new();
    private static readonly Dictionary<long, FamilyMember> userInputs = new();
    private static readonly FamilyMemberDialogManager dialogManager = new();
    static async Task Main()
    {
        var token = "8273248222:AAFvNsMk8ZORnv1Jdbs1r20WgJDPEK3vT9U";
        _botClient = new TelegramBotClient(token);

        using var cts = new CancellationTokenSource();

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { }
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
        cts.Cancel();
    }

    private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message && update.Message.Text != null)
        {
            var chatId = update.Message.Chat.Id;
            var text = update.Message.Text.Trim();

            switch (text.ToLowerInvariant())
            {
                case "/start":
                    var keyboard = new ReplyKeyboardMarkup(new[]
                    {
                    new KeyboardButton[] { new KeyboardButton("Добавить члена семьи👨‍👩‍👧‍👦"), new KeyboardButton("Информация о члене семьиℹ️") },
                    new KeyboardButton[] { new KeyboardButton("Отношения в семье💬") }
                })
                    { ResizeKeyboard = true };
                    await botClient.SendMessage(chatId, "Добро пожаловать! Выберите действие:", replyMarkup: keyboard, cancellationToken: cancellationToken);
                    break;
                case "добавить члена семьи👨‍👩‍👧‍👦":
                    var reply = dialogManager.StartDialog(chatId);
                    await botClient.SendMessage(chatId, reply, cancellationToken: cancellationToken);
                    break;
                default:
                    var (nextPrompt, completedMember) = dialogManager.ProcessInput(chatId, text);
                    if (completedMember != null)
                    {
                        try
                        {
                            var jsonContent = JsonConvert.SerializeObject(completedMember);
                            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                            var response = await httpClient.PostAsync("http://localhost:5274/api/family", content, cancellationToken);

                            string serverError = string.Empty;

                            if (!response.IsSuccessStatusCode)
                            {
                                // Прочитать тело ответа с ошибкой
                                serverError = await response.Content.ReadAsStringAsync();
                            }
                            string result = response.IsSuccessStatusCode
                                ? "Член семьи добавлен успешно!"
                                : $"Ошибка при добавлении члена семьи.{serverError}";
                            await botClient.SendMessage(chatId, result, cancellationToken: cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            await botClient.SendMessage(chatId, $"Ошибка соединения: {ex.Message}", cancellationToken: cancellationToken);
                        }
                    }
                    else if (!string.IsNullOrEmpty(nextPrompt))
                    {
                        await botClient.SendMessage(chatId, nextPrompt, cancellationToken: cancellationToken);
                    }
                    else
                    {
                        await botClient.SendMessage(chatId, "Команда не распознана. Используйте /start, чтобы начать.", cancellationToken: cancellationToken);
                    }
                    break;
            }
        }
    }
    private static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
    }
}
