using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AngularCSharp.Processors;
using AngularCSharp.ValueObjects;
using HtmlAgilityPack;

namespace AngularCSharp.Helpers
{
    public class TemplateEngine
    {
        #region Properties

        public Dependencies Dependencies { get; private set; }

        public IProcessor[] Processors { get; set; }

        #endregion

        #region Constructor

        public TemplateEngine()
        {
            this.Dependencies = new Dependencies();
            this.Processors = GetDefaultProcessors();
        }

        #endregion

        #region Public methods

        public string ProcessTemplate(HtmlDocument htmlDocumentInput, object model)
        {
            // Process template
            var variables = this.Dependencies.ValueFinder.GetAllProperties(model);
            var htmlDocumentOutput = new HtmlDocument();

            foreach (HtmlNode childNode in htmlDocumentInput.DocumentNode.ChildNodes)
            {
                var context = new NodeContext(variables, childNode, this.Dependencies, this);
                ProcessNode(context, htmlDocumentOutput.DocumentNode);
            }

            // Return HTML
            var writer = new StringWriter();
            htmlDocumentOutput.Save(writer);
            return writer.ToString();
        }

        #endregion

        #region Private methods

        public virtual void ProcessNode(NodeContext context, HtmlNode targetNode)
        {
            // Process current node
            var results = new ProcessResults();
            results.OutputNodes.Add(context.CurrentNode.CloneNode(false));

            foreach (IProcessor processor in GetDefaultProcessors())
            {
                processor.ProcessNode(context, results);

                if (results.StopProcessing)
                {
                    break;
                }
            }

            // Append nodes to target node
            foreach (HtmlNode outputNode in results.OutputNodes)
            {
                targetNode.AppendChild(outputNode);
            }

            // Process childs
            if (!results.SkipChildNodes)
            {
                foreach (HtmlNode childNode in context.CurrentNode.ChildNodes)
                {
                    // Change context
                    var childContext = context.ChangeContext(childNode);

                    // Find target node
                    HtmlNode childTargetNode = targetNode.HasChildNodes ? targetNode.LastChild : targetNode;

                    // Process child node
                    ProcessNode(childContext, childTargetNode);
                }
            }
        }

        private IProcessor[] GetDefaultProcessors()
        {
            return new IProcessor[]
            {
                new IfProcessor(),
                new ForProcessor(),
                new ExpressionsProcessor(),
                new TemplateProcessor()
            };
        }

        #endregion
    }
}
