using AngularCsharp.ValueObjects;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
