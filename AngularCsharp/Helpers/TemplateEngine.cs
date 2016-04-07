using AngularCsharp.Processors;
using AngularCsharp.ValueObjects;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AngularCsharp.Helpers
{
    internal class TemplateEngine
    {
        #region Properties

        public Logger Logger { get; set; }

        public IProcessor[] Processors { get; set; }


        public ValueFinder ValueFinder { get; set; }

        #endregion

        #region Constructor

        public TemplateEngine()
        {
            this.Logger = new Logger();
            this.Processors = GetDefaultProcessors();
            this.ValueFinder = new ValueFinder();
        }

        #endregion

        #region Public methods

        internal string ProcessTemplate(HtmlDocument htmlDocumentInput, object model)
        {
            // Process template
            var variables = GetGlobalVariables(model);
            var htmlDocumentOutput = new HtmlDocument();

            foreach (HtmlNode childNode in htmlDocumentInput.DocumentNode.ChildNodes)
            {
                var context = new NodeContext(variables, childNode, htmlDocumentOutput, ValueFinder, Logger);
                ProcessNode(context, htmlDocumentOutput.DocumentNode);
            }

            // Return html
            var writer = new StringWriter();
            htmlDocumentOutput.Save(writer);
            return writer.ToString();
        }

        #endregion

        #region Private methods

        private void ProcessNode(NodeContext context, HtmlNode targetNode)
        {
            // Process current node
            HtmlNode[] nodes = null;
            foreach (IProcessor processor in GetDefaultProcessors())
            {
                var results = processor.ProcessNode(context);

                if (results.OutputNodes.Length > 0)
                {
                    nodes = results.OutputNodes;

                    // Don't call multiple processors, cancel as soon one processor returns nodes
                    break;
                }
            }

            // Append nodes to target node
            foreach (HtmlNode node in nodes)
            {
                targetNode.AppendChild(node);
            }

            // Process childs
            foreach (HtmlNode childNode in context.CurrentNode.ChildNodes)
            {
                // Change context
                var childContext = context.ChangeContext(childNode);

                // Process child node
                ProcessNode(childContext, targetNode.LastChild);
            }
        }

        private IProcessor[] GetDefaultProcessors()
        {
            return new IProcessor[]
            {
                new ExpressionsProcessor()
            };
        }

        private Dictionary<string, object> GetGlobalVariables(object model)
        {
            // TODO: Move this function to ValueFinder class
            var dict = new Dictionary<string, object>();
            foreach (PropertyInfo propertyInfo in model.GetType().GetProperties())
            {
                dict.Add(propertyInfo.Name, propertyInfo.GetValue(model));
            }

            return dict;
        }

        #endregion
    }
}
