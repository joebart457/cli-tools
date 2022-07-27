using cli.Extensions;
using cli.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cli.Models
{
    public class CommandVerb
    {
        private string _verb;
        private string? _description;

        public string Name { get { return _verb; } }
        private CommandOption? PreviousCommand { get; set; }
        public HashSet<CommandOption> Options { get; set; } = new HashSet<CommandOption> { };
        private Func<Args, int>? _action;
        public CommandVerb(string verb)
        {
            _verb = verb;
        }

        public int Execute(Args args)
        {
            if (_action == null) throw new InvalidOperationException($"action was null for verb {_verb}");
            return _action(args);
        }

        public string GetHelpText(int nestingLevel)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"{(_verb == CommandConstants.WildCard? "<any>": _verb)} : {_description}\n".Nest(nestingLevel));
            foreach(var option in Options)
            {
                sb.Append(option.GetHelpText(nestingLevel + 1));
            }
            return sb.ToString();
        }

        public CommandVerb Description(string description)
        {
            _description = description;
            return this;
        }

        public CommandVerb Action(Func<Args, int> action)
        {
            _action = action;
            return this;
        }

        #region OptionChaining
        public CommandVerb Option(string abbr, string verbose)
        {
            PreviousCommand = new CommandOption(abbr, verbose);
            if (Options.Contains(PreviousCommand)) throw new ArgumentException($"option with key {abbr}:{verbose} already exists.");
            Options.Add(PreviousCommand);
            return this;
        }

        public CommandVerb WithValidator(Func<string, bool> validator)
        {
            if (PreviousCommand == null) throw new ArgumentNullException(nameof(PreviousCommand));
            PreviousCommand.SetValidator(validator);
            return this;
        }

        public CommandVerb WithDefault(string defaultValue)
        {
            if (PreviousCommand == null) throw new ArgumentNullException(nameof(PreviousCommand));
            PreviousCommand.SetDefault(defaultValue);
            return this;
        }

        public CommandVerb WithDescription(string description)
        {
            if (PreviousCommand == null) throw new ArgumentNullException(nameof(PreviousCommand));
            PreviousCommand.SetDescription(description);
            return this;
        }

        public CommandVerb Required(bool required = true)
        {
            if (PreviousCommand == null) throw new ArgumentNullException(nameof(PreviousCommand));
            PreviousCommand.SetRequired(required);
            return this;
        }

        public CommandVerb IsFlag(bool isFlag = true)
        {
            if (PreviousCommand == null) throw new ArgumentNullException(nameof(PreviousCommand));
            PreviousCommand.SetIsFlag(isFlag);
            return this;
        }

#endregion

    }
}
