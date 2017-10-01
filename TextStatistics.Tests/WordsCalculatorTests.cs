using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TextStatistics.Tests
{
    [TestClass]
    public class WordsCalculatorTests
    {
        [TestMethod]
        public void Calc_CaseSensitive_ShouldBeCaseSensitive()
        {
            // Arrange
            var source = new[]
            {
                "Lorem", "ipsum", "dolor", "sit", "posuere",
                "lorem", "ipSum", "dOlor", "SIT", "posuerE"
            };

            var target = new WordsCalculator();

            var wordsComparer = StringComparer.InvariantCulture;

            // Act
            var actual = target.Calculate(source, wordsComparer);

            // Assert
            actual.ShouldBeEquivalentTo(new Dictionary<string, uint>
            {
                {"Lorem", 1},
                {"ipsum", 1},
                {"dolor", 1},
                {"sit", 1},
                {"posuere", 1},
                {"lorem", 1},
                {"ipSum", 1},
                {"dOlor", 1},
                {"SIT", 1},
                {"posuerE", 1}
            });
        }

        [TestMethod]
        public void Calc_CaseInsensitive_ShouldBeCaseInsensitive()
        {
            // Arrange
            var source = new[]
            {
                "Lorem", "ipsum", "dolor", "sit", "posuere",
                "lorem", "ipSum", "dOlor", "SIT", "posuerE"
            };

            var target = new WordsCalculator();

            var wordsComparer = StringComparer.InvariantCultureIgnoreCase;

            // Act
            var actual = target.Calculate(source, wordsComparer);

            // Assert
            actual.ShouldBeEquivalentTo(new Dictionary<string, uint>
            {
                {"Lorem", 2},
                {"ipsum", 2},
                {"dolor", 2},
                {"sit", 2},
                {"posuere", 2},
            });
        }

        [TestMethod]
        public void Calc_CaseInsensitiveCyrillic_ShouldBeCaseInsensitive()
        {
            // Arrange
            var source = new[]
            {
                "Что", "что"
            };

            var target = new WordsCalculator();

            var wordsComparer = StringComparer.InvariantCultureIgnoreCase;

            // Act
            var actual = target.Calculate(source, wordsComparer);

            // Assert
            actual.ShouldBeEquivalentTo(new Dictionary<string, uint>
            {
                {"Что", 2}
            });
        }
    }
}
