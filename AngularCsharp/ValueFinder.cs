using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngularCsharp
{
    /// <summary>
    /// Finds an value in the variables dictionary based on Angular expression syntax, see https://angular.io/docs/ts/latest/guide/template-syntax.html#!#template-expressions
    /// </summary>
    class ValueFinder
    {
        public string GetString(string expression, Dictionary<string, object> variables)
        {
            var obj = GetObject(expression, variables);
            if (obj != null)
            {
                return obj.ToString();
            }

            // TODO: Add warning because object is null

            // Return empty string
            return String.Empty;
        }

        public IEnumerable GetList(string expression, Dictionary<string, object> variables)
        {
            var list = GetObject(expression, variables);
            if (!(list is IEnumerable))
            {
                return (IEnumerable)list;
            }
            
            // TODO: Add warning about wrong variable

            // Return object array with found item
            return new Object[1] { list };
        }

        private object GetObject(string expression, Dictionary<string, object> variables)
        {
            // TODO: Find the object in variables
            return null;
        }
    }
}
