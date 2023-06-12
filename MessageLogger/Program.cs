// See https://aka.ms/new-console-template for more information
using MessageLogger.Data;
using MessageLogger.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;



//Wrap in using statement to create db connection
using(var context = new MessageLoggerContext())
{
Console.WriteLine("Welcome to Message Logger!");
Console.WriteLine();
Console.WriteLine("Let's create a user profile for you.");
Console.Write("What is your name? ");
string name = Console.ReadLine();
Console.Write("What is your username? (one word, no spaces!) ");
string username = Console.ReadLine();
User user = new User(name, username);
context.Users.Add(user);
context.SaveChanges();
    user = context.Users.Single(u => u.Username == username);
Console.WriteLine();
Console.WriteLine("To log out of your user profile, enter `log out`.");

Console.WriteLine();
Console.Write("Add a message (or `quit` to exit): ");

string userInput = Console.ReadLine();


while (userInput.ToLower() != "quit")
{
    //message creation
    while (userInput.ToLower() != "log out")
    {
        user.Messages.Add(new Message(userInput)); //add message to db, reference user from db instead
            context.SaveChanges();

        foreach (var message in user.Messages) //update to reference database
        {
            Console.WriteLine($"{user.Name} {message.CreatedAt:t}: {message.Content}");
        }

        Console.Write("Add a message: ");

        userInput = Console.ReadLine();
        Console.WriteLine();
    }


    //switching user
    //new
    Console.Write("Would you like to log in a `new` or `existing` user? Or, `quit`? ");
    userInput = Console.ReadLine();
    if (userInput.ToLower() == "new")
    {
        Console.Write("What is your name? ");
        name = Console.ReadLine();
        Console.Write("What is your username? (one word, no spaces!) ");
        username = Console.ReadLine();
        user = new User(name, username);
        context.Users.Add(user); //add to database instead
            context.SaveChanges();
            user = context.Users.Single(u => u.Username == username);
            Console.Write("Add a message: ");

        userInput = Console.ReadLine();

    }
    //existing
    else if (userInput.ToLower() == "existing")
    {
        Console.Write("What is your username? ");
        username = Console.ReadLine();
        user = null;
        foreach (var existingUser in context.Users) //in database instead of list
        {
            if (existingUser.Username == username)
            {
                user = existingUser; //user = database user
            }
        }
        
        if (user != null)
        {
            Console.Write("Add a message: ");
            userInput = Console.ReadLine();
        }
        else
        {
            Console.WriteLine("could not find user");
            userInput = "quit";

        }
    }

}

//outro message
Console.WriteLine("Thanks for using Message Logger!");
    foreach (var u in context.Users.Include(u => u.Messages)) //in context instead
{
    Console.WriteLine($"{u.Name} wrote {u.Messages.Count} messages.");
}
}