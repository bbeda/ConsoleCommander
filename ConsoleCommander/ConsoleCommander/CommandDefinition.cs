using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCommander
{
    public class CommandDefinition
    {
        private readonly MethodInfo MethodInfo;

        public CommandDefinition(MethodInfo methodInfo)
        {
            this.MethodInfo = methodInfo;
            this.CommandPrefix = SetPrefix(methodInfo);
            this.CommandName = methodInfo.Name;

            var parameters = methodInfo.GetParameters();
            this.RequiredArgumentTypes = parameters.Where(p => !p.IsOptional).Select(p => p.ParameterType).ToArray();
            this.OptionalArgumentTypes = parameters.Where(p => p.IsOptional).Select(p => p.ParameterType).ToArray();
            this.AllArgumentTypes = parameters.Select(p => p.ParameterType).ToArray();

            this.Pattern = GetPattern();
            this.Display = GetDisplay();
        }

        private string SetPrefix(MethodInfo methodInfo)
        {
            var commandAttribute = methodInfo.DeclaringType.GetCustomAttributes<CommandAttribute>().FirstOrDefault();
            if (commandAttribute != null)
            {
                if (!string.IsNullOrEmpty(commandAttribute.Name))
                {
                    return commandAttribute.Name;
                }
                else if (commandAttribute.IsGlobal)
                {
                    return string.Empty;
                }
            }
            else
            {
                return methodInfo.DeclaringType.Name;
            }

            return string.Empty;
        }

        public Type[] RequiredArgumentTypes { get; private set; }

        public Type[] OptionalArgumentTypes { get; private set; }

        public Type[] AllArgumentTypes { get; private set; }

        public string CommandPrefix { get; private set; }

        public string CommandName { get; private set; }

        public string Pattern { get; private set; }

        public string Display { get; private set; }

        public string Execute(object[] args)
        {
            if (this.MethodInfo.ReturnType != typeof(void))
            {
                return this.MethodInfo.Invoke(null, args)?.ToString();
            }
            else
            {
                this.MethodInfo.Invoke(null, args);
                return "Success";
            }
        }

        private string GetPattern()
        {
            var sb = new StringBuilder();
            sb.Append("^");
            if (!string.IsNullOrEmpty(this.CommandPrefix))
            {
                sb.Append(this.CommandPrefix);
                sb.Append(@"\.");
            }

            sb.Append(this.CommandName);

            var argIndex = 0;
            if (this.RequiredArgumentTypes.Any())
            {
                foreach (var arg in RequiredArgumentTypes)
                {
                    sb.Append(@"\s");
                    sb.Append($"p{argIndex}");
                    argIndex++;
                }
            }

            if (this.OptionalArgumentTypes.Any())
            {
                sb.Append(@"\s?");
                sb.Append($@"([p0-9]*\s?){{0,{this.OptionalArgumentTypes.Count()}}}");

            }
            sb.Append("$");
            return sb.ToString();
        }


        public override string ToString()
        {
            return this.Display;
        }

        private string GetDisplay()
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(this.CommandPrefix))
            {
                sb.Append(this.CommandPrefix + ".");
            }

            sb.Append(this.CommandName);

            var ix = 0;
            foreach (var p in this.RequiredArgumentTypes)
            {
                sb.Append(" " + this.MethodInfo.GetParameters()[ix].Name + ":" + p.Name);
                ix++;
            }
            ix = 0;
            foreach (var p in this.OptionalArgumentTypes)
            {
                sb.Append(" [" + this.MethodInfo.GetParameters()[ix].Name + ":" + p.Name + "]");
                ix++;
            }

            return sb.ToString();
        }
    }
}
