using AngularCSharp.ValueObjects;
using HtmlAgilityPack;
using System.Collections.Generic;

namespace AngularCSharp.Processors
{
    public class IfProcessor : IProcessor
    {
        private const string ATTRIBUTE_NAME = "*ngif";

        public void ProcessNode(NodeContext nodeContext, ProcessResults results)
        {
            List<HtmlNode> nodesToRemove = new List<HtmlNode>();

            foreach (HtmlNode outputNode in results.OutputNodes)
            {
                if (outputNode.Attributes.Contains(ATTRIBUTE_NAME))
                {
                    if (IsTrue(nodeContext))
                    {
                        // true -> only remove *ngif attribute
                        outputNode.Attributes.Remove(ATTRIBUTE_NAME);
                    }
                    else
                    {
                        // false -> skip processing of child nodes and remove current node from outputNodes list
                        nodesToRemove.Add(outputNode);
                        results.SkipChildNodes = true;
                    }
                }
            }

            foreach (HtmlNode nodeToRemove in nodesToRemove)
            {
                results.OutputNodes.Remove(nodeToRemove);
            }
        }

        private bool IsTrue(NodeContext nodeContext)
        {
            var expression = nodeContext.CurrentNode.Attributes["*ngif"].Value;
            var variables = nodeContext.CurrentVariables;

            return nodeContext.Dependencies.ExpressionResolver.IsTrue(expression, variables);
        }
    }
}
