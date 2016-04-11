using AngularCSharp.ValueObjects;
using HtmlAgilityPack;
using System.Collections.Generic;

namespace AngularCSharp.Processors
{
    public class TemplateProcessor : IProcessor
    {
        private const string TAG_NAME = "template";

        public void ProcessNode(NodeContext nodeContext, ProcessResults results)
        {
            List<HtmlNode> nodesToAdd = new List<HtmlNode>();
            List<HtmlNode> nodesToRemove = new List<HtmlNode>();

            foreach (HtmlNode currentNode in results.OutputNodes)
            {
                if (currentNode.Name == TAG_NAME && this.NodeHasAngularAttribute(nodeContext.CurrentNode))
                {
                    nodesToAdd.AddRange(currentNode.ChildNodes);
                    nodesToRemove.Add(currentNode);
                }
            }

            foreach (HtmlNode nodeToRemove in nodesToRemove)
            {
                results.OutputNodes.Remove(nodeToRemove);
            }

            foreach (HtmlNode nodeToAdd in nodesToAdd)
            {
                results.OutputNodes.Add(nodeToAdd);
            }
        }

        private bool NodeHasAngularAttribute(HtmlNode currentNode)
        {
            foreach (HtmlAttribute attribute in currentNode.Attributes)
            {
                if (attribute.Name.StartsWith("*ng"))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
