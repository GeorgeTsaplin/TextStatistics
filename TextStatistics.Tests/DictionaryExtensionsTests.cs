using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TextStatistics.Tests
{
    [TestClass]
    public class DictionaryExtensionsTests
    {
        [TestMethod]
        public void AddRange_OtherIsEmpty_DoNothing()
        {
            // Arrange
            var expected = new Dictionary<string, int>
            {
                { "one", 5 },
                { "two", 34 }
            };

            var target = new Dictionary<string, int>(expected);

            // Act
            DictionaryExtensions.AddRange(target, null, (lhs, rhs) => lhs + rhs);

            // Assert
            target.ShouldBeEquivalentTo(expected);
            
        }

        [TestMethod]
        public void AddRange_ThisIsEmpty_ShiuldMerge()
        {
            // Arrange
            var other = new Dictionary<string, int>
            {
                { "one", 5 },
                { "two", 34 }
            };

            var target = new Dictionary<string, int>();

            // Act
            DictionaryExtensions.AddRange(target, other, (lhs, rhs) => lhs + rhs);

            // Assert
            target.ShouldBeEquivalentTo(other);

        }

        [TestMethod]
        public void AddRange_OtherIsNotEmpty_ShouldAggregate()
        {
            // Arrange
            var target = new Dictionary<string, int>
            {
                { "one", 5 },
                { "two", 34 }
            };

            var other = new Dictionary<string, int>
            {
                { "two", 67 },
                { "three", 2 }
            };

            // Act
            DictionaryExtensions.AddRange(target, other, (lhs, rhs) => lhs + rhs);

            // Assert
            target.ShouldBeEquivalentTo(new Dictionary<string, int>
            {
                { "one", 5 },
                { "two", 34+67 },
                { "three", 2 }
            });

        }
    }
}
