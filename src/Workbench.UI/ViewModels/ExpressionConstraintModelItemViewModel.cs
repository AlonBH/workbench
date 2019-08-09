﻿using Caliburn.Micro;
using Workbench.Core.Models;
using Workbench.Validators;

namespace Workbench.ViewModels
{
    public sealed class ExpressionConstraintModelItemViewModel : ConstraintModelItemViewModel
    {
        private readonly IWindowManager _windowManager;

        public ExpressionConstraintModelItemViewModel(ExpressionConstraintModel theExpressionConstraint, IWindowManager theWindowManager)
            : base(theExpressionConstraint)
        {
            Validator = new ExpressionConstraintModelItemViewModelValidator();
            ExpressionText = theExpressionConstraint.Expression.Text;
            ExpressionConstraint = theExpressionConstraint;
            _windowManager = theWindowManager;
        }

        /// <summary>
        /// Gets or sets the expression constraint model.
        /// </summary>
        public ExpressionConstraintModel ExpressionConstraint { get; }

        public override void Edit()
        {
            var expressionConstraintEditor = new ExpressionConstraintEditorViewModel();
            expressionConstraintEditor.ConstraintName = ExpressionConstraint.Name;
            expressionConstraintEditor.ConstraintExpression = ExpressionConstraint.Expression.Text;
            var result = _windowManager.ShowDialog(expressionConstraintEditor);
            if (!result.HasValue) return;
            DisplayName = ExpressionConstraint.Name.Text = expressionConstraintEditor.ConstraintName;
            ExpressionText = ExpressionConstraint.Expression.Text = expressionConstraintEditor.ConstraintExpression;
        }
    }
}