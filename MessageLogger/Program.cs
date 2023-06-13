// See https://aka.ms/new-console-template for more information
using MessageLogger;
using MessageLogger.Data;
using MessageLogger.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Npgsql.Replication;



//Wrap in using statement to create db connection
using (var context = new MessageLoggerContext())
{
    Prompt.Output("welcome");
    User user = null;
    string userInput = "log out";

    while (userInput.ToLower() != "quit")
    {
        //message creation
        while (userInput.ToLower() != "log out")
        {
            userInput = AddMessage(context, user, userInput);
        }

        //switching user
        //new
        Prompt.Output("newOrExisting");
        userInput = Console.ReadLine();
        if (userInput.ToLower() == "new")
        {
            user = CreateUser(context);
            Prompt.Output("addMessage");
            userInput = Console.ReadLine();
        }

        //existing
        else if (userInput.ToLower() == "existing")
        {
            user = ExistingUser(context);

            if (user != null)
            {
                Prompt.DisplayMessages(context, user);
                Prompt.Output("addMessage");
                userInput = Console.ReadLine();
            }
            else
            {
                Prompt.Output("noUser");
                userInput = "new";
            }
        }

    }
    Prompt.Outro(context);
    Prompt.UsersOrderedByMessageCount(context);
    Prompt.MostCommonWord(context, 5);
}


static User CreateUser(MessageLoggerContext context)
{
    Console.Write("What is your name? ");
    string name = Console.ReadLine();
    Console.Write("What is your username? (one word, no spaces!) ");
    string username = Console.ReadLine();
    User user = new User(name, username);
    context.Users.Add(user);
    context.SaveChanges();
    user = context.Users.Single(u => u.Username == username);
    return user;
}
static User ExistingUser(MessageLoggerContext context)
{
    Prompt.DisplayUsers(context);
    Console.Write("What is your username? ");
    string username = Console.ReadLine();
    User user = null;
    foreach (var existingUser in context.Users)
    {
        if (existingUser.Username == username)
        {
            user = existingUser;
        }
    }
    return user;
}
static string AddMessage(MessageLoggerContext context, User user, string userInput)
{
    user.Messages.Add(new Message(userInput)); //add message to db, reference user from db instead
    context.SaveChanges();
    Prompt.DisplayMessages(context, user);
    Prompt.Output("addMessage");
    userInput = Console.ReadLine();
    Console.WriteLine();
    return userInput;
}
