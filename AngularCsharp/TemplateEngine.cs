using AngularCsharp.Processors;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AngularCsharp
{
    public class TemplateEngine
    {
        #region Fields

        private IProcessor[] _Processors;

        #endregion

        #region Constructor

        public TemplateEngine()
        {
            _Processors = GetProcessors();
        }

        #endregion

        #region Public methods

        public string ProcessTemplate(HtmlDocument document, object model)
        {
            // Process template
            var variables = GetGlobalVariables(model);
            var finalDocument = new HtmlDocument();

            foreach (HtmlNode childNode in document.DocumentNode.ChildNodes)
            {
                var context = new NodeContext(variables, childNode, finalDocument);
                ProcessNode(context, finalDocument.DocumentNode);
            }

            // Return html
            var writer = new StringWriter();
            finalDocument.Save(writer);
            return writer.ToString();
        }

        #endregion

        #region Private methods

        private void ProcessNode(NodeContext context, HtmlNode targetNode)
        {
            // Process current node
            HtmlNode[] nodes = null;
            foreach (IProcessor processor in GetProcessors())
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

        private IProcessor[] GetProcessors()
        {
            return new IProcessor[]
            {
                new SimpleCopyProcessor()
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
