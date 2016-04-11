using System.Collections.Generic;

namespace AngularCSharp.Helpers
{
    public class Logger
    {
        #region Fields

        private List<string> warnings = new List<string>();

        #endregion

        #region Properties

        /// <summary>
        /// Returns all warnings
        /// </summary>
        public string[] Warnings
        {
            get
            {
                return this.warnings.ToArray();
            }
        }

        /// <summary>
        /// Returns true when warnings are available
        /// </summary>
        public bool HasWarnings
        {
            get
            {
                return this.warnings.Count > 0;
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
            this.warnings.Add(warning);
        }

        #endregion
    }
}