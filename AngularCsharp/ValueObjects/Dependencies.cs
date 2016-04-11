using AngularCSharp.Helpers;

namespace AngularCSharp.ValueObjects
{
    public class Dependencies
    {
        #region Properties

        public Logger Logger { get; set; }

        public ValueFinder ValueFinder;

        public ExpressionResolver ExpressionResolver;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructer, instanicates default dependencies
        /// </summary>
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
