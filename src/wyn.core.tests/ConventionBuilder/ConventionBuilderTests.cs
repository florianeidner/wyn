using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using wyn.core.Models;

namespace wyn.core.tests
{
    [TestClass]
    public class ConventionBuilderTests
    {
        [TestMethod]
        public void CreateFromString_ShouldCall_PipelineSteps()
        {
            // Arrange
            List<IConventionBuilderStep> steps = new();

            var firstStep = new Mock<IConventionBuilderStep>();
            var secondStep = new Mock<IConventionBuilderStep>();
            
            steps.Add(firstStep.Object);
            steps.Add(secondStep.Object);

            string convention = "someconvention";

            var sut = new ConventionBuilder(steps);
            
            // Act
            sut.CreateFromConventionString(convention);

            // Assert
            firstStep.Verify(s => s.Build(It.IsAny<WynConvention>()), Times.Once);
            secondStep.Verify(s => s.Build(It.IsAny<WynConvention>()), Times.Once);

        }

        [TestMethod]
        public void CreateFromString_ShouldCall_PipelineSteps_InOrder()
        {
            //Arrange

            int lastCaller = 0;
            const int firstCaller = 1;
            const int secondCaller = 2;

            List<IConventionBuilderStep> steps1 = new();
            List<IConventionBuilderStep> steps2 = new();

            var firstStep = new Mock<IConventionBuilderStep>();
            firstStep.Setup(
                s => s.Build(It.IsAny<WynConvention>())).Returns<WynConvention>((a) => {
                    lastCaller = firstCaller;
                    return a; });
            var secondStep = new Mock<IConventionBuilderStep>();
            secondStep.Setup(
                s => s.Build(It.IsAny<WynConvention>())).Returns<WynConvention>((a) => {
                    lastCaller = secondCaller;
                    return a;
                });


            steps1.Add(firstStep.Object);
            steps1.Add(secondStep.Object);

            steps2.Add(secondStep.Object);
            steps2.Add(firstStep.Object);

            string convention = "someconvention";

            var sut1 = new ConventionBuilder(steps1);
            var sut2 = new ConventionBuilder(steps2);

            // Act & Assert 1
            sut1.CreateFromConventionString(convention);
            Assert.AreEqual(secondCaller, lastCaller);

            // Reset
            lastCaller = 0;

            // Act & Assert 2
            sut2.CreateFromConventionString(convention);
            Assert.AreEqual(firstCaller, lastCaller);

        }

        [TestMethod]
        public void Constructor_CreatesBuilder_WithDefaultSteps()
        {
            var sut = new ConventionBuilder();

            var pipeline = sut.BuildSteps.GetEnumerator();

            pipeline.MoveNext();
            Assert.IsInstanceOfType(pipeline.Current, typeof(ConventionParser));

            pipeline.MoveNext();
            Assert.IsInstanceOfType(pipeline.Current, typeof(ConventionProviderBuilder));
        }
    }
}
