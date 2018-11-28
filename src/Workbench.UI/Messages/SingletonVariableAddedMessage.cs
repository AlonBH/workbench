﻿using System;
using System.Diagnostics.Contracts;
using Workbench.Core.Models;

namespace Workbench.Messages
{
    /// <summary>
    /// Message sent when a new singleton variable is added to the model.
    /// </summary>
    public class SingletonVariableAddedMessage : WorkspaceChangedMessage
    {
        /// <summary>
        /// Initialize a new singleton variable added message with the new variable.
        /// </summary>
        /// <param name="theNewVariable">New variable.</param>
        public SingletonVariableAddedMessage(SingletonVariableModel theNewVariable)
        {
            Contract.Requires<ArgumentNullException>(theNewVariable != null);
            NewVariable = theNewVariable;
        }

        /// <summary>
        /// Gets the new variable.
        /// </summary>
        public SingletonVariableModel NewVariable { get; private set; }

        /// <summary>
        /// Gets the new variable name.
        /// </summary>
        public string NewVariableName
        {
            get
            {
                Contract.Assume(NewVariable != null);
                return NewVariable.Name;
            }
        }
    }
}
