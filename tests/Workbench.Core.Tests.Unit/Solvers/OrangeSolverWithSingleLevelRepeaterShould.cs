﻿using NUnit.Framework;
using Workbench.Core.Models;
using Workbench.Core.Solvers;

namespace Workbench.Core.Tests.Unit.Solvers
{
    [TestFixture]
    public class OrangeSolverWithSingleLevelRepeaterShould
    {
        [Test]
        public void SolveWithRepeaterConstraintReturnsStatusSuccess()
        {
            var sut = CreateSolver();
            var actualResult = sut.Solve(MakeModel());
            Assert.That(actualResult.Status, Is.EqualTo(SolveStatus.Success));
        }

        [Test]
        public void SolveWithRepeaterSatisfiesConstraints()
        {
            var sut = CreateSolver();
            var actualResult = sut.Solve(MakeModel());
            var actualSnapshot = actualResult.Snapshot;
            var x = actualSnapshot.GetAggregateLabelByVariableName("x");
            Assert.That(x.GetValueAt(0), Is.Not.EqualTo(x.GetValueAt(1)));
            Assert.That(x.GetValueAt(0), Is.Not.EqualTo(x.GetValueAt(2)));
        }

        private static OrangeSolver CreateSolver()
        {
            return new OrangeSolver();
        }

        private static ModelModel MakeModel()
        {
            var workspace = new WorkspaceBuilder("A simple repeater test")
                                          .AddAggregate("x", 10, "1..10")
                                          .WithConstraintExpression("$x[0] <> $x[i] | i in 1..9")
                                          .Build();

            return workspace.Model;
        }
    }
}
