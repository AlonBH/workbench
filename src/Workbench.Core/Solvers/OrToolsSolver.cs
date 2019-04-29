﻿using System;
using System.Diagnostics.Contracts;
using Google.OrTools.ConstraintSolver;
using Workbench.Core.Models;

namespace Workbench.Core.Solvers
{
    /// <summary>
    /// Constraint solver implemented using Google or-tools library.
    /// </summary>
    public class OrToolsSolver : ISolvable, IDisposable
    {
        private Solver solver;
        private readonly OrToolsCache orToolsCache = new OrToolsCache();
        private readonly ValueMapper valueMapper = new ValueMapper();

        /// <summary>
        /// Solve the model using the Google or-tools solver.
        /// </summary>
        /// <param name="theModel">The model to solve.</param>
        /// <returns>Solve result.</returns>
        public SolveResult Solve(ModelModel theModel)
        {
            Contract.Requires<ArgumentNullException>(theModel != null);

            if (!new ModelValidator(theModel).Validate()) return SolveResult.InvalidModel;

            // A model with zero variables crashes the or-tools solver...
            if (theModel.IsEmpty) return new SolveResult(SolveStatus.Success, new SolutionSnapshot(), TimeSpan.Zero);

            this.solver = new Solver(theModel.Name.Text);

            var modelConverter = new ModelConverter(this.solver, this.orToolsCache, this.valueMapper);
            modelConverter.ConvertFrom(theModel);

            // Search
            var decisionBuilder = solver.MakePhase(this.orToolsCache.Variables,
                                                   Solver.CHOOSE_FIRST_UNBOUND,
                                                   Solver.INT_VALUE_DEFAULT);
            var collector = CreateCollector();
            var solveResult = this.solver.Solve(decisionBuilder, collector);
            if (!solveResult) return SolveResult.Failed;

            var snapshotExtractor = new OrSnapshotExtractor(this.orToolsCache, this.valueMapper);
            var solutionSnapshot = snapshotExtractor.ExtractValuesFrom(collector);

            return new SolveResult(SolveStatus.Success, solutionSnapshot, TimeSpan.FromMilliseconds(this.solver.WallTime()));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.solver == null) return;
            this.solver.Dispose();
            this.solver = null;
        }

        private SolutionCollector CreateCollector()
        {
            var collector = this.solver.MakeFirstSolutionCollector();
            foreach (var variableTuple in this.orToolsCache.SingletonVariableMap)
                collector.Add(variableTuple.Value.Item2);
            foreach (var variableTuple in this.orToolsCache.AggregateVariableMap)
            {
                var variablesInsideAggregate = variableTuple.Value.Item2;
                foreach (var intVar in variablesInsideAggregate)
                    collector.Add(intVar);
            }
            foreach (var bucketTuple in this.orToolsCache.BucketMap)
            {
                var bundleMaps = bucketTuple.Value.GetBundleMaps();
                foreach (var bundleMap in bundleMaps)
                {
                    foreach (var singletonVariableMap in bundleMap.GetVariableMaps())
                    {
                        collector.Add(singletonVariableMap.SolverVariable);
                    }
                }
            }

            return collector;
        }
    }
}
