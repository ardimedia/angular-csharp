using AngularCSharp.Helpers;

namespace AngularCSharp.ValueObjects
{
    public class Dependencies
    {
        #region Constructor

        /// <summary>
        /// Constructer, instanicates default dependencies
        /// </summary>
        public Dependencies()
        {
            GetDefaults();
        }

        #endregion

        #region Public properties

        public Logger Logger { get; set; }

        public ValueFinder ValueFinder { get; set; }

        public ExpressionResolver ExpressionResolver { get; set; }

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
