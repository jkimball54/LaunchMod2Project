using MessageLogger.Data;
using MessageLogger.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MessageLogger
{
    public class Prompt
    {
        public static void Output(string prompt)
        {
            switch(prompt)
            {
                case "welcome":
                    Console.WriteLine("Welcome to Message Logger!");
                    Console.WriteLine();
                    Console.WriteLine();
                    break;
                case "controls":
                    Console.WriteLine("To log out of your user profile, enter `log out`.");
                    Console.WriteLine();
                    Console.Write("Add a message (or `quit` to exit): ");
                    break;
                case "addMessage":
                    Console.Write("Add a message: ");
                    break;
                case "newOrExisting":
                    Console.WriteLine("(new/existing) New or existing user?: ");
                    break;
                case "noUser":
                    Console.WriteLine("User not found! Lets create one for you.");
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
            var userAll = context.Users.Include(u => u.Messages).Single(u => u.Username == currentUser.Username);
            foreach(var message in userAll.Messages)
            {
                Console.WriteLine($"{message.CreatedAt:t}: {message.Content}");
            }
        }
        public static string UsersOrderedByMessageCount(MessageLoggerContext context)
        {
            var userAll = context.Users.Include(u => u.Messages).OrderByDescending(u => u.Messages.Count);
            string builtString = null;
            foreach(var u in userAll)
            {
                builtString += $"{u.Username}: {u.Messages.Count}\n";
            }
            return builtString;
        }
        public static string MostCommonWord(MessageLoggerContext context, int minUsageNum)
        {
            List<string> allMessageStrings = context.Messages.Select(m => m.Content).ToList();
            List<string> singleWordList = new List<string>();
            foreach(string messageString in allMessageStrings)
            {
                var splitString = messageString.Split(" ");
                singleWordList.AddRange(splitString);
            }
            string builtString = null;
            foreach(string word in singleWordList.Distinct())
            {
                if(singleWordList.Where(w => w == word).Count() < minUsageNum)
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
            for(var firstEverMessageHour = context.Messages.Min(m => m.CreatedAt.Hour); firstEverMessageHour < lastEverMessageHour; firstEverMessageHour++)
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
            if(hour < 12)
            {
                return $"{hour} AM";
            }
            else if(hour == 12)
            {
                return $"{hour} PM";
            }
            else if(hour == 0)
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
