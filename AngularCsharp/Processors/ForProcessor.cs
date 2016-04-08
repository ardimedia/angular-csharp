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
        private const string ATTRIBUTE_NAME = "*ngFor";

        private const string ATTRIBUTE_PATTERN = @"#(\w+)\s+of\s+(\w+)";

        public ProcessResults ProcessNode(NodeContext nodeContext)
        {
            var results = new ProcessResults();
            HtmlAttributeCollection attributes = nodeContext.CurrentNode.Attributes;

            if (attributes.Contains(ATTRIBUTE_NAME))
            {
                results = ProcessFor(nodeContext, attributes[ATTRIBUTE_NAME], results);
            }

            return results;
        }

        private ProcessResults ProcessFor(NodeContext nodeContext, HtmlAttribute htmlAttribute, ProcessResults results)
        {
            Match match = Regex.Match(htmlAttribute.Value, ATTRIBUTE_PATTERN);
            if (!match.Success)
            {
                return results;
            }

            List<HtmlNode> nodes = new List<HtmlNode>();
            IEnumerable list = this.GetList(nodeContext, match.Groups[2].Value, nodeContext.CurrentVariables);
            foreach (object item in list)
            {
                nodes.Add(ProcessItem(nodeContext, match.Groups[1].Value, item));
            }

            results.OutputNodes = nodes.ToArray();
            results.SkipChildNodes = true;
            results.StopProcessing = true;
            return results;
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
    }
}
