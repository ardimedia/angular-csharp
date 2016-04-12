using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AngularCSharp.Processors;
using AngularCSharp.ValueObjects;
using HtmlAgilityPack;

namespace AngularCSharp.Helpers
{
    /// <summary>
    /// This is the core component of AngularCSharp. It iterates through all node, copy it and passes it to all specified processors.
    /// </summary>
    public class TemplateEngine
    {
        #region Constructor

        /// <summary>
        /// Constructor (initialize default dependencies and processors)
        /// </summary>
        public TemplateEngine()
        {
            this.Dependencies = new Dependencies();
            this.Processors = GetDefaultProcessors();
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Contains all needed dependencies
        /// </summary>
        public Dependencies Dependencies { get; private set; }

        /// <summary>
        /// Contains all needed classes for template processing
        /// </summary>
        public IProcessor[] Processors { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Processes the HTML document, replaces variables with specified model and returns processed HTML string
        /// </summary>
        /// <param name="htmlDocumentInput">DOM tree of HTML template</param>
        /// <param name="model">Any object with properties</param>
        /// <returns></returns>
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

        /// <summary>
        /// Processing a single node (prepare ProcessResults instance, pass it to all processors, call the same method on child nodes)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="targetNode"></param>
        public virtual void ProcessNode(NodeContext context, HtmlNode targetNode)
        {
            // Prepare ProcessResults instance
            var results = new ProcessResults();
            results.OutputNodes.Add(context.CurrentNode.CloneNode(false));

            // Iterate through all availabe processors
            foreach (IProcessor processor in GetDefaultProcessors())
            {
                // Call processor
                processor.ProcessNode(context, results);

                // Stop processing when necessary (processors can control this)
                if (results.StopProcessing)
                {
                    break;
                }
            }

            // Append output nodes to target DOM
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
