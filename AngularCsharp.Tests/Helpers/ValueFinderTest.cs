using System.Collections.Generic;
using AngularCsharp.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AngularCsharp.Tests.Helpers
{
    [TestClass]
    public class ValueFinderTest
    {
        #region ValueFinder_GetString

        [TestMethod]
        public void ValueFinder_GetString()
        {
            // Assign
            var sut = new ValueFinder();
            var order = new { number = "1000", customer = new { number = "20000", name = new { firstName = "Jim", lastName = "Blue" } } };
            var dictionary = new Dictionary<string, object>() { { "number", order.number }, { "customer", order.customer } };
            var lookup = new System.Collections.ObjectModel.ReadOnlyDictionary<string, object>(dictionary);

            // Act
            var result1 = sut.GetString("number", lookup);
            var result2 = sut.GetString("customer.number", lookup);
            var result3 = sut.GetString("customer.name.firstName", lookup);

            // Assert
            Assert.AreEqual(order.number, result1);
            Assert.AreEqual(order.customer.number, result2);
            Assert.AreEqual(order.customer.name.firstName, result3);
        }

        #endregion
    }
}