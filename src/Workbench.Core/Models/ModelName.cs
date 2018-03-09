﻿using System;
using System.Diagnostics.Contracts;

namespace Workbench.Core.Models
{
    [Serializable]
    public sealed class ModelName : AbstractModel
    {
        private string _name;

        /// <summary>
        /// Initializes a new model name with a default name.
        /// </summary>
        /// <param name="theName"></param>
        public ModelName(string theName)
        {
            Contract.Requires<ArgumentNullException>(theName != null);
            Text = theName;
        }

        /// <summary>
        /// Initializes a new model name with default values.
        /// </summary>
        public ModelName()
        {
            Text = string.Empty;
        }

        public static implicit operator string(ModelName aName) =>
            // Convert the model name into the name as a string
            aName.Text;

        public string Text
        {
            get { return _name; }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null);
                _name = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Is the name equal to the model name?
        /// </summary>
        /// <param name="theName">Name string</param>
        /// <returns>True if equal, False if not equal.</returns>
        public bool IsEqualTo(string theName)
        {
            return Text == theName;
        }
    }
}
