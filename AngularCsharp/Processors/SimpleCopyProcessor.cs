using AngularCsharp.ValueObjects;
using HtmlAgilityPack;

namespace AngularCsharp.Processors
{
    public class SimpleCopyProcessor : IProcessor
    {
        public ProcessResults ProcessNode(NodeContext nodeContext)
        {
            var newNode = nodeContext.CurrentNode.CloneNode(false);
            var newNodes = new HtmlNode[] { newNode };

            return new ProcessResults() { OutputNodes = newNodes };
        }
    }
}
