using System;
using System.Globalization;
using CsvHelper.Configuration;

namespace NGitStats.ConsoleApp.Models
{
    public class CommitStat
    {
        public string Hash { get; set; }

        public DateTime Date { get; set; }

        public string AuthorName { get; set; }

        public string AuthorEmail { get; set; }

        public string Message { get; set; }

        public int LinesAdded { get; set; }

        public int LinesDeleted { get; set; }
    }

    public sealed class CommitStatMap : ClassMap<CommitStat>
    {
        public CommitStatMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.Date).TypeConverterOption.Format("yyyy-MM-dd hh:mm:ss");
        }
    }
}
