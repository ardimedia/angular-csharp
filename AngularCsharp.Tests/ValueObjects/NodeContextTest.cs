using AngularCsharp.Helpers;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AngularCsharp.ValueObjects.Tests.ValueObjects
{
    [TestClass()]
    public class NodeContextTest
    {
        [TestMethod()]
        public void ValueObjects_NodeContext_Constructor()
        {
            // Assign
            Dictionary <string, object> variables = new Dictionary<string, object>() { { "name", "value" } };
            HtmlNode node = (new HtmlDocument()).CreateElement("div");
            HtmlDocument targetDocument = new HtmlDocument();
            Dependencies dependencies = new Dependencies();
            TemplateEngine templateEngine = new TemplateEngine();

            // Act
            var sut = new NodeContext(variables, node, targetDocument, dependencies, templateEngine);

            // Assert
            Assert.AreEqual(variables.Count, sut.CurrentVariables.Count, "Variables dictionary is not equal");
            Assert.AreEqual(variables["name"], sut.CurrentVariables["name"], "Variables dictionary is not equal");
            Assert.AreSame(node, sut.CurrentNode, "CurrentNode is wrong");
            Assert.AreSame(targetDocument, sut.TargetDocument, "TargetDocument is wrong");
            Assert.AreSame(dependencies, sut.Dependencies, "Dependencies is wrong");
        }

        [TestMethod()]
        public void ValueObjects_NodeContext_Constructor_ChangeContext_Shorthand()
        {
            // Assign
            Dictionary<string, object> variables = new Dictionary<string, object>();
            HtmlNode node = (new HtmlDocument()).CreateElement("div");
            HtmlNode newNode = (new HtmlDocument()).CreateElement("p");
            NodeContext sut = new NodeContext(variables, node, new HtmlDocument(), new Dependencies(), new TemplateEngine());

            // Act
            var changedSut = sut.ChangeContext(newNode);

            // Assert
            Assert.AreSame(sut.CurrentNode, node);
            Assert.AreSame(changedSut.CurrentNode, newNode);
            Assert.AreNotSame(sut, changedSut);
        }

        [TestMethod()]
        public void ValueObjects_NodeContext_Constructor_ChangeContext_AdditionalVariables()
        {
            // Assign
            var variables = new Dictionary<string, object>() { { "name", "test" }, { "old", "hello" } };
            var node = (new HtmlDocument()).CreateElement("div");
            var newNode = (new HtmlDocument()).CreateElement("p");
            var sut = new NodeContext(variables, node, new HtmlDocument(), new Dependencies(), new TemplateEngine());
            var additionalVariables = new Dictionary<string, object>() { { "name", "value" }, { "new", "test" } };

            // Act
            var changedSut = sut.ChangeContext(additionalVariables, newNode);

            // Assert
            Assert.AreSame(sut.CurrentNode, node, "CurrentNode must not change in old instance");
            Assert.AreSame(changedSut.CurrentNode, newNode, "CurrentNode in new instance is wrong");

            Assert.AreNotSame(sut.CurrentVariables, variables, "CurrentVariables in old instance must not change");
            Assert.AreEqual(3, changedSut.CurrentVariables.Count, "Number of variables in new instance is wrong");
            Assert.AreEqual(variables["old"], changedSut.CurrentVariables["old"], "'old' variable is missing or wrong");
            Assert.AreEqual(additionalVariables["name"], changedSut.CurrentVariables["name"], "'name' variable must be updated");
            Assert.AreEqual(additionalVariables["new"], changedSut.CurrentVariables["new"], "'new' variable is missing or wrong");

            Assert.AreNotSame(sut, changedSut, "new instance must not be the same as old instance");
        }
    }
}