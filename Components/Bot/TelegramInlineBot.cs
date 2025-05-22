using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using UrlSamurai.Data;

namespace UrlSamurai.Components.Bot;

public class TelegramInlineBot(string botToken, IServiceScopeFactory scopeFactory)
{
    private readonly TelegramBotClient _botClient = new(botToken);

    public void Start()
    {
        var cts = new CancellationTokenSource();

        _botClient.StartReceiving(
            HandleUpdate,
            HandleError,
            new ReceiverOptions { AllowedUpdates = Array.Empty<UpdateType>() }, // listen to all
            cancellationToken: cts.Token
        );

        _ = Task.Run(async () =>
        {
            var me = await _botClient.GetMe(cts.Token);
            Console.WriteLine($"Bot @{me.Username} is running... Press Enter to exit.");
        }, cts.Token);
    }

    private async Task HandleUpdate(ITelegramBotClient bot, Update update, CancellationToken token)
    {
        if (update.InlineQuery is { } inlineQuery)
        {
            await HandleUrl(bot, inlineQuery.Query?.Trim(), inlineQuery.Id, token, isInline: true, chatId: null);
            return;
        }

        if (update.Message is { Text: { } text, Chat: { } chat })
        {
            await HandleUrl(bot, text.Trim(), null, token, isInline: false, chatId: chat.Id);
        }
    }

    private async Task HandleUrl(
        ITelegramBotClient bot,
        string? inputUrl,
        string? inlineQueryId,
        CancellationToken token,
        bool isInline,
        long? chatId
    )
    {
        Console.WriteLine($"PASSED URL >>> {inputUrl}");
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        if (string.IsNullOrWhiteSpace(inputUrl) || !UrlValidator.IsValid(inputUrl))
        {
            var message = $"{inputUrl} is not a valid URL";

            if (isInline)
            {
                var errorResult = new InlineQueryResultArticle(
                    id: "invalid-url",
                    title: "Invalid URL",
                    inputMessageContent: new InputTextMessageContent(message))
                {
                    Description = "Please enter a valid absolute URL"
                };

                await bot.AnswerInlineQuery(
                    inlineQueryId!,
                    results: [errorResult],
                    isPersonal: true,
                    cacheTime: 0,
                    cancellationToken: token
                );
            }
            else if (chatId.HasValue)
            {
                await bot.SendMessage(chatId.Value, message, cancellationToken: token);
            }

            return;
        }

        var newUrl = new Data.Entities.Urls
        {
            UrlValue = inputUrl,
            CreatedAt = DateTime.UtcNow,
        };

        await db.Urls.AddAsync(newUrl, token);
        await db.SaveChangesAsync(token);

        var shortLink = $"https://www.sshare.dev/u/{newUrl.ShortId}";

        if (isInline)
        {
            var successResult = new InlineQueryResultArticle(
                id: newUrl.Id.ToString(),
                title: "Shortened URL",
                inputMessageContent: new InputTextMessageContent(shortLink))
            {
                Description = inputUrl
            };

            await bot.AnswerInlineQuery(
                inlineQueryId!,
                results: [successResult],
                isPersonal: true,
                cacheTime: 0,
                cancellationToken: token
            );
        }
        else if (chatId.HasValue)
        {
            await bot.SendMessage(chatId.Value, shortLink, cancellationToken: token);
        }
    }

    private Task HandleError(ITelegramBotClient bot, Exception exception, CancellationToken token)
    {
        Console.WriteLine($"[Telegram Error] {exception}");
        return Task.CompletedTask;
    }
    
    internal static class UrlValidator
    {
        private static readonly Regex UrlRegex = new(@"^https?:\/\/(?:www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b(?:[-a-zA-Z0-9()@:%_\+.~#?&\/=]*)$", RegexOptions.Compiled);

        public static bool IsValid(string? url)
        {
            return !string.IsNullOrWhiteSpace(url) && UrlRegex.IsMatch(url);
        }
    }
}
