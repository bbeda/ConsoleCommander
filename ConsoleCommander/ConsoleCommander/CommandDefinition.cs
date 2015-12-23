using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Debug.Assert(args.Length == this.AllArgumentTypes.Length);

            var coercedArgs = args.Select((a, ix) => CoerceValue(a, this.AllArgumentTypes[ix])).ToArray();
            if (this.MethodInfo.ReturnType != typeof(void))
            {
                return this.MethodInfo.Invoke(null, coercedArgs)?.ToString();
            }
            else
            {
                this.MethodInfo.Invoke(null, coercedArgs);
                return "Success";
            }
        }

        private object CoerceValue(object value, Type targetType)
        {
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (value == null)
                {
                    return null;
                }
                targetType = Nullable.GetUnderlyingType(targetType);
            }

            var targetCode = Type.GetTypeCode(targetType);

            if (targetType == typeof(Guid))
            {
                return Guid.Parse(Convert.ToString(value));
            }


            switch (targetCode)
            {
                case TypeCode.Empty:
                    if (targetType.IsValueType)
                    {
                        return Activator.CreateInstance(targetType);
                    }
                    return null;
                case TypeCode.Object:
                    return value;
                case TypeCode.DBNull:
                    return DBNull.Value;
                case TypeCode.Boolean:
                    return Convert.ToBoolean(value);
                case TypeCode.Char:
                    return Convert.ToChar(value);
                case TypeCode.SByte:
                    return Convert.ToSByte(value);
                case TypeCode.Byte:
                    return Convert.ToByte(value);
                case TypeCode.Int16:
                    return Convert.ToInt16(value);
                case TypeCode.UInt16:
                    return Convert.ToUInt16(value);
                case TypeCode.Int32:
                    return Convert.ToInt32(value);
                case TypeCode.UInt32:
                    return Convert.ToUInt32(value);
                case TypeCode.Int64:
                    return Convert.ToInt64(value);
                case TypeCode.UInt64:
                    return Convert.ToUInt64(value);
                case TypeCode.Single:
                    return Convert.ToSingle(value);
                case TypeCode.Double:
                    return Convert.ToDouble(value);
                case TypeCode.Decimal:
                    return Convert.ToDecimal(value);
                case TypeCode.DateTime:
                    return Convert.ToDateTime(value);
                case TypeCode.String:
                    return Convert.ToString(value);
                default:
                    return value;
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
