﻿using System;
using System.Diagnostics;
using Google.OrTools.ConstraintSolver;
using Workbench.Core.Models;
using Workbench.Core.Nodes;
using Workbench.Core.Parsers;
using Workbench.Core.Solvers;

namespace Workbench.Core.Repeaters
{
    /// <summary>
    /// Process a constraint repeater by expanding the expression an 
    /// appropriate number of times.
    /// </summary>
    internal class OrConstraintRepeater
    {
        private OrConstraintRepeaterContext context;
        private readonly OrToolsCache cache;
        private readonly Solver solver;
        private readonly ModelModel model;
        private readonly OrValueMapper valueMapper;

        internal OrConstraintRepeater(Solver theSolver, OrToolsCache theCache, ModelModel theModel, OrValueMapper theValueMapper)
        {
            this.solver = theSolver;
            this.cache = theCache;
            this.model = theModel;
            this.valueMapper = theValueMapper;
        }

        internal void Process(OrConstraintRepeaterContext theContext)
        {
            this.context = theContext;
            if (!theContext.HasRepeaters)
            {
                ProcessSimpleConstraint(theContext.Constraint.Expression.Node);
            }
            else
            {
                var expressionTemplateWithoutExpanderText = StripExpanderFrom(theContext.Constraint.Expression.Text);
                while (theContext.Next())
                {
                    var expressionText = InsertCounterValuesInto(expressionTemplateWithoutExpanderText);
                    var expandedConstraintExpressionResult = new ConstraintExpressionParser().Parse(expressionText);
                    ProcessSimpleConstraint(expandedConstraintExpressionResult.Root);
                }
            }
        }

        internal OrConstraintRepeaterContext CreateContextFrom(ExpressionConstraintModel constraint)
        {
            return new OrConstraintRepeaterContext(constraint, this.model);
        }

        private void ProcessSimpleConstraint(ConstraintExpressionNode constraintExpressionNode)
        {
            Constraint newConstraint;
            var lhsExpr = GetExpressionFrom(constraintExpressionNode.InnerExpression.LeftExpression);
            if (constraintExpressionNode.InnerExpression.RightExpression.IsExpression)
            {
                var rhsExpr = GetExpressionFrom(constraintExpressionNode.InnerExpression.RightExpression);
                newConstraint = CreateConstraintBy(constraintExpressionNode.InnerExpression.Operator, lhsExpr, rhsExpr);
            }
            else if (constraintExpressionNode.InnerExpression.RightExpression.InnerExpression is BucketVariableReferenceExpressionNode)
            {
                var rhsExpr = GetExpressionFrom(constraintExpressionNode.InnerExpression.RightExpression);
                newConstraint = CreateConstraintBy(constraintExpressionNode.InnerExpression.Operator, lhsExpr, rhsExpr);
            }
            else if (constraintExpressionNode.InnerExpression.RightExpression.IsVariable)
            {
                var rhsVariable = GetVariableFrom(constraintExpressionNode.InnerExpression.RightExpression);
                newConstraint = CreateConstraintBy(constraintExpressionNode.InnerExpression.Operator, lhsExpr, rhsVariable);
            }
            else if (constraintExpressionNode.InnerExpression.RightExpression.InnerExpression is BucketVariableReferenceNode)
            {
                var rhsVariable = GetVariableFrom(constraintExpressionNode.InnerExpression.RightExpression);
                newConstraint = CreateConstraintBy(constraintExpressionNode.InnerExpression.Operator, lhsExpr, rhsVariable);
            }
            else if (constraintExpressionNode.InnerExpression.RightExpression.InnerExpression is IntegerLiteralNode)
            {
                newConstraint = CreateConstraintBy(constraintExpressionNode.InnerExpression.Operator,
                                                   lhsExpr,
                                                   Convert.ToInt32(constraintExpressionNode.InnerExpression.RightExpression.GetLiteral()));
            }
            else if (constraintExpressionNode.InnerExpression.RightExpression.InnerExpression is ItemNameNode node)
            {
                var lhsVariableName = GetVariableNameFrom(constraintExpressionNode.InnerExpression.LeftExpression);
                var lhsVariable = this.model.GetVariableByName(lhsVariableName);
                var range = this.valueMapper.GetDomainValueFor(lhsVariable);
                var modelValue = GetModelValueFrom(node);
                var solverValue = range.MapTo(modelValue);
                newConstraint = CreateConstraintBy(constraintExpressionNode.InnerExpression.Operator, lhsExpr, solverValue);
            }
            else if (constraintExpressionNode.InnerExpression.RightExpression.InnerExpression is CharacterLiteralNode literalNode)
            {
                var lhsVariableName = GetVariableNameFrom(constraintExpressionNode.InnerExpression.LeftExpression);
                var lhsVariable = this.model.GetVariableByName(lhsVariableName);
                var range = this.valueMapper.GetDomainValueFor(lhsVariable);
                var modelValue = GetModelValueFrom(literalNode);
                var solverValue = range.MapTo(modelValue);
                newConstraint = CreateConstraintBy(constraintExpressionNode.InnerExpression.Operator, lhsExpr, solverValue);
            }
            else
            {
                throw new NotImplementedException("Unknown constraint expression.");
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
                    variableExpression = GetVariableFrom(aggregateExpression);
                    op = aggregateExpression.Operator;
                    infixStatement = aggregateExpression.InfixStatement;
                }

                switch (op)
                {
                    case VariableExpressionOperatorType.Add:
                        return this.solver.MakeSum(variableExpression, GetValueFrom(infixStatement));

                    case VariableExpressionOperatorType.Subtract:
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

            switch (theExpression.InnerExpression)
            {
                case SingletonVariableReferenceNode singletonVariableReferenceNode:
                    return this.cache.GetSingletonVariableByName(singletonVariableReferenceNode.VariableName);

                case AggregateVariableReferenceNode aggregateVariableReference:
                    return this.cache.GetAggregateVariableByName(aggregateVariableReference.VariableName, aggregateVariableReference.SubscriptStatement.Subscript);

                case BucketVariableReferenceNode bucketVariableReference:
                    return this.cache.GetBucketVariableByName(bucketVariableReference.BucketName, bucketVariableReference.SubscriptStatement.Subscript, bucketVariableReference.VariableName);

                default:
                    throw new NotImplementedException("Unknown variable expression.");
            }
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
            if (string.IsNullOrWhiteSpace(expressionTemplateText))
                throw new ArgumentException("Expression template must not be empty", nameof(expressionTemplateText));

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
            Debug.Assert(infixStatement.IsCounterReference || infixStatement.IsLiteral);
            if (infixStatement.IsLiteral) return infixStatement.Literal.Value;
            var counter = context.GetCounterContextByName(infixStatement.CounterReference.CounterName);
            return counter.CurrentValue;
        }

        private object GetModelValueFrom(ItemNameNode theItemNameNode)
        {
            return theItemNameNode.Value;
        }

        private object GetModelValueFrom(CharacterLiteralNode theCharacterLiteralNode)
        {
            return theCharacterLiteralNode.Value;
        }

        private string GetVariableNameFrom(ExpressionNode lhsExpression)
        {
            switch (lhsExpression.InnerExpression)
            {
                case SingletonVariableReferenceNode singletonVariableReferenceNode:
                    return singletonVariableReferenceNode.VariableName;

                case AggregateVariableReferenceExpressionNode aggregateVariableReferenceNode:
                    return aggregateVariableReferenceNode.VariableReference.VariableName;

                default:
                    throw new NotImplementedException("Unknown variable reference.");
            }
        }
    }
}
