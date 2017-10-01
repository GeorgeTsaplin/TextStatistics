using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using CommandLine.Text;

namespace TextStatistics
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            if (!CommandLine.Parser.Default.ParseArgumentsStrict(args, options))
            {
                Console.WriteLine(options.GetHelp());

                Console.ReadLine();

                return;
            }

            var wordsComparer = options.CaseInsensitive
                ? StringComparer.InvariantCultureIgnoreCase
                : StringComparer.InvariantCulture;

            var calculator =
                new TextStatisticsCalculator(new TextSplitter(), new WordsCalculator(), options.Threads);

            IReadOnlyDictionary<string, uint> result;
            using (var reader = new TextReader(File.OpenText(options.InputPath), (uint)options.BufferSize*1024))
            {
                result = calculator.CalcStatistics(reader, wordsComparer, TimeSpan.FromSeconds(options.Timeout));
            }

            using (var writer = File.CreateText(options.OutputPath))
            {
                result.OrderByDescending(x => x.Value)
                    .ThenBy(x => x.Key)
                    .Select(kvp => $"{kvp.Key} {kvp.Value}")
                    .ForEachEx(x => writer.WriteLine(x));

                writer.Flush();
            }
        }

        class Options
        {
            [Option('i', "input", Required = true, HelpText = "Path to input file")]
            public string InputPath { get; set; }

            [Option('o', "output", Required = true, HelpText = "Path to output file")]
            public string OutputPath { get; set; }

            [Option('b', "buffer", Required = false, DefaultValue = (uint)500, HelpText = "Buffer size, in KB")]
            public uint BufferSize { get; set; }

            [Option('p', "parallel", Required = false, DefaultValue = (byte)1, HelpText = "Number of thread for parallel execution")]
            public byte Threads { get; set; }

            [Option('c', "caseinsensitive", Required = false, DefaultValue = false, HelpText = "Case-insensitive text processing")]
            public bool CaseInsensitive { get; set; }

            [Option('t', "timeout", Required = false, DefaultValue = ushort.MaxValue, HelpText = "timeout, in sec")]
            public ushort Timeout { get; set; }

            [HelpOption]
            public string GetHelp() => HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
