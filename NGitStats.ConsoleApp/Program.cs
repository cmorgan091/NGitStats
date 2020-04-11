using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CommandLine;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NGitStats.ConsoleApp.Interfaces;

namespace NGitStats.ConsoleApp
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var returnCode = -1;

            // make a list of all verbs
            var availableCommandTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(IVerb).IsAssignableFrom(t) && !t.IsInterface)
                .ToArray();

            var command = GetCommandFromArgs(args, availableCommandTypes) as IRequest<int>;

            if (command != null)
            {
                var mediator = BuildMediator();

                try
                {
                    returnCode = await mediator.Send(command);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Executing command ({command.GetType().Name})");
                   
                    Console.WriteLine(ex.Message);
                }
            }

            return returnCode;
        }

        private static object GetCommandFromArgs(string[] args, Type[] availableVerbTypes)
        {
            // parse the command line
            var parsedArguments = new Parser(cfg =>
                {
                    cfg.CaseInsensitiveEnumValues = true;
                    cfg.HelpWriter = Console.Out;
                })
                .ParseArguments(args, availableVerbTypes);

            return parsedArguments.Tag == ParserResultType.Parsed ? ((Parsed<object>)parsedArguments).Value : null;
        }

        private static IMediator BuildMediator()
        {
            var services = new ServiceCollection();

            services.AddMediatR(typeof(IVerb).Assembly);

            var provider = services.BuildServiceProvider();

            return provider.GetRequiredService<IMediator>();
        }
    }
}
