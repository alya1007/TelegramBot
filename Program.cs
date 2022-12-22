using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using System;
using static System.Console;
using System.Data.SQLite;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.InputFiles;

namespace telegramBot
{
    class Program
    {
        const string Token = "5730985318:AAESAViYQ4Xfv6zvv-Bxx76jtsA1sIiujgU";

        public static SQLiteConnection? DB;
        static void Main(string[] args)
        {

            while(true)
            {
                try
                {
                    GetUpdates().Wait();
                }
                catch(Exception ex)
                {
                    WriteLine($"1. Error: { ex }");
                    System.Environment.Exit(0);
                }
            }
            
        }

        static async Task GetUpdates(){
            //bot authorization
            TelegramBotClient bot = new TelegramBotClient(Token);
            int offset = 0;
            int timeout = 0;
            try{
                //disconnect webhook
                await bot.SetWebhookAsync("");
                while(true)
                {
                    //getting updates
                    var updates = await bot.GetUpdatesAsync(offset, timeout);

                    //for each message in update
                    foreach(var update in updates)
                    {
                        var message = update.Message;
                        if(message != null) {
                            switch(message.Type){
                                //if text message
                                case MessageType.Text:
                                    WriteLine($"{message.Chat.Username ?? "anon" } sent message { message?.Text }");
                                    if(message!.Text == "/start")
                                    {
                                        var keyboard = new ReplyKeyboardMarkup(new[]
                                        {
                                            new[]
                                            {
                                                new KeyboardButton("Register"),
                                                new KeyboardButton("Give sticker")
                                            },
                                            new[]
                                            {
                                                new KeyboardButton("Give image"),
                                            }
                                            
                                        });
                                        await bot.SendTextMessageAsync(message.Chat.Id, $"Hello, { message.Chat.FirstName }. I'm a bot.", ParseMode.Html, null, null, null, null, null, null, keyboard);                                    }
                                    if(message.Text == "Register")
                                    {
                                        Registration(message.Chat.Id.ToString(), message.Chat.Username?.ToString() ?? "anon");
                                        await bot.SendTextMessageAsync(message.Chat.Id, "User registered");
                                    }
                                    if(message.Text == "Give sticker")
                                    {
                                        var sticker = new InputOnlineFile("CAACAgIAAxkBAAEGKkJjU6aGmiDUEMXgfQP1JA2dAbRMfgAC8AYAAipVGAJ1Vka51YfsmSoE");
                                        await bot.SendStickerAsync(message.Chat.Id, sticker);
                                    }
                                    if(message.Text == "Give image")
                                    {
                                        FileStream fs = File.OpenRead(@"images/cat.jpg");
                                        InputOnlineFile myPhoto = new InputOnlineFile(fs, "myPhoto.jpg");
                                        await bot.SendPhotoAsync(message.Chat.Id, myPhoto, "Mad cat");
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        offset = update.Id + 1;
                    }
                }
            }
            catch(Exception ex)
            {
                WriteLine($"2. Error: { ex }");
                System.Environment.Exit(0);
            }
        }

        //method to registrate users
        public static void Registration(string chatId, string? username){
            try
            {
                DB = new SQLiteConnection("Data Source=data.db;");
                DB.Open();
                SQLiteCommand regcmd = DB.CreateCommand();
                //add data into db
                regcmd.CommandText = ("INSERT INTO users VALUES(@chatId, @name)");
                regcmd.Parameters.AddWithValue("@chatId", chatId);
                regcmd.Parameters.AddWithValue("@name", username);
                regcmd.ExecuteNonQuery();
                DB.Close();
            }
            catch(Exception ex){
                WriteLine($"3. Error: { ex }");
                System.Environment.Exit(0);
            }
        }
    }
}