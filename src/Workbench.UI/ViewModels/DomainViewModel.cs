﻿using System;
using System.Diagnostics.Contracts;
using Workbench.Core.Models;

namespace Workbench.ViewModels
{
    /// <summary>
    /// View model for a domain.
    /// </summary>
    public sealed class DomainViewModel : GraphicViewModel
    {
        private DomainGraphicModel model;

        public DomainViewModel(DomainGraphicModel theDomainModel)
            : base(theDomainModel)
        {
            Contract.Requires<ArgumentNullException>(theDomainModel != null);
            this.Model = theDomainModel;
            this.Expression = new DomainExpressionViewModel(theDomainModel.Expression);
        }

        /// <summary>
        /// Gets or sets the domain expression.
        /// </summary>
        public DomainExpressionViewModel Expression { get; set; }

        /// <summary>
        /// Get whether the domain expression is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.Expression.Text);
            }
        }

        /// <summary>
        /// Gets or sets the domain model.
        /// </summary>
        public new DomainGraphicModel Model
        {
            get { return this.model; }
            set
            {
                base.Model = value;
                this.model = value;
            }
        }
    }
}
