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
    private static readonly RelationshipDialogManager relationshipDialogManager = new();
    private static readonly Dictionary<long, int> lastAddedMemberIds = new(); // chatId -> 
    static readonly string[] relationTypes = new[]
{
    "Отец", "Мать", "Брат", "Сестра", "Жена", "Муж", "Дочь", "Сын", "Внук", "Внучка"
};


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

            // 1. Обычный диалог добавления FamilyMember
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
                    return;
                case "добавить члена семьи👨‍👩‍👧‍👦":
                    var reply = dialogManager.StartDialog(chatId);
                    await botClient.SendMessage(chatId, reply, cancellationToken: cancellationToken);
                    return;
            }

            // 2. Финализация FamilyMember и предложение добавить связь
            var (nextPrompt, completedMember) = dialogManager.ProcessInput(chatId, text);
            if (completedMember != null)
            {
                try
                {
                    var jsonContent = JsonConvert.SerializeObject(completedMember);
                    var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync("http://localhost:5274/api/family", content, cancellationToken);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        var memberFromServer = JsonConvert.DeserializeObject<FamilyMember>(responseBody);
                        int newId = memberFromServer?.Id ?? 0;
                        lastAddedMemberIds[chatId] = newId;

                        var inlineKeyboard = new InlineKeyboardMarkup(new[]
                        {
                        new[] {
                            InlineKeyboardButton.WithCallbackData("Да ✅", "add_relationship_yes"),
                            InlineKeyboardButton.WithCallbackData("Нет ❌", "add_relationship_no")
                        }
                    });

                        await botClient.SendMessage(chatId, "Член семьи добавлен! Хотите добавить связь с родственником?", replyMarkup: inlineKeyboard, cancellationToken: cancellationToken);
                    }
                    else
                    {
                        var serverError = await response.Content.ReadAsStringAsync();
                        await botClient.SendMessage(chatId, $"Ошибка при добавлении: {serverError}", cancellationToken: cancellationToken);
                    }
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
            // 3. Вблизи RelationshipDialogManager после выбора родственника и описания связи
            else if (relationshipDialogManager.IsActive(chatId))
            {
                var (relPrompt, completedRel) = relationshipDialogManager.ProcessInput(chatId, text);
                if (completedRel != null)
                {
                    var relJson = JsonConvert.SerializeObject(completedRel);
                    var relContent = new StringContent(relJson, System.Text.Encoding.UTF8, "application/json");
                    var relResponse = await httpClient.PostAsync("http://localhost:5274/api/relationship", relContent, cancellationToken);

                    var replyText = relResponse.IsSuccessStatusCode
                        ? "Связь успешно добавлена!"
                        : $"Ошибка создания связи: {await relResponse.Content.ReadAsStringAsync()}";
                    await botClient.SendMessage(chatId, replyText, cancellationToken: cancellationToken);
                }
                else if (!string.IsNullOrEmpty(relPrompt))
                {
                    await botClient.SendMessage(chatId, relPrompt, cancellationToken: cancellationToken);
                }
            }
            else
            {
                await botClient.SendMessage(chatId, "Команда не распознана. Используйте /start, чтобы начать.", cancellationToken: cancellationToken);
            }
        }
        else if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery != null)
        {
            var callback = update.CallbackQuery;
            var chatId = callback.Message.Chat.Id;
            if (callback.Data == "add_relationship_yes")
            {
                await botClient.AnswerCallbackQuery(callback.Id, cancellationToken: cancellationToken);

                // Получение списка родственников и отображение inline-кнопок
                var response = await httpClient.GetAsync("http://localhost:5274/api/family/all", cancellationToken);
                var familyList = JsonConvert.DeserializeObject<List<FamilyMember>>(await response.Content.ReadAsStringAsync());

                var buttons = new List<InlineKeyboardButton[]>();
                foreach (var member in familyList)
                {
                    if (lastAddedMemberIds.TryGetValue(chatId, out var newId) && member.Id == newId)
                        continue; // не показываем только что созданного

                    buttons.Add(new[]
                    {
                    InlineKeyboardButton.WithCallbackData($"{member.FirstName} {member.LastName} {member.ParentName}", $"choose_relative_{member.Id}")
                });
                }
                var relativesKeyboard = new InlineKeyboardMarkup(buttons);

                await botClient.SendMessage(chatId, "Выберите родственника из списка:", replyMarkup: relativesKeyboard, cancellationToken: cancellationToken);
            }
            else if (callback.Data == "add_relationship_no")
            {
                await botClient.AnswerCallbackQuery(callback.Id, cancellationToken: cancellationToken);
                await botClient.SendMessage(chatId, "Ок! Если захотите добавить связь — используйте меню.", cancellationToken: cancellationToken);
            }
            else if (callback.Data.StartsWith("choose_relative_"))
            {
                await botClient.AnswerCallbackQuery(callback.Id, cancellationToken: cancellationToken);
                int relatedMemberId = int.Parse(callback.Data.Replace("choose_relative_", ""));
                if (lastAddedMemberIds.TryGetValue(chatId, out int familyMemberId))
                {
                    relationshipDialogManager.Start(chatId, familyMemberId);
                    relationshipDialogManager.SetRelatedMember(chatId, relatedMemberId);

                    // Формируем сетку для выбора типа связи
                    var buttons = new List<List<InlineKeyboardButton>>();
                    int rowLen = 3;
                    for (int i = 0; i < relationTypes.Length; i += rowLen)
                    {
                        var row = new List<InlineKeyboardButton>();
                        for (int j = i; j < i + rowLen && j < relationTypes.Length; j++)
                        {
                            var type = relationTypes[j];
                            row.Add(InlineKeyboardButton.WithCallbackData(type, $"relation_type_{type}"));
                        }
                        buttons.Add(row);
                    }

                    var relationTypesKeyboard = new InlineKeyboardMarkup(buttons);

                    await botClient.SendMessage(
                        chatId,
                        "Выберите тип связи:",
                        replyMarkup: relationTypesKeyboard,
                        cancellationToken: cancellationToken
                    );
                }
            }
            else if (callback.Data.StartsWith("relation_type_"))
            {
                await botClient.AnswerCallbackQuery(callback.Id, cancellationToken: cancellationToken);
                string relationType = callback.Data.Replace("relation_type_", "");

                if (relationshipDialogManager.IsActive(chatId))
                {
                    // Передай relationType в менеджер, чтобы он мог "финализировать" диалог
                    var (relPrompt, completedRel) = relationshipDialogManager.ProcessInput(chatId, relationType, isRelationType: true);
                    if (completedRel != null)
                    {
                        var relJson = JsonConvert.SerializeObject(completedRel);
                        var relContent = new StringContent(relJson, System.Text.Encoding.UTF8, "application/json");
                        var relResponse = await httpClient.PostAsync("http://localhost:5274/api/relationship", relContent, cancellationToken);

                        var replyText = relResponse.IsSuccessStatusCode
                            ? "Связь успешно добавлена!"
                            : $"Ошибка создания связи: {await relResponse.Content.ReadAsStringAsync()}";
                        await botClient.SendMessage(chatId, replyText, cancellationToken: cancellationToken);
                    }
                    else if (!string.IsNullOrEmpty(relPrompt))
                    {
                        await botClient.SendMessage(chatId, relPrompt, cancellationToken: cancellationToken);
                    }
                }
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
