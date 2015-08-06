﻿using System;

namespace Dyna.Core.Entities
{
    public class Variable
    {
        /// <summary>
        /// Initialize a variable with a variable name.
        /// </summary>
        public Variable(string theName)
        {
            if (string.IsNullOrWhiteSpace(theName))
                throw new ArgumentException("theName");
            this.Name = theName;
        }

        /// <summary>
        /// Initialize a default variable.
        /// </summary>
        public Variable()
        {
            this.Name = string.Empty;
        }

        /// <summary>
        /// Gets or sets the variable name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the domain.
        /// </summary>
        public Domain Domain { get; internal set; }

        /// <summary>
        /// Gets or sets the model the variable is a part of.
        /// </summary>
        public Model Model { get; set; }

        /// <summary>
        /// Attach the variable to a shared domain.
        /// </summary>
        /// <param name="domain">Shared domain.</param>
        public void AttachTo(Domain domain)
        {
            if (domain == null)
                throw new ArgumentNullException("domain");
            this.Domain = domain;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
