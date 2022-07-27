using cli.Extensions;
using cli.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cli.Models
{
    public class CommandGroup
    {

        private string _name;
        private CommandGroup? PreviousGroup { get; set; }
        private CommandVerb? PreviousVerb { get; set; }
        public Dictionary<string, CommandGroup> Groups { get; set; }
        public Dictionary<string, CommandVerb> Verbs { get; set; }
        private bool _showHelpIfUnresolvable = false;
        public CommandGroup(string name)
        {
            _name = name;
            Groups = new Dictionary<string, CommandGroup>();
            Verbs = new Dictionary<string, CommandVerb>();
            PreviousGroup = this;
        }

        public string GetHelpText(int __nestingLevel = 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"|{_name}:".Nest(__nestingLevel));
            foreach (var verb in Verbs)
            {
                sb.Append(verb.Value.GetHelpText(__nestingLevel + 1));
            }
            foreach (var group in Groups)
            {
                sb.Append(group.Value.GetHelpText(__nestingLevel + 1));
            }
            return sb.ToString();
        }

        public int Execute(string[] args)
        {
            if (!_showHelpIfUnresolvable) return Resolve(args);
            try
            {
                return Resolve(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR -- {ex.Message}");
                Console.WriteLine($"Usage: {GetHelpText()}");
            }
            return 0;
        }
        private int Resolve(string[] args)
        {
            if (args.Length == 0) throw new ArgumentException("no arguments specified");

            if (Groups.TryGetValue(args[0], out var commandGroup))
            {
                return commandGroup.Resolve(args.Skip(1).ToArray());
            }
            if (Verbs.TryGetValue(args[0], out var commandVerb))
            {
                return commandVerb.Execute(new Args(commandVerb.Options, args.Skip(1).ToArray()));
            }
            // If neither are found, try to find a wildcard option
            if (Verbs.TryGetValue(CommandConstants.WildCard, out var wildcardVerb))
            {
                return wildcardVerb.Execute(new Args(args[0], wildcardVerb.Options, args.Skip(1).ToArray()));
            }
            throw new ArgumentException($"unable to resolve argument {args[0]}");
        }

        public CommandGroup Group(string groupName)
        {
            PreviousGroup = new CommandGroup(groupName);
            Groups.Add(groupName, PreviousGroup);
            return this;
        }

        public CommandGroup SubGroup(CommandGroup group)
        {
            PreviousGroup?.Groups.Add(group._name, group);
            return this;
        }

        public CommandGroup ShowHelpIfUnresolvable(bool value = true)
        {
            _showHelpIfUnresolvable = value;
            return this;
        }


        #region VerbChaining

        public CommandGroup Verb(string verbName = CommandConstants.WildCard)
        {
            if (PreviousGroup == null) throw new ArgumentNullException(nameof(PreviousGroup));
            PreviousVerb = new CommandVerb(verbName);
            PreviousGroup.AddVerb(PreviousVerb);
            return this;
        }

        private void AddVerb(CommandVerb verb)
        {         
            Verbs.Add(verb.Name, verb);
        }


        public CommandGroup Option(string abbr, string verbose)
        {
            if (PreviousVerb == null) throw new ArgumentNullException(nameof(PreviousVerb));
            PreviousVerb.Option(abbr, verbose);
            return this;
        }

        public CommandGroup WithValidator(Func<string, bool> validator)
        {
            if (PreviousVerb == null) throw new ArgumentNullException(nameof(PreviousVerb));
            PreviousVerb.WithValidator(validator);
            return this;
        }

        public CommandGroup WithDefault(string defaultValue)
        {
            if (PreviousVerb == null) throw new ArgumentNullException(nameof(PreviousVerb));
            PreviousVerb.WithDefault(defaultValue);
            return this;
        }

        public CommandGroup WithDescription(string description)
        {
            if (PreviousVerb == null) throw new ArgumentNullException(nameof(PreviousVerb));
            PreviousVerb.WithDescription(description);
            return this;
        }

        public CommandGroup Required(bool required = true)
        {
            if (PreviousVerb == null) throw new ArgumentNullException(nameof(PreviousVerb));
            PreviousVerb.Required(required);
            return this;
        }

        public CommandGroup IsFlag(bool isFlag = true)
        {
            if (PreviousVerb == null) throw new ArgumentNullException(nameof(PreviousVerb));
            PreviousVerb.IsFlag(isFlag);
            return this;
        }

        public CommandGroup Description(string description)
        {
            if (PreviousVerb == null) throw new ArgumentNullException(nameof(PreviousVerb));
            PreviousVerb.Description(description);
            return this;
        }

        public CommandGroup Action(Func<Args, int> action)
        {
            if (PreviousVerb == null) throw new ArgumentNullException(nameof(PreviousVerb));
            PreviousVerb.Action(action);
            return this;
        }

        #endregion
    }
}
