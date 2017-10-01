using System;
using System.Text;

namespace TextStatistics
{
    public class TextReader : IDisposable
    {
        private readonly System.IO.TextReader source;

        private StringBuilder leftFromPreviousRead = new StringBuilder();

        private readonly char[] buffer;

        public TextReader(System.IO.TextReader source, uint batch)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            const int MaxBatch = int.MaxValue;
            if (batch == 0 || batch > MaxBatch)
            {
                throw new ArgumentOutOfRangeException(nameof(batch), batch, $"Must be grater then zero and not greater then {MaxBatch}");
            }

            this.source = source;
            this.Batch = (int)batch;
            this.buffer = new char[batch];
        }

        public int Batch { get; }

        private object sync = new object();
        public string ReadNext()
        {
            lock (sync)
            {
                int bytesAmount = source.ReadBlock(this.buffer, 0, this.Batch);
                if (bytesAmount == 0)
                {
                    var lastPart = this.leftFromPreviousRead.ToString();
                    this.leftFromPreviousRead = new StringBuilder();

                    return string.IsNullOrWhiteSpace(lastPart) ? null : lastPart;
                }

                const char Space = ' ';

                var result = this.leftFromPreviousRead;

                // read stream to the end
                if (bytesAmount < this.Batch)
                {
                    result.Append(this.buffer, 0, bytesAmount);

                    this.leftFromPreviousRead = new StringBuilder();

                    return result.ToString();
                }

                int lastSpaceIndex = LastIndexOf(this.buffer, bytesAmount - 1, Space);
                if (lastSpaceIndex == -1)
                {
                    throw new NotSupportedException(
                        $"Incoming text does not have spaces at fragment of {this.Batch} bytes, try to increase buffer size");
                }

                result.Append(this.buffer, 0, lastSpaceIndex);

                this.leftFromPreviousRead =
                    new StringBuilder(new string(this.buffer, lastSpaceIndex + 1, bytesAmount - lastSpaceIndex - 1));

                return result.ToString();
            }
        }

        public void Dispose()
        {
            this.source.Dispose();
        }

        private static int LastIndexOf(char[] source, int startFrom, char value)
        {
            int maxIndex = source.Length >= startFrom ? startFrom : source.Length - 1;
            for (int i = maxIndex; i >= 0; i--)
            {
                if (source[i] == value)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
