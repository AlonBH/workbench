﻿using System.Collections.Generic;

namespace DynaApp.ViewModels
{
    /// <summary>
    /// View model for a variable.
    /// </summary>
    public sealed class VariableViewModel : GraphicViewModel
    {
        /// <summary>
        /// Initialize a variable with the new name.
        /// </summary>
        public VariableViewModel(string newName)
            : base(newName)
        {
            this.PopulateConnectors();
        }

        /// <summary>
        /// Gets the list of all connections attached to the variable. 
        /// </summary>
        public ICollection<ConnectionViewModel> AttachedConnections
        {
            get
            {
                List<ConnectionViewModel> attachedConnections = new List<ConnectionViewModel>();

                foreach (var connector in this.Connectors)
                {
                    if (connector.AttachedConnection != null)
                    {
                        attachedConnections.Add(connector.AttachedConnection);
                    }
                }

                return attachedConnections;
            }
        }

        private void PopulateConnectors()
        {
            this.Connectors.Add(new ConnectorViewModel());
            this.Connectors.Add(new ConnectorViewModel());
            this.Connectors.Add(new ConnectorViewModel());
            this.Connectors.Add(new ConnectorViewModel());
        }

        /// <summary>
        /// Is the destination graphic connectable to the variable?
        /// </summary>
        /// <param name="destinationGraphic">Destination being connected to.</param>
        /// <returns>True if the destination can be connected, False if it cannot be connected.</returns>
        public override bool IsConnectableTo(GraphicViewModel destinationGraphic)
        {
            // Variables cannot connect to other variables...
            var destinationAsVariable = destinationGraphic as VariableViewModel;
            if (destinationAsVariable != null) return false;

            return true;
        }
    }
}
