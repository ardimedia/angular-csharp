using HtmlAgilityPack;

namespace AngularCsharp
{
    public class HtmlOperation
    {
        public static HtmlNodeCollection GetNodesNgFor(string html)
        {
            html = NgForOperation.FixHtml(html);

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            HtmlNodeCollection htmlNodeCollection = htmlDoc.DocumentNode.SelectNodes("//*[@ngfor]");

            return htmlNodeCollection;
        }
    }
}
