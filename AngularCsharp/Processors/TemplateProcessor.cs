using AngularCSharp.ValueObjects;
using HtmlAgilityPack;
using System.Collections.Generic;

namespace AngularCSharp.Processors
{
    /// <summary>
    /// Processes <template> nodes
    /// </summary>
    public class TemplateProcessor : IProcessor
    {
        #region Private constants

        /// <summary>
        /// Template HTML tag name
        /// </summary>
        private const string TAG_NAME = "template";

        #endregion

        #region IProcessor methods

        /// <summary>
        /// Removes <template> nodes, which has initially a Angular attribute
        /// </summary>
        /// <param name="nodeContext">Current node context</param>
        /// <param name="results">Processor results object</param>
        public void ProcessNode(NodeContext nodeContext, ProcessResults results)
        {
            // Prepare lists for add/remove HTML nodes (can't modify and iterate list on the same time)
            List<HtmlNode> nodesToAdd = new List<HtmlNode>();
            List<HtmlNode> nodesToRemove = new List<HtmlNode>();

            // Iterate through output nodes
            foreach (HtmlNode currentNode in results.OutputNodes)
            {
                // Find out, if current node is a <template> node and hat initially an Angular attribute
                if (currentNode.Name == TAG_NAME && this.NodeHasAngularAttribute(nodeContext.CurrentNode))
                {
                    // Add child nodes (has values when this node had a *ngFor attribute)
                    nodesToAdd.AddRange(currentNode.ChildNodes);

                    // Remove this node
                    nodesToRemove.Add(currentNode);
                }
            }

            // Remove HTML nodes
            foreach (HtmlNode nodeToRemove in nodesToRemove)
            {
                results.OutputNodes.Remove(nodeToRemove);
            }

            // Add HTML nodes
            foreach (HtmlNode nodeToAdd in nodesToAdd)
            {
                results.OutputNodes.Add(nodeToAdd);
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Find out if this node had Angular attributes
        /// </summary>
        /// <param name="currentNode">Current node</param>
        /// <returns>Returns true if specified node had Angular attributes</returns>
        private bool NodeHasAngularAttribute(HtmlNode currentNode)
        {
            // Iterate all attributes of original node
            foreach (HtmlAttribute attribute in currentNode.Attributes)
            {
                // Attribute starts with *ng
                if (attribute.Name.StartsWith("*ng"))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
