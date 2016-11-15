using System;
using System.Diagnostics.Contracts;
using System.Linq;
using Caliburn.Micro;
using Workbench.ViewModels;

namespace Workbench.Commands
{
    public class EditMapCommand : CommandBase
    {
        private readonly IWindowManager windowManager;
        private readonly WorkspaceViewModel workspace;

        public EditMapCommand(IWindowManager theWindowManager, WorkspaceViewModel theWorkspace)
        {
            Contract.Requires<ArgumentNullException>(theWindowManager != null);
            Contract.Requires<ArgumentNullException>(theWorkspace != null);

            this.windowManager = theWindowManager;
            this.workspace = theWorkspace;
        }

        public override void Execute(object parameter)
        {
            var selectedMapVisualizers = this.workspace.Solution.GetSelectedMapVisualizers();
            if (!selectedMapVisualizers.Any()) return;
            var mapEditorViewModel = new MapEditorViewModel();
            mapEditorViewModel.BackgroundImagePath = selectedMapVisualizers.First().Model.Model.BackgroundImagePath;
            var showDialogResult = this.windowManager.ShowDialog(mapEditorViewModel);
            if (showDialogResult.HasValue && showDialogResult.Value)
            {
                foreach (var mapVisualizer in selectedMapVisualizers)
                {
                    mapVisualizer.Model.Model.BackgroundImagePath = mapEditorViewModel.BackgroundImagePath;
                }
            }
        }
    }
}
