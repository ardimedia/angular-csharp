using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using AngularCSharp.Tests._TestData.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AngularCSharp.Tests
{
    [TestClass]
    public class AngularServiceTest
    {
        #region AngularService_Constructor

        [TestMethod]
        public void AngularService_Constructor_Empty()
        {
            // Act
            AngularService sut = new AngularService(string.Empty);

            // Assert
            Assert.IsNotNull(sut);
            Assert.IsFalse(sut.Logger.HasWarnings);
        }

        [TestMethod]
        public void AngularService_Constructor_String()
        {
            // Act
            string template = "Hello!";
            AngularService sut = new AngularService(template);

            // Assert
            Assert.IsNotNull(sut);
            Assert.IsFalse(sut.Logger.HasWarnings);
        }

        [TestMethod]
        public void AngularService_Constructor_Html()
        {
            // Act
            string template = "<p>Hello!</p>";
            AngularService sut = new AngularService(template);

            // Assert
            Assert.IsNotNull(sut);
            Assert.IsFalse(sut.Logger.HasWarnings);
        }

        [TestMethod]
        public void AngularService_Constructor_Html_Invalid()
        {
            // Act
            string template = "<h1>Hello!</p>";
            AngularService sut = new AngularService(template);

            // Log
            AngularServiceDump(sut);

            // Assert
            Assert.IsNotNull(sut);
            Assert.IsTrue(sut.Logger.HasWarnings);
            Assert.AreEqual(1, sut.Logger.Warnings.Length);
        }

        #endregion

        #region AngularService_Render_Input_Empty

        [TestMethod]
        public void AngularService_Render_Input_Empty_NoTagToProcess()
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
        public void AngularService_Render_Input_Empty_TagNotProcessed()
        {
            // Assign
            string template = "Hello {{firstName}}!";
            AngularService sut = new AngularService(template);

            // Act
            string result = sut.Render(new { });

            // Log
            AngularServiceDump(sut);

            // Assert
            Assert.IsTrue(sut.Logger.HasWarnings);
            Assert.AreEqual(1, sut.Logger.Warnings.Length);
        }

        #endregion

        #region AngularService_Render_Input_Property

        [TestMethod]
        public void AngularService_Render_Input_Property_1()
        {
            // Assign
            string template = "Hello {{firstName}}!";
            string firstName = "Jim";
            string expected = $"Hello {firstName}!";
            AngularService sut = new AngularService(template);

            // Act
            string result = sut.Render(new { firstName });

            // Assert
            Assert.AreEqual<string>(expected, result);
            Assert.IsFalse(sut.Logger.HasWarnings);
        }

        [TestMethod]
        public void AngularService_Render_Input_Property_2()
        {
            // Assign
            string template = "Hello {{firstName}} {{lastName}}!";
            string firstName = "Jim";
            string lastName = "Blue";
            string expected = $"Hello {firstName } {lastName}!";
            AngularService sut = new AngularService(template);

            // Act
            string result = sut.Render(new { firstName, lastName });

            // Log
            Console.WriteLine(result);

            // Assert
            Assert.AreEqual<string>(expected, result);
            Assert.IsFalse(sut.Logger.HasWarnings);
        }

        [TestMethod]
        public void AngularService_Render_Input_Property_TagNotProcessed()
        {
            // Assign
            string template = "Hello {{firstName1}} {{lastName1}}!";
            string firstName = "Jim";
            string lastName = "Blue";
            AngularService sut = new AngularService(template);

            // Act
            string result = sut.Render(new { firstName, lastName });

            // Log
            AngularServiceDump(sut);

            // Assert
            Assert.IsTrue(sut.Logger.HasWarnings);
            Assert.AreEqual<int>(2, sut.Logger.Warnings.Count());
        }

        #endregion

        #region AngularService_Render_Input_Class

        [TestMethod]
        public void AngularService_Render_Input_Class_1()
        {
            // Assign
            string template = "Hello {{person.FirstName}} {{person.LastName}}!";
            Person person = new Person() { FirstName = "Jim", LastName = "Blue" };
            string expected = $"Hello {person.FirstName} {person.LastName}!";
            AngularService sut = new AngularService(template);

            // Act
            string result = sut.Render(new { person });

            // Log
            Console.WriteLine(result);

            // Assert
            Assert.AreEqual<string>(expected, result);
            Assert.IsFalse(sut.Logger.HasWarnings);
        }

        [TestMethod]
        public void AngularService_Render_Input_Class_TagNotProcessed()
        {
            // Assign
            string template = "Hello {{person.FirstName1}} {{person.LastName1}}!";
            Person person = new Person() { FirstName = "Jim", LastName = "Blue" };
            AngularService sut = new AngularService(template);

            // Act
            string result = sut.Render(new { person });

            // Log
            AngularServiceDump(sut);

            // Assert
            Assert.IsTrue(sut.Logger.HasWarnings);
            Assert.AreEqual(2, sut.Logger.Warnings.Length);
        }

        #endregion

        #region AngularService_Render_IfProcessor

        [TestMethod]
        public void AngularService_Render_IfProcessor_True()
        {
            // Assign
            var template = "<p *ngif=\"customer.isOK\">{{customer.salutation}}</p><hr>";
            var model = new { customer = new { salutation = "Hallo Herbert!", isOK = true } };
            var expected = $"<p>{model.customer.salutation}</p><hr>";
            var sut = new AngularService(template);

            // Act
            var result = sut.Render(model);

            // Assert
            Assert.AreEqual(expected, result);
            Assert.IsFalse(sut.Logger.HasWarnings);
        }

        [TestMethod]
        public void AngularService_Render_IfProcessor_False()
        {
            // Assign
            var template = "<p *ngif=\"customer.isOK\">{{customer.salutation}}</p><hr>";
            var sut = new AngularService(template);
            var expected = "<hr>";
            var model = new { customer = new { salutation = "Hallo Herbert!", isOK = false } };

            // Act
            var result = sut.Render(model);

            // Assert
            Assert.AreEqual(expected, result);
            Assert.IsFalse(sut.Logger.HasWarnings);
        }

        [TestMethod]
        public void AngularService_Render_IfProcessor_Null()
        {
            // Assign
            var template = "<p *ngif=\"customer\">Hallo Null!</p><hr>";
            var model = new { customer = (object) null };
            var expected = "<hr>";
            var sut = new AngularService(template);

            // Act
            var result = sut.Render(model);

            // Assert
            Assert.AreEqual(expected, result);
            Assert.IsFalse(sut.Logger.HasWarnings);
        }

        [TestMethod]
        public void AngularService_Render_IfProcessor_NotNull()
        {
            // Assign
            var template = "<p *ngif=\"!customer\">Hallo Null!</p><hr>";
            var model = new { customer = (object) null };
            var expected = "<p>Hallo Null!</p><hr>";
            var sut = new AngularService(template);

            // Act
            var result = sut.Render(model);

            // Assert
            Assert.AreEqual(expected, result);
            Assert.IsFalse(sut.Logger.HasWarnings);
        }

        #endregion

        #region AngularService_Render_ForProcessor

        [TestMethod]
        public void AngularService_Render_ForProcessor_Items_0()
        {
            // Assign
            var template = "<p *ngFor=\"#person of persons\">{{person.FirstName}}</p>";
            List<Person> persons = Person.GetRandoms(0);
            var model = new { persons };
            var expected = "";
            var sut = new AngularService(template);

            // Act
            var result = sut.Render(model);

            // Assert
            Assert.AreEqual(expected, result);
            Assert.IsFalse(sut.Logger.HasWarnings);
        }

        [TestMethod]
        public void AngularService_Render_ForProcessor_Items_1()
        {
            // Assign
            var template = "<p *ngFor=\"#person of persons\">{{person.FirstName}}</p>";

            List<Person> persons = Person.GetRandoms(1);
            var model = new { persons };
            var expected = "";
            foreach (var person in persons)
            {
                expected += $"<p>{person.FirstName}</p>";
            }

            var sut = new AngularService(template);

            // Act
            var result = sut.Render(model);

            // Assert
            Assert.AreEqual(expected, result);
            Assert.IsFalse(sut.Logger.HasWarnings);
        }

        [TestMethod]
        public void AngularService_Render_ForProcessor_Items_5()
        {
            // Assign
            var template = "<p *ngFor=\"#person of persons\">{{person.FirstName}}</p>";

            List<Person> persons = Person.GetRandoms(5);
            var model = new { persons };
            var expected = "";
            foreach (var person in persons)
            {
                expected += $"<p>{person.FirstName}</p>";
            }

            var sut = new AngularService(template);

            // Act
            var result = sut.Render(model);

            // Assert
            Assert.AreEqual(expected, result);
            Assert.IsFalse(sut.Logger.HasWarnings);
        }

        [TestMethod]
        public void AngularService_Render_ForProcessor_Items_1_Inner_Html()
        {
            // Assign
            var template = "<p *ngFor=\"#person of persons\"><div>{{person.FirstName}}</div></p>";

            List<Person> persons = Person.GetRandoms(1);
            var model = new { persons };
            var expected = "";
            foreach (var person in persons)
            {
                expected += $"<p><div>{person.FirstName}</div></p>";
            }

            var sut = new AngularService(template);

            // Act
            var result = sut.Render(model);

            // Assert
            Assert.AreEqual(expected, result);
            Assert.IsFalse(sut.Logger.HasWarnings);
        }

        #endregion

        #region AngularService_Render_TemplateProcessor

        [TestMethod]
        public void AngularService_Render_TemplateProcessor_WithFor_Process()
        {
            // Assign
            string template = "<template *ngFor=\"#person of persons\"><p>{{person.firstName}} {{person.lastName}}</p></template>";
            var model = new { persons = new[] { new { firstName = "Max", lastName = "Mustermann" } } };
            string expected = "<p>Max Mustermann</p>";
            AngularService sut = new AngularService(template);

            // Act
            string result = sut.Render(model);

            // Assert
            Assert.AreEqual(expected, result);
            Assert.IsFalse(sut.Logger.HasWarnings);
        }

        [TestMethod]
        public void AngularService_Render_TemplateProcessor_WithFor_Ignore()
        {
            // Assign
            string template = "<template><p *ngFor=\"#person of persons\">{{person.firstName}} {{person.lastName}}</p></template>";
            var model = new { persons = new[] { new { firstName = "Max", lastName = "Mustermann" } } };
            string expected = "<template><p>Max Mustermann</p></template>";
            AngularService sut = new AngularService(template);

            // Act
            string result = sut.Render(model);

            // Assert
            Assert.AreEqual(expected, result);
            Assert.IsFalse(sut.Logger.HasWarnings);
        }

        #endregion

        #region AngularService_Render_Integration_Tests

        [TestMethod]
        [DeploymentItem(@"!TestData\Html\template1.html", @"!TestData")]
        [DeploymentItem(@"!TestData\Html\template1.result.html", @"!TestData")]
        public void AngularService_Render_Integration_ComplexTemplate()
        {
            // Assign
            var template = System.IO.File.ReadAllText(@"!TestData\template1.html");
            var model = new
            {
                customer = new { salutation = "Hallo Herbert!" },
                items = new[] {
                new { number = "001", title = "TEST 1", status = "done" },
                new { number = "002", title = "TEST 2", status = "new" }
            }
            };
            string expected = System.IO.File.ReadAllText(@"!TestData\template1.result.html");
            var sut = new AngularService(template);

            // Act
            var result = sut.Render(model);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DeploymentItem(@"!TestData\Html\printApprovalOpenSalesRep.html", @"!TestData")]
        [DeploymentItem(@"!TestData\Html\printApprovalOpenSalesRep.result.txt", @"!TestData")]
        public void AngularService_Render_Integration_PrintApprovalOpenSalesRep()
        {
            // Assign
            Thread.CurrentThread.CurrentCulture = new CultureInfo("de-LI"); // Make sure date time are correctly formated (also on build server)

            string template = System.IO.File.ReadAllText(@"!TestData\printApprovalOpenSalesRep.html");
            var salesAgent = new { FullName = "Jim Blue" };
            var customerPrintApprovals = PrintApproval.GetValidItems(5);
            var salesRepPrintApprovals = PrintApproval.GetValidItems(5);
            var model = new { salesAgent, customerPrintApprovals, salesRepPrintApprovals };
            string expected = System.IO.File.ReadAllText(@"!TestData\printApprovalOpenSalesRep.result.txt");
            AngularService sut = new AngularService(template);

            // Act
            string result = sut.Render(model);

            // Log
            foreach (var warning in sut.Logger.Warnings)
            {
                Console.WriteLine($"Warning: {warning}");
            }

            // Assert
            #region Assert result (file content)

            string[] resultLines = result.Split(new String[] { Environment.NewLine }, StringSplitOptions.None);
            string[] expectedLines = expected.Split(new String[] { Environment.NewLine }, StringSplitOptions.None);
            for (int i = 0; i < resultLines.Count(); i++)
            {
                Assert.AreEqual<string>(expectedLines[i], resultLines[i], $"Error on line { i + 1 }");
            }
            Assert.AreEqual<int>(expectedLines.Count(), resultLines.Count(), "Line count must be the same.");

            #endregion
            Assert.AreEqual<int>(0, sut.Logger.Warnings.Count(), "No warnings expected. See warnings in [output].");
        }

        #endregion

        #region Private Methods

        private static void AngularServiceDump(AngularService angularService)
        {
            Console.WriteLine($"HasWarninigs: {angularService.Logger.HasWarnings}");

            foreach (var warning in angularService.Logger.Warnings)
            {
                Console.WriteLine($"Warnings: {warning}");
            }
        }

        #endregion
    }
}
