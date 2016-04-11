using System;
using System.Collections.Generic;
using AngularCSharp.Helpers;
using AngularCSharp.ValueObjects;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AngularCSharp.Processors.Tests.Processors
{
    [TestClass]
    public class ForProcessorTest
    {
        #region Processors_ForProcessor_Invalid_Syntax

        [TestMethod]
        public void Processors_ForProcessor_Invalid_Syntax_Empty()
        {
            // Assign
            ForProcessor sut = new ForProcessor();
            HtmlDocument htmlDocument = GetHtmlDocument("<p *ngFor=\"\">Invalid Syntax</p>");
            HtmlNode htmlNode = htmlDocument.DocumentNode.FirstChild;
            NodeContext nodeContext = GetNodeContextInstance(htmlNode);
            ProcessResults results = new ProcessResults();
            results.OutputNodes.Add(htmlNode.CloneNode(false));

            // Act
            sut.ProcessNode(nodeContext, results);

            // Assert results
            Assert.IsNotNull(results);
            Assert.AreEqual(1, nodeContext.Dependencies.Logger.Warnings.Length);
        }

        [TestMethod]
        public void Processors_ForProcessor_Invalid_Syntax_in_InsteadOf_of()
        {
            // Assign
            ForProcessor sut = new ForProcessor();
            HtmlDocument htmlDocument = GetHtmlDocument("<p *ngFor=\"#person in persons\">Invalid Syntax</p>");
            HtmlNode htmlNode = htmlDocument.DocumentNode.FirstChild;
            NodeContext nodeContext = GetNodeContextInstance(htmlNode);
            ProcessResults results = new ProcessResults();
            results.OutputNodes.Add(htmlNode.CloneNode(false));

            // Act
            sut.ProcessNode(nodeContext, results);

            // Assert results
            Assert.IsNotNull(results);
            Assert.AreEqual(1, nodeContext.Dependencies.Logger.Warnings.Length);
        }

        #endregion

        #region Processors_ForProcessor_ProcessNode

        [TestMethod()]
        public void Processors_ForProcessor_ProcessNode_Ignore()
        {
            // Assign
            ForProcessor toc = new ForProcessor();
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
            Assert.IsFalse(results.StopProcessing);
        }

        [TestMethod()]
        public void Processors_ForProcessor_ProcessNode_0_items()
        {
            // Assign
            ForProcessor toc = new ForProcessor();

            HtmlDocument htmlDocument = GetHtmlDocument("<p *ngFor=\"#customer of customers\">Hallo #{{number}}</p>");
            HtmlNode htmlNode = htmlDocument.DocumentNode.FirstChild;

            var customers = new Object[0];
            Dictionary<string, object> variables = new Dictionary<string, object>() { { "customers", customers } };

            Mock<ValueFinder> valueFinderMock = new Mock<ValueFinder>();
            valueFinderMock.Setup(mock => mock.GetList("customers", variables)).Returns(customers);

            Mock<TemplateEngine> templateEngineMock = new Mock<TemplateEngine>();
            templateEngineMock.Setup(mock => mock.ProcessNode(It.IsAny<NodeContext>(), It.IsAny<HtmlNode>()));

            NodeContext nodeContext = GetNodeContextInstance(htmlNode, variables, valueFinderMock.Object, templateEngineMock.Object);
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

        [TestMethod()]
        public void Processors_ForProcessor_ProcessNode_2_items()
        {
            // Assign
            ForProcessor toc = new ForProcessor();

            HtmlDocument htmlDocument = GetHtmlDocument("<p *ngFor=\"#customer of customers\">Hallo #{{number}}</p>");
            HtmlNode htmlNode = htmlDocument.DocumentNode.FirstChild;

            var customers = new[] { new { number = 1 }, new { number = 2 } };
            Dictionary<string, object> variables = new Dictionary<string, object>() { { "customers", customers } };

            Mock<ValueFinder> valueFinderMock = new Mock<ValueFinder>();
            valueFinderMock.Setup(mock => mock.GetList("customers", It.IsAny<IDictionary<string, object>>())).Returns(customers);
            int index = 0;
            valueFinderMock.Setup(mock => mock.GetString("number", It.IsAny<IDictionary<string, object>>())).Returns(() => customers[index].number.ToString()).Callback(() => index++);

            NodeContext nodeContext = GetNodeContextInstance(htmlNode, variables, valueFinderMock.Object);
            ProcessResults results = new ProcessResults();
            results.OutputNodes.Add(htmlNode.CloneNode(false));

            // Act
            toc.ProcessNode(nodeContext, results);

            // Assert results
            Assert.IsNotNull(results);
            Assert.IsNotNull(results.OutputNodes);
            Assert.AreEqual(2, results.OutputNodes.Count);
            Assert.IsTrue(results.SkipChildNodes);

            Assert.AreEqual("<p>Hallo #" + customers[0].number + "</p>", results.OutputNodes[0].OuterHtml);
            Assert.AreEqual("<p>Hallo #" + customers[1].number + "</p>", results.OutputNodes[1].OuterHtml);
        }

        #endregion

        #region Private Methods

        private HtmlDocument GetHtmlDocument(string html)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            return htmlDocument;
        }

        private NodeContext GetNodeContextInstance(HtmlNode node, Dictionary<string, object> variables = null, ValueFinder valueFinder = null, TemplateEngine templateEngine = null)
        {
            if (variables == null)
            {
                variables = new Dictionary<string, object>();
            }

            if (templateEngine == null)
            {
                templateEngine = new TemplateEngine();
            }

            Dependencies dependencies = new Dependencies();

            if (valueFinder != null)
            {
                dependencies.ValueFinder = valueFinder;
            }

            return new NodeContext(variables, node, dependencies, new TemplateEngine());
        }

        #endregion
    }
}