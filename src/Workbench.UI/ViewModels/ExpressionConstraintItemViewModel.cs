﻿using System;
using System.Diagnostics.Contracts;
using Workbench.Core.Models;

namespace Workbench.ViewModels
{
    public sealed class ExpressionConstraintItemViewModel : ConstraintItemViewModel
    {
        public ExpressionConstraintItemViewModel(ExpressionConstraintModel theExpressionConstraint)
            : base(theExpressionConstraint)
        {
            Contract.Requires<ArgumentNullException>(theExpressionConstraint != null);
            ExpressionText = theExpressionConstraint.Expression.Text;
            ExpressionConstraint = theExpressionConstraint;
        }

        /// <summary>
        /// Gets or sets the constraint model.
        /// </summary>
        public ExpressionConstraintModel ExpressionConstraint { get; }
    }
}