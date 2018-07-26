﻿using System;
using System.Diagnostics.Contracts;
using Caliburn.Micro;
using Workbench.Core.Models;

namespace Workbench.ViewModels
{
    /// <summary>
    /// All different constraint displayed in the model editor.
    /// </summary>
    public sealed class AllDifferentConstraintModelItemViewModel : ConstraintModelItemViewModel
    {
        private readonly IWindowManager _windowManager;

        public AllDifferentConstraintModelItemViewModel(AllDifferentConstraintModel theAllDifferentModel, IWindowManager theWindowManager)
            : base(theAllDifferentModel)
        {
            Contract.Requires<ArgumentNullException>(theAllDifferentModel != null);
            Contract.Requires<ArgumentNullException>(theWindowManager != null);

            AllDifferentConstraint = theAllDifferentModel;
            DisplayName = AllDifferentConstraint.Name;
            ExpressionText = AllDifferentConstraint.Expression.Text;
            _windowManager = theWindowManager;
        }

        /// <summary>
        /// Gets or sets the constraint model.
        /// </summary>
        public AllDifferentConstraintModel AllDifferentConstraint { get; }

        public override void Edit()
        {
            var allDifferentConstraintEditor = new AllDifferentConstraintEditorViewModel();
            allDifferentConstraintEditor.ConstraintName = AllDifferentConstraint.Name;
            allDifferentConstraintEditor.ConstraintExpression = AllDifferentConstraint.Expression.Text;
            var result = _windowManager.ShowDialog(allDifferentConstraintEditor);
            if (!result.HasValue) return;
            DisplayName = AllDifferentConstraint.Name.Text = allDifferentConstraintEditor.ConstraintName;
            ExpressionText = AllDifferentConstraint.Expression.Text = allDifferentConstraintEditor.ConstraintExpression;
        }
    }
}