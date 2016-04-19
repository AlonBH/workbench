﻿using System;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using Workbench.Core.Models;
using Workbench.Services;
using Workbench.ViewModels;

namespace Workbench.Commands
{
    /// <summary>
    /// Add a new chessboard visualizer to the solution designer.
    /// </summary>
    public class AddChessboardVisualizerCommand : CommandBase
    {
        private readonly WorkspaceViewModel workspace;
        private readonly TitleBarViewModel titleBar;
        private readonly IDataService dataService;
        private readonly IEventAggregator eventAggregator;
        private readonly IViewModelService viewModelService;

        public AddChessboardVisualizerCommand(WorkspaceViewModel theWorkspace,
                                              TitleBarViewModel theTitleBar,
                                              IEventAggregator theEventAggregator,
                                              IDataService theDataService,
                                              IViewModelService theViewModelService)
        {
            Contract.Requires<ArgumentNullException>(theWorkspace != null);
            Contract.Requires<ArgumentNullException>(theTitleBar != null);
            Contract.Requires<ArgumentNullException>(theEventAggregator != null);
            Contract.Requires<ArgumentNullException>(theDataService != null);
            Contract.Requires<ArgumentNullException>(theViewModelService != null);

            this.workspace = theWorkspace;
            this.titleBar = theTitleBar;
            this.eventAggregator = theEventAggregator;
            this.dataService = theDataService;
            this.viewModelService = theViewModelService;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">
        /// Data used by the command. 
        /// If the command does not require data to be passed, this object can be set to null.
        /// </param>
        public override void Execute(object parameter)
        {
            var newVisualizerLocation = Mouse.GetPosition(Application.Current.MainWindow);
            var newVisualizerModel = new ChessboardVisualizerModel(newVisualizerLocation);
            this.CreateDesigner(newVisualizerModel);
            this.CreateViewer(newVisualizerModel);
            this.titleBar.UpdateTitle();
        }

        private void CreateViewer(ChessboardVisualizerModel newVisualizerModel)
        {
            var visualizerViewerViewModel = new ChessboardVisualizerViewerViewModel(newVisualizerModel);
            this.workspace.AddViewer(visualizerViewerViewModel);
        }

        private void CreateDesigner(ChessboardVisualizerModel newVisualizerModel)
        {
            var visualizerDesignViewModel = new ChessboardVisualizerDesignViewModel(newVisualizerModel,
                                                                                    this.eventAggregator,
                                                                                    this.dataService,
                                                                                    this.viewModelService);
            this.workspace.AddDesigner(visualizerDesignViewModel);
        }
    }
}
