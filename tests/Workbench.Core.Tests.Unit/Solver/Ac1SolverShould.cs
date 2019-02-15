﻿using NUnit.Framework;
using Workbench.Core.Models;
using Workbench.Core.Solver;

namespace Workbench.Core.Tests.Unit.Solver
{
    [TestFixture]
    public class Ac1SolverShould
    {
        [Test]
        public void SolveValidModelWithStatusSuccess()
        {
            var sut = new Ac1Solver();
            var actualResult = sut.Solve(CreateModel());
            Assert.That(actualResult.Status, Is.EqualTo(SolveStatus.Success));
        }

        private ModelModel CreateModel()
        {
            var a = new WorkspaceBuilder("Simplest Non-Empty Model Possible")
                            .AddSingleton("A", "1..5")
                            .AddSingleton("B", "1..10")
                            .WithConstraintExpression("$A > $B")
                            .Build();

            return a.Model;
        }
    }
}
