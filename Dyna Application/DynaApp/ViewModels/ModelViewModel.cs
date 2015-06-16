﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using DynaApp.Views;

namespace DynaApp.ViewModels
{
    /// <summary>
    /// A view model for a model.
    /// </summary>
    public sealed class ModelViewModel : AbstractModelBase
    {
        /// <summary>
        /// Initialize a model view model with default values.
        /// </summary>
        public ModelViewModel()
        {
            this.Graphics = new ObservableCollection<GraphicViewModel>();
            this.Variables = new ObservableCollection<VariableViewModel>();
            this.Domains = new ObservableCollection<DomainViewModel>();
            this.Constraints = new ObservableCollection<ConstraintViewModel>();
            this.Connections = new ObservableCollection<ConnectionViewModel>();
        }

        /// <summary>
        /// Gets the collection of domains in the model.
        /// </summary>
        public ObservableCollection<VariableViewModel> Variables { get; private set; }

        /// <summary>
        /// Gets the collection of domains in the model.
        /// </summary>
        public ObservableCollection<DomainViewModel> Domains { get; private set; }

        /// <summary>
        /// Gets the collection of constraints in the model.
        /// </summary>
        public ObservableCollection<ConstraintViewModel> Constraints { get; private set; }

        /// <summary>
        /// Gets the collection of all graphic items in the model.
        /// </summary>
        public ObservableCollection<GraphicViewModel> Graphics { get; private set; }

        /// <summary>
        /// Gets the collection of connections in the model.
        /// </summary>
        public ObservableCollection<ConnectionViewModel> Connections { get; private set; }

        /// <summary>
        /// Add a new variable to the model.
        /// </summary>
        /// <param name="newVariableViewModel">New variable.</param>
        public void AddVariable(VariableViewModel newVariableViewModel)
        {
            if (newVariableViewModel == null)
                throw new ArgumentNullException("newVariableViewModel");
            this.Graphics.Add(newVariableViewModel);
            this.Variables.Add(newVariableViewModel);
        }

        /// <summary>
        /// Add a new domain to the model.
        /// </summary>
        /// <param name="newDomainViewModel">New domain.</param>
        public void AddDomain(DomainViewModel newDomainViewModel)
        {
            if (newDomainViewModel == null)
                throw new ArgumentNullException("newDomainViewModel");
            this.Graphics.Add(newDomainViewModel);
            this.Domains.Add(newDomainViewModel);
        }

        /// <summary>
        /// Add a new constraint to the model.
        /// </summary>
        /// <param name="newConstraintViewModel">New constraint.</param>
        public void AddConstraint(ConstraintViewModel newConstraintViewModel)
        {
            if (newConstraintViewModel == null)
                throw new ArgumentNullException("newConstraintViewModel");
            this.Graphics.Add(newConstraintViewModel);
            this.Constraints.Add(newConstraintViewModel);
        }

        /// <summary>
        /// Connect the variable to the graphic.
        /// </summary>
        /// <param name="fromVariable">Variable to connect.</param>
        /// <param name="toGraphic">Graphic to connect to.</param>
        public void Connect(VariableViewModel fromVariable, GraphicViewModel toGraphic)
        {
            Trace.Assert(fromVariable.IsConnectableTo(toGraphic));
            var connection = new ConnectionViewModel();
            connection.InitiateConnection(this.AssignConnector(fromVariable));
            connection.CompleteConnection(this.AssignConnector(toGraphic));
            this.Connections.Add(connection);
        }

        /// <summary>
        /// Called when the user has started to drag out a connector, thus creating a new connection.
        /// </summary>
        public ConnectionViewModel ConnectionDragStarted(ConnectorViewModel draggedOutConnector, Point curDragPoint)
        {
            if (draggedOutConnector.AttachedConnection != null)
            {
                //
                // There is an existing connection attached to the connector that has been dragged out.
                // Remove the existing connection from the view-model.
                //
                this.Connections.Remove(draggedOutConnector.AttachedConnection);
            }

            //
            // Create a new connection to add to the view-model.
            //
            var connection = new ConnectionViewModel();

            //
            // Link the source connector to the connector that was dragged out.
            //
            connection.InitiateConnection(draggedOutConnector);

            //
            // Set the position of destination connector to the current position of the mouse cursor.
            //
            connection.DestinationConnectorHotspot = curDragPoint;

            //
            // Add the new connection to the view-model.
            //
            this.Connections.Add(connection);

            return connection;
        }

        /// <summary>
        /// Called to query the application for feedback while the user is dragging the connection.
        /// </summary>
        public void QueryConnnectionFeedback(ConnectorViewModel draggedOutConnector, ConnectorViewModel draggedOverConnector, out object feedbackIndicator, out bool connectionOk)
        {
            if (draggedOutConnector == draggedOverConnector)
            {
                //
                // Can't connect to self!
                // Provide feedback to indicate that this connection is not valid!
                //
                feedbackIndicator = new ConnectionBadIndicator();
                connectionOk = false;
            }
            else
            {
                var sourceConnector = draggedOutConnector;
                var destinationConnector = draggedOverConnector;

                //
                // Only allow connections from output connector to input connector (ie each
                // connector must have a different type).
                // Also only allocation from one node to another, never one node back to the same node.
                //
                connectionOk = sourceConnector.Parent.IsConnectableTo(destinationConnector.Parent);

                if (connectionOk)
                {
                    // 
                    // Yay, this is a valid connection!
                    // Provide feedback to indicate that this connection is ok!
                    //
                    feedbackIndicator = new ConnectionOkIndicator();
                }
                else
                {
                    //
                    // Connectors with the same connector type (eg input & input, or output & output)
                    // can't be connected.
                    // Only connectors with separate connector type (eg input & output).
                    // Provide feedback to indicate that this connection is not valid!
                    //
                    feedbackIndicator = new ConnectionBadIndicator();
                }
            }
        }

        /// <summary>
        /// Called as the user continues to drag the connection.
        /// </summary>
        public void ConnectionDragging(ConnectionViewModel connection, Point curDragPoint)
        {
            if (connection.DestinationConnector == null)
            {
                connection.DestinationConnectorHotspot = curDragPoint;
            }
            else
            {
                connection.SourceConnectorHotspot = curDragPoint;
            }
        }

        /// <summary>
        /// Called when the user has finished dragging out the new connection.
        /// </summary>
        public void ConnectionDragCompleted(ConnectionViewModel newConnection, ConnectorViewModel connectorDraggedOut, ConnectorViewModel connectorDraggedOver)
        {
            if (connectorDraggedOver == null)
            {
                //
                // The connection was unsuccessful.
                // Maybe the user dragged it out and dropped it in empty space.
                //
                this.Connections.Remove(newConnection);
                return;
            }

            //
            // Only allow connections from output connector to input connector (ie each
            // connector must have a different type).
            // Also only allocation from one node to another, never one node back to the same node.
            //
            var connectionOk = connectorDraggedOut.Parent.IsConnectableTo(connectorDraggedOver.Parent);

            if (!connectionOk)
            {
                //
                // Connections between connectors that have the same type,
                // eg input -> input or output -> output, are not allowed,
                // Remove the connection.
                //
                this.Connections.Remove(newConnection);
                return;
            }

            //
            // The user has dragged the connection on top of another valid connector.
            //

            //
            // Remove any existing connection between the same two connectors.
            //
            var existingConnection = FindConnection(connectorDraggedOut, connectorDraggedOver);
            if (existingConnection != null)
            {
                this.Connections.Remove(existingConnection);
            }

            //
            // Finalize the connection by attaching it to the connector
            // that the user dragged the mouse over.
            //
            newConnection.CompleteConnection(connectorDraggedOver);
        }

        /// <summary>
        /// Delete the connection.
        /// </summary>
        /// <param name="connectionToDelete">Connection to delete.</param>
        public void DeleteConnection(ConnectionViewModel connectionToDelete)
        {
            this.Connections.Remove(connectionToDelete);
        }

        /// <summary>
        /// Assign an available connector.
        /// </summary>
        /// <param name="theGraphic">Graphic containing the connectors.</param>
        /// <returns>Available connector.</returns>
        private ConnectorViewModel AssignConnector(GraphicViewModel theGraphic)
        {
            return theGraphic.Connectors.FirstOrDefault(x => x.AttachedConnection == null);
        }

        /// <summary>
        /// Retrieve a connection between the two connectors.
        /// Returns null if there is no connection between the connectors.
        /// </summary>
        private ConnectionViewModel FindConnection(ConnectorViewModel sourceConnector, ConnectorViewModel destinationConnector)
        {
            var connection = sourceConnector.AttachedConnection;
            if (connection.DestinationConnector == destinationConnector)
            {
                //
                // Found a connection that is outgoing from the source connector
                // and incoming to the destination connector.
                //
                return connection;
            }

            return null;
        }
    }
}
