﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Dyna.Core.Models
{
    /// <summary>
    /// A model for specifying the problem.
    /// <remarks>Just a very simple finite integer domain at the moment.</remarks>
    /// </summary>
    [Serializable]
    public class ModelModel : ModelBase
    {
        private readonly List<string> errors = new List<string>();

        /// <summary>
        /// Initialize a model with a model name.
        /// </summary>
        /// <param name="theName">Model name.</param>
        public ModelModel(string theName)
            : this()
        {
            if (string.IsNullOrWhiteSpace(theName))
                throw new ArgumentException("theName");
            this.Name = theName;
        }

        /// <summary>
        /// Initialize a model model with default values.
        /// </summary>
        public ModelModel()
        {
            this.Variables = new List<VariableModel>();
            this.Domains = new List<DomainModel>();
            this.Constraints = new List<ConstraintModel>();
        }

        /// <summary>
        /// Gets or sets the model name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the variables.
        /// </summary>
        public List<VariableModel> Variables { get; set; }

        /// <summary>
        /// Gets the domains.
        /// </summary>
        public List<DomainModel> Domains { get; set; }

        /// <summary>
        /// Gets the constraints.
        /// </summary>
        public List<ConstraintModel> Constraints { get; set; }

        /// <summary>
        /// Add a new constraint to the model.
        /// </summary>
        /// <param name="newConstraint">New constraint.</param>
        public void AddConstraint(ConstraintModel newConstraint)
        {
            if (newConstraint == null)
                throw new ArgumentNullException("newConstraint");
            newConstraint.AssignIdentity();
            this.Constraints.Add(newConstraint);
        }

        /// <summary>
        /// Delete the constraint from the model.
        /// </summary>
        /// <param name="constraintToDelete">Constraint to delete.</param>
        public void DeleteConstraint(ConstraintModel constraintToDelete)
        {
            if (constraintToDelete == null)
                throw new ArgumentNullException("constraintToDelete");
            this.Constraints.Remove(constraintToDelete);
        }

        /// <summary>
        /// Add a new variable to the model.
        /// </summary>
        /// <param name="newVariable">New variable.</param>
        public void AddVariable(VariableModel newVariable)
        {
            if (newVariable == null)
                throw new ArgumentNullException("newVariable");
            newVariable.AssignIdentity();
            this.Variables.Add(newVariable);
        }

        /// <summary>
        /// Gets the model validation errors.
        /// </summary>
        public IEnumerable<String> Errors
        {
            get
            {
                return this.errors;
            }
        }

        /// <summary>
        /// Delete the variable from the model.
        /// </summary>
        /// <param name="variableToDelete">Variable to delete.</param>
        public void DeleteVariable(VariableModel variableToDelete)
        {
            if (variableToDelete == null)
                throw new ArgumentNullException("variableToDelete");
            this.Variables.Remove(variableToDelete);
        }

        public void AddDomain(DomainModel newDomain)
        {
            if (newDomain == null)
                throw new ArgumentNullException("newDomain");
            newDomain.AssignIdentity();
            this.Domains.Add(newDomain);
        }

        public void AddSharedDomain(DomainModel newDomain)
        {
            if (newDomain == null)
                throw new ArgumentNullException("newDomain");
            if (string.IsNullOrWhiteSpace(newDomain.Name))
                throw new ArgumentException("Shared domains must have a name.", "newDomain");
            this.Domains.Add(newDomain);
        }

        public void RemoveSharedDomain(DomainModel oldDomain)
        {
            if (oldDomain == null)
                throw new ArgumentNullException("oldDomain");
            this.Domains.Add(oldDomain);
        }

        /// <summary>
        /// Delete the domain from the model.
        /// </summary>
        /// <param name="domainToDelete">Domain to delete.</param>
        public void DeleteDomain(DomainModel domainToDelete)
        {
            if (domainToDelete == null)
                throw new ArgumentNullException("domainToDelete");
            this.Domains.Remove(domainToDelete);
        }

        /// <summary>
        /// Get the variable matching the variable name.
        /// </summary>
        /// <param name="theVariableName">The variable name.</param>
        /// <returns>Variable model.</returns>
        public VariableModel GetVariableByName(string theVariableName)
        {
            return this.Variables.FirstOrDefault(variable => variable.Name == theVariableName);
        }

        /// <summary>
        /// Validate the model and ensure consistency between the domains and Variables.
        /// <remarks>Populates errors into the <see cref="Errors"/> collection.</remarks>
        /// </summary>
        /// <returns>True if the model is valid, False if it is not valid.</returns>
        public bool Validate()
        {
            this.errors.Clear();
            var expressionsValid = this.ValidateConstraintExpressions();
            if (!expressionsValid) return false;
            return this.ValidateSharedDomains();
        }

        /// <summary>
        /// Get the shared domain matching the given name.
        /// </summary>
        /// <param name="theSharedDomainName">Shared domain name.</param>
        /// <returns>Shared domain matching the name.</returns>
        public DomainModel GetSharedDomainByName(string theSharedDomainName)
        {
            if (string.IsNullOrWhiteSpace(theSharedDomainName))
                throw new ArgumentException("theSharedDomainName");
            return this.Domains.FirstOrDefault(x => x.Name == theSharedDomainName);
        }

        /// <summary>
        /// Create a new model with the given name.
        /// </summary>
        /// <param name="theModelName">Model name.</param>
        /// <returns>Fluent interface context.</returns>
        public static ModelContext Create(string theModelName)
        {
            return new ModelContext(new ModelModel(theModelName));
        }

        private bool ValidateConstraintExpressions()
        {
            foreach (var aConstraint in this.Constraints)
            {
                if (this.Variables.FirstOrDefault(x => x.Name == aConstraint.Expression.Left.Name) == null)
                {
                    this.errors.Add(string.Format("Missing variable {0}", aConstraint.Expression.Right.Variable));
                    return false;
                }

                if (aConstraint.Expression.Right.IsVarable)
                {
                    if (this.Variables.FirstOrDefault(x => x.Name == aConstraint.Expression.Right.Variable.Name) == null)
                    {
                        this.errors.Add(string.Format("Missing variable {0}", aConstraint.Expression.Right.Variable));
                        return false;
                    }
                }
            }

            return true;
        }

        private bool ValidateSharedDomains()
        {
            foreach (var variable in this.Variables)
            {
                if (variable.Domain == null)
                {
                    this.errors.Add("Missing domain");
                    return false;
                }

                // Make sure the domain is a shared domain...
                if (string.IsNullOrWhiteSpace(variable.Domain.Name))
                    continue;

                var sharedDomain = this.GetSharedDomainByName(variable.Domain.Name);
                if (sharedDomain == null)
                {
                    this.errors.Add(string.Format("Missing shared domain {0}", variable.Domain.Name));
                    return false;
                }
            }

            return true;
        }
    }
}