using System.Collections.Generic;

namespace AngularCSharp.Helpers
{
    public class Logger
    {
        #region Constructor

        public Logger()
        {
            WarningsList = new List<string>();
        }

        #endregion

        #region Properties

        private List<string> WarningsList { get; set; }

        /// <summary>
        /// Returns all warnings
        /// </summary>
        public string[] Warnings
        {
            get
            {
                return this.WarningsList.ToArray();
            }
        }

        /// <summary>
        /// Returns true when warnings are available
        /// </summary>
        public bool HasWarnings
        {
            get
            {
                return this.WarningsList.Count > 0;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Add warning for later retrieval
        /// </summary>
        /// <param name="warning"></param>
        public void AddWarning(string warning)
        {
            this.WarningsList.Add(warning);
        }

        #endregion
    }
}