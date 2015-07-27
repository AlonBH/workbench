﻿using System;

namespace DynaApp.Models
{
    [Serializable]
    public class ValueModel
    {
        public ValueModel(VariableModel variableModel)
        {
            this.Variable = variableModel;
        }

        public ValueModel()
        {
            this.Variable = new VariableModel();
        }

        public VariableModel Variable { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public int Value { get; set; }
    }
}
