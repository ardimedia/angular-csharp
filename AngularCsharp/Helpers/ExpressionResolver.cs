using System;
using System.Collections.Generic;
using AngularCSharp.Exceptions;

namespace AngularCSharp.Helpers
{
    public class ExpressionResolver
    {
        #region Fields

        private ValueFinder valueFinder;

        private Logger logger;

        #endregion

        #region Constructors

        public ExpressionResolver(ValueFinder valueFinder, Logger logger)
        {
            this.valueFinder = valueFinder;
            this.logger = logger;
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
            } catch (ValueNotFoundException)
            {
                this.logger.AddWarning($"Value { expression } not found");
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
