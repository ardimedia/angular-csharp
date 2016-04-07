using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngularCsharp.ValueObjects;
using AngularCsharp.Helpers;
using HtmlAgilityPack;
using AngularCsharp.Exceptions;

namespace AngularCsharp.Processors
{
    public class ExpressionsProcessor : IProcessor
    {
        #region Constants

        const string REGEX_PATTERN = @"{{([\w.]*)}}";

        #endregion

        #region Fields

        private ValueFinder valueFinder = new ValueFinder();

        #endregion

        #region IProcessor methods

        public ProcessResults ProcessNode(NodeContext nodeContext)
        {
            var newNode = nodeContext.CurrentNode.CloneNode(false);

            if (newNode.NodeType == HtmlAgilityPack.HtmlNodeType.Text)
            {
                newNode = nodeContext.TargetDocument.CreateTextNode(ReplaceFields(nodeContext));
            }

            return new ProcessResults() { OutputNodes = new HtmlNode[] { newNode }};
        }

        #endregion

        #region Private methods

        private string ReplaceFields(NodeContext nodeContext)
        {
            var input = nodeContext.CurrentNode.InnerText;
            var matches = Regex.Matches(input, REGEX_PATTERN);

            foreach (Match match in matches)
            {
                try
                {
                    var fieldValue = valueFinder.GetString(match.Groups[1].Value, nodeContext.CurrentVariables);
                    input = input.Replace(match.Value, fieldValue);
                } catch (ValueNotFoundException ex)
                {
                    nodeContext.Dependencies.Logger.AddWarning(String.Format("Value {0} not found", match.Groups[1].Value));
                }
            }

            return input;
        }

        #endregion
    }
}
