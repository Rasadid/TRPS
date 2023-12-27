using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Bot
{
    internal class Program
    {
        private static ITelegramBotClient bot = new TelegramBotClient("5673829170:AAE5bOrjbkZtvo9rj2Ulf_WAeBsEX7r7_2c");

        private static bool _start = true;

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                Console.WriteLine(message.Chat.Id);
                if (message.Text.ToLower() == "/start" && _start)
                {
                    try
                    {
                        BotActions.StartBot(botClient, update);
                        _start = false;
                        return;
                    }
                    catch
                    {
                        return;
                    }
                }
                if (message.Text.ToLower() == "дай новую книгу")
                {
                    try
                    {
                        BotActions.NewBook(botClient, update);
                        return;
                    }
                    catch
                    {
                        return;
                    }
                }
                if ((message.Text.ToLower() == "1" || message.Text.ToLower() == "2" || message.Text.ToLower() == "3" || message.Text.ToLower() == "4" || message.Text.ToLower() == "5"))
                {
                    try
                    {
                        BotActions.Evaluation(botClient, update);
                        return;
                    }
                    catch
                    {
                        return;
                    }
                }
                try
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Я ещё не научился таким командам...");
                }
                catch
                {

                }
            }
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.ReadLine();
        }
    }
}