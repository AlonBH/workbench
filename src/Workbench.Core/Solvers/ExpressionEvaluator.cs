﻿using System;
using System.Diagnostics;
using Workbench.Core.Nodes;

namespace Workbench.Core.Solvers
{
    /// <summary>
    /// Evaluate one side of a constraint expressions.
    /// </summary>
    internal sealed class ExpressionEvaluator
    {
        /// <summary>
        /// Evaluate the expression.
        /// </summary>
        /// <param name="expression">Constraint expression.</param>
        /// <param name="possibleValue">Possible value to be assigned to a variable.</param>
        /// <returns>Processed value through the expression</returns>
        internal int Evaluate(ExpressionNode expression, int possibleValue)
        {
            if (!expression.IsExpression) return possibleValue;

            VariableExpressionOperatorType op;
            InfixStatementNode infixStatement;
            if (expression.IsSingletonExpression)
            {
                var singletonExpression = (SingletonVariableReferenceExpressionNode) expression.InnerExpression;
                op = singletonExpression.Operator;
                infixStatement = singletonExpression.InfixStatement;
            }
            else
            {
                var aggregateExpression = (AggregateVariableReferenceExpressionNode) expression.InnerExpression;
                op = aggregateExpression.Operator;
                infixStatement = aggregateExpression.InfixStatement;
            }

            switch (op)
            {
                case VariableExpressionOperatorType.Add:
                    return possibleValue + GetValueFrom(infixStatement);

                case VariableExpressionOperatorType.Subtract:
                    return possibleValue - GetValueFrom(infixStatement);

                default:
                    throw new NotImplementedException();
            }
        }

        private int GetValueFrom(InfixStatementNode infixStatement)
        {
            Debug.Assert(infixStatement.IsLiteral);

            return infixStatement.Literal.Value;
        }
    }
}
