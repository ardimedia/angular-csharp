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

        /// <summary>
        /// Name of ngFor attribute
        /// </summary>
        private const string ATTRIBUTE_NAME = "*ngFor";

        /// <summary>
        /// Regex pattern for *ngFor attribute
        /// </summary>
        private const string ATTRIBUTE_PATTERN = @"#(\w+)\s+of\s+(\w+)";

        #endregion

        #region IProcessor methods

        /// <summary>
        /// Process nodes with *ngFor attributes
        /// </summary>
        /// <param name="nodeContext"></param>
        /// <param name="results"></param>
        public void ProcessNode(NodeContext nodeContext, ProcessResults results)
        {
            foreach (HtmlNode htmlNode in results.OutputNodes)
            {
                // Get node attributes
                HtmlAttributeCollection attributes = htmlNode.Attributes;

                // Chid if nnode contains *ngFor attribute
                if (attributes.Contains(ATTRIBUTE_NAME))
                {
                    // Process this node
                    ProcessFor(htmlNode, nodeContext, attributes[ATTRIBUTE_NAME], results);
                }
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Process *ngFor node (get list from model, clone node for each item)
        /// </summary>
        /// <param name="htmlNode"></param>
        /// <param name="nodeContext"></param>
        /// <param name="htmlAttribute"></param>
        /// <param name="results"></param>
        private void ProcessFor(HtmlNode htmlNode, NodeContext nodeContext, HtmlAttribute htmlAttribute, ProcessResults results)
        {
            // Parse attribute value
            Match match = Regex.Match(htmlAttribute.Value, ATTRIBUTE_PATTERN);
            if (!match.Success)
            {
                // Attribute value is invalid: Add warning and leave node unchanged
                nodeContext.Dependencies.Logger.AddWarning($"Invalid ngFor value { htmlAttribute.Value }");
                return;
            }

            // Init list of output nodes for node clones
            List<HtmlNode> outputNodes = new List<HtmlNode>();

            // Get list from model
            IEnumerable list = this.GetList(nodeContext, match.Groups[2].Value, nodeContext.CurrentVariables);

            // Clone node for each item in list
            foreach (object item in list)
            {
                // Process node for a single item
                HtmlNode outputNode = ProcessItem(nodeContext, match.Groups[1].Value, item);

                // Add node to output list
                outputNodes.Add(outputNode);
            }

            // Set output nodes
            results.OutputNodes = outputNodes;

            // Skip child nodes, because they has been already processed in this.ProcessItem()
            results.SkipChildNodes = true;
        }

        /// <summary>
        /// Returns list for a single node from current variables
        /// </summary>
        /// <param name="nodeContext">Current node context</param>
        /// <param name="variableName">Angular2 expression of variable</param>
        /// <param name="variables">Current variables</param>
        /// <returns></returns>
        private IEnumerable GetList(NodeContext nodeContext, string variableName, IDictionary<string,object> variables)
        {
            try
            {
                // Return list from value finder
                return nodeContext.Dependencies.ValueFinder.GetList(variableName, nodeContext.CurrentVariables);
            }
            catch (ValueNotFoundException)
            {
                // Invalid value, add warning and return empty array
                nodeContext.Dependencies.Logger.AddWarning($"Value { variableName } not found");
                return new Object[0];
            }
        }

        /// <summary>
        /// Process list item for current node
        /// </summary>
        /// <param name="nodeContext">Current node context</param>
        /// <param name="variableName">Angular2 expression</param>
        /// <param name="item">Current list item</param>
        /// <returns></returns>
        private HtmlNode ProcessItem(NodeContext nodeContext, string variableName, object item)
        {
            // Clone HTML node
            HtmlNode nodeOutput = nodeContext.CurrentNode.CloneNode(false);

            // Remove *ngFor attribute
            nodeOutput.Attributes.Remove(ATTRIBUTE_NAME);

            // Prepare dictionary with additional variables from list item
            Dictionary<string, object> additionalVariables = new Dictionary<string, object>() { { variableName, item } };

            // Process child items with TemplateEngine and additional variables from list item
            foreach (HtmlNode childNode in nodeContext.CurrentNode.ChildNodes)
            {
                // Change node context
                NodeContext childNodeContext = nodeContext.ChangeContext(additionalVariables, childNode);

                // Process node
                nodeContext.TemplateEngine.ProcessNode(childNodeContext, nodeOutput);
            }

            // Return modified output node
            return nodeOutput;
        }

        #endregion
    }
}
