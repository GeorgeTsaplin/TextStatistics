using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TextStatistics.Tests
{
    [TestClass]
    public class TextStatisticsCalculatorTests
    {
        [TestMethod]
        public void Calc_LoremIpsum_ShouldReturnCorrect()
        {
            // Arrange
            var source = @"Нет никого, кто любил бы боль саму по себе, кто искал бы её и кто хотел бы иметь её просто потому, что это боль";
            var target = new TextStatisticsCalculator(new TextSplitter(), new WordsCalculator(), 3);

            // Act
            var actual = target.CalcStatistics(
                new TextReader(new StringReader(source), 15),
                StringComparer.InvariantCultureIgnoreCase,
                TimeSpan.FromSeconds(1000));

            // Assert
            actual.ShouldBeEquivalentTo(new Dictionary<string, uint>
            {
                { "Нет", 1 },
                { "никого", 1 },
                { "кто", 3 },
                { "любил", 1 },
                { "бы", 3 },
                { "боль", 2 },
                { "саму", 1 },
                { "по", 1 },
                { "себе", 1 },
                { "искал", 1 },
                { "её", 2 },
                { "и", 1 },
                { "хотел", 1 },
                { "иметь", 1 },
                { "просто", 1 },
                { "потому", 1 },
                { "что", 1 },
                { "это", 1 }
            });
        }
    }
}
