﻿using Workbench.Core.Models;
using Workbench.ViewModels;

namespace Workbench.Services
{
    public interface IWorkspaceLoader
    {
        /// <summary>
        /// Map a workspace model to a workspace view model.
        /// </summary>
        /// <param name="theWorkspaceModel">Workspace model.</param>
        /// <returns>Workspace view model.</returns>
        WorkspaceViewModel Load(WorkspaceModel theWorkspaceModel);
    }
}