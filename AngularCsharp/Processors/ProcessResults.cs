﻿using HtmlAgilityPack;

namespace AngularCSharp.Processors
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
