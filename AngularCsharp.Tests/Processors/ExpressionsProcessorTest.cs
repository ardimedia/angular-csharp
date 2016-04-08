using AngularCsharp.Helpers;
using AngularCsharp.ValueObjects;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AngularCsharp.Processors.Tests.Processors
{
    [TestClass()]
    public class ExpressionsProcessorTest
    {
        [TestMethod()]
        public void Processors_ExpressionProcessor_ProcessNode_ElementNode()
        {
            // Assign
            ExpressionsProcessor toc = new ExpressionsProcessor();
            HtmlDocument htmlDocument = GetHtmlDocument("<p>");
            HtmlNode htmlNode = htmlDocument.DocumentNode.FirstChild;
            NodeContext nodeContext = GetNodeContextInstance(htmlNode);

            // Act
            ProcessResults results = toc.ProcessNode(nodeContext);

            // Assert results
            Assert.IsNotNull(results);

            // Assert results.OutputNodes
            Assert.AreEqual(1, results.OutputNodes.Length);
            Assert.AreEqual(htmlNode.InnerText, results.OutputNodes[0].InnerText);

            // Assert other results properties
            Assert.IsFalse(results.SkipChildNodes);
            Assert.IsFalse(results.StopProcessing);
        }

        [TestMethod()]
        public void Processors_ExpressionProcessor_ProcessNode_ElementNode_StaticText()
        {
            // Assign
            ExpressionsProcessor toc = new ExpressionsProcessor();
            HtmlDocument htmlDocument = GetHtmlDocument("Hello World!");
            HtmlNode htmlNode = htmlDocument.DocumentNode.FirstChild;
            NodeContext nodeContext = GetNodeContextInstance(htmlNode);

            // Act
            ProcessResults results = toc.ProcessNode(nodeContext);

            // Assert results
            Assert.IsNotNull(results);

            // Assert results.OutputNodes
            Assert.AreEqual(1, results.OutputNodes.Length);
            Assert.AreEqual(htmlNode.InnerText, results.OutputNodes[0].InnerText);

            // Assert other results properties
            Assert.IsFalse(results.SkipChildNodes);
            Assert.IsFalse(results.StopProcessing);
        }

        [TestMethod()]
        public void Processors_ExpressionProcessor_ProcessNode_ElementNode_Variables()
        {
            // Assign
            ExpressionsProcessor toc = new ExpressionsProcessor();
            HtmlDocument htmlDocument = GetHtmlDocument("Hello {{firstName}} {{lastName}}!");
            HtmlNode htmlNode = htmlDocument.DocumentNode.FirstChild;
            Dictionary<string, object> variables = new Dictionary<string, object>() { { "firstName", "Max" }, { "lastName", "Mustermann" } };
            NodeContext nodeContext = GetNodeContextInstance(htmlNode, variables);
            string expected = "Hello Max Mustermann!";

            // Act
            ProcessResults results = toc.ProcessNode(nodeContext);

            // Assert results
            Assert.IsNotNull(results);

            // Assert results.OutputNodes
            Assert.AreEqual(1, results.OutputNodes.Length);
            Assert.AreEqual(expected, results.OutputNodes[0].InnerText);

            // Assert other results properties
            Assert.IsFalse(results.SkipChildNodes);
            Assert.IsFalse(results.StopProcessing);
        }

        private HtmlDocument GetHtmlDocument(string html)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            return htmlDocument;
        }

        private NodeContext GetNodeContextInstance(HtmlNode node, Dictionary<string,object> variables = null)
        {
            if (variables == null)
            {
                variables = new Dictionary<string, object>();
            }

            return new NodeContext(variables, node, new HtmlDocument(), new Dependencies(), new TemplateEngine());
        }
    }
}