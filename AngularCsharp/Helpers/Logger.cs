using System.Collections.Generic;

namespace AngularCsharp.Helpers
{
    public class Logger
    {
        #region Fields

        private List<string> warnings = new List<string>();

        #endregion

        #region Properties

        public string[] Warnings
        {
            get
            {
                return this.warnings.ToArray();
            }
        }

        public bool HasWarnings
        {
            get
            {
                return this.warnings.Count > 0;
            }
        }

        #endregion

        #region Public methods

        public void AddWarning(string warning)
        {
            this.warnings.Add(warning);
        }

        #endregion
    }
}