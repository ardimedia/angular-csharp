﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AngularCsharp.Helpers;
using HtmlAgilityPack;

namespace AngularCsharp.ValueObjects
{
    public class NodeContext
    {
        #region Properties

        public ReadOnlyDictionary<string, object> CurrentVariables { get; private set; }

        public HtmlNode CurrentNode { get; private set; }

        /// <summary>
        /// TODO brauchts das?
        /// </summary>
        public HtmlDocument TargetDocument { get; private set; }

        public Dependencies Dependencies { get; private set; }

        public TemplateEngine TemplateEngine { get; private set; }

        #endregion

        #region Constructor

        public NodeContext(Dictionary<string,object> variables, HtmlNode node, HtmlDocument targetDocument, Dependencies dependencies, TemplateEngine templateEngine)
        {
            this.CurrentVariables = new ReadOnlyDictionary<string, object>(variables);
            this.CurrentNode = node;
            this.TargetDocument = targetDocument;
            this.Dependencies = dependencies;
            this.TemplateEngine = templateEngine;
        }

        #endregion

        #region Public methods

        public NodeContext ChangeContext(HtmlNode node)
        {
            var additionalVariables = new Dictionary<string, object>();
            return ChangeContext(additionalVariables, node);
        }

        /// <summary>
        /// Returns a new instance with the change context. The context of this instance will stay unchanged.
        /// </summary>
        public NodeContext ChangeContext(Dictionary<string, object> additionalVariables, HtmlNode node)
        {
            // Make a copy of current dictionary
            var dictionary = this.CurrentVariables.ToDictionary(entry => entry.Key, entry => entry.Value);

            // Add additional variables
            if (additionalVariables != null)
            {
                foreach (KeyValuePair<string, object> item in additionalVariables)
                {
                    if (dictionary.ContainsKey(item.Key))
                    {
                        dictionary[item.Key] = item.Value;
                    } else
                    {
                        dictionary.Add(item.Key, item.Value);
                    }
                }
            }

            // Return new NodeContext instance
            return new NodeContext(dictionary, node, this.TargetDocument, this.Dependencies, this.TemplateEngine);
        }

        #endregion
    }
}
