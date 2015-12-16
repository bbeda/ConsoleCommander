using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleCommander
{
    [Command(isGlobal: true)]
    public class CommandManager
    {
        static readonly Dictionary<string, CommandDefinition> Commands;
        static CommandManager()
        {
            Commands = GetCommands().ToDictionary(k => k.Pattern.ToLowerInvariant());
        }

        [CommandIgnore]
        public static string Execute(string input)
        {
            var command = new ConsoleCommand(input);
            var matchingCommand = Commands.Where(c => Regex.IsMatch(command.Pattern, c.Key, RegexOptions.IgnoreCase)).Select(g => g.Value).FirstOrDefault();

            if (matchingCommand != null)
            {
                return matchingCommand.Execute(command.Arguments.Cast<object>().ToArray());
            }
            return "Not found!";
        }

        public static string Ls()
        {
            return ListInternal(null);
        }

        public static string Ls(string prefix)
        {
            return ListInternal(prefix);
        }

        private static string ListInternal(string prefix)
        {
            var sb = new StringBuilder();
            foreach (var command in Commands.Where(c => string.IsNullOrEmpty(prefix) || c.Value.CommandPrefix.ToLowerInvariant() == prefix.ToLowerInvariant()))
            {
                sb.AppendLine(command.Value.ToString());
            }
            return sb.ToString();
        }

        private static CommandDefinition[] GetCommands()
        {
            var commandClasses = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Where(t => t.GetCustomAttribute<CommandAttribute>() != null));
            return commandClasses.SelectMany(c => c.GetMethods().Where(m => m.DeclaringType == c && !m.GetCustomAttributes<CommandIgnoreAttribute>().Any())).Select(m => new CommandDefinition(m)).ToArray();
        }

    }
}
