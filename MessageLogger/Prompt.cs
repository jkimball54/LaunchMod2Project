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
            }
        }
    }
}
