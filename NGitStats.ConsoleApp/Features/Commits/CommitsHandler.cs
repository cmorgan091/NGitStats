using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using LibGit2Sharp;
using MediatR;
using NGitStats.ConsoleApp.Models;
using CompareOptions = LibGit2Sharp.CompareOptions;

namespace NGitStats.ConsoleApp.Features.Commits
{
    public class CommitsHandler : IRequestHandler<CommitsRequest, int>
    {
        public async Task<int> Handle(CommitsRequest request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Reading in git repo from: {request.RepoPath}");

            using (var repo = new Repository(request.RepoPath))
            {
                Console.WriteLine($"Total commits found: {repo.Commits.Count()}");

                // convert to stats
                Console.WriteLine($"Calculating statistics...");
                var stats = repo.Commits
                    .Select(x => GetCommitStat(repo, x))
                    .ToList();

                Console.WriteLine($"Writing output to: {request.OutputPath}");

                // write out to csv
                using (var writer = new StreamWriter(request.OutputPath))
                {
                    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        csv.Configuration.RegisterClassMap<CommitStatMap>();
                        await csv.WriteRecordsAsync(stats);
                    }
                }

            }

            Console.WriteLine("File output successfully");

            return 0;
        }

        private CommitStat GetCommitStat(Repository repo, Commit commit)
        {
            var patch = GetDiffPatch(repo, commit);

            var stat = new CommitStat
            {
                Hash = commit.Sha,
                Date = commit.Author.When.DateTime,
                Message = commit.MessageShort,
                AuthorName = commit.Author.Name,
                AuthorEmail = commit.Author.Email,
                LinesAdded = patch.LinesAdded,
                LinesDeleted = patch.LinesDeleted,
            };

            return stat;
        }

        private Patch GetDiffPatch(Repository repo, Commit commit)
        {
            var compareOption = new CompareOptions
            {
                Algorithm = DiffAlgorithm.Minimal,
                Similarity = new SimilarityOptions
                {
                    RenameDetectionMode = RenameDetectionMode.Renames
                }
            };

            // grab the parent
            var parent = commit.Parents.FirstOrDefault();

            return repo.Diff.Compare<Patch>(parent?.Tree, commit.Tree, compareOption);
        }
    }
}
