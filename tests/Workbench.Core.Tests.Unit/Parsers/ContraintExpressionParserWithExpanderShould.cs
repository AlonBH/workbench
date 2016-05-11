﻿using NUnit.Framework;
using Workbench.Core.Parsers;

namespace Workbench.Core.Tests.Unit.Parsers
{
    [TestFixture]
    public class ContraintExpressionParserWithExpanderShould
    {
        [Test]
        public void ParseWithSingleLevelExpanderExpressionReturnsStutusSuccess()
        {
            var sut = CreateSut();
            var expressionParseResult = sut.Parse("x[i] <> x[i] + 1 | i in 1..10");
            Assert.That(expressionParseResult.Status, Is.EqualTo(ConstraintExpressionParseStatus.Success));
        }

        [Test]
        public void ParseWithMultiLevelExpanderExpressionReturnsStutusSuccess()
        {
            var sut = CreateSut();
            var expressionParseResult = sut.Parse("x[i] <> x[j] | i,j in 1..10,1..10");
            Assert.That(expressionParseResult.Status, Is.EqualTo(ConstraintExpressionParseStatus.Success));
        }

        [Test]
        public void ParseWithMultiLevelExpanderUsingVariableScopeExpressionReturnsStutusSuccess()
        {
            var sut = CreateSut();
            var expressionParseResult = sut.Parse("x[i] <> x[j] | i,j in 1..10,1..i");
            Assert.That(expressionParseResult.Status, Is.EqualTo(ConstraintExpressionParseStatus.Success));
        }

        [Test]
        public void ParseWithMultiLevelExpanderUsingVariableLimitExpressionReturnsStutusSuccess()
        {
            var sut = CreateSut();
            var expressionParseResult = sut.Parse("x[i] <> x[j] | i,j in 10,i");
            Assert.That(expressionParseResult.Status, Is.EqualTo(ConstraintExpressionParseStatus.Success));
        }

        [Test]
        public void ParseWithMultiLevelExpanderUsingInfixStatementReturnsStutusSuccess()
        {
            var sut = CreateSut();
            var expressionParseResult = sut.Parse("x[i] + i <> x[j] + j | i,j in 8,i");
            Assert.That(expressionParseResult.Status, Is.EqualTo(ConstraintExpressionParseStatus.Success));
        }

        private static ConstraintExpressionParser CreateSut()
        {
            return new ConstraintExpressionParser();
        }
    }
}
