using HtmlAgilityPack;
using System.Collections.Generic;

namespace AngularCSharp.Processors
{
    public class ProcessResults
    {
        #region Properties

        public List<HtmlNode> OutputNodes = new List<HtmlNode>();

        public bool SkipChildNodes { get; set; }

        public bool StopProcessing { get; set; }

        #endregion
    }
}
