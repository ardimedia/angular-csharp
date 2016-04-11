using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AngularCSharp.Exceptions;
using AngularCSharp.ValueObjects;
using HtmlAgilityPack;

namespace AngularCSharp.Processors
{
    public class ForProcessor : IProcessor
    {
        #region Private constants

        private const string ATTRIBUTE_NAME = "*ngFor";

        private const string ATTRIBUTE_PATTERN = @"#(\w+)\s+of\s+(\w+)";

        #endregion

        #region IProcessor methods

        public void ProcessNode(NodeContext nodeContext, ProcessResults results)
        {
            HtmlAttributeCollection attributes = nodeContext.CurrentNode.Attributes;

            foreach (HtmlNode htmlNode in results.OutputNodes)
            {
                if (attributes.Contains(ATTRIBUTE_NAME))
                {
                    ProcessFor(htmlNode, nodeContext, attributes[ATTRIBUTE_NAME], results);
                }
            }
        }

        #endregion

        #region Private methods

        private void ProcessFor(HtmlNode htmlNode, NodeContext nodeContext, HtmlAttribute htmlAttribute, ProcessResults results)
        {
            Match match = Regex.Match(htmlAttribute.Value, ATTRIBUTE_PATTERN);
            if (!match.Success)
            {
                nodeContext.Dependencies.Logger.AddWarning($"Invalid ngFor value { htmlAttribute.Value }");
                return;
            }

            List<HtmlNode> outputNodes = new List<HtmlNode>();
            IEnumerable list = this.GetList(nodeContext, match.Groups[2].Value, nodeContext.CurrentVariables);
            foreach (object item in list)
            {
                outputNodes.Add(ProcessItem(nodeContext, match.Groups[1].Value, item));
            }

            results.OutputNodes = outputNodes;
            results.SkipChildNodes = true;
        }

        private IEnumerable GetList(NodeContext nodeContext, string variableName, IDictionary<string,object> variables)
        {
            try
            {
                return nodeContext.Dependencies.ValueFinder.GetList(variableName, nodeContext.CurrentVariables);
            }
            catch (ValueNotFoundException)
            {
                nodeContext.Dependencies.Logger.AddWarning($"Value { variableName } not found");
                return new Object[0];
            }
        }

        private HtmlNode ProcessItem(NodeContext nodeContext, string variableName, object item)
        {
            HtmlNode nodeOutput = nodeContext.CurrentNode.CloneNode(false);
            nodeOutput.Attributes.Remove(ATTRIBUTE_NAME);

            Dictionary<string, object> additionalVariables = new Dictionary<string, object>() { { variableName, item } };

            foreach (HtmlNode childNode in nodeContext.CurrentNode.ChildNodes)
            {
                NodeContext childNodeContext = nodeContext.ChangeContext(additionalVariables, childNode);
                nodeContext.TemplateEngine.ProcessNode(childNodeContext, nodeOutput);
            }

            return nodeOutput;
        }

        #endregion
    }
}
