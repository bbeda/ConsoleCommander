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
            var commands = GetCommands();
            while (true)
            {
                Console.Write("console>");
                var input = Console.ReadLine();
                var command = new ConsoleCommand(input);
                var matchingCommand = commands.Where(c => Regex.IsMatch(command.Pattern, c.Pattern, RegexOptions.IgnoreCase)).FirstOrDefault();
                if (matchingCommand != null)
                {
                    Console.WriteLine(matchingCommand.Execute(command.Arguments.Cast<object>().ToArray()));
                }
            }
        }


        static CommandDefinition[] GetCommands()
        {
            var commandClasses = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetCustomAttribute<CommandAttribute>() != null);
            return commandClasses.SelectMany(c => c.GetMethods().Where(m => m.DeclaringType == c)).Select(m => new CommandDefinition(m)).ToArray();
        }

    }
}
