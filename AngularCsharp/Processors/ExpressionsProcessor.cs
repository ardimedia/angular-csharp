using AngularCSharp.Exceptions;
using AngularCSharp.ValueObjects;
using HtmlAgilityPack;
using System;
using System.Text.RegularExpressions;

namespace AngularCSharp.Processors
{
    public class ExpressionsProcessor : IProcessor
    {
        #region Constants

        const string REGEX_PATTERN = @"{{([\w.]*)}}";

        #endregion

        #region IProcessor methods

        public void ProcessNode(NodeContext nodeContext, ProcessResults results)
        {
            foreach (HtmlNode node in results.OutputNodes)
            {
                if (node.NodeType == HtmlNodeType.Text)
                {
                    node.InnerHtml = ReplaceFields(node, nodeContext);
                }
            }
        }

        #endregion

        #region Private methods

        private string ReplaceFields(HtmlNode node, NodeContext nodeContext)
        {
            var input = node.InnerText;
            var matches = Regex.Matches(input, REGEX_PATTERN);

            foreach (Match match in matches)
            {
                try
                {
                    var fieldValue = nodeContext.Dependencies.ValueFinder.GetString(match.Groups[1].Value, nodeContext.CurrentVariables);
                    input = input.Replace(match.Value, fieldValue);
                } catch (ValueNotFoundException)
                {
                    nodeContext.Dependencies.Logger.AddWarning(String.Format("Value {0} not found", match.Groups[1].Value));
                }
            }

            return input.Replace("<", "&lt;").Replace(">", "&gt;");
        }

        #endregion
    }
}
