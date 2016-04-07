using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngularCsharp.Processors
{
    public class ProcessResults
    {
        #region Properties

        public HtmlNode[] OutputNodes;

        public bool SkipChildNodes { get; set; }

        public bool StopProcessing { get; set; }

        #endregion
    }
}
