using MessageLogger.Data;
using MessageLogger.Model;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

namespace MessageLogger
{
    public class Prompt
    {
        public static void Output(string prompt)
        {
            switch (prompt)
            {
                case "welcome":
                    AnsiConsole.Write(
                        new FigletText("Message Logger")
                        .Centered()
                        .Color(Color.Yellow));
                    AnsiConsole.Progress()
                                        .Start(ctx =>
                                        {
                                            // Define tasks
                                            var task1 = ctx.AddTask("[green italic bold]Loading[/]");
                                            var task2 = ctx.AddTask("[green italic bold]Connecting to Database[/]");

                                            while (!ctx.IsFinished)
                                            {
                                                task1.Increment(1.5);
                                                Thread.Sleep(10);
                                                task2.Increment(0.5);
                                            }
                                        });
                    Console.Clear();
                    break;
                case "addMessage":
                    AnsiConsole.Markup("[bold invert grey]ADD A MESSAGE:[/] ");
                    break;
            }
        }
        public static void Outro(MessageLoggerContext context)
        {
            Console.WriteLine("Thanks for using Message Logger!");
            foreach (var u in context.Users.Include(u => u.Messages)) //in context instead
            {
                Console.WriteLine($"{u.Name} wrote {u.Messages.Count} messages.");
            }
        }
        public static void DisplayUsers(MessageLoggerContext context)
        {
            Console.WriteLine("Current Users");
            foreach (var u in context.Users) //in context instead
            {
                Console.WriteLine($"|{u.Id}| {u.Username}");
            }
        }
        public static void DisplayMessages(MessageLoggerContext context, User currentUser)
        {
            Console.Clear();
            var panel = new Panel($"[invert bold]MESSAGE LOGGER[/] [red]Logged in as:[/][yellow]{currentUser.Username}[/] [dim grey] Controls - Enter your message or type 'quit' to log out[/]");
            panel.Expand();
            AnsiConsole.Write(panel);
            var userAll = context.Users.Include(u => u.Messages).Single(u => u.Username == currentUser.Username);
            foreach (var message in userAll.Messages)
            {
                Console.WriteLine($"{message.CreatedAt:t}: {message.Content}");
            }
        }
        public static string UsersOrderedByMessageCount(MessageLoggerContext context)
        {
            var userAll = context.Users.Include(u => u.Messages).OrderByDescending(u => u.Messages.Count);
            string builtString = null;
            foreach (var u in userAll)
            {
                builtString += $"{u.Username}: {u.Messages.Count}\n";
            }
            return builtString;
        }
        public static string MostCommonWord(MessageLoggerContext context, int minUsageNum)
        {
            List<string> allMessageStrings = context.Messages.Select(m => m.Content).ToList();
            List<string> singleWordList = new List<string>();
            foreach (string messageString in allMessageStrings)
            {
                var splitString = messageString.Split(" ");
                singleWordList.AddRange(splitString);
            }
            string builtString = null;
            foreach (string word in singleWordList.Distinct())
            {
                if (singleWordList.Where(w => w == word).Count() < minUsageNum)
                {
                    continue;
                }
                else
                {
                    builtString += $"'{word}' occurs {singleWordList.Where(w => w == word).Count()} times.\n";
                }
            }
            return builtString;
        }

        //I am not a fan of everything beyond this point
        public static string HourOfMostMessages(MessageLoggerContext context)
        {
            //count messages that occur between range of time (1 hour)
            //move through each hour from first occurance to last occurence
            //for loop
            var lastEverMessageHour = context.Messages.Max(m => m.CreatedAt.Hour) + 1;
            string builtString = null;
            for (var firstEverMessageHour = context.Messages.Min(m => m.CreatedAt.Hour); firstEverMessageHour < lastEverMessageHour; firstEverMessageHour++)
            {
                //var firstHour
                var firstHour = firstEverMessageHour;
                //var NextHour
                var nextHour = firstEverMessageHour + 1;
                //var totalMessages = context.Messages.Where(m.Date => m.Date < nextHour && m.Date >= firstHour).Count();
                var totalMessages = context.Messages.Where(m => m.CreatedAt.Hour < nextHour && m.CreatedAt.Hour >= firstHour).Count();
                builtString += $"{TwelveHourTime(firstHour)}: {totalMessages}\n";
            }
            return builtString;
        }
        public static string TwelveHourTime(int hour)
        {
            if (hour < 12)
            {
                return $"{hour} AM";
            }
            else if (hour == 12)
            {
                return $"{hour} PM";
            }
            else if (hour == 0)
            {
                return $"{hour} AM";
            }
            else
            {
                hour = hour - 12;
                return $"{hour} PM";
            }
        }
    }
}
