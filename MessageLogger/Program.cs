// See https://aka.ms/new-console-template for more information
using MessageLogger;
using MessageLogger.Data;
using MessageLogger.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Npgsql.Replication;
using Spectre.Console;


using (var context = new MessageLoggerContext())
{
    //Optionally wipe DB and then Seed with users/messages
    MessageLoggerDataSeeder.SeedUsersAndMessages(context, true);

    Prompt.Output("welcome");
    User user = null;
    string userInput = Prompt.MainMenu();

    while(userInput != "Quit")
    {
        switch (userInput)
        {
            case "New User":
                user = CreateUser(context);
                AddMessage(context, user, userInput);
                Console.Clear();
                userInput = Prompt.MainMenu();
                break;
            case "Existing User":
                user = ExistingUser(context);
                AddMessage(context, user, userInput);
                Console.Clear();
                userInput = Prompt.MainMenu();
                break;
            case "Statistics":
                Prompt.StatsTable(context);
                AnsiConsole.Markup("[grey slowblink]Press any key to return to main menu...[/]");
                Console.ReadKey();
                Console.Clear();
                userInput = Prompt.MainMenu();
                break;
        }
    }
    Prompt.Output("thankYou");
}

static User CreateUser(MessageLoggerContext context)
{
    var panel = new Panel("[invert bold]MESSAGE LOGGER[/] [red]Create New User[/] [dim grey] Controls - Type your responses[/]");
    panel.Expand();
    AnsiConsole.Write(panel);
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
    var panel = new Panel("[invert bold]MESSAGE LOGGER[/] [red]Existing User[/] [dim grey] Controls - Select a user from the menu with the arrow keys[/]");
    panel.Expand();
    AnsiConsole.Write(panel);
    User user = null;
    var username = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
        .Title("Please select a [red]User to Login[/].")
        .PageSize(context.Users.Count())
        .MoreChoicesText("[grey](Move up and down to reveal more users)[/]")
        .AddChoices(context.Users.Select(u => u.Username).ToList()));
    return context.Users.Single(u => u.Username == username);
}
static void AddMessage(MessageLoggerContext context, User user, string userInput)
{
    userInput = null;
    while(userInput != "quit")
    {
        Prompt.DisplayMessages(context, user);
        Prompt.Output("addMessage");
        userInput = Console.ReadLine();
        user.Messages.Add(new Message(userInput)); //add message to db, reference user from db instead
        context.SaveChanges();
    }
}
