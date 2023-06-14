using MessageLogger.Data;
using MessageLogger.Model;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

namespace MessageLogger
{
    public class Prompt
    {
        //Switch statement to output different prompts for user
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
                case "thankYou":
                    Console.Clear();
                    AnsiConsole.Write(
                        new FigletText("Thank you!")
                        .Centered()
                        .Color(Color.Red));
                    break;
            }
        }
        //Renders Main menu, returns menu selection
        public static string MainMenu()
        {
            var panel = new Panel("[invert bold]MESSAGE LOGGER[/] [red]Main Menu[/] [dim grey] Controls - Select an option from the menu with the arrow keys[/]");
            panel.Expand();
            AnsiConsole.Write(panel);
            string selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .PageSize(4)
                .AddChoices(new[]
                {
            "New User",
            "Existing User",
            "Statistics",
            "Quit"
                }));
            Console.Clear();
            return selection;
        }
        //Displays all previous user messages when logged into an existing user
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
        
        //Linq to get Users sorted by message count
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
        //Linq to find the most common word
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
        //Linq to find the hours of highest activity
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
        //Bit of code golf using ternary operator for getting 12hour time from 24hour time
        public static string TwelveHourTime(int hour)
        {
            return 
                (
                hour < 12 ? $"{hour} AM":
                hour == 12 ? $"{hour} PM": 
                hour == 0 ? $"{hour +12} AM":
                $"{hour - 12} PM"
                );
        }
        //Renders stats table with the three linq query statistics displayed
        public static void StatsTable(MessageLoggerContext context)
        {
            Console.Clear();
            var statTable = new Table();
            statTable.Border(TableBorder.Double);
            statTable.Title("Message Logger Statistics");
            statTable.AddColumn(new TableColumn("[yellow]User Message Highscores[/]").Centered());
            statTable.AddColumn(new TableColumn("[yellow]Most Common Words[/]").Centered());
            statTable.AddColumn(new TableColumn("[yellow]Hour of Most Messages[/]").Centered());
            statTable.AddRow(
                            Prompt.UsersOrderedByMessageCount(context),
                            Prompt.MostCommonWord(context, 5),
                            Prompt.HourOfMostMessages(context)
                            );

            AnsiConsole.Write(statTable);
        }





        //Depreciated Methods
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
    }
}
