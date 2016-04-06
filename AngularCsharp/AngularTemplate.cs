using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngularCsharp
{
    class AngularTemplate
    {
        #region Fields

        private object _DomTree;

        #endregion

        #region Constructors

        public AngularTemplate(string template)
        {
            _DomTree = ParseTemplate(template);
        }

        #endregion

        #region Public methods

        public string Render(object model)
        {
            // Call RenderNode for root element
            return RenderNode(_DomTree, model);
        }

        #endregion

        #region Private methods

        private object ParseTemplate(string template)
        {
            // Create DOM-Tree from template with 3rd party library
            return null;
        }
        
        private string RenderNode(object domElement, object model)
        {
            // Render DOM element, resolve *ng-if / *ng-for and variables
            foreach (var child in domElement.Childs)
            {
                RenderNode(child, model);
            }


        }

        #endregion
    }
}
