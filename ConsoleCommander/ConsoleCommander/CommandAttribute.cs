using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCommander
{
    public class CommandAttribute : Attribute
    {
        public string Name { get; private set; }

        public bool IsGlobal { get; private set; }

        public CommandAttribute()
        {
            
        }

        public CommandAttribute(string name)
        {
            this.Name = name;
        }

        public CommandAttribute(bool isGlobal)
        {
            this.IsGlobal = isGlobal;
        }
    }
}
