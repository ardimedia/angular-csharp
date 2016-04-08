using AngularCsharp.ValueObjects;
using HtmlAgilityPack;

namespace AngularCsharp.Processors
{
    public class IfProcessor : IProcessor
    {
        private const string ATTRIBUTE_NAME = "*ngif";

        public ProcessResults ProcessNode(NodeContext nodeContext)
        {
            var results = new ProcessResults();

            if (nodeContext.CurrentNode.Attributes.Contains(ATTRIBUTE_NAME))
            {
                results.StopProcessing = true;
                var isTrue = IsTrue(nodeContext);

                if (isTrue)
                {
                    var outputNode = nodeContext.CurrentNode.CloneNode(false);
                    outputNode.Attributes.Remove(ATTRIBUTE_NAME);
                    results.OutputNodes = new HtmlNode[] { outputNode };
                } else
                {
                    results.SkipChildNodes = true;
                    results.OutputNodes = new HtmlNode[0];
                }
            }

            return results;
        }

        private bool IsTrue(NodeContext nodeContext)
        {
            var expression = nodeContext.CurrentNode.Attributes["*ngif"].Value;
            var variables = nodeContext.CurrentVariables;

            return nodeContext.Dependencies.ExpressionResolver.IsTrue(expression, variables);
        }
    }
}
