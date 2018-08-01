﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Caliburn.Micro;
using Workbench.Core.Models;

namespace Workbench.ViewModels
{
    public sealed class SolutionViewerPanelViewModel : Screen
    {
        private IObservableCollection<SingletonLabelModel> _singletonLabels;
        private IObservableCollection<CompoundLabelModel> _compoundLabels;
        private IObservableCollection<LabelModel> _labels;

        public SolutionViewerPanelViewModel()
        {
            DisplayName = "Solution";
            _singletonLabels = new BindableCollection<SingletonLabelModel>();
            _compoundLabels = new BindableCollection<CompoundLabelModel>();
            _labels = new BindableCollection<LabelModel>();
        }

        /// <summary>
        /// Gets the labels in the solution.
        /// </summary>
        public IObservableCollection<SingletonLabelModel> SingletonLabels
        {
            get { return _singletonLabels; }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null);
                _singletonLabels = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets the compound labels in the solution.
        /// </summary>
        public IObservableCollection<CompoundLabelModel> CompoundLabels
        {
            get { return _compoundLabels; }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null);
                _compoundLabels = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets all of the labels in the solution.
        /// </summary>
        public IObservableCollection<LabelModel> Labels
        {
            get { return _labels; }
            set
            {
                Contract.Requires<ArgumentNullException>(value != null);
                _labels = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Reset the contents of the solution.
        /// </summary>
        public void Reset()
        {
            Labels.Clear();
            SingletonLabels.Clear();
            CompoundLabels.Clear();
        }

        /// <summary>
        /// Add a label.
        /// </summary>
        /// <param name="newLabel">New label.</param>
        public void AddLabel(LabelModel newSingletonLabel)
        {
            Contract.Requires<ArgumentNullException>(newSingletonLabel != null);
            Labels.Add(newSingletonLabel);
        }

        public void BindTo(SolutionModel theSolution)
        {
            var allLabels = new List<LabelModel>(theSolution.Snapshot.SingletonLabels);
            allLabels.AddRange(theSolution.Snapshot.AggregateLabels);
            Labels = new BindableCollection<LabelModel>(allLabels);
            SingletonLabels = new BindableCollection<SingletonLabelModel>(theSolution.Snapshot.SingletonLabels);
            CompoundLabels = new BindableCollection<CompoundLabelModel>(theSolution.Snapshot.AggregateLabels);
        }
    }
}
