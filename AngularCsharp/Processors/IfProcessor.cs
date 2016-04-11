using AngularCSharp.ValueObjects;
using HtmlAgilityPack;
using System.Collections.Generic;

namespace AngularCSharp.Processors
{
    /// <summary>
    /// Processes *ngIf nodes
    /// </summary>
    public class IfProcessor : IProcessor
    {
        #region Private constants

        /// <summary>
        /// Name of ngIf attribute
        /// </summary>
        private const string ATTRIBUTE_NAME = "*ngif";

        #endregion

        #region IProcessor methods

        /// <summary>
        /// Processes ngIf nodes
        /// </summary>
        /// <param name="nodeContext"></param>
        /// <param name="results"></param>
        public void ProcessNode(NodeContext nodeContext, ProcessResults results)
        {
            // Prepare list for nodes to remove (can't iterate and remove at the same time)
            List<HtmlNode> nodesToRemove = new List<HtmlNode>();

            // Iterate through all outpout nodes
            foreach (HtmlNode outputNode in results.OutputNodes)
            {
                // Only for nodes with *ngIf attribute
                if (outputNode.Attributes.Contains(ATTRIBUTE_NAME))
                {
                    // Evaluate Angular2 expression
                    bool isTrue = IsTrue(nodeContext, outputNode);

                    if (isTrue)
                    {
                        // Expression ist true, remove *ngIf attribute
                        outputNode.Attributes.Remove(ATTRIBUTE_NAME);
                    }
                    else
                    {
                        // Expression is false, skip processing of child nodes and remove current node from outputNodes list
                        nodesToRemove.Add(outputNode);
                        results.SkipChildNodes = true;
                    }
                }
            }

            // Remove nodes
            foreach (HtmlNode nodeToRemove in nodesToRemove)
            {
                results.OutputNodes.Remove(nodeToRemove);
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Returns if specified expression is true
        /// </summary>
        /// <param name="nodeContext"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool IsTrue(NodeContext nodeContext, HtmlNode node)
        {
            // Get attribute value
            var expression = node.Attributes["*ngif"].Value;

            // Get variables
            var variables = nodeContext.CurrentVariables;

            // Return value from ExpressionResolver class
            return nodeContext.Dependencies.ExpressionResolver.IsTrue(expression, variables);
        }

        #endregion
    }
}
