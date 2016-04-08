﻿using AngularCsharp.Processors;
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

        internal string ProcessTemplate(HtmlDocument htmlDocumentInput, object model)
        {
            // Process template
            var variables = GetGlobalVariables(model);
            var htmlDocumentOutput = new HtmlDocument();

            foreach (HtmlNode childNode in htmlDocumentInput.DocumentNode.ChildNodes)
            {
                var context = new NodeContext(variables, childNode, htmlDocumentOutput, Dependencies);
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
            var results = new ProcessResults();
            foreach (IProcessor processor in GetDefaultProcessors())
            {
                results = processor.ProcessNode(context);

                if (results.OutputNodes != null)
                {
                    // Don't call multiple processors, cancel as soon one processor returns nodes
                    break;
                }
            }

            // Append nodes to target node
            if (results.OutputNodes != null)
            {
                foreach (HtmlNode node in results.OutputNodes)
                {
                    targetNode.AppendChild(node);
                }
            }

            // Process childs
            if (!results.SkipChildNodes)
            {
                foreach (HtmlNode childNode in context.CurrentNode.ChildNodes)
                {
                    // Change context
                    var childContext = context.ChangeContext(childNode);

                    // Process child node
                    ProcessNode(childContext, targetNode.LastChild);
                }
            }
        }

        private IProcessor[] GetDefaultProcessors()
        {
            return new IProcessor[]
            {
                new IfProcessor(),
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