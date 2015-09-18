﻿using System;
using Dyna.Core.Models;
using NUnit.Framework;

namespace Dyna.Core.Tests.Unit.Models
{
    [TestFixture]
    public class ConstraintModelTests
    {
        [Test]
        public void Initialize_With_Raw_Expression_Parses_Expected_Variable_Name_On_Left()
        {
            var sut = new ConstraintModel("x > 1");
            Assert.That((object) sut.Expression.Left.Name, Is.EqualTo("x"));
        }

        [Test]
        public void Initialize_With_Raw_Expression_Parses_Expected_Operator()
        {
            var sut = new ConstraintModel("     a1    >    999      ");
            Assert.That((object) sut.Expression.OperatorType, Is.EqualTo(OperatorType.Greater));
        }

        [Test]
        public void Initialize_With_Raw_Expression_Parses_Expected_Literal_On_Right()
        {
            var sut = new ConstraintModel("y <= 44");
            Assert.That((object) sut.Expression.Right.Literal.Value, Is.EqualTo(44));
        }

        [Test]
        public void Initialize_With_Raw_Expression_Parses_Expected_Variable_Name_On_Right()
        {
            var sut = new ConstraintModel("y = x");
            Assert.That((object) sut.Expression.Right.Variable.Name, Is.EqualTo("x"));
        }

        [Test]
        public void Initialize_With_Empty_Expression_Throws_ArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new ConstraintModel(string.Empty));
        }
    }
}
