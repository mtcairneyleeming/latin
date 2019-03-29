using database.Helpers;
using McMaster.Extensions.CommandLineUtils;
using Serilog;

namespace cli
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Console().CreateLogger();


            var cli = BuildApp();
            cli.Execute(args);
        }

        private static CommandLineApplication BuildApp()
        {
            var app = new CommandLineApplication
            {
                Name = "latin"
            };
            app.HelpOption("-h|--help");
            app.OnExecute(() =>
            {
                Log.Information("Welcome to the cli for managing the magister latin learning system!");
                Log.Information("To show help, run this command with the flag '-h'");
                return 0;
            });

            app.Command("api-load", WiktionaryAPILoaders.BuildLoader);
            app.Command("help", BuildHelper);
            app.Command("load", BuildDumpLoader);
            return app;
        }

        private static void BuildDumpLoader(CommandLineApplication cli)
        {
            cli.Command("pages", command =>
            {
                command.Description = "Load all pages";
                command.HelpOption("-h|--help");

                command.OnExecute(() =>
                {
                    WiktionaryDumpLoader.Load();
                    return 0;
                });
            });
        }


        private static void BuildHelper(CommandLineApplication cli)
        {
            cli.Command("morph", command =>
            {
                command.Description = "Print the data for a given morph code";
                command.HelpOption("-h|--help");
                var morphArgument = command.Argument("[morphCode]",
                    "The morphCode you'd like info on", true);
                command.OnExecute(() =>
                {
                    var d = MorphCodeParser.ParseCode(morphArgument.Value);
                    foreach (var line in d) Log.Information(line);
                    return 0;
                });
            });
        }
    }
}