using System;
using System.Collections.Generic;
using AngularCSharp.Exceptions;

namespace AngularCSharp.Helpers
{
    /// <summary>
    /// Resolves expresssions (for example in ngIf attribute)
    /// </summary>
    public class ExpressionResolver
    {
        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="valueFinder">ValueFinder dependency</param>
        /// <param name="logger">Logger dependency</param>
        public ExpressionResolver(ValueFinder valueFinder, Logger logger)
        {
            this.valueFinder = valueFinder;
            this.logger = logger;
        }

        #endregion

        #region Private properties

        private ValueFinder valueFinder { get; set; }

        private Logger logger { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Determines if the specified expressions is true
        /// </summary>
        /// <param name="expression">Returns true if the value of the expression is true or an available object</param>
        /// <param name="variables">Dictionary with all available variables</param>
        /// <returns></returns>
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
