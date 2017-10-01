using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TextStatistics.Tests
{
    [TestClass]
    public class TextReaderTests
    {
        [TestMethod]
        public void ReadNext_BatchMoreThenFullSize_ShouldReturnNullOnSecondCall()
        {
            // Arrange
            const string ExpectedStr = "Lorem ipsum dolor sit posuere";

            var target = new TextReader(new System.IO.StringReader(ExpectedStr), (uint)ExpectedStr.Length * 2);

            // Act
            var actualFirst = target.ReadNext();
            var actualSecond = target.ReadNext();

            // Assert
            actualFirst.ShouldBeEquivalentTo(ExpectedStr);
            actualSecond.Should().BeNull();
        }

        [TestMethod]
        public void ReadNext_EmptySource_ShouldReturnNull()
        {
            // Arrange
            var target = new TextReader(new System.IO.StringReader(string.Empty), 100);

            // Act
            var actualFirst = target.ReadNext();
            var actualSecond = target.ReadNext();

            // Assert
            actualFirst.Should().BeNull();
            actualSecond.Should().BeNull();
        }

        [TestMethod]
        public void ReadNext_SourceDoNotspliitingByBatchSize_ShouldMergeWords()
        {
            // Arrange
            const string ExpectedStr = "Lorem ipsum dolor sit posuere";
            const int Batch = 10;

            var target = new TextReader(new System.IO.StringReader(ExpectedStr), Batch);

            var actual = new List<string>();

            // Act
            do
            {
                var result = target.ReadNext();
                if (result == null)
                {
                    break;
                }

                actual.Add(result);
            } while (true);

            // Assert
            actual.Should().HaveCount(3);
            actual[0].ShouldBeEquivalentTo("Lorem");
            actual[1].ShouldBeEquivalentTo("ipsum dolor");
            actual[2].ShouldBeEquivalentTo("sit posuere");
        }
    }
}
