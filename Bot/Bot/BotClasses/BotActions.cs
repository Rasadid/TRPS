using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace Bot
{
    internal class BotActions
    {

        public static Book? _lastBook = default!;

        public static async void StartBot(ITelegramBotClient botClient, Update update)
        {
            bool _reply = BotMessages.AddNewUser((int)update.Message.Chat.Id);
            if (_reply)
            {
                try
                {
                    await botClient.SendTextMessageAsync(update.Message.Chat, "Привет!\nЭто сверхумный бот для подбора крутых книг на основе ваших предпочтений.\nИщите новые книги, ставьте оценки и подбирайте ещё более подходящие книги!");
                }
                catch
                {

                }
                var keyboard = new ReplyKeyboardMarkup(new[]
                {
                        new[]
                        {
                            new KeyboardButton("Дай новую книгу")
                        }
                    })
                {
                    ResizeKeyboard = true,
                    OneTimeKeyboard = false,
                };
                try
                {
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Предложить вам новую книгу?", replyMarkup: keyboard);
                }
                catch
                {

                }
                return;
            }
        }

        public static async void NewBook(ITelegramBotClient botClient, Update update)
        {
            MessageForBot _reply = BotMessages.GetBook((int)update.Message.Chat.Id);
            _lastBook = _reply.Book;
            try
            {
                await botClient.SendPhotoAsync(update.Message.Chat, _reply.Image, _reply.Message);
            }
            catch
            {

            }

            KeyboardButton[] row1 = new KeyboardButton[]
{
        new KeyboardButton("Дай новую книгу")
};

            KeyboardButton[] row2 = new KeyboardButton[]
{
         new KeyboardButton("1"),
                                new KeyboardButton("2"),
                                new KeyboardButton("3"),
                                new KeyboardButton("4"),
                                new KeyboardButton("5")
};

            KeyboardButton[][] keyboard = new KeyboardButton[][]
{
    row1,
    row2
};

            ReplyKeyboardMarkup replyKeyboard = new ReplyKeyboardMarkup(keyboard)
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = false,
            };
            try
            {
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Поставьте оценку для более подходящих рекомендаций", replyMarkup: replyKeyboard);
            }
            catch
            {

            }
        }


        public static async void Evaluation(ITelegramBotClient botClient, Update update)
        {
            BotMessages.SetEvaluation((int)update.Message.Chat.Id, int.Parse(update.Message.Text.ToLower()), _lastBook);
            var keyboard = new ReplyKeyboardMarkup(new[]
                    {
                        new[]
                        {
                            new KeyboardButton("Дай новую книгу")
                        }
                    })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = false,
            };
            try
            {
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Начни получать новые рекомендации!", replyMarkup: keyboard);
            }
            catch
            {

            }
        }
    }
}
