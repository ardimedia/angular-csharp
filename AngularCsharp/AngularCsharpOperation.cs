using System.Collections.Generic;

namespace AngularCsharp
{
    public class AngularCsharpOperation
    {
        public static List<string> VerifyIfAllHaveBeenProcessed(string html)
        {
            List<string> warnings = new List<string>();

            if (html.Contains("{{") || html.Contains("}}"))
            {
                warnings.Add("Not all substitutions ({{...}}) have been processed.");
            }

            return warnings;
        }

        public static string ReplaceValues(string html, Dictionary<string, string> values)
        {
            foreach (KeyValuePair<string, string> value in values)
            {
                html = html.Replace("{{" + value.Key + "}}", value.Value);
            }

            return html;
        }
    }
}
