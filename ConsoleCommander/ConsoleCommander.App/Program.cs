using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleCommander.App
{
    class Program
    {
        static void Main(string[] args)
        {
            bool quitRequest = false;
            CommandManager.OnQuit += (s, e) => quitRequest = true;
            while (true)
            {
                Console.Write("console>");
                var input = Console.ReadLine();
                var result = CommandManager.Execute(input);
                Console.WriteLine(result);
                if (quitRequest)
                {
                    break;
                }
            }

            Console.WriteLine("Closing..");
        }

    }
}
