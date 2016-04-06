using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AngularCsharp;
using AngularCsharp.Tests;

namespace AngularCsharp.Tests
{
    [TestClass]
    public class AngularServiceTest
    {
        [TestMethod]
        public void TestComplexTemplate()
        {
            // Assign
            var template = System.IO.File.ReadAllText(@"..\..\!TestData\angular2\complex-template.html");
            var service = new AngularService(template);
            var model = new { customer = new { salutation = "Hallo Herbert!" }, items = new[] { new { number = "0001", title = "Test", status = "offen" } } };

            // Act
            var result = service.Render(model);
            
            // Assert
            //TODO: Assert result
        }
    }
}
