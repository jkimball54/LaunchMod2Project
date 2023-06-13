﻿using MessageLogger.Data;
using MessageLogger.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var userAll = context.Users.Include(u => u.Messages).Single(u => u.Username == currentUser.Username);
            foreach(var message in userAll.Messages)
            {
                Console.WriteLine($"{message.CreatedAt:t}: {message.Content}");
            }
        }
        public static void UsersOrderedByMessageCount(MessageLoggerContext context)
        {
            Console.WriteLine("Users Ordered by Message Count");
            var userAll = context.Users.Include(u => u.Messages).OrderByDescending(u => u.Messages.Count);
            foreach(var u in userAll)
            {
                Console.WriteLine($"{u.Username}: {u.Messages.Count}");
            }
        }
        public static void MostCommonWord(MessageLoggerContext context, int minUsageNum)
        {
            List<string> allMessageStrings = context.Messages.Select(m => m.Content).ToList();
            List<string> singleWordList = new List<string>();
            foreach(string messageString in allMessageStrings)
            {
                var splitString = messageString.Split(" ");
                singleWordList.AddRange(splitString);
            }
            Console.WriteLine("Most Commonly used word");
            foreach(string word in singleWordList.Distinct())
            {
                if(singleWordList.Where(w => w == word).Count() < minUsageNum)
                {
                    continue;
                }
                else
                {
                    Console.WriteLine($"'{word}' occurs {singleWordList.Where(w => w == word).Count()} times.");
                }
            }
            
        }
        public static void HourOfMostMessages()
        {

        }

    }
}
