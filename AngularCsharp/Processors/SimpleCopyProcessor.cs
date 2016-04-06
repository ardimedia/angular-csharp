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
            var oldNode = nodeContext.CurrentNode;
            HtmlNode[] newNodes = new HtmlNode[0];

            switch (oldNode.NodeType)
            {
                case HtmlNodeType.Element:
                    var element = nodeContext.TargetDocument.CreateElement(oldNode.Name);
                    foreach (HtmlAttribute attribute in oldNode.Attributes)
                        element.SetAttributeValue(attribute.Name, attribute.Value);
                    newNodes = new HtmlNode[] { element };
                    break;
                case HtmlNodeType.Text:
                    var text = nodeContext.TargetDocument.CreateTextNode(oldNode.InnerText);
                    newNodes = new HtmlNode[] { text };
                    break;
                case HtmlNodeType.Comment:
                    var comment = nodeContext.TargetDocument.CreateComment(oldNode.InnerText);
                    newNodes = new HtmlNode[] { comment };
                    break;
            }

            return new ProcessResults() { OutputNodes = newNodes };
        }
    }
}
