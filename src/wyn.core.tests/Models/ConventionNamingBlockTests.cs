using Microsoft.VisualStudio.TestTools.UnitTesting;
using wyn.core.Models;

namespace wyn.core.tests.Models
{
    [TestClass]
    public class ConventionNamingBlockTests
    {
        [DataTestMethod]
        [DataRow(@"^\w{2, 6}\Z","sin")]
        public void IsValid_ShouldReturnTrueWithoutErrors_OnValidProperties(string r, string d)
        {
            var sut = new ConventionNamingBlock()
            {
                Regex = r,
                Default = d
            };

            var result = sut.IsValid();

            Assert.IsTrue(result.Item1);
            Assert.AreEqual(0, result.Item2.Count);
        }

        [DataTestMethod]
        [DataRow("", null)]
        [DataRow(null,"")]
        [DataRow(null, null)]
        public void IsValid_ShouldReturnFalseWithErrors_OnInvalidProperties(string r, string d)
        {
            var sut = new ConventionNamingBlock()
            {
                Regex = r,
                Default = d
            };

            var result = sut.IsValid();

            Assert.IsTrue(!result.Item1);
            Assert.AreNotEqual(0, result.Item2.Count);

        }
    }
}
