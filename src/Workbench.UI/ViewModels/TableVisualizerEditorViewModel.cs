using System;
using System.Diagnostics.Contracts;
using Caliburn.Micro;
using Workbench.Core.Models;
using Workbench.Messages;
using Workbench.Services;

namespace Workbench.ViewModels
{
    public class TableVisualizerEditorViewModel : EditorViewModel, IHandle<TableSelectionChanged>
    {
        private TableViewModel table;

        public TableVisualizerEditorViewModel(TableVisualizerModel theTableModel,
                                              IEventAggregator theEventAggregator,
                                              IDataService theDataService,
                                              IViewModelService theViewModelService)
            : base(theTableModel, theEventAggregator, theDataService, theViewModelService)
        {
            Contract.Requires<ArgumentNullException>(theTableModel != null);
            Contract.Requires<ArgumentNullException>(theEventAggregator != null);
            Contract.Requires<ArgumentNullException>(theDataService != null);
            Contract.Requires<ArgumentNullException>(theViewModelService != null);

            Model = theTableModel;
            this.eventAggregator.Subscribe(this);
            Table = new TableViewModel(theTableModel.Table, theEventAggregator);
        }

        /// <summary>
        /// Gets or sets the map view model.
        /// </summary>
        public TableViewModel Table
        {
            get { return this.table; }
            set
            {
                this.table = value;
                NotifyOfPropertyChange();
            }
        }

        public void AddColumn(TableColumnModel newColumn)
        {
            Contract.Requires<ArgumentNullException>(newColumn != null);
            Table.AddColumn(newColumn);
        }

        public void Resize(int columns, int rows)
        {
            Contract.Assume(Table != null);
            Table.Resize(columns, rows);
        }

        public void AddRow(TableRowModel newRow)
        {
            Contract.Requires<ArgumentNullException>(newRow != null);
            Contract.Assume(Table != null);
            Table.AddRow(newRow);
        }

        public void DeleteColumnSelected()
        {
            Table.DeleteColumnSelected();
        }

        public void DeleteRowSelected()
        {
            
        }

        /// <summary>
        /// Handle the table selection changed event.
        /// </summary>
        /// <param name="theMessage">Table selection changed event.</param>
        public void Handle(TableSelectionChanged theMessage)
        {
            // The grid "steals" focus from the container, so make the container selected again...
            IsSelected = true;
        }
    }
}
