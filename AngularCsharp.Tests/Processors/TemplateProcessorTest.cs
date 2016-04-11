using AngularCSharp.Helpers;
using AngularCSharp.ValueObjects;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AngularCSharp.Processors.Tests.Processors
{
    [TestClass]
    public class TemplateProcessorTest
    {

        #region Processors_TemplateProcessor_ProcessNode

        [TestMethod]
        public void Processors_TemplateProcessor_ProcessNode()
        {
            // Assign
            HtmlDocument originalDocument = GetHtmlDocument("<template *ngFor=\"#person of persons\"><p>{{person.firstName}} {{person.lastName}}</p></template>");
            HtmlDocument documentAfterOtherProcessors = GetHtmlDocument("<template><p>Max Mustermann</p></template>");
            HtmlDocument expectedDocument = GetHtmlDocument("<p>Max Mustermann</p>");

            NodeContext nodeContext = GetNodeContextInstance(originalDocument.DocumentNode.FirstChild);

            ProcessResults results = new ProcessResults();
            results.OutputNodes.Add(documentAfterOtherProcessors.DocumentNode.FirstChild);

            TemplateProcessor sut = new TemplateProcessor();

            // Act
            sut.ProcessNode(nodeContext, results);

            // Assert results
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.OutputNodes.Count);
            Assert.IsFalse(results.SkipChildNodes);
            Assert.IsFalse(results.StopProcessing);
        }

        #endregion

        #region Processors_TemplateProcessor_Ignore

        // TODO: 2016-04-10/hp: if the template tag is used, remove it from the output
        [TestMethod]
        public void Processors_TemplateProcessor_Ignore()
        {
            // Assign
            HtmlDocument originalDocument = GetHtmlDocument("<template><p *ngFor=\"#person of persons\">{{person.firstName}} {{person.lastName}}</p></template>");
            HtmlDocument documentAfterOtherProcessors = GetHtmlDocument("<template><p>Max Mustermann</p></template>");
            HtmlDocument expectedDocument = GetHtmlDocument("<template><p>Max Mustermann</p></template>");

            NodeContext nodeContext = GetNodeContextInstance(originalDocument.DocumentNode.FirstChild);

            ProcessResults results = new ProcessResults();
            results.OutputNodes.Add(documentAfterOtherProcessors.DocumentNode.FirstChild);

            TemplateProcessor sut = new TemplateProcessor();

            // Act
            sut.ProcessNode(nodeContext, results);

            // Assert results
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.OutputNodes.Count);
            Assert.AreEqual<string>(expectedDocument.DocumentNode.FirstChild.OuterHtml, results.OutputNodes[0].OuterHtml);
            Assert.IsFalse(results.SkipChildNodes);
            Assert.IsFalse(results.StopProcessing);
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