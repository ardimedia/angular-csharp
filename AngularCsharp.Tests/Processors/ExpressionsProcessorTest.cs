using System.Collections.Generic;
using AngularCSharp.Helpers;
using AngularCSharp.ValueObjects;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AngularCSharp.Processors.Tests.Processors
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
            ProcessResults results = new ProcessResults();
            results.OutputNodes.Add(htmlNode.CloneNode(false));

            // Act
            toc.ProcessNode(nodeContext, results);

            // Assert results
            Assert.IsNotNull(results);

            // Assert results.OutputNodes
            Assert.AreEqual(1, results.OutputNodes.Count);
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
            ProcessResults results = new ProcessResults();
            results.OutputNodes.Add(htmlNode.CloneNode(false));

            // Act
            toc.ProcessNode(nodeContext, results);

            // Assert results
            Assert.IsNotNull(results);

            // Assert results.OutputNodes
            Assert.AreEqual(1, results.OutputNodes.Count);
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
            ProcessResults results = new ProcessResults();
            results.OutputNodes.Add(htmlNode.CloneNode(false));

            // Act
            toc.ProcessNode(nodeContext, results);

            // Assert results
            Assert.IsNotNull(results);

            // Assert results.OutputNodes
            Assert.AreEqual(1, results.OutputNodes.Count);
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

            return new NodeContext(variables, node, new Dependencies(), new TemplateEngine());
        }
    }
}