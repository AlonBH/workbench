﻿using NUnit.Framework;
using Workbench.Core.Models;

namespace Workbench.Core.Tests.Unit.Models
{
    [TestFixture]
    public class ModelModelInvalidModelTests
    {
        [Test]
        public void Validate_With_An_Invalid_Model_Returns_False()
        {
            var sut = MakeModelWithMissingVariable();
            var actualValidationResult = sut.Validate();
            Assert.That(actualValidationResult, Is.False);
        }

        [Test]
        public void Validate_With_A_Model_Missing_Variable_Populates_Errors()
        {
            var sut = MakeModelWithMissingVariable();
            sut.Validate();
            Assert.That(sut.Errors, Is.Not.Empty);
        }

        [Test]
        public void Validate_With_A_Model_Missing_Shared_Domain_Populates_Errors()
        {
            var sut = MakeModelWithMissingSharedDomain();
            sut.Validate();
            Assert.That(sut.Errors, Is.Not.Empty);
        }

        private static ModelModel MakeModelWithMissingVariable()
        {
            return ModelModel.Create("An invalid model")
                             .AddSingleton("x", "1..9")
                             .AddSingleton("y", "1..9")
                             .WithConstraint("x > z")
                             .Build();
        }

        private static ModelModel MakeModelWithMissingSharedDomain()
        {
            return ModelModel.Create("An model missing shared domain")
                             .WithSharedDomain("a", "1..10")
                             .AddSingleton("x", "b")
                             .AddSingleton("y", "1..9")
                             .WithConstraint("x > y")
                             .Build();
        }
    }
}
