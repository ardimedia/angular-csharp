using AngularCsharp.Helpers;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngularCsharp
{
    public class AngularService
    {
        #region Fields

        private HtmlDocument htmlDocument;

        private TemplateEngine engine;

        #endregion

        #region Properties

        public Logger Logger
        {
            get { return this.engine.Dependencies.Logger; }
        }

        #endregion

        #region Constructors

        public AngularService(string template)
        {
            // Parse HTML
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(template);
            this.htmlDocument = htmlDocument;

            // Load engine
            this.engine = new TemplateEngine();
        }

        #endregion

        #region Public methods

        public string Render(object model)
        {
            return this.engine.ProcessTemplate(htmlDocument, model);
        }

        #endregion
    }
}
