using AngularCsharp.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngularCsharp.Helpers
{
    public class ExpressionResolver
    {
        #region Fields

        private ValueFinder valueFinder;

        #endregion

        #region Constructors

        public ExpressionResolver(ValueFinder valueFinder)
        {
            this.valueFinder = valueFinder;
        }

        #endregion

        #region Methods

        public virtual bool IsTrue(string expression, IDictionary<string, object> variables)
        {
            if (expression.Substring(0, 1) == "!")
            {
                return !IsTrue(expression.Substring(1), variables);
            }

            object value = null;

            try
            {
                value = this.valueFinder.GetObject(expression, variables);
            } catch (ValueNotFoundException ex)
            {
                // TODO: Log warning
            }

            if (value == null)
            {
                return false;
            }

            if (value is Boolean)
            {
                return (bool)value;
            }

            return true;
        }

        #endregion
    }
}
