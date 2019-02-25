﻿using NUnit.Framework;
using Workbench.Core.Models;
using Workbench.Core.Solvers;

namespace Workbench.Core.Tests.Unit.Solvers
{
    /// <summary>
    /// Example model taken from Constraint Processing by Rina Dechter pg 52 para 2.
    /// </summary>
    /// <remarks>This model is likely to be beyond the first iteration of the solver
    /// due to the ternary constraints and the fact that I don't know how to handle those
    /// yet. Binarization as explained here http://ktiml.mff.cuni.cz/~bartak/constraints/binary.htmlThis may
    /// be a possible answer.</remarks>
    [TestFixture]
    public class Ac1SolverWithSimpleUnsolvableModelShould
    {
        [Test]
        public void SolveReturningStatusFail()
        {
            var sut = new Ac1Solver();
            var actualResult = sut.Solve(CreateModel());
            Assert.That(actualResult.Status, Is.EqualTo(SolveStatus.Fail));
        }

        private ModelModel CreateModel()
        {
            var a = new WorkspaceBuilder("Simple model that cannot be solved")
                            .WithSharedDomain("D", "\"red\", \"blue\"")
                            .AddSingleton("x", "$D")
                            .AddSingleton("y", "$D")
                            .AddSingleton("z", "$D")
                            .WithConstraintExpression("$x = $y")
                            .WithConstraintExpression("$y = $z")
                            .WithConstraintExpression("$x <> $z")
                            .Build();

            return a.Model;
        }
    }
}
