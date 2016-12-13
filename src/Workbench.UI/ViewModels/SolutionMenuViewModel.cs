﻿using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using Workbench.Commands;
using Workbench.Core.Models;

namespace Workbench.ViewModels
{
    /// <summary>
    /// View model for the solution main menu.
    /// </summary>
    public class SolutionMenuViewModel
    {
        private readonly WorkspaceViewModel workspace;
        private readonly IWindowManager windowManager;

        /// <summary>
        /// Initialize the solution menu view model with default values.
        /// </summary>
        public SolutionMenuViewModel(IWindowManager theWindowManager, WorkspaceViewModel theWorkspace)
        {
            Contract.Requires<ArgumentNullException>(theWindowManager != null);
            Contract.Requires<ArgumentNullException>(theWorkspace != null);

            this.workspace = theWorkspace;
            this.windowManager = theWindowManager;
            AddChessboardVisualizerCommand = IoC.Get<AddChessboardVisualizerCommand>();
            AddGridVisualizerCommand = IoC.Get<AddMapVisualizerCommand>();
            EditSolutionCommand = IoC.Get<EditSolutionCommand>();
            EditGridVisualizerCommand = new CommandHandler(EditGridHandler, _ => CanEditGridExecute);
            AddColumnCommand = new CommandHandler(AddColumnHandler, _ => CanEditGridExecute);
        }

        /// <summary>
        /// Gets the Solution|Add Column command.
        /// </summary>
        public ICommand AddColumnCommand { get; private set; }

        /// <summary>
        /// Gets the Solution|Add Map command
        /// </summary>
        public ICommand AddGridVisualizerCommand { get; private set; }

        /// <summary>
        /// Gets the Solution|Add Chessboard command.
        /// </summary>
        public ICommand AddChessboardVisualizerCommand { get; private set; }

        /// <summary>
        /// Gets the Solution|Edit Solution command.
        /// </summary>
        public ICommand EditSolutionCommand { get; private set; }

        /// <summary>
        /// Gets the Solution|Edit Map command
        /// </summary>
        public ICommand EditGridVisualizerCommand { get; private set; }

        /// <summary>
        /// Gets whether the "Solution|Edit Grid" menu item can be executed.
        /// </summary>
        public bool CanEditGridExecute
        {
            get { return this.workspace.Solution.GetSelectedGridVisualizers().Any(); }
        }

        private void AddColumnHandler()
        {
            var selectedGridVisualizers = this.workspace.Solution.GetSelectedGridVisualizers();
            if (!selectedGridVisualizers.Any()) return;
            var selectedGridVisualizer = selectedGridVisualizers.First();
            var columnNameEditor = new ColumnNameEditorViewModel();
            var result = this.windowManager.ShowDialog(columnNameEditor);
            if (result.GetValueOrDefault())
            {
                selectedGridVisualizer.AddColumn(new GridColumnModel(columnNameEditor.ColumnName));
            }
        }

        private void EditGridHandler()
        {
            var selectedGridVisualizers = this.workspace.Solution.GetSelectedGridVisualizers();
            if (!selectedGridVisualizers.Any()) return;
            var selectedGridVisualizer = selectedGridVisualizers.First();
            var gridEditor = new GridEditorViewModel();
            gridEditor.Columns = selectedGridVisualizer.Model.Grid.Columns.Count;
            gridEditor.Rows = selectedGridVisualizer.Model.Grid.Rows.Count;
            var result = this.windowManager.ShowDialog(gridEditor);
            if (result.GetValueOrDefault())
            {
                selectedGridVisualizer.Resize(gridEditor.Columns, gridEditor.Rows);
            }
        }
    }
}
