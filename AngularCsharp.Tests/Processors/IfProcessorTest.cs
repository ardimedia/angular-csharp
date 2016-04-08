using AngularCsharp.Helpers;
using AngularCsharp.ValueObjects;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;

namespace AngularCsharp.Processors.Tests.Processors
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

            // Act
            ProcessResults results = toc.ProcessNode(nodeContext);

            // Assert results
            Assert.IsNotNull(results);
            Assert.IsNull(results.OutputNodes);
            Assert.IsFalse(results.SkipChildNodes);
            Assert.IsFalse(results.StopProcessing);
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

            // Act
            ProcessResults results = toc.ProcessNode(nodeContext);

            // Assert results
            Assert.IsNotNull(results);
            Assert.IsNotNull(results.OutputNodes);
            Assert.AreEqual(1, results.OutputNodes.Length);
            Assert.IsFalse(results.SkipChildNodes);
            Assert.IsTrue(results.StopProcessing);
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

            // Act
            ProcessResults results = toc.ProcessNode(nodeContext);

            // Assert results
            Assert.IsNotNull(results);
            Assert.IsNotNull(results.OutputNodes);
            Assert.AreEqual(0, results.OutputNodes.Length);
            Assert.IsTrue(results.SkipChildNodes);
            Assert.IsTrue(results.StopProcessing);
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