using cli.Extensions;
using cli.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cli.Models
{
    public class CommandOption
    {
        private string _abbr;
        private string _verbose;
        private string? _default;
        private string? _description;
        private Func<string, bool>? _validator;
        private bool _required = false;
        private bool _isFlag = false;

        public string Abbr { get { return _abbr; } }
        public string Verbose { get { return _verbose; } }
        public bool IsFlag { get { return _isFlag; } }
        public bool Required { get { return _required; } }
        public string? Default { get { return _default; } }
        public Func<string, bool>? Validator { get { return _validator; } }
        public CommandOption(string abbr, string verbose)
        {
            _abbr = abbr;
            _verbose = verbose;
        }

        public static CommandOption WildCard()
        {
            return new CommandOption(CommandConstants.WildCard, CommandConstants.WildCard);
        }
        public void SetValidator(Func<string, bool> validator)
        {
            _validator = validator;
        }

        public void SetDefault(string defaultValue)
        {
            _default = defaultValue;
        }

        public void SetDescription(string description)
        {
            _description = description;
        }

        public void SetRequired(bool required)
        {
            _required = required;
        }
        public void SetIsFlag(bool isFlag)
        {
            _isFlag = isFlag;
            if (isFlag && _default == null) _default = "false";
        }

        public string GetHelpText(int nestingLevel)
        {
            string helpText = $"-{_abbr} | --{_verbose} : {(_required ? "(required)" : "")} {_description}\n";
            return helpText.Nest(nestingLevel) ?? "";
        }

        public override string ToString()
        {
            return $"-{_abbr} | --{_verbose}";
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(_abbr, _verbose);
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj is CommandOption opt)
            {
                return _abbr == opt._abbr &&
                    _verbose == opt._verbose;
            }
            return false;
        }
    }
}
