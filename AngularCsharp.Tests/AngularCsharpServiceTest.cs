using System;
using System.Collections.Generic;
using AngularCsharp.Tests._TestData.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AngularCsharp.Tests
{
    [TestClass]
    public class AngularCsharpServiceTest
    {
        #region AngularCsharpService_Constructor_Template_Empty

        [TestMethod]
        public void AngularCsharpService_Constructor_Template_Empty()
        {
            // Act
            AngularCsharpService sut = new AngularCsharpService(string.Empty);

            // Assert
            Assert.IsNotNull(sut);
            Assert.AreEqual<string>(string.Empty, sut.Template);
            Assert.AreEqual<int>(0, sut.Warnings.Count);
        }

        #endregion

        #region AngularCsharpService_Constructor_Template_String

        [TestMethod]
        public void AngularCsharpService_Constructor_Template_String()
        {
            // Act
            string template = "Hello!";
            AngularCsharpService sut = new AngularCsharpService(template);

            // Assert
            Assert.IsNotNull(sut);
            Assert.AreEqual<string>(template, sut.Template);
            Assert.AreEqual<int>(0, sut.Warnings.Count);
        }

        #endregion

        #region AngularCsharpService_Convert_NoInputs

        [TestMethod]
        public void AngularCsharpService_Convert_NoInputs()
        {
            // Assign
            string template = "Hello!";
            AngularCsharpService sut = new AngularCsharpService(template);

            // Act
            string result = sut.Convert(new { });
            
            // Log
            Console.WriteLine(result);

            // Assert
            Assert.AreEqual<string>(template, result);
            Assert.AreEqual<int>(0, sut.Warnings.Count);
        }

        [TestMethod]
        public void AngularCsharpService_Convert_NoInputs_TagsNotProcessed()
        {
            // Assign
            string template = "Hello {{firstName}}!";
            AngularCsharpService sut = new AngularCsharpService(template);

            // Act
            string result = sut.Convert(new { });
            
            // Log
            Console.WriteLine(result);

            // Assert
            Assert.AreEqual<string>(template, result);
            Assert.AreEqual<int>(1, sut.Warnings.Count);
        }

        #endregion

        #region AngularCsharpService_Convert_Inputs

        [TestMethod]
        public void AngularCsharpService_Convert_Inputs_TagProcessed()
        {
            // Assign
            string firstName = "Jim";
            string template = "Hello {{firstName}}!";
            string resultShould = "Hello " + firstName + "!";
            AngularCsharpService sut = new AngularCsharpService(template);

            // Act
            string result = sut.Convert(new { firstName });
            
            // Log
            Console.WriteLine(result);

            // Assert
            Assert.AreEqual<string>(resultShould, result);
            Assert.AreEqual<int>(0, sut.Warnings.Count);
        }

        [TestMethod]
        public void AngularCsharpService_Convert_Inputs_TagsProcessed()
        {
            // Assign
            string firstName = "Jim";
            string lastName = "Blue";
            string template = "Hello {{firstName}} {{lastName}}!";
            string resultShould = "Hello " + firstName + " " + lastName + "!";
            AngularCsharpService sut = new AngularCsharpService(template);

            // Act
            string result = sut.Convert(new { firstName, lastName });
            
            // Log
            Console.WriteLine(result);

            // Assert
            Assert.AreEqual<string>(resultShould, result);
            Assert.AreEqual<int>(0, sut.Warnings.Count);
        }

        [TestMethod]
        public void AngularCsharpService_Convert_Inputs_Objects_TagsProcessed()
        {
            // Assign
            Person person = new Person() { FirstName = "Jim", LastName = "Blue" };
            person.Email = new Email() { EmailAddress = "harry@ardimedia.com" };
            List<string> calls = new List<string>() { "a", "b", "c" };
            string template = "Hello {{person.FirstName}} {{person.LastName}}!";
            string resultShould = "Hello " + person.FirstName + " " + person.LastName + "!";
            AngularCsharpService sut = new AngularCsharpService(template);

            // Act
            //string result = sut.Convert(new { person });
            string result = sut.Convert(new { person, resultShould, calls });
            
            // Log
            Console.WriteLine(result);

            // Assert
            Assert.AreEqual<string>(resultShould, result);
            Assert.AreEqual<int>(0, sut.Warnings.Count);
        }

        #endregion
    }
}
