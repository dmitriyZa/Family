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
                    await botClient.SendMessage(message.Chat.Id, "Введите имя:", cancellationToken: cancellationToken);
                    userInputs[message.Chat.Id] = new FamilyMember();
                    break;
                default:
                    if (userInputs.ContainsKey(message.Chat.Id))
                    {
                        var userFamilyMember = userInputs[message.Chat.Id];

                        if (string.IsNullOrEmpty(userFamilyMember.FirstName))
                        {
                            userFamilyMember.FirstName = message.Text;
                            await botClient.SendMessage(message.Chat.Id, "Введите фамилию:", cancellationToken: cancellationToken);
                        }
                        else if (string.IsNullOrEmpty(userFamilyMember.LastName))
                        {
                            userFamilyMember.LastName = message.Text;
                            await botClient.SendMessage(message.Chat.Id, "Введите дату рождения (ГГГГ-ММ-ДД):", cancellationToken: cancellationToken);
                        }
                        else if (userFamilyMember.DateOfBirth == DateTime.MinValue)
                        {
                            if (DateTime.TryParse(message.Text, out DateTime birthDate))
                            {
                                userFamilyMember.DateOfBirth = birthDate;
                                await botClient.SendMessage(message.Chat.Id, "Введите биографию:", cancellationToken: cancellationToken);
                            }
                            else
                            {
                                await botClient.SendMessage(message.Chat.Id, "Неверный формат даты. Попробуйте еще раз (ГГГГ-ММ-ДД):", cancellationToken: cancellationToken);
                            }
                        }
                        else if (string.IsNullOrEmpty(userFamilyMember.Biography))
                        {
                            userFamilyMember.Biography = message.Text;
                            await botClient.SendMessage(message.Chat.Id, "Введите степень родства:", cancellationToken: cancellationToken);
                        }
                        else if (string.IsNullOrEmpty(userFamilyMember.RelationshipDegree))
                        {
                            userFamilyMember.RelationshipDegree = message.Text;

                            try
                            {
                                // Отправка данных на сервер
                                var jsonContent = JsonConvert.SerializeObject(userFamilyMember);
                                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                                var response = await httpClient.PostAsync("http://localhost:5000/api/family", content, cancellationToken);

                                if (response.IsSuccessStatusCode)
                                {
                                    await botClient.SendMessage(message.Chat.Id, "Член семьи добавлен успешно!", cancellationToken: cancellationToken);
                                }
                                else
                                {
                                    await botClient.SendMessage(message.Chat.Id, "Ошибка при добавлении члена семьи.", cancellationToken: cancellationToken);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                await botClient.SendMessage(message.Chat.Id, $"Ошибка соединения: {ex.Message}", cancellationToken: cancellationToken);
                            }

                            userInputs.Remove(message.Chat.Id); // Удаляем временные данные
                        }
                    }
                    else
                    {
                        await botClient.SendMessage(message.Chat.Id, "Команда не распознана. Используйте /start, чтобы начать.", cancellationToken: cancellationToken);
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
