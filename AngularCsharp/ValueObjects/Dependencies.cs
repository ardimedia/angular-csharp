using AngularCSharp.Helpers;

namespace AngularCSharp.ValueObjects
{
    public class Dependencies
    {
        #region Properties

        public Logger Logger;

        public ValueFinder ValueFinder;

        public ExpressionResolver ExpressionResolver;

        #endregion

        #region Constructor

        public Dependencies()
        {
            GetDefaults();
        }

        #endregion

        #region Private methods

        public void GetDefaults()
        {
            this.Logger = new Logger();
            this.ValueFinder = new ValueFinder();
            this.ExpressionResolver = new ExpressionResolver(ValueFinder, this.Logger);
        }

        #endregion
    }
}
