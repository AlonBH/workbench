﻿using System;
using System.Diagnostics.Contracts;
using Caliburn.Micro;
using Workbench.Core.Models;
using Workbench.Core.Solver;
using Workbench.Messages;
using Workbench.Services;
using System.Collections.Generic;
using System.Windows.Input;

namespace Workbench.ViewModels
{
    /// <summary>
    /// View model for the aplication work area.
    /// </summary>
    public sealed class WorkspaceViewModel : Conductor<IScreen>.Collection.OneActive
    {
        private bool isDirty;
        private readonly IEventAggregator eventAggregator;
        private readonly IWindowManager windowManager;
        private ModelEditorTabViewModel _modelEditor;
        private readonly IViewModelFactory _viewModelFactory;

        /// <summary>
        /// Initialize a work area view model with a data service, window manager and event aggregator.
        /// </summary>
        public WorkspaceViewModel(IDataService theDataService,
                                 IWindowManager theWindowManager,
                                 IEventAggregator theEventAggregator,
                                 IViewModelFactory theViewModelFactory)
        {
            Contract.Requires<ArgumentNullException>(theDataService != null);
            Contract.Requires<ArgumentNullException>(theWindowManager != null);
            Contract.Requires<ArgumentNullException>(theEventAggregator != null);
            Contract.Requires<ArgumentNullException>(theViewModelFactory != null);

            this.eventAggregator = theEventAggregator;
            this.windowManager = theWindowManager;
            _viewModelFactory = theViewModelFactory;

            WorkspaceModel = theDataService.GetWorkspace();
            Solution = WorkspaceModel.Solution;
            ChessboardVisualizers = new BindableCollection<IScreen>();
            TableVisualizers = new BindableCollection<IScreen>();
            DeleteCommand = new CommandHandler(DeleteAction);
        }

        /// <summary>
        /// Gets or sets the workspace model.
        /// </summary>
        public WorkspaceModel WorkspaceModel { get; }

        /// <summary>
        /// Gets the display model.
        /// </summary>
        public DisplayModel Display
        {
            get { return WorkspaceModel.Display; }
        }

        /// <summary>
        /// Gets the Delete command.
        /// </summary>
        public ICommand DeleteCommand { get; }

        /// <summary>
        /// Gets or sets the workspace editor.
        /// </summary>
        public ModelEditorTabViewModel ModelEditor
        {
            get
            {
                return this._modelEditor;
            }
            set
            {
                this._modelEditor = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets all chessboard visualizers.
        /// </summary>
        public BindableCollection<IScreen> ChessboardVisualizers { get; }

        /// <summary>
        /// Gets all table visualizers.
        /// </summary>
        public BindableCollection<IScreen> TableVisualizers { get; }

        /// <summary>
        /// Gets or sets the solution model.
        /// </summary>
        public SolutionModel Solution { get; }

        /// <summary>
        /// Gets or sets the work area dirty flag.
        /// </summary>
        public bool IsDirty
        {
            get { return this.isDirty; }
            set
            {
                if (this.isDirty == value) return;
                this.isDirty = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Solve the constraint model.
        /// </summary>
        public SolveResult SolveModel()
        {
            var isModelValid = Validate();
            if (!isModelValid) return SolveResult.InvalidModel;
            var solveResult = WorkspaceModel.Solve();
            if (!solveResult.IsSuccess) return SolveResult.Failed;
            DisplaySolution(WorkspaceModel.Solution);
            this.eventAggregator.PublishOnUIThread(new ModelSolvedMessage(solveResult));

            return solveResult;
        }

        /// <summary>
        /// Create a new singleton variable.
        /// </summary>
        /// <param name="newVariable">New variable.</param>
        /// <returns>New singleton variable view model.</returns>
        public void AddSingletonVariable(SingletonVariableModel newVariable)
        {
            Contract.Requires<ArgumentNullException>(newVariable != null);

            ModelEditor.AddSingletonVariable(newVariable);
            IsDirty = true;
            this.eventAggregator.PublishOnUIThread(new SingletonVariableAddedMessage(newVariable));
        }

        /// <summary>
        /// Create a new aggregate variable.
        /// </summary>
        /// <param name="newVariable">New variable name.</param>
        /// <returns>New aggregate variable view model.</returns>
        public void AddAggregateVariable(AggregateVariableModel newVariable)
        {
            Contract.Requires<ArgumentNullException>(newVariable != null);

            ModelEditor.AddAggregateVariable(newVariable);
            IsDirty = true;
            this.eventAggregator.PublishOnUIThread(new AggregateVariableAddedMessage(newVariable));
        }

        /// <summary>
        /// Create a new domain at a specific location.
        /// </summary>
        /// <param name="newDomain">New domain.</param>
        public void AddDomain(DomainModel newDomain)
        {
            Contract.Requires<ArgumentNullException>(newDomain != null);

            ModelEditor.AddDomain(newDomain);
            IsDirty = true;
        }

        /// <summary>
        /// Create a new expression constraint.
        /// </summary>
        /// <param name="newConstraint">New constraint name.</param>
        public void AddExpressionConstraint(ExpressionConstraintModel newConstraint)
        {
            Contract.Requires<ArgumentNullException>(newConstraint != null);

            ModelEditor.AddConstraint(newConstraint);
            IsDirty = true;
        }

        /// <summary>
        /// Create a new all different constraint.
        /// </summary>
        /// <param name="newConstraint">New all different constraint.</param>
        public void AddAllDifferentConstraint(AllDifferentConstraintModel newConstraint)
        {
            Contract.Requires<ArgumentNullException>(newConstraint != null);

            ModelEditor.AddConstraint(newConstraint);
            IsDirty = true;
        }

        /// <summary>
        /// Add a new chessboard visualizer to the workspace.
        /// </summary>
        /// <param name="newVisualizer">New chessboard visualizer.</param>
        public void AddChessboardVisualizer(ChessboardVisualizerModel newVisualizer)
        {
            Contract.Requires<ArgumentNullException>(newVisualizer != null);

//            ChessboardVisualizers.Add(newVisualizer);
            IsDirty = true;
        }

        /// <summary>
        /// Add a new table visualizer to the workspace.
        /// </summary>
        /// <param name="newVisualizer">New table visualizer.</param>
        public void AddTableVisualizer(TableModel newVisualizer)
        {
            Contract.Requires<ArgumentNullException>(newVisualizer != null);

//            TableVisualizers.Add(newVisualizer);
            IsDirty = true;
        }

        /// <summary>
        /// Delete all selected graphics.
        /// </summary>
        public void DeleteSelectedGraphics()
        {
            /*
             * TODO: this is awful. need something that ties both the editor and 
             * viewer together so that both can be deleted easily. If I'm on the 
             * editor and move to the viewer I probably will expect the editor that 
             * is selected on the editor to be selected on the viewer as well.
             */
        }

        /// <summary>
        /// Delete the variable from the view-model.
        /// </summary>
        public void DeleteVariable(VariableModel variable)
        {
            Contract.Requires<ArgumentNullException>(variable != null);

            ModelEditor.DeleteVariable(variable);
            IsDirty = true;
            this.eventAggregator.PublishOnUIThread(new VariableDeletedMessage(variable));
        }

        public void BindTo(SolutionModel theSolution)
        {
        }

        public bool CanDeleteSelectedExecute()
        {
#if false
            if (SelectedDisplay == "Editor")
            {
                return Editor.Items.Any(_ => _.IsSelected);
            }
            else
            {
                throw new NotImplementedException("Selection is not implemented for the viewer");
            }
#else
            return false;
#endif
        }

        /// <summary>
        /// Get all selected grid visualizers.
        /// </summary>
        /// <returns>Collection of selected grid visualizers.</returns>
        public IReadOnlyCollection<IScreen> GetSelectedTableVisualizers()
        {
#if false
            if (SelectedDisplay == "Editor")
            {
                return TableVisualizers.Where(gridVisualizer => gridVisualizer.Editor.IsSelected)
                    .ToList();
            }
            return TableVisualizers.Where(gridVisualizer => gridVisualizer.Viewer.IsSelected)
                                  .ToList();
#else
            return null;
#endif
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            ModelEditor = _viewModelFactory.CreateModelEditor();
            Items.Add(ModelEditor);
        }

        /// <summary>
        /// Display the solution.
        /// </summary>
        /// <param name="theSolution">A valid solution.</param>
        private void DisplaySolution(SolutionModel theSolution)
        {
            BindTo(theSolution);
        }

        /// <summary>
        /// Solve the model.
        /// </summary>
        private bool Validate()
        {
            var validationContext = new ModelValidationContext();
            var isModelValid = WorkspaceModel.Model.Validate(validationContext);
            if (isModelValid) return true;
            Contract.Assume(validationContext.HasErrors);
            DisplayErrorDialog(validationContext);

            return false;
        }

        /// <summary>
        /// Display a dialog box with a display of all of the model errors.
        /// </summary>
        /// <param name="theContext">Validation context.</param>
        private void DisplayErrorDialog(ModelValidationContext theContext)
        {
            var errorsViewModel = CreateModelErrorsFrom(theContext);
            this.windowManager.ShowDialog(errorsViewModel);
        }

        /// <summary>
        /// Create a model errros view model from a model.
        /// </summary>
        /// <param name="theContext">Validation context.</param>
        /// <returns>View model with all errors in the model.</returns>
        private static ModelErrorsViewModel CreateModelErrorsFrom(ModelValidationContext theContext)
        {
            Contract.Requires<ArgumentNullException>(theContext != null);
            Contract.Requires<InvalidOperationException>(theContext.HasErrors);

            var errorsViewModel = new ModelErrorsViewModel();
            foreach (var error in theContext.Errors)
            {
                var errorViewModel = new ModelErrorViewModel
                {
                    Message = error
                };
                errorsViewModel.Errors.Add(errorViewModel);
            }

            return errorsViewModel;
        }

        /// <summary>
        /// Delete all selected graphics.
        /// </summary>
        private void DeleteAction()
        {
            DeleteSelectedGraphics();
//            TitleBar.UpdateTitle();
        }

        [ContractInvariantMethod]
        private void WorkspaceInvariants()
        {
            Contract.Invariant(WorkspaceModel != null);
        }
    }
}
