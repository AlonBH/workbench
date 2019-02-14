﻿using NUnit.Framework;
using Workbench.Core.Parsers;

namespace Workbench.Core.Tests.Unit.Parsers
{
    [TestFixture]
    public class ConstraintExpressionParserShould
    {
        [TestCase("$x > 1")]
        [TestCase("$x[1] != 1")]
        [TestCase("1 <> $x[1]")]
        [TestCase("$x[1] + 1 <> $x[1] + 1")]
        [TestCase("$x + 1 <> 1")]
        [TestCase("$x <> 'a'")]
        [TestCase("$x <> john")]
        public void ParseWithValidExpressionReturnsStatusSuccess(string constraintExpression)
        {
            var sut = CreateSut();
            var expressionParseResult = sut.Parse(constraintExpression);
            Assert.That(expressionParseResult.Status, Is.EqualTo(ParseStatus.Success));
        }

        private ConstraintExpressionParser CreateSut()
        {
            return new ConstraintExpressionParser();
        }
    }
}
