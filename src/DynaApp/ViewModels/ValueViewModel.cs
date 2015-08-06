﻿using System;

namespace DynaApp.ViewModels
{
    /// <summary>
    /// View model for a value bound to a variable.
    /// </summary>
    public sealed class ValueViewModel : AbstractViewModel
    {
        private int value;
        private VariableViewModel variable;

        /// <summary>
        /// Initialize the bound variable with a variable.
        /// </summary>
        /// <param name="theVariable">Variable to bind the value to.</param>
        public ValueViewModel(VariableViewModel theVariable)
        {
            if (theVariable == null)
                throw new ArgumentNullException("theVariable");
            this.Variable = theVariable;
        }

        /// <summary>
        /// Gets the variable name.
        /// </summary>
        public string Name
        {
            get { return this.Variable.Name; }
            set
            {
                this.Variable.Name = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the variable.
        /// </summary>
        public VariableViewModel Variable
        {
            get { return this.variable; }
            set
            {
                this.variable = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the bound value.
        /// </summary>
        public int Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
                OnPropertyChanged();
            }
        }
    }
}
