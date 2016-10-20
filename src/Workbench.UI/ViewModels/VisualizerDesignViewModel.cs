using System;
using System.Diagnostics.Contracts;
using Caliburn.Micro;
using Workbench.Core.Models;
using Workbench.Messages;
using Workbench.Services;

namespace Workbench.ViewModels
{
    public abstract class VisualizerDesignViewModel : GraphicViewModel,
                                                      IHandle<SingletonVariableAddedMessage>,
                                                      IHandle<AggregateVariableAddedMessage>,
                                                      IHandle<VariableDeletedMessage>
    {
        private IObservableCollection<VariableViewModel> availableVariables;
#if false
        private VariableViewModel selectedVariable;
#endif
        protected IEventAggregator eventAggregator;
        protected IDataService dataService;
        protected IViewModelService viewModelService;
        private VisualizerModel model;

        protected VisualizerDesignViewModel(GraphicModel theConstraintModel,
                                            IEventAggregator theEventAggregator,
                                            IDataService theDataService,
                                            IViewModelService theViewModelService)
            : base(theConstraintModel)
        {
            Contract.Requires<ArgumentNullException>(theConstraintModel != null);
            Contract.Requires<ArgumentNullException>(theEventAggregator != null);
            Contract.Requires<ArgumentNullException>(theDataService != null);
            Contract.Requires<ArgumentNullException>(theViewModelService != null);

            AvailableVariables = new BindableCollection<VariableViewModel>();
            this.eventAggregator = theEventAggregator;
            this.dataService = theDataService;
            this.viewModelService = theViewModelService;
        }

        /// <summary>
        /// Gets or sets the available variables available to bind to.
        /// </summary>
        public IObservableCollection<VariableViewModel> AvailableVariables
        {
            get { return availableVariables; }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                availableVariables = value;
                NotifyOfPropertyChange();
            }
        }

#if false
        /// <summary>
        /// Gets or sets the selected variable to bind to.
        /// </summary>
        public VariableViewModel SelectedVariable
        {
            get { return this.selectedVariable; }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                this.selectedVariable = value;
                if (this.SelectedVariable != null)
                {
                    var variableToBindTo = this.dataService.GetVariableByName(this.SelectedVariable.Name);
                    Model.ExecuteWith(variableToBindTo);
    }
                NotifyOfPropertyChange();

                var newVariableBoundMessage 
                    = new VisualizerBoundMessage(this.Model, this.SelectedVariable);
                this.eventAggregator.BeginPublishOnUIThread(newVariableBoundMessage);
            }
        }
#endif

        /// <summary>
        /// Gets or sets the variable model.
        /// </summary>
        public new VisualizerModel Model
        {
            get { return this.model; }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null);

                base.Model = value;
                this.model = value;
            }
        }

        /// <summary>
        /// Handle the singleton variable added message.
        /// </summary>
        /// <param name="theMessage">Variable added message.</param>
        public void Handle(SingletonVariableAddedMessage theMessage)
        {
            this.AvailableVariables.Add(theMessage.NewVariable);
        }

        /// <summary>
        /// Handle the aggregate variable added message.
        /// </summary>
        /// <param name="message">Variable added message.</param>
        public void Handle(AggregateVariableAddedMessage message)
        {
            this.AvailableVariables.Add(message.Added);
        }

        /// <summary>
        /// Handle the variable delete message.
        /// </summary>
        /// <param name="message">Variable deleted message.</param>
        public void Handle(VariableDeletedMessage message)
        {
            this.AvailableVariables.Remove(message.Deleted);
#if false
            if (this.SelectedVariable == message.Deleted)
                this.SelectedVariable = null;

#endif
        }

        /// <summary>
        /// Called when initializing the visualizer.
        /// </summary>
        protected override void OnInitialize()
        {
            base.OnInitialize();
            this.PopulateAvailableVariables();
        }

        /// <summary>
        /// Populate the available variables collection.
        /// </summary>
        private void PopulateAvailableVariables()
        {
            this.AvailableVariables.Clear();
            var allVariables = this.viewModelService.GetAllVariables();
            foreach (var aVariable in allVariables)
            {
                this.AvailableVariables.Add(aVariable);
            }
        }

        protected void SelectVariableBinding()
        {
#if false
            this.selectedVariable = this.viewModelService.GetVariableByIdentity(this.Model.Binding.VariableId);
#endif
        }
    }
}
