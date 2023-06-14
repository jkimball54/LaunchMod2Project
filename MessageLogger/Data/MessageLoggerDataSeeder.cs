using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using MessageLogger.Model;

namespace MessageLogger.Data
{
    public class MessageLoggerDataSeeder
    {
        public static void SeedUsersAndMessages(MessageLoggerContext context)
        {
            if(!context.Users.Any())
            {
                Message message1 = new Message("Hey!");
                Message message2 = new Message("Im tired.");
                Message message3 = new Message("It's sunny!");
                Message message4 = new Message("I want food.");
                Message message5 = new Message("Hello!");
                Message message6 = new Message("To be or not to be.");
                Message message7 = new Message("I think therefore I am.");

                var users = new List<User>
                {
                    new User("Jeremy", "Jeremy123"),
                    new User("Will", "Will123"),
                    new User("John", "John123"),
                    new User("Olivia", "Olivia123"),
                    new User("Rob", "Rob123")
            };
                users[0].Messages.Add(message1);
                users[0].Messages.Add(message6);
                users[0].Messages.Add(message7);
                users[1].Messages.Add(message2);
                users[2].Messages.Add(message3);
                users[3].Messages.Add(message4);
                users[4].Messages.Add(message5);

                context.Users.AddRange(users);
                context.SaveChanges();
            }
        }
    }
}
