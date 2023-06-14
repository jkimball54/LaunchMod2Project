// See https://aka.ms/new-console-template for more information
using MessageLogger;
using MessageLogger.Data;
using MessageLogger.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Npgsql.Replication;
using Spectre.Console;



//Wrap in using statement to create db connection
using (var context = new MessageLoggerContext())
{
    Prompt.Output("welcome");
    User user = null;
    string userInput = MainMenu();

    while(userInput != "Quit")
    {
        switch (userInput)
        {
            case "New User":
                user = CreateUser(context);
                AddMessage(context, user, userInput);
                Console.Clear();
                userInput = MainMenu();
                break;
            case "Existing User":
                user = ExistingUser(context);
                AddMessage(context, user, userInput);
                Console.Clear();
                userInput = MainMenu();
                break;
            case "Statistics":
                Console.Clear();
                var statTable = new Table();
                statTable.Border(TableBorder.Double);
                statTable.Title("Message Logger Statistics");
                statTable.AddColumn(new TableColumn("[yellow]User Message Highscores[/]").Centered());
                statTable.AddColumn(new TableColumn("[yellow]Most Common Word[/]").Centered());
                statTable.AddColumn(new TableColumn("[yellow]Hour of Most Messages[/]").Centered());
                statTable.AddRow(
                                Prompt.UsersOrderedByMessageCount(context), 
                                Prompt.MostCommonWord(context, 5),
                                Prompt.HourOfMostMessages(context)
                                );
                
                AnsiConsole.Write(statTable);
                Console.Write("Press any key to return to main menu...");
                Console.ReadKey();
                Console.Clear();
                userInput = MainMenu();
                break;
        }
    }
    Console.Clear();
    AnsiConsole.Write(
        new FigletText("Thank you!")
        .Centered()
        .Color(Color.White));
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
static string MainMenu()
{
    string selection = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
        .Title("Please select an [red]option[/].")
        .PageSize(4)
        .AddChoices(new[]
        {
            "New User",
            "Existing User",
            "Statistics",
            "Quit"
        }));

    return selection;
}