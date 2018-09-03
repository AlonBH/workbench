﻿using Caliburn.Micro;
using Workbench.ViewModels;
using Moq;
using NUnit.Framework;
using Workbench.Services;

namespace Workbench.UI.Tests.Unit.ViewModels
{
    [TestFixture]
    public class WorkspaceViewModelEmptyTests
    {
        [Test]
        public void SolveWithValidModelReturnsSuccessStatus()
        {
            var sut = CreateValidWorkspace();
            ScreenExtensions.TryActivate(sut);
            var actualStatus = sut.SolveModel();
            Assert.That(actualStatus.IsSuccess, Is.True);
            ScreenExtensions.TryDeactivate(sut, true);
        }

        private static WorkspaceViewModel CreateValidWorkspace()
        {
            var workspaceViewModel = new WorkspaceViewModel(CreateDataService(),
                                                            CreateWindowManager(),
                                                            CreateEventAggregator(),
                                                            CreateViewModelFactory().Object);
            return workspaceViewModel;
        }

        private static IDataService CreateDataService()
        {
            return new DataService(Mock.Of<IWorkspaceReaderWriter>());
        }

        private static Mock<IViewModelFactory> CreateViewModelFactory()
        {
            var viewModelFactoryMock = new Mock<IViewModelFactory>();
            viewModelFactoryMock.Setup(_ => _.CreateModelEditor())
                                .Returns(new ModelEditorTabViewModel(new AppRuntime(), CreateDataService(), CreateWindowManager(), CreateEventAggregator()));
            return viewModelFactoryMock;
        }

        private static IEventAggregator CreateEventAggregator()
        {
            return new Mock<IEventAggregator>().Object;
        }

        private static IWindowManager CreateWindowManager()
        {
            return new Mock<IWindowManager>().Object;
        }
    }
}
