﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCommander.App
{
    public class CommandDefinition
    {
        private readonly MethodInfo MethodIndo;

        public CommandDefinition(MethodInfo methodInfo)
        {
            this.MethodIndo = methodInfo;
            var commandAttribute = methodInfo.DeclaringType.GetCustomAttributes<CommandAttribute>().FirstOrDefault();
            if (commandAttribute != null)
            {
                if (!string.IsNullOrEmpty(commandAttribute.Name))
                {
                    this.CommandPrefix = commandAttribute.Name;
                }
                else
                    if (commandAttribute.IsGlobal)
                {
                    this.CommandPrefix = string.Empty;
                }
            }
            else
            {
                this.CommandPrefix = methodInfo.DeclaringType.Name;
            }

            this.CommandName = methodInfo.Name;

            var parameters = methodInfo.GetParameters();
            this.RequiredArguments = parameters.Where(p => !p.IsOptional).Select(p => p.ParameterType).ToArray();
            this.OptionalArguments = parameters.Where(p => p.IsOptional).Select(p => p.ParameterType).ToArray();
        }

        public Type[] RequiredArguments { get; private set; }

        public Type[] OptionalArguments { get; private set; }

        public string CommandPrefix { get; private set; }

        public string CommandName { get; private set; }

        public string Execute(object[] args)
        {
            return this.MethodIndo.Invoke(null, args)?.ToString();
        }

        public string Pattern
        {
            get
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
                if (this.RequiredArguments.Any())
                {
                    foreach (var arg in RequiredArguments)
                    {
                        sb.Append(@"\s");
                        sb.Append($"p{argIndex}");
                        argIndex++;
                    }
                }

                if (this.OptionalArguments.Any())
                {
                    sb.Append(@"\s?");
                    sb.Append($@"([p0-9]*\s?){{0,{this.OptionalArguments.Count()}}}");

                }
                sb.Append("$");
                return sb.ToString();
            }
        }
    }
}