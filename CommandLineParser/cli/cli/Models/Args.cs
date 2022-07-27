using cli.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cli.Models
{
    public class Args
    {

        private Dictionary<CommandOption, string> _lookup;
        private HashSet<CommandOption> _options;
        public Args(HashSet<CommandOption> options, string[] args)
        {
            _lookup = new Dictionary<CommandOption, string>();
            _options = options;
            Resolve(args);
        }

        public Args(string implicitOptionValue, HashSet<CommandOption> options, string[] args)
        {
            _lookup = new Dictionary<CommandOption, string>();
            _options = options;
            Resolve(args);
            _lookup.Add(CommandOption.WildCard(), implicitOptionValue);
        }

        public Ty Verb<Ty>()
        {
            return Get<Ty>(CommandOption.WildCard());
        }
        public Ty Get<Ty>(CommandOption option)
        {
            if (_lookup.TryGetValue(option, out var val))
            {
                return val.ToType<Ty>();
            }
            throw new KeyNotFoundException($"{option} was not provided");
        }

        public Ty Get<Ty>(string abbr, string verbose)
        {
            return Get<Ty>(new CommandOption(abbr, verbose));
        }

        public string ValueOf(CommandOption option)
        {
            if (_lookup.TryGetValue(option, out var val))
            {
                return val;
            }
            throw new KeyNotFoundException($"{option} was not provided");
        }

        public string ValueOf(string abbr, string verbose)
        {
            return ValueOf(new CommandOption(abbr, verbose));
        }

        private void Resolve(string[] args)
        {
            CommandOption? option = null;
            for (int i = 0; i < args.Length; i++)
            {
                if (option == null)
                {
                    option = GetOption(args[i]);
                }
                else
                {
                    // otherwise, an option was already specified, so just grab the value and validate it
                    if (option.Validator != null)
                    {
                        if (!option.Validator(args[i])) throw new Exception($"validation failed on argument {args[i]} for option {option}");
                    }
                    _lookup.Add(option, args[i]);
                    option = null; // ready for another option to be specified 
                }
            }
            _options.Where(option => !option.Required && option.Default != null).Except(_lookup.Keys).ToList().ForEach(option =>
            {
                if (option?.Default != null)
                    _lookup.Add(option, option.Default);
            });
            var remainingRequired = _options.Where(option => option.Required).Except(_lookup.Keys);
            if (remainingRequired.Any())
            {
                StringBuilder sb = new StringBuilder();
                foreach (var requiredOption in remainingRequired)
                {
                    sb.Append(" ");
                    sb.Append(requiredOption.ToString());
                }
                throw new ArgumentException($"Command requires options ({sb}) to be specified");
            }
        }

        private CommandOption? GetOption(string clArg)
        {
            CommandOption? option = null;
            if (clArg.StartsWith("--"))
            {
                var optionString = clArg.Remove(0, 2);
                option = _options.FirstOrDefault(opt => opt.Verbose == optionString);
            }
            else if (clArg.StartsWith("-"))
            {
                var optionString = clArg.Remove(0, 1);
                option = _options.FirstOrDefault(opt => opt.Abbr == optionString);     
            } 
            else throw new ArgumentException($"expected option starting with '-' or '--' but got {clArg}");

            if (option == null) throw new ArgumentException($"option '{clArg}' is not supported");
            if (option.IsFlag)
            {
                _lookup.Add(option, "true");
                return null;
            }
            return option;
        }

    }
}
