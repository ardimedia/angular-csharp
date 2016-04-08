using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AngularCsharp.ValueObjects.Tests.ValueObjects
{
    [TestClass()]
    public class DependenciesTest
    {
        [TestMethod()]
        public void ValueObjects_Dependencies_Constructor_Defaults()
        {
            // Act
            var sut = new Dependencies();

            // Assert
            Assert.IsNotNull(sut.ExpressionResolver);
            Assert.IsNotNull(sut.Logger);
            Assert.IsNotNull(sut.ValueFinder);
        }
    }
}
