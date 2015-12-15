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

            while (true)
            {
                Console.Write("console>");
                var input = Console.ReadLine();
                Console.WriteLine(CommandManager.Execute(input));
            }
        }

    }
}
