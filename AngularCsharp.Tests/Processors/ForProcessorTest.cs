using System;
using System.Collections.Generic;
using AngularCSharp.Helpers;
using AngularCSharp.ValueObjects;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AngularCSharp.Processors.Tests.Processors
{
    [TestClass()]
    public class ForProcessorTest
    {
        #region Processors_ForProcessor_Invalid_Syntax

        // TODO: 2016-04-09/hp: How can we see, that *ngFor uses a invalid syntax
        [Ignore]
        [TestMethod()]
        public void Processors_ForProcessor_Invalid_Syntax_Empty()
        {
            // Assign
            ForProcessor sut = new ForProcessor();
            HtmlDocument htmlDocument = GetHtmlDocument("<p *ngFor=\"\">Invalid Syntax</p>");
            HtmlNode htmlNode = htmlDocument.DocumentNode.FirstChild;
            NodeContext nodeContext = GetNodeContextInstance(htmlNode);

            // Act
            ProcessResults results = sut.ProcessNode(nodeContext);

            // Assert results
            Assert.IsNotNull(results);
        }

        // TODO: 2016-04-09/hp: How can we see, that *ngFor uses a invalid syntax
        [Ignore]
        [TestMethod()]
        public void Processors_ForProcessor_Invalid_Syntax_in_InsteadOf_of()
        {
            // Assign
            ForProcessor sut = new ForProcessor();
            HtmlDocument htmlDocument = GetHtmlDocument("<p *ngFor=\"#person in persons\">Invalid Syntax</p>");
            HtmlNode htmlNode = htmlDocument.DocumentNode.FirstChild;
            NodeContext nodeContext = GetNodeContextInstance(htmlNode);

            // Act
            ProcessResults results = sut.ProcessNode(nodeContext);

            // Assert results
            Assert.IsNotNull(results);
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

            // Act
            ProcessResults results = toc.ProcessNode(nodeContext);

            // Assert results
            Assert.IsNotNull(results);
            Assert.IsNull(results.OutputNodes);
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

            // Act
            ProcessResults results = toc.ProcessNode(nodeContext);

            // Assert results
            Assert.IsNotNull(results);
            Assert.IsNotNull(results.OutputNodes);
            Assert.AreEqual(0, results.OutputNodes.Length);
            Assert.IsTrue(results.SkipChildNodes);
            Assert.IsTrue(results.StopProcessing);
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

            // Act
            ProcessResults results = toc.ProcessNode(nodeContext);

            // Assert results
            Assert.IsNotNull(results);
            Assert.IsNotNull(results.OutputNodes);
            Assert.AreEqual(2, results.OutputNodes.Length);
            Assert.IsTrue(results.SkipChildNodes);
            Assert.IsTrue(results.StopProcessing);

            Assert.AreEqual("<p>Hallo #" + customers[0].number + "</p>", results.OutputNodes[0].OuterHtml);
            Assert.AreEqual("<p>Hallo #" + customers[1].number + "</p>", results.OutputNodes[1].OuterHtml);
        }

        #endregion

        #region Processors_ForProcessor_ProcessNode

        // TODO: 2016-04-10/hp: if the template tag is used, remove it from the output
        [Ignore]
        [TestMethod()]
        public void Processors_ForProcessor_Template_Tag()
        {
            // Assign
            ForProcessor sut = new ForProcessor();
            HtmlDocument htmlDocument = GetHtmlDocument("<template *ngFor=\"\"><p>{{firstName}}</p></template>");
            HtmlNode htmlNode = htmlDocument.DocumentNode.FirstChild;
            NodeContext nodeContext = GetNodeContextInstance(htmlNode);
            string expected = "<p>Jim</p><p>Blue</p>";

            // Act
            ProcessResults results = sut.ProcessNode(nodeContext);

            // Assert results
            Assert.IsNotNull(results);
            //Assert.AreEqual<string>(expected, results.OutputNodes);
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

            return new NodeContext(variables, node, new HtmlDocument(), dependencies, new TemplateEngine());
        }

        #endregion
    }
}