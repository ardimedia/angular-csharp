using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AngularCsharp;
using AngularCsharp.Tests;
using AngularCsharp.Tests._TestData.Domain;
using System.Collections.Generic;

namespace AngularCsharp.Tests
{
    [TestClass]
    public class AngularServiceTest
    {
        #region AngularService_Constructor_Template_Empty

        [TestMethod]
        public void AngularService_Constructor_Template_Empty()
        {
            // Act
            AngularService sut = new AngularService(string.Empty);

            // Assert
            Assert.IsNotNull(sut);
            Assert.IsFalse(sut.Logger.HasWarnings);
        }

        #endregion

        #region AngularService_Constructor_Template_String

        [TestMethod]
        public void AngularService_Constructor()
        {
            // Act
            string template = "Hello!";
            AngularService sut = new AngularService(template);

            // Assert
            Assert.IsNotNull(sut);
            Assert.IsFalse(sut.Logger.HasWarnings);
        }

        #endregion

        #region AngularService_Render_NoInputs

        [TestMethod]
        public void AngularService_Render_NoInputs()
        {
            // Assign
            string template = "Hello!";
            AngularService sut = new AngularService(template);

            // Act
            string result = sut.Render(new { });

            // Log
            Console.WriteLine(result);

            // Assert
            Assert.AreEqual<string>(template, result);
            Assert.IsFalse(sut.Logger.HasWarnings);
        }

        [TestMethod]
        public void AngularService_Render_NoInputs_TagsNotProcessed()
        {
            // Assign
            string template = "Hello {{firstName}}!";
            AngularService sut = new AngularService(template);

            // Act
            string result = sut.Render(new { });

            // Log
            Console.WriteLine(result);

            // Assert
            Assert.AreEqual<string>(template, result);
            Assert.AreEqual(1, sut.Logger.AllWarnings.Length);
        }

        #endregion

        #region AngularService_Render_Inputs

        [TestMethod]
        public void AngularService_Render_Inputs_TagProcessed()
        {
            // Assign
            string firstName = "Jim";
            string template = "Hello {{firstName}}!";
            string resultShould = "Hello " + firstName + "!";
            AngularService sut = new AngularService(template);

            // Act
            string result = sut.Render(new { firstName });

            // Assert
            Assert.AreEqual<string>(resultShould, result);
            Assert.IsFalse(sut.Logger.HasWarnings);
        }

        [TestMethod]
        public void AngularService_Render_Inputs_TagsProcessed()
        {
            // Assign
            string firstName = "Jim";
            string lastName = "Blue";
            string template = "Hello {{firstName}} {{lastName}}!";
            string resultShould = "Hello " + firstName + " " + lastName + "!";
            AngularService sut = new AngularService(template);

            // Act
            string result = sut.Render(new { firstName, lastName });

            // Log
            Console.WriteLine(result);

            // Assert
            Assert.AreEqual<string>(resultShould, result);
            Assert.IsFalse(sut.Logger.HasWarnings);
        }

        [TestMethod]
        public void AngularService_Render_Inputs_Objects_TagsProcessed()
        {
            // Assign
            Person person = new Person() { FirstName = "Jim", LastName = "Blue" };
            person.Email = new Email() { EmailAddress = "harry@ardimedia.com" };
            List<string> calls = new List<string>() { "a", "b", "c" };
            string template = "Hello {{person.FirstName}} {{person.LastName}}!";
            string resultShould = "Hello " + person.FirstName + " " + person.LastName + "!";
            AngularService sut = new AngularService(template);

            // Act
            string result = sut.Render(new { person, resultShould, calls });

            // Log
            Console.WriteLine(result);

            // Assert
            Assert.AreEqual<string>(resultShould, result);
            Assert.IsFalse(sut.Logger.HasWarnings);
        }

        #endregion

        [TestMethod]
        public void AngularService_IfProcessor_True()
        {
            // Assign
            var template = "<p *ngif=\"customer\">{{customer.salutation}}</p><hr>";
            var sut = new AngularService(template);
            var model = new { customer = new { salutation = "Hallo Herbert!" }, items = new[] { new { number = "0001", title = "Test", status = "offen" } } };

            // Act
            var result = sut.Render(model);

            // Assert
            Assert.AreEqual("<p>" + model.customer.salutation + "</p><hr>", result);
            Assert.IsFalse(sut.Logger.HasWarnings);
        }

        [TestMethod]
        public void AngularService_IfProcessor_False()
        {
            // Assign
            var template = "<p *ngif=\"customer\">{{customer.salutation}}</p><hr>";
            var sut = new AngularService(template);
            var model = new { customer = (object)null, items = new[] { new { number = "0001", title = "Test", status = "offen" } } };

            // Act
            var result = sut.Render(model);

            // Assert
            Assert.AreEqual("<hr>", result);
            Assert.IsFalse(sut.Logger.HasWarnings);
        }

        [TestMethod]
        public void AngularService_IfProcessor_Not()
        {
            // Assign
            var template = "<p *ngif=\"!customer\">Hallo!</p><hr>";
            var sut = new AngularService(template);
            var model = new { customer = (object)null, items = new[] { new { number = "0001", title = "Test", status = "offen" } } };

            // Act
            var result = sut.Render(model);

            // Assert
            Assert.AreEqual("<p>Hallo!</p><hr>", result);
            Assert.IsFalse(sut.Logger.HasWarnings);
        }

        [TestMethod]
        public void AngularService_ComplexTemplate()
        {
            // Assign
            var template = System.IO.File.ReadAllText(@"..\..\!TestData\angular2\complex-template.html");
            var model = new { customer = new { salutation = "Hallo Herbert!" }, items = new[] { new { number = "0001", title = "Test", status = "offen" } } };
            var sut = new AngularService(template);

            // Act
            var result = sut.Render(model);

            // Assert
            Assert.AreEqual(System.IO.File.ReadAllText(@"..\..\!TestData\angular2\complex-template-final.html"), result);
        }
    }
}
