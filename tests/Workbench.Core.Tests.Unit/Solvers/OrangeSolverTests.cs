﻿using NUnit.Framework;
using Workbench.Core.Models;
using Workbench.Core.Solvers;

namespace Workbench.Core.Tests.Unit.Solvers
{
    [TestFixture]
    public class OrangeSolverTests
    {
        [Test]
        public void Solve_With_Model_Returns_Status_Success()
        {
            using (var sut = new OrangeSolver())
            {
                var actualResult = sut.Solve(MakeModel());

                Assert.That(actualResult.Status, Is.EqualTo(SolveStatus.Success));
            }
        }

        [Test]
        public void Solve_With_Model_Satisfies_Constraint()
        {
            using (var sut = new OrangeSolver())
            {
                var actualResult = sut.Solve(MakeModel());

                var actualSnapshot = actualResult.Snapshot;
                var x = actualSnapshot.GetLabelByVariableName("x");
                var y = actualSnapshot.GetLabelByVariableName("y");
                var z = actualSnapshot.GetCompoundLabelByVariableName("z");
                Assert.That(x.Value, Is.Not.EqualTo(y.Value));
                Assert.That(y.Value, Is.GreaterThan(z.GetValueAt(0)));
            }
        }

        [Test]
        public void Solve_With_Model_Solution_Within_Domain()
        {
            using (var sut = new OrangeSolver())
            {
                var actualResult = sut.Solve(MakeModel());

                var actualSnapshot = actualResult.Snapshot;
                var x = actualSnapshot.GetLabelByVariableName("x");
                var y = actualSnapshot.GetLabelByVariableName("y");
                Assert.That(x.Value, Is.InRange(1, 9));
                Assert.That(y.Value, Is.InRange(1, 9));
            }
        }

        private static ModelModel MakeModel()
        {
            var workspace = new WorkspaceBuilder("A test")
                                          .AddSingleton("x", "1..9")
                                          .AddSingleton("y", "1..9")
                                          .AddAggregate("z", 1, "1..9")
                                          .WithConstraintExpression("$x < $y")
                                          .WithConstraintExpression("$y > $z[0]")
                                          .Build();

            return workspace.Model;
        }
    }
}
