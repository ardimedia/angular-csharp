using AngularCSharp.Exceptions;
using AngularCSharp.ValueObjects;
using HtmlAgilityPack;
using System;
using System.Text.RegularExpressions;

namespace AngularCSharp.Processors
{
    /// <summary>
    /// Replaces Angular2 expressions (ie. {{model.variable1}} placeholders) with the value of model property
    /// </summary>
    public class ExpressionsProcessor : IProcessor
    {
        #region Constants

        /// <summary>
        /// Regex pattern to find Angular2 expressions
        /// </summary>
        const string REGEX_PATTERN = @"{{([\w.]*)}}";

        #endregion

        #region IProcessor methods

        /// <summary>
        /// Process node (find variables in HTML text nodes and replaces them by model values)
        /// </summary>
        /// <param name="nodeContext"></param>
        /// <param name="results"></param>
        public void ProcessNode(NodeContext nodeContext, ProcessResults results)
        {
            // Iterate through output nodes (prepared by TemplateEngine and modified by preceding processors)
            foreach (HtmlNode node in results.OutputNodes)
            {
                // Find only text nodes
                if (node.NodeType == HtmlNodeType.Text)
                {
                    // Replace field
                    node.InnerHtml = ReplaceFields(node, nodeContext);
                }
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Returns node text with replaced Angular2 expressions
        /// </summary>
        /// <param name="node">Current HTML node</param>
        /// <param name="nodeContext">Current node context</param>
        /// <returns></returns>
        private string ReplaceFields(HtmlNode node, NodeContext nodeContext)
        {
            // Get node text
            var input = node.InnerText;

            // Find all expressions in text
            var matches = Regex.Matches(input, REGEX_PATTERN);

            // Iterate through all expressions
            foreach (Match match in matches)
            {
                try
                {
                    // Get field value from model
                    var fieldValue = nodeContext.Dependencies.ValueFinder.GetString(match.Groups[1].Value, nodeContext.CurrentVariables);

                    // Replace expression by value
                    input = input.Replace(match.Value, fieldValue);
                } catch (ValueNotFoundException)
                {
                    // Add warning because value was not found in model
                    nodeContext.Dependencies.Logger.AddWarning(String.Format("Value {0} not found", match.Groups[1].Value));
                }
            }

            // No HTML in models is allowed, but other characters (umlauts etc.) should stay the same
            input = input.Replace("<", "&lt;").Replace(">", "&gt;");

            // Return replaced text
            return input;
        }

        #endregion
    }
}
