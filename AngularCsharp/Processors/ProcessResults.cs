using HtmlAgilityPack;
using System.Collections.Generic;

namespace AngularCSharp.Processors
{
    /// <summary>
    /// Contains processing results
    /// </summary>
    public class ProcessResults
    {
        #region Properties

        /// <summary>
        /// Output nodes, which are prepared from TemplateEngine and should be changed by processor classes
        /// </summary>
        public List<HtmlNode> OutputNodes = new List<HtmlNode>();

        /// <summary>
        /// Specifies, if processing of child nodes should be skipped after processing this node
        /// </summary>
        public bool SkipChildNodes { get; set; }

        /// <summary>
        /// Specified, if processing of this node should be stopped imediately after current processor
        /// </summary>
        public bool StopProcessing { get; set; }

        #endregion
    }
}
