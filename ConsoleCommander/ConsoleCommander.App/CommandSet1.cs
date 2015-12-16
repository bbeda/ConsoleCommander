using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCommander.App
{
    [Command("Test")]
    public class CommandSet1
    {
        public static string Command1(string param1)
        {
            return $"Executed Command1, {param1}";
        }

        public static string Command2()
        {
            return $"Executed Command2";
        }

        public static string Command4(int i)
        {
            return $"Executed Command 4 param " + i.ToString();
        }

        public static string Command3(string param1, string param2 = "val")
        {
            return $"Executed Command3 {param1} {param2}";
        }

        public static string Command1(string param1, string param2 = "val")
        {
            return $"Executed Command1  {param1} {param2}";
        }

        public static void Command5(string param1)
        {
            Console.WriteLine($"Executed Command5  {param1}");
        }
    }
}
