using System.Linq;
using AngularCSharp.Helpers;
using HtmlAgilityPack;

namespace AngularCSharp
{
    /// <summary>
    /// This is the entry point of AngularCSharp, it allows parsing and processing of Angular2 templates.
    /// </summary>
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

        /// <summary>
        /// Angular Service constructor
        /// </summary>
        /// <param name="template">HTML template with Angular2 syntax</param>
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

        /// <summary>
        /// Processes the template with specified model and returns plain HTML
        /// </summary>
        /// <param name="model">Model class which contains properties for Angular variables</param>
        /// <returns>Plain HTML string</returns>
        public string Render(object model)
        {
            return this.templateEngine.ProcessTemplate(this.htmlDocument, model);
        }

        #endregion
    }
}