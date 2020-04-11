using CommandLine;
using MediatR;
using NGitStats.ConsoleApp.Interfaces;

namespace NGitStats.ConsoleApp.Features.Commits
{
    [Verb("commits", HelpText = "Export details of each commit")]
    public class CommitsRequest : IVerb, IRequest<int>
    {
        [Option('r', "repo", HelpText = "The path to the repo")]
        public string RepoPath { get; set; }

        [Option('o', "outputfile", HelpText = "The path to the file you with to output to")]
        public string OutputPath { get; set; }

        [Option("overwrite", HelpText = "Overwrite existing file if it exists")]
        public bool Overwrite { get; set; }
    }
}
