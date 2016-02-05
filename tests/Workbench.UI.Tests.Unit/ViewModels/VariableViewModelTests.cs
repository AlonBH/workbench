﻿using System;
using Caliburn.Micro;
using Moq;
using Workbench.ViewModels;
using NUnit.Framework;
using Workbench.Core.Models;
using Workbench.Messages;

namespace Workbench.UI.Tests.Unit.ViewModels
{
    [TestFixture]
    public class VariableViewModelTests
    {
        private readonly Mock<IEventAggregator> eventAggregatorMock = new Mock<IEventAggregator>();

        [Test]
        public void RenameVariableWithValidNewNamePublishesVariableRenamedMessage()
        {
            var sut = CreateVariable();
            sut.Name = "NewName";
            this.eventAggregatorMock.Verify(_ => _.Publish(It.IsAny<VariableRenamedMessage>(), It.IsAny<Action<System.Action>>()),
                                            Times.Once);
        }

        [Test]
        public void UpdateVariableDomainExpressionWithDomainReferenceUpdatesModel()
        {
            var sut = CreateVariable();
            sut.DomainExpression.Text = "x";
            Assert.That(sut.DomainExpression.Model.DomainReference.DomainName, Is.EqualTo("x"));
        }

        [Test]
        public void UpdateDomainExpressionWithInlineDomainUpdatesModel()
        {
            var sut = CreateVariable();
            sut.DomainExpression.Text = "1..10";
            Assert.That(sut.DomainExpression.Model.InlineDomain.Size, Is.EqualTo(10));
        }

        private VariableViewModel CreateVariable()
        {
            return new VariableViewModel(new VariableModel("X"),
                                         this.eventAggregatorMock.Object);
        }
    }
}
