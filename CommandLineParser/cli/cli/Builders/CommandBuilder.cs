using cli.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cli.Builders
{
    public static class CommandBuilder
    {
        public static CommandVerb Verb(string name)
        {
            var verb = new CommandVerb(name);
            return verb;
        }

        public static CommandGroup CommandGroup(string groupName = "")
        {
            return new CommandGroup(groupName);
        }
    }
}
