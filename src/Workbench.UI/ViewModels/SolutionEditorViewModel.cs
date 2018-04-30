using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using Caliburn.Micro;

namespace Workbench.ViewModels
{
    public class SolutionEditorViewModel : Screen
    {
        private ObservableCollection<VisualizerExpressionEditorViewModel> bindingExpressions;
        private VisualizerExpressionEditorViewModel selectedExpression;
        private string currentExpression;

        /// <summary>
        /// Initialize a solution editor with default values.
        /// </summary>
        public SolutionEditorViewModel()
        {
            BindingExpressions = new ObservableCollection<VisualizerExpressionEditorViewModel>();
            CurrentExpression = string.Empty;
            SelectedExpression = new VisualizerExpressionEditorViewModel();
        }

        /// <summary>
        /// Gets or sets the expression in the add/edit editor.
        /// </summary>
        public string CurrentExpression
        {
            get { return this.currentExpression; }
            set
            {
                this.currentExpression = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets the selected expression in the expression list.
        /// </summary>
        public VisualizerExpressionEditorViewModel SelectedExpression
        {
            get { return this.selectedExpression; }
            set
            {
                this.selectedExpression = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets the visualizer binding expression.
        /// </summary>
        public ObservableCollection<VisualizerExpressionEditorViewModel> BindingExpressions
        {
            get { return this.bindingExpressions; }
            set
            {
                this.bindingExpressions = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Add a new visualizer expression.
        /// </summary>
        public void AddOrUpdateExpression()
        {
            Contract.Assume(!string.IsNullOrWhiteSpace(CurrentExpression));
            // Assume it is new for now...
            var newExpressionEditor = new VisualizerExpressionEditorViewModel(CurrentExpression);
            BindingExpressions.Add(newExpressionEditor);
            SelectedExpression = newExpressionEditor;
            CurrentExpression = string.Empty;
        }

        /// <summary>
        /// Delete a visualizer expression.
        /// </summary>
        public void DeleteExpression()
        {
            Contract.Assume(SelectedExpression != null);
            BindingExpressions.Remove(SelectedExpression);
            SelectedExpression = new VisualizerExpressionEditorViewModel();
        }
        
        /// <summary>
        /// Okay button clicked.
        /// </summary>
        public void AcceptButton()
        {
            TryClose(true);
        }
    }
}
