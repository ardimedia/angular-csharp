using System;
using HtmlAgilityPack;

namespace AngularCsharp
{
    public class NgForOperation
    {
        /// <summary>
        /// Fix *ngFor for processing with HtmlAgilityPack.
        /// </summary>
        public static string FixHtml(string html)
        {
            return html.Replace(" *ngFor=", " ngfor=");
        }

        public static string GetParameters(string html)
        {
            html = NgForOperation.FixHtml(html);

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            HtmlNode htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//*[@ngfor]");

            return htmlNode.Attributes["ngfor"].Value;
        }

        public static string GetParameterCollectionName(string html)
        {
            string parameters = NgForOperation.GetParameters(html);

            // Calculate where collection name starts
            int posCollectionNameStart = parameters.IndexOf(" of ", StringComparison.Ordinal);
            if (posCollectionNameStart == -1)
            {
                throw new Exception($"Could not parse *ngFor parameters (of not found): {parameters}");
            }
            posCollectionNameStart = posCollectionNameStart + 4;

            // Calculate collection name length
            int posCollectionNameLength = parameters.IndexOf(" ", posCollectionNameStart, StringComparison.Ordinal);
            if (posCollectionNameLength == -1)
            {
                posCollectionNameLength = parameters.Length - posCollectionNameStart;
            }
            else
            {
                posCollectionNameLength = posCollectionNameLength - posCollectionNameStart;
            }

            // Get collection name
            string collectionName = parameters.Substring(posCollectionNameStart, posCollectionNameLength);

            return collectionName;
        }

        public static string GetParameterItemName(string html)
        {
            string parameters = NgForOperation.GetParameters(html);

            int posItemNameStart = 1;

            // Calculate item name length
            int posItemNameLength = parameters.IndexOf(" of ", posItemNameStart, StringComparison.Ordinal);
            if (posItemNameLength == -1)
            {
                throw new Exception($"Could not parse *ngFor parameters (of not found): {parameters}");
            }
            else
            {
                posItemNameLength = posItemNameLength - posItemNameStart;
            }

            // Get item name
            string itemName = parameters.Substring(posItemNameStart, posItemNameLength);

            return itemName;
        }
    }
}
