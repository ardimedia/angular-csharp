﻿using AngularCsharp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngularCsharp.ValueObjects
{
    public class Dependencies
    {
        #region Properties

        public Logger Logger;

        public ValueFinder ValueFinder;

        public ExpressionResolver ExpressionResolver;

        #endregion

        #region Constructor

        public Dependencies()
        {
            GetDefaults();
        }

        #endregion

        #region Private methods

        public void GetDefaults()
        {
            Logger = new Logger();
            ValueFinder = new ValueFinder();
            ExpressionResolver = new ExpressionResolver(ValueFinder);
        }

        #endregion
    }
}