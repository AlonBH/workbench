﻿using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Google.OrTools.ConstraintSolver;
using Workbench.Core.Models;
using Workbench.Core.Nodes;
using Workbench.Core.Parsers;
using Workbench.Core.Solver;

namespace Workbench.Core.Repeaters
{
    /// <summary>
    /// Process a constraint repeater by expanding the expression the 
    /// appropriate number of times.
    /// </summary>
    internal class ConstraintRepeater
    {
        private ConstraintRepeaterContext context;
        private readonly OrToolsCache cache;
        private readonly Google.OrTools.ConstraintSolver.Solver solver;
        private readonly ModelModel model;

        public ConstraintRepeater(Google.OrTools.ConstraintSolver.Solver theSolver, OrToolsCache theCache, ModelModel theModel)
        {
            Contract.Requires<ArgumentNullException>(theSolver != null);
            Contract.Requires<ArgumentNullException>(theCache != null);
            this.solver = theSolver;
            this.cache = theCache;
            this.model = theModel;
        }

        public void Process(ConstraintRepeaterContext theContext)
        {
            Contract.Requires<ArgumentNullException>(theContext != null);
            this.context = theContext;
            if (!theContext.HasRepeaters)
            {
                ProcessSimpleConstraint(theContext.Constraint.Expression.Node);
            }
            else
            {
                var expressionTemplateWoutExpanderText = StripExpanderFrom(theContext.Constraint.Expression.Text);
                while (theContext.Next())
                {
                    var expressionText = InsertCounterValuesInto(expressionTemplateWoutExpanderText);
                    var expandedConstraintExpressionResult = new ConstraintExpressionParser().Parse(expressionText);
                    ProcessSimpleConstraint(expandedConstraintExpressionResult.Root);
                }
            }
        }

        public ConstraintRepeaterContext CreateContextFrom(ExpressionConstraintGraphicModel constraint)
        {
            return new ConstraintRepeaterContext(constraint, this.model);
        }

        private void ProcessSimpleConstraint(ConstraintExpressionNode constraintExpressionNode)
        {
            Contract.Requires<ArgumentNullException>(constraintExpressionNode != null);
            Constraint newConstraint;
            var lhsExpr = GetExpressionFrom(constraintExpressionNode.InnerExpression.LeftExpression);
            if (constraintExpressionNode.InnerExpression.RightExpression.IsExpression)
            {
                var rhsExpr = GetExpressionFrom(constraintExpressionNode.InnerExpression.RightExpression);
                newConstraint = CreateConstraintBy(constraintExpressionNode.InnerExpression.Operator, lhsExpr, rhsExpr);
            }
            else if (constraintExpressionNode.InnerExpression.RightExpression.IsVarable)
            {
                var rhsVariable = GetVariableFrom(constraintExpressionNode.InnerExpression.RightExpression);
                newConstraint = CreateConstraintBy(constraintExpressionNode.InnerExpression.Operator, lhsExpr, rhsVariable);
            }
            else
            {
                newConstraint = CreateConstraintBy(constraintExpressionNode.InnerExpression.Operator,
                                                   lhsExpr,
                                                   constraintExpressionNode.InnerExpression.RightExpression.GetLiteral());
            }
            this.solver.Add(newConstraint);
        }

        private Constraint CreateConstraintBy(OperatorType operatorType, IntExpr lhsExpr, IntExpr rhsExpr)
        {
            switch (operatorType)
            {
                case OperatorType.Equals:
                    return this.solver.MakeEquality(lhsExpr, rhsExpr);

                case OperatorType.NotEqual:
                    return this.solver.MakeNonEquality(lhsExpr, rhsExpr);

                case OperatorType.GreaterThanOrEqual:
                    return this.solver.MakeGreaterOrEqual(lhsExpr, rhsExpr);

                case OperatorType.LessThanOrEqual:
                    return this.solver.MakeLessOrEqual(lhsExpr, rhsExpr);

                case OperatorType.Greater:
                    return this.solver.MakeGreater(lhsExpr, rhsExpr);

                case OperatorType.Less:
                    return this.solver.MakeLess(lhsExpr, rhsExpr);

                default:
                    throw new NotImplementedException("Not sure how to represent this operator type.");
            }
        }

        private Constraint CreateConstraintBy(OperatorType operatorType, IntExpr lhsExpr, int rhs)
        {
            switch (operatorType)
            {
                case OperatorType.Equals:
                    return this.solver.MakeEquality(lhsExpr, rhs);

                case OperatorType.NotEqual:
                    return this.solver.MakeNonEquality(lhsExpr, rhs);

                case OperatorType.GreaterThanOrEqual:
                    return this.solver.MakeGreaterOrEqual(lhsExpr, rhs);

                case OperatorType.LessThanOrEqual:
                    return this.solver.MakeLessOrEqual(lhsExpr, rhs);

                case OperatorType.Greater:
                    return this.solver.MakeGreater(lhsExpr, rhs);

                case OperatorType.Less:
                    return this.solver.MakeLess(lhsExpr, rhs);

                default:
                    throw new NotImplementedException("Not sure how to represent this operator type.");
            }
        }

        private IntExpr GetExpressionFrom(ExpressionNode theExpression)
        {
            if (theExpression.IsExpression)
            {
                IntExpr variableExpression;
                VariableExpressionOperatorType op;
                InfixStatementNode infixStatement;
                if (theExpression.IsSingletonExpression)
                {
                    var singletonExpression = (SingletonVariableReferenceExpressionNode)theExpression.InnerExpression;
                    variableExpression = GetVariableFrom((SingletonVariableReferenceExpressionNode)theExpression.InnerExpression);
                    op = singletonExpression.Operator;
                    infixStatement = singletonExpression.InfixStatement;
                }
                else
                {
                    var aggregateExpression = (AggregateVariableReferenceExpressionNode)theExpression.InnerExpression;
                    variableExpression = GetVariableFrom((AggregateVariableReferenceExpressionNode)theExpression.InnerExpression);
                    op = aggregateExpression.Operator;
                    infixStatement = aggregateExpression.InfixStatement;
                }

                switch (op)
                {
                    case VariableExpressionOperatorType.Add:
                        return this.solver.MakeSum(variableExpression, GetValueFrom(infixStatement));

                    case VariableExpressionOperatorType.Minus:
                        return this.solver.MakeSum(variableExpression, -GetValueFrom(infixStatement));

                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {
                return GetVariableFrom(theExpression);
            }
        }

        private IntVar GetVariableFrom(ExpressionNode theExpression)
        {
            Debug.Assert(!theExpression.IsLiteral);

            if (theExpression.IsSingletonReference)
            {
                var singletonVariableReference = (SingletonVariableReferenceNode)theExpression.InnerExpression;
                return this.cache.GetSingletonVariableByName(singletonVariableReference.VariableName);
            }

            var aggregateVariableReference = (AggregateVariableReferenceNode)theExpression.InnerExpression;
            return this.cache.GetAggregateVariableByName(aggregateVariableReference.VariableName, aggregateVariableReference.SubscriptStatement.Subscript);
        }

        private IntExpr GetVariableFrom(AggregateVariableReferenceExpressionNode aggregateExpression)
        {
            return this.cache.GetAggregateVariableByName(aggregateExpression.VariableReference.VariableName, aggregateExpression.VariableReference.SubscriptStatement.Subscript);
        }

        private IntExpr GetVariableFrom(SingletonVariableReferenceExpressionNode singletonExpression)
        {
            return this.cache.GetSingletonVariableByName(singletonExpression.VariableReference.VariableName);
        }

        private string StripExpanderFrom(string expressionText)
        {
            var expanderKeywordPos = expressionText.IndexOf("|", StringComparison.Ordinal);
            var raw = expressionText.Substring(0, expanderKeywordPos);
            return raw.Trim();
        }

        private string InsertCounterValuesInto(string expressionTemplateText)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(expressionTemplateText));

            var accumulatingTemplateText = expressionTemplateText;
            foreach (var aCounter in this.context.Counters)
            {
                accumulatingTemplateText = InsertCounterValueInto(accumulatingTemplateText,
                                                                  aCounter.CounterName,
                                                                  aCounter.CurrentValue);
            }

            return accumulatingTemplateText;
        }

        private string InsertCounterValueInto(string expressionTemplateText, string counterName, int counterValue)
        {
            return expressionTemplateText.Replace(counterName, Convert.ToString(counterValue));
        }

        private int GetValueFrom(InfixStatementNode infixStatement)
        {
            Contract.Requires<ArgumentNullException>(infixStatement != null);
            Contract.Requires<ArgumentException>(infixStatement.IsCounterReference ||
                                                 infixStatement.IsLiteral);
            if (infixStatement.IsLiteral) return infixStatement.Literal.Value;
            var counter = context.GetCounterContextByName(infixStatement.CounterReference.CounterName);
            return counter.CurrentValue;
        }
    }
}
