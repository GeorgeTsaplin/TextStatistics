using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace TextStatistics
{
    public class TextStatisticsCalculator
    {
        private readonly TextSplitter textSplitter;

        private readonly WordsCalculator wordsCalculator;

        private readonly byte threadAmount;

        /// <summary> Calculate words statistics at text 
        /// </summary>
        /// <param name="textSplitter">text splitter</param>
        /// <param name="wordsCalculator">words calculator</param>
        /// <param name="threadAmount">amount of used threads, should be not greated then available system CPU cores</param>
        public TextStatisticsCalculator(TextSplitter textSplitter, WordsCalculator wordsCalculator, byte threadAmount)
        {
            if (textSplitter == null)
            {
                throw new ArgumentNullException(nameof(textSplitter));
            }

            if (wordsCalculator == null)
            {
                throw new ArgumentNullException(nameof(wordsCalculator));
            }

            this.textSplitter = textSplitter;
            this.wordsCalculator = wordsCalculator;
            this.threadAmount = threadAmount;
        }

        public IReadOnlyDictionary<string, uint> CalcStatistics(TextReader reader, IEqualityComparer<string> wordsComparer, TimeSpan timeout)
        {
            var workersData = new WorkerData[this.threadAmount];
            for (int i = 0; i < this.threadAmount; i++)
            {
                var workerData = new WorkerData(new ManualResetEvent(false), wordsComparer);
                workersData[i] = workerData;

                ThreadPool.QueueUserWorkItem(
                    state =>
                    {
                        var data = (WorkerData)state;
                        do
                        {
                            var textToAnalyze = reader.ReadNext();
                            if (textToAnalyze == null)
                            {
                                break;
                            }

                            var splittedText = this.textSplitter.SplitWords(textToAnalyze);
                            var wordsStat = this.wordsCalculator.Calculate(splittedText, wordsComparer);
                            data.Union(wordsStat);
                        } while (true);

                        data.Trigger.Set();
                    },
                    workerData);
            }

            WaitWorkers(workersData, timeout);

            var firstResult = workersData.First().Result;
            var result = new Dictionary<string, uint>(firstResult, firstResult.Comparer);

            workersData.Skip(1).ForEachEx(item => result.AddRange(item.Result, WorkerData.AggregatorFunc));

            return result;
        }

        private static void WaitWorkers(WorkerData[] workers, TimeSpan timeout)
        {
            // HACK gtsaplin: WaitHandle.WaitAll raises exception
            var leftTimeout = timeout;
            var watch = new Stopwatch();

            workers.ForEachEx(item =>
            {
                watch.Start();
                if (!item.Trigger.WaitOne(leftTimeout))
                {
                    throw new TimeoutException($"Cannot calculate statistics for specified time - {timeout}");
                }
                watch.Stop();
                leftTimeout = leftTimeout.Add(TimeSpan.FromTicks(-watch.ElapsedTicks));
            });
        }

        private class WorkerData
        {
            internal static readonly Func<uint, uint, uint> AggregatorFunc = (lhs, rhs) => lhs + rhs;

            public WorkerData(EventWaitHandle trigger, IEqualityComparer<string> wordsComparer)
            {
                this.Trigger = trigger;
                this.Result = new Dictionary<string, uint>(wordsComparer);
            }

            public EventWaitHandle Trigger { get; }

            public Dictionary<string, uint> Result { get; }

            public void Union(Dictionary<string, uint> other)
            {
                this.Result.AddRange(other, AggregatorFunc);
            }
        }
    }
}
