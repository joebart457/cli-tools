using cli.Builders;
using System;
using System.Linq;

namespace cli 
{
    internal class Program
    {
        static void Main(string[] args)
        {
			Console.WriteLine(string.Join(",", args));
			var group = CommandBuilder.CommandGroup("")
				.Group("do")
					.Verb("create")
					.Option("o", "object")
						.WithValidator(s => s == "identity" || s == "entry")
						.WithDefault("identity")
					.Option("f", "firstname")
						.WithValidator(s => !string.IsNullOrEmpty(s))
					.Option("i", "index")
					.Required()
					.Action(args =>
                    {
						int i = args.Get<int>(new Models.CommandOption("i", "index"));
						Console.WriteLine(i);
						return 0;
                    })
					.Verb("remove")
					.Option("o", "object")
						.WithValidator(s => s == "identity" || s == "entry")
						.WithDefault("identity")
					.Option("f", "firstname")
						.WithValidator(s => !string.IsNullOrEmpty(s))
				.Group("remove")
					.Verb("create")
					.Option("o", "object")
						.WithValidator(s => s == "identity" || s == "entry")
						.WithDefault("identity")
					.Option("f", "firstname")
						.WithValidator(s => !string.IsNullOrEmpty(s))
					.Verb("remove")
					.Option("o", "object")
						.WithValidator(s => s == "identity" || s == "entry")
						.WithDefault("identity")
					.Option("f", "firstname")
						.WithValidator(s => !string.IsNullOrEmpty(s));

			Console.WriteLine(group.GetHelpText());
			group.Execute(new string[] {"do", "create"});

		}
	}
}