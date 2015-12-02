﻿using System;
using System.Windows.Input;
using Caliburn.Micro;
using Dyna.Core.Models;

namespace DynaApp.ViewModels
{
    /// <summary>
    /// View model for a variable domain expression.
    /// </summary>
    public sealed class VariableDomainExpressionViewModel : PropertyChangedBase
    {
        private string text;
        private bool isExpressionEditing;

        /// <summary>
        /// Initialize a variable domain expression with a raw expression.
        /// </summary>
        /// <param name="rawExpression">Raw expression text.</param>
        public VariableDomainExpressionViewModel(string rawExpression)
        {
            if (string.IsNullOrWhiteSpace(rawExpression))
                throw new ArgumentException("rawExpression");
            this.Model = new VariableDomainExpressionModel();
            this.Text = rawExpression;
        }

        /// <summary>
        /// Initialize a variable domain expression with default values.
        /// </summary>
        public VariableDomainExpressionViewModel()
        {
            this.Model = new VariableDomainExpressionModel();
            this.Text = string.Empty;
        }

        /// <summary>
        /// Gets or sets the variable domain expression model.
        /// </summary>
        public VariableDomainExpressionModel Model { get; set; }

        /// <summary>
        /// Gets or sets the domain expression text.
        /// </summary>
        public string Text
        {
            get { return this.text; }
            set
            {
                if (this.text == value) return;
                this.text = value;
                this.Model.Text = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets whether the expression is being edited.
        /// </summary>
        public bool IsExpressionEditing
        {
            get { return this.isExpressionEditing; }
            set
            {
                if (this.isExpressionEditing == value) return;
                this.isExpressionEditing = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets the domain expression edit command.
        /// </summary>
        public ICommand EditExpressionCommand
        {
            get
            {
                return new CommandHandler(() => this.IsExpressionEditing = true, _ => true);
            }
        }
    }
}
