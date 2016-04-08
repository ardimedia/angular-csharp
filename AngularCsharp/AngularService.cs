using System.Linq;
using AngularCsharp.Helpers;
using HtmlAgilityPack;

namespace AngularCsharp
{
    public class AngularService
    {
        #region Fields

        private HtmlDocument htmlDocument;

        private TemplateEngine templateEngine;

        #endregion

        #region Properties

        // TODO: 2016-04-07/hp: Redesign, should probably provide a Logger to the templateEngine, not access it in this way.
        public Logger Logger
        {
            get { return this.templateEngine.Dependencies.Logger; }
        }

        #endregion

        #region Constructors

        public AngularService(string template)
        {
            // Initialize engine
            this.templateEngine = new TemplateEngine();

            // Parse HTML
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(template);
            this.htmlDocument = htmlDocument;

            // Handle parse errors
            if (htmlDocument.ParseErrors.Count() > 0)
            {
                foreach (var parseError in htmlDocument.ParseErrors)
                {
                    this.Logger.AddWarning($"ParseError: Line [{parseError.Line}] Code [{parseError.Code}] Reason [{parseError.Reason}]");
                }
            }
        }

        #endregion

        #region Public methods

        public string Render(object model)
        {
            return this.templateEngine.ProcessTemplate(this.htmlDocument, model);
        }

        #endregion
    }
}