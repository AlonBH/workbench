﻿using System;
using NUnit.Framework;
using Workbench.Core.Models;
using Workbench.Core.Solvers;

namespace Workbench.Core.Tests.Unit.Solvers
{
    [TestFixture]
    public class OrangeSolverWithComplexExpressionValidModelShould
    {
        [Test]
        public void SolveReturningStatusSuccess()
        {
            var sut = new OrangeSolver();
            var simpleModel = CreateWorkspace().Model;
            var actualResult = sut.Solve(simpleModel);
            Assert.That(actualResult.Status, Is.EqualTo(SolveStatus.Success));
        }

        [Test]
        public void SolveReturningLabelInValidRange()
        {
            var sut = new OrangeSolver();
            var simpleModel = CreateWorkspace().Model;
            var actualResult = sut.Solve(simpleModel);
            var colsLabel = actualResult.Snapshot.GetCompoundLabelByVariableName("cols");
            Assert.That(colsLabel.Values, Is.All.InRange(1, 4));
        }

        [Test]
        public void SolveReturningLabelSatisfiesConstraint()
        {
            var sut = new OrangeSolver();
            var simpleModel = CreateWorkspace().Model;
            var actualResult = sut.Solve(simpleModel);
            var colsLabel = actualResult.Snapshot.GetCompoundLabelByVariableName("cols");
            Assert.That(Convert.ToInt32(colsLabel.GetValueAt(1)) + 1, Is.GreaterThan(2));
        }

        private WorkspaceModel CreateWorkspace()
        {
            return new WorkspaceBuilder("More complex constraint expression model utilizing a binary constraint")
                            .AddAggregate("cols", 4, "1..size(cols)")
                            .WithConstraintExpression("$cols[1] + 1 > 2")
                            .Build();
        }
    }
}
