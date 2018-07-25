using System.Diagnostics;
using Caliburn.Micro;
using Workbench.Core.Models;
using Workbench.ViewModels;

namespace Workbench.Services
{
    /// <summary>
    /// Maps a variable model into a view model.
    /// </summary>
    public sealed class VariableMapper
    {
        private readonly IEventAggregator eventAggregator;

        public VariableMapper(IEventAggregator theEventAggregator)
        {
            this.eventAggregator = theEventAggregator;
        }

        public SingletonVariableModelItemViewModel MapFrom(SingletonVariableGraphicModel theVariableModel)
        {
            Debug.Assert(theVariableModel.HasIdentity);

#if false
            var variableViewModel = new SingletonVariableViewModel(theVariableModel, this.eventAggregator);

            this.cache.CacheVariable(variableViewModel);

            return variableViewModel;
#else
            return null;
#endif
        }

        public AggregateVariableModelItemViewModel MapFrom(AggregateVariableGraphicModel theVariableModel)
        {
            Debug.Assert(theVariableModel.HasIdentity);

#if false
            var variableViewModel = new AggregateVariableViewModel(theVariableModel, this.eventAggregator);

            this.cache.CacheVariable(variableViewModel);

            return variableViewModel;
#else
            return null;
#endif
        }
    }
}