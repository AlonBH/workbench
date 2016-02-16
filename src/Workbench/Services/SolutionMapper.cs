﻿using System;
using Workbench.Core.Models;
using Workbench.ViewModels;

namespace Workbench.Services
{
    /// <summary>
    /// Map the solution model into a view model.
    /// </summary>
    internal class SolutionMapper
    {
        private readonly ValueMapper valueMapper;
        private ViewModelCache viewModelCache;

        internal SolutionMapper(ViewModelCache theCache)
        {
            if (theCache == null)
                throw new ArgumentNullException("theCache");
            this.viewModelCache = theCache;
            this.valueMapper = new ValueMapper(theCache);
        }

        /// <summary>
        /// Map a solution model into a view model.
        /// </summary>
        /// <param name="theSolutionModel">Solution model.</param>
        /// <returns>Solution view model.</returns>
        internal SolutionViewerViewModel MapFrom(SolutionModel theSolutionModel)
        {
            var solutionViewModel = new SolutionViewerViewModel(theSolutionModel);
            foreach (var valueModel in theSolutionModel.SingletonValues)
            {
                solutionViewModel.AddValue(this.valueMapper.MapFrom(valueModel));
            }

            foreach (var aVisualizer in theSolutionModel.Display.Visualizers)
            {
                var newViewer = new VariableVisualizerViewerViewModel(aVisualizer);
                if (aVisualizer.Binding != null)
                    newViewer.Binding = this.viewModelCache.GetVariableByIdentity(aVisualizer.Binding.Id);
                solutionViewModel.ActivateItem(newViewer);
            }

            return solutionViewModel;
        }
    }
}