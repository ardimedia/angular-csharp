using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AngularCsharp.Helpers;
using AngularCsharp.Exceptions;
using System.Collections.Generic;

namespace AngularCsharp.Tests.Helpers
{
    [TestClass]
    public class ExpressionResolverTest
    {
        [TestMethod]
        public void Helpers_ExpressionResolver_Constructor()
        {
            // Assign
            var valueFinderMock = new Mock<ValueFinder>();

            // Act
            var sut = new ExpressionResolver(valueFinderMock.Object);

            // Assert
            Assert.IsNotNull(sut);
        }

        [TestMethod]
        public void Helpers_ExpressionResolver_IsTrue_True()
        {
            // Assign
            var variables = GetVariables();
            var valueFinderMock = new Mock<ValueFinder>();
            valueFinderMock.Setup(mock => mock.GetObject("customerName", variables)).Returns(variables["customerName"]);
            var sut = new ExpressionResolver(valueFinderMock.Object);

            // Act
            var result = sut.IsTrue("customerName", variables);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Helpers_ExpressionResolver_IsTrue_ValueNotExist()
        {
            // Assign
            var variables = GetVariables();
            var valueFinderMock = new Mock<ValueFinder>();
            valueFinderMock.Setup(mock => mock.GetObject("order", variables)).Throws(new ValueNotFoundException());
            var sut = new ExpressionResolver(valueFinderMock.Object);

            // Act
            var result = sut.IsTrue("order", variables);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Helpers_ExpressionResolver_IsTrue_Not()
        {
            // Assign
            var variables = GetVariables();
            var valueFinderMock = new Mock<ValueFinder>();
            valueFinderMock.Setup(mock => mock.GetObject("customerName", variables)).Returns(variables["customerName"]);
            var sut = new ExpressionResolver(valueFinderMock.Object);

            // Act
            var result = sut.IsTrue("!customerName", variables);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Helpers_ExpressionResolver_IsFalse()
        {
            // Assign
            var variables = GetVariables();
            var valueFinderMock = new Mock<ValueFinder>();
            valueFinderMock.Setup(mock => mock.GetObject("hasOrders", variables)).Returns(false);
            var sut = new ExpressionResolver(valueFinderMock.Object);

            // Act
            var result = sut.IsTrue("hasOrders", variables);

            // Assert
            Assert.IsFalse(result);
        }

        private Dictionary<string, object> GetVariables()
        {
            return new Dictionary<string, object>()
            {
                { "customerName", "Herbert Scheffknecht" },
                { "customer", new { firstName = "Herbert", lastName = "Scheffknecht"} },
                { "hasOrders", false }
            };
        }
    }
}
