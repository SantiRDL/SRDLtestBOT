// See https://aka.ms/new-console-template for more information
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

string token = "6114703153:AAGLlzVqgCjrpBnTgx2xMWASlkay0V3E8Ao";
var botClient = new TelegramBotClient(token);

using CancellationTokenSource cts = new ();

    // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
 ReceiverOptions receiverOptions = new ()
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

// Send cancellation request to stop bot
cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Message is not { } message)
        return;
    // Only process text messages
    if (message.Text is not { } messageText)
        return;

    var chatId = message.Chat.Id;

    Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

    if (messageText== "hola")
    {
        Message specialMessage = await botClient.SendTextMessageAsync(
        chatId: chatId,
        text: "Hello world!",
        cancellationToken: cancellationToken);
        return;
    }
    
    if (messageText == "poll")
    {
    Message pollMessage = await botClient.SendPollAsync(
    chatId: chatId,
    question: "Did you ever hear the tragedy of Darth Plagueis The Wise?",
    options: new[]
    {
        "Yes for the hundredth time!",
        "No, who`s that?",
        "It is not a story they would tell you..."
    },
    cancellationToken: cancellationToken);
    return;
    }
    


    // Echo received message text
    Message sentMessage = await botClient.SendTextMessageAsync(
        chatId: chatId,
        text: "You said:\n" + messageText,
        cancellationToken: cancellationToken);
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}
