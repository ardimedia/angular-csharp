using System.Collections.Generic;
using AngularCSharp.Helpers;
using AngularCSharp.ValueObjects;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AngularCSharp.Processors.Tests.Processors
{
    [TestClass()]
    public class IfProcessorTest
    {
        [TestMethod()]
        public void Processors_ExpressionProcessor_ProcessNode_Ignore()
        {
            // Assign
            IfProcessor toc = new IfProcessor();
            HtmlDocument htmlDocument = GetHtmlDocument("<p>Hallo</p>");
            HtmlNode htmlNode = htmlDocument.DocumentNode.FirstChild;
            NodeContext nodeContext = GetNodeContextInstance(htmlNode);
            ProcessResults results = new ProcessResults();
            results.OutputNodes.Add(htmlNode.CloneNode(false));

            // Act
            toc.ProcessNode(nodeContext, results);

            // Assert results
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.OutputNodes.Count);
            Assert.IsFalse(results.SkipChildNodes);
        }

        [TestMethod()]
        public void Processors_ExpressionProcessor_ProcessNode_True()
        {
            // Assign
            IfProcessor toc = new IfProcessor();
            HtmlDocument htmlDocument = GetHtmlDocument("<p *ngif=\"customer\">Hallo</p>");
            HtmlNode htmlNode = htmlDocument.DocumentNode.FirstChild;
            Mock<ExpressionResolver> expressionResolverMock = new Mock<ExpressionResolver>(new Mock<ValueFinder>().Object);
            Dictionary<string, object> variables = new Dictionary<string, object>() { { "customer", new { number = "2000" } } };
            expressionResolverMock.Setup(mock => mock.IsTrue("customer", variables)).Returns(true);
            NodeContext nodeContext = GetNodeContextInstance(htmlNode, variables, expressionResolverMock.Object);
            ProcessResults results = new ProcessResults();
            results.OutputNodes.Add(htmlNode.CloneNode(false));

            // Act
            toc.ProcessNode(nodeContext, results);

            // Assert results
            Assert.IsNotNull(results);
            Assert.IsNotNull(results.OutputNodes);
            Assert.AreEqual(1, results.OutputNodes.Count);
            Assert.IsFalse(results.SkipChildNodes);
        }

        [TestMethod()]
        public void Processors_ExpressionProcessor_ProcessNode_False()
        {
            // Assign
            IfProcessor toc = new IfProcessor();
            HtmlDocument htmlDocument = GetHtmlDocument("<p *ngif=\"customer\">Hallo</p>");
            HtmlNode htmlNode = htmlDocument.DocumentNode.FirstChild;
            Mock<ExpressionResolver> expressionResolverMock = new Mock<ExpressionResolver>(new Mock<ValueFinder>().Object);
            Dictionary<string, object> variables = new Dictionary<string, object>();
            expressionResolverMock.Setup(mock => mock.IsTrue("customer", variables)).Returns(false);
            NodeContext nodeContext = GetNodeContextInstance(htmlNode, variables, expressionResolverMock.Object);
            ProcessResults results = new ProcessResults();
            results.OutputNodes.Add(htmlNode.CloneNode(false));

            // Act
            toc.ProcessNode(nodeContext, results);

            // Assert results
            Assert.IsNotNull(results);
            Assert.IsNotNull(results.OutputNodes);
            Assert.AreEqual(0, results.OutputNodes.Count);
            Assert.IsTrue(results.SkipChildNodes);
        }

        private HtmlDocument GetHtmlDocument(string html)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            return htmlDocument;
        }

        private NodeContext GetNodeContextInstance(HtmlNode node, Dictionary<string, object> variables = null, ExpressionResolver expressionResolver = null)
        {
            if (variables == null)
            {
                variables = new Dictionary<string, object>();
            }

            Dependencies dependencies = new Dependencies();

            if (expressionResolver != null)
            {
                dependencies.ExpressionResolver = expressionResolver;
            }

            return new NodeContext(variables, node, new HtmlDocument(), dependencies, new TemplateEngine());
        }
    }
}