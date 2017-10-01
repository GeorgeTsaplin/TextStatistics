using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TextStatistics.Tests
{
    [TestClass]
    public class TextSplitterTests
    {
        [TestMethod]
        public void SplitWords_HasPunctuation_ShouldReturnWithoutPunctuation()
        {
            // Arrange
            var targetStr = @"Lorem, ipsum!? dolor; sit: posuere.";
            var target = new TextSplitter();

            // Act
            var actual = target.SplitWords(targetStr);

            // Assert
            actual.ShouldBeEquivalentTo(new[] { "Lorem", "ipsum", "dolor", "sit", "posuere" });
        }

        [TestMethod]
        public void SplitWords_HasDoubleSpaces_ShouldReturnWithoutSpaces()
        {
            // Arrange
            var targetStr = @"Lorem ipsum  dolor   sit     posuere.";
            var target = new TextSplitter();

            // Act
            var actual = target.SplitWords(targetStr);

            // Assert
            actual.ShouldBeEquivalentTo(new[] { "Lorem", "ipsum", "dolor", "sit", "posuere" });
        }
    }
}
