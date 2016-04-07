using System.Collections.Generic;
using AngularCsharp.Tests._TestData.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AngularCsharp.Helpers;

namespace AngularCsharp.Tests.Values
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
            var key = "person.FirstName";
            var person = new Person() { FirstName = "Jim", LastName = "Blue" };
            var lookup = new Dictionary<string, object>();
            lookup.Add("person", person);

            // Act
            var result = sut.GetString(key, new System.Collections.ObjectModel.ReadOnlyDictionary<string, object>(lookup));

            // Assert
            Assert.AreEqual<string>(person.FirstName, result);
        }

        #endregion
    }
}