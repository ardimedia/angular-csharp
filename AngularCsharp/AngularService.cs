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

        private HtmlDocument _DomTree;

        private TemplateEngine _Engine;

        #endregion

        #region Constructors

        public AngularService(string template)
        {
            // Parse HTML
            var doc = new HtmlDocument();
            doc.LoadHtml(template);
            _DomTree = doc;

            // Load engine
            _Engine = new TemplateEngine();
        }

        #endregion

        #region Public methods

        public string Render(object model)
        {
            return _Engine.ProcessTemplate(_DomTree, model);
        }

        #endregion
    }
}
