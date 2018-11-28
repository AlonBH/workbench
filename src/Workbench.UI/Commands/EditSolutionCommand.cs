using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using Caliburn.Micro;
using Workbench.Core.Models;
using Workbench.ViewModels;

namespace Workbench.Commands
{
    public class EditSolutionCommand : CommandBase
    {
        private readonly IWindowManager _windowManager;
        private readonly WorkspaceViewModel _workspace;

        public EditSolutionCommand(IWindowManager theWindowManager, WorkspaceDocumentViewModel theDocument)
        {
            Contract.Requires<ArgumentNullException>(theWindowManager != null);
            Contract.Requires<ArgumentNullException>(theDocument != null);

            _windowManager = theWindowManager;
            _workspace = theDocument.Workspace;
        }

        public override void Execute(object parameter)
        {
            var solutionEditorViewModel = new SolutionEditorViewModel();
            solutionEditorViewModel.BindingExpressions = CreateVisualizerCollectionFrom(null);
            var showDialogResult = this._windowManager.ShowDialog(solutionEditorViewModel);
            if (showDialogResult.GetValueOrDefault())
            {
                UpdateBindingsFrom(solutionEditorViewModel.BindingExpressions);
            }
        }

        /// <summary>
        /// Update binding models from the visualizer expression editor view models.
        /// </summary>
        /// <param name="bindingExpressions">Binding expression editors.</param>
        private void UpdateBindingsFrom(IEnumerable<VisualizerExpressionEditorViewModel> bindingExpressions)
        {
            foreach (var visualizerEditor in bindingExpressions)
            {
                if (visualizerEditor.Id == default(int))
                {
                    // New expression
#if false
                    var aNewExpression = new VisualizerBindingExpressionModel(visualizerEditor.Text);
                    this._workspace.Display.AddBindingEpxression(aNewExpression);
#endif
                }
                else
                {
                    // Update existing expression
#if false
                    var visualizerBinding = this._workspace.Display.GetVisualizerBindingById(visualizerEditor.Id);
                    visualizerBinding.Text = visualizerEditor.Text;
#endif
                }
            }
        }

        /// <summary>
        /// Create binding visualizer editor view models from the expression models.
        /// </summary>
        /// <param name="bindings">Visualizer expression models.</param>
        /// <returns>View model editors for the expressions.</returns>
        private ObservableCollection<VisualizerExpressionEditorViewModel> CreateVisualizerCollectionFrom(IEnumerable<VisualizerBindingExpressionModel> bindings)
        {
            var visualizerExpressions = new ObservableCollection<VisualizerExpressionEditorViewModel>();
            foreach (var binding in bindings)
            {
                visualizerExpressions.Add(new VisualizerExpressionEditorViewModel(binding.Id, binding.Text));
            }

            return visualizerExpressions;
        }
    }
}
