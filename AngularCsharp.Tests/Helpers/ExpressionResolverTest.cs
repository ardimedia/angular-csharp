using System.Collections.Generic;
using AngularCSharp.Exceptions;
using AngularCSharp.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AngularCSharp.Tests.Helpers
{
    [TestClass]
    public class ExpressionResolverTest
    {
        #region Helpers_ExpressionResolver_Constructor

        [TestMethod]
        public void Helpers_ExpressionResolver_Constructor()
        {
            // Assign
            Mock<ValueFinder> valueFinderMock = new Mock<ValueFinder>();
            Logger logger = new Logger();

            // Act
            var sut = new ExpressionResolver(valueFinderMock.Object, logger);

            // Assert
            Assert.IsNotNull(sut);
        }

        #endregion

        #region Helpers_ExpressionResolver_IsTrue

        [TestMethod]
        public void Helpers_ExpressionResolver_IsTrue_True()
        {
            // Assign
            Dictionary<string,object> variables = GetVariables();
            Mock<ValueFinder> valueFinderMock = new Mock<ValueFinder>();
            valueFinderMock.Setup(mock => mock.GetObject("customerName", variables)).Returns(variables["customerName"]);
            Logger logger = new Logger();
            var sut = new ExpressionResolver(valueFinderMock.Object, logger);

            // Act
            var result = sut.IsTrue("customerName", variables);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Helpers_ExpressionResolver_IsTrue_False()
        {
            // Assign
            Dictionary<string, object> variables = GetVariables();
            Mock<ValueFinder> valueFinderMock = new Mock<ValueFinder>();
            valueFinderMock.Setup(mock => mock.GetObject("hasOrders", variables)).Returns(false);
            Logger logger = new Logger();
            var sut = new ExpressionResolver(valueFinderMock.Object, logger);

            // Act
            var result = sut.IsTrue("hasOrders", variables);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Helpers_ExpressionResolver_IsTrue_ValueNotExist()
        {
            // Assign
            Dictionary<string,object> variables = GetVariables();
            Mock<ValueFinder> valueFinderMock = new Mock<ValueFinder>();
            valueFinderMock.Setup(mock => mock.GetObject("order", variables)).Throws(new ValueNotFoundException());
            Logger logger = new Logger();
            var sut = new ExpressionResolver(valueFinderMock.Object, logger);

            // Act
            var result = sut.IsTrue("order", variables);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(1, logger.Warnings.Length);
        }

        [TestMethod]
        public void Helpers_ExpressionResolver_IsTrue_Not()
        {
            // Assign
            Dictionary<string, object> variables = GetVariables();
            Mock<ValueFinder> valueFinderMock = new Mock<ValueFinder>();
            valueFinderMock.Setup(mock => mock.GetObject("customerName", variables)).Returns(variables["customerName"]);
            Logger logger = new Logger();
            var sut = new ExpressionResolver(valueFinderMock.Object, logger);

            // Act
            var result = sut.IsTrue("!customerName", variables);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region Helpers_ExpressionResolver_Variables

        private Dictionary<string, object> GetVariables()
        {
            return new Dictionary<string, object>()
            {
                { "customerName", "Herbert Scheffknecht" },
                { "customer", new { firstName = "Herbert", lastName = "Scheffknecht"} },
                { "hasOrders", false }
            };
        }

        #endregion
    }
}
