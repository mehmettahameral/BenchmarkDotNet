﻿using System.Linq;
using BenchmarkDotNet.Helpers;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;

namespace BenchmarkDotNet.Exporters
{
    public class MarkdownExporter : ExporterBase
    {
        protected override string FileExtension => "md";
        protected override string FileNameSuffix => $"-{Dialect.ToLower()}";

        private string Dialect { get; set; }

        public static readonly IExporter Default = new MarkdownExporter
        {
            Dialect = nameof(Default)
        };

        public static readonly IExporter StackOverflow = new MarkdownExporter
        {
            prefix = "    ",
            Dialect = nameof(StackOverflow)
        };

        public static readonly IExporter GitHub = new MarkdownExporter
        {
            Dialect = nameof(GitHub),
            useCodeBlocks = true,
            codeBlocksSyntax = "ini"
        };

        private string prefix = string.Empty;
        private bool useCodeBlocks = false;
        private string codeBlocksSyntax = string.Empty;

        private MarkdownExporter()
        {
        }

        public override void ExportToLog(Summary summary, ILogger logger)
        {
            if (useCodeBlocks)
                logger.WriteLine($"```{codeBlocksSyntax}");
            logger = new LoggerWithPrefix(logger, prefix);
            logger.WriteLineInfo(EnvironmentHelper.GetCurrentInfo().ToFormattedString("Host"));
            logger.NewLine();

            PrintTable(summary.Table, logger);

            // TODO: move this logic to an analyser
            var benchmarksWithTroubles = summary.Reports.Values.Where(r => !r.GetResultRuns().Any()).Select(r => r.Benchmark).ToList();
            if (benchmarksWithTroubles.Count > 0)
            {
                logger.NewLine();
                logger.WriteLineError("Benchmarks with troubles:");
                foreach (var benchmarkWithTroubles in benchmarksWithTroubles)
                    logger.WriteLineError("  " + benchmarkWithTroubles.ShortInfo);
            }
        }

        private void PrintTable(SummaryTable table, ILogger logger)
        {
            if (table.FullContent.Length == 0)
            {
                logger.WriteLineError("There are no benchmarks found ");
                logger.NewLine();
                return;
            }
            table.PrintCommonColumns(logger);
            logger.NewLine();

            if (useCodeBlocks)
            {
                logger.Write("```");
                logger.NewLine();
            }

            table.PrintLine(table.FullHeader, logger, "", " |");
            logger.NewLine();
            logger.WriteLineStatistic(string.Join("", table.Columns.Where(c => c.NeedToShow).Select(c => new string('-', c.Width) + " |")));
            foreach (var line in table.FullContent)
            {
                table.PrintLine(line, logger, "", " |");
                logger.NewLine();
            }
        }
    }
}