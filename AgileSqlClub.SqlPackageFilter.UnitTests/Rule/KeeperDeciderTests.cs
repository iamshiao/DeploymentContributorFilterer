﻿using System.Collections.Generic;
using AgileSqlClub.SqlPackageFilter.Filter;
using AgileSqlClub.SqlPackageFilter.Rules;
using Microsoft.SqlServer.Dac.Model;
using Moq;
using NUnit.Framework;
using Microsoft.SqlServer.Dac.Deployment;

namespace AgileSqlClub.SqlPackageFilter.UnitTests
{
    [TestFixture]
    public class KeeperDeciderTests
    {
 
        [Test]
        public void Create_Step_Does_Not_Call_Keep_Rule()
        {
            var keepRule = new Mock<FilterRule>();
            keepRule.Setup(p => p.Operation()).Returns(FilterOperation.Keep);
            keepRule.Setup(p => p.Matches(It.IsAny<ObjectIdentifier>(), It.IsAny<ModelTypeClass>(), It.IsAny<DeploymentStep>())).Callback(() => Assert.Fail("Rule should not have been called"));

            var decider = new KeeperDecider(new List<FilterRule>() { keepRule.Object });
            decider.ShouldRemoveFromPlan(new ObjectIdentifier("aa"), ModelSchema.Aggregate, StepType.Create);

        }

        [Test]
        public void Drop_Step_Does_Call_Keep_Rule()
        {
            var keepRule = new Mock<FilterRule>();
            keepRule.Setup(p => p.Operation()).Returns(FilterOperation.Keep);
            keepRule.Setup(p => p.Matches(It.IsAny<ObjectIdentifier>(), It.IsAny<ModelTypeClass>(), It.IsAny<DeploymentStep>())).Returns(true);

            var decider = new KeeperDecider(new List<FilterRule>() { keepRule.Object });
            var result = decider.ShouldRemoveFromPlan(new ObjectIdentifier("aa"), ModelSchema.Aggregate, StepType.Drop);

            Assert.IsTrue(result);
        }

        [Test]
        public void Create_Step_Does_Call_Ignore_Rule()
        {
            var ignoreRule = new Mock<FilterRule>();
            ignoreRule.Setup(p => p.Operation()).Returns(FilterOperation.Ignore);
            ignoreRule.Setup(p => p.Matches(It.IsAny<ObjectIdentifier>(), It.IsAny<ModelTypeClass>(), It.IsAny<DeploymentStep>())).Returns(true);

            var decider = new KeeperDecider(new List<FilterRule>() { ignoreRule.Object });
            var result = decider.ShouldRemoveFromPlan(new ObjectIdentifier("aa"), ModelSchema.Aggregate, StepType.Create);

            Assert.IsTrue(result);
        }


        [Test]
        public void Drop_Step_Does_Call_Ignore_Rule()
        {
            var ignoreRule = new Mock<FilterRule>();
            ignoreRule.Setup(p => p.Operation()).Returns(FilterOperation.Ignore);
            ignoreRule.Setup(p => p.Matches(It.IsAny<ObjectIdentifier>(), It.IsAny<ModelTypeClass>(), It.IsAny<DeploymentStep>())).Returns(true);

            var decider = new KeeperDecider(new List<FilterRule>() { ignoreRule.Object });
            var result = decider.ShouldRemoveFromPlan(new ObjectIdentifier("aa"), ModelSchema.Aggregate, StepType.Drop);

            Assert.IsTrue(result);
        }


        [Test]
        public void Alter_Step_Does_Call_Ignore_Rule()
        {
            var ignoreRule = new Mock<FilterRule>();
            ignoreRule.Setup(p => p.Operation()).Returns(FilterOperation.Ignore);
            ignoreRule.Setup(p => p.Matches(It.IsAny<ObjectIdentifier>(), It.IsAny<ModelTypeClass>(), null)).Returns(true);

            var decider = new KeeperDecider(new List<FilterRule>() { ignoreRule.Object });
            var result = decider.ShouldRemoveFromPlan(new ObjectIdentifier("aa"), ModelSchema.Aggregate, StepType.Alter);

            Assert.IsTrue(result);
        }

        [Test]
        public void Alter_Step_With_TableColumns_Removes_From_Plan()
        {
           //Not possible to have this unit test as AlterStep isn't mockable or instantiatable :( and i don't want to do it through reflection :)
        }

        [Test]
        public void All_Rules_Ignored_For_Other_Steps()
        {
            var ignoreRule = new Mock<FilterRule>();
            ignoreRule.Setup(p => p.Operation()).Returns(FilterOperation.Ignore);
            ignoreRule.Setup(p => p.Matches(It.IsAny<ObjectIdentifier>(), It.IsAny<ModelTypeClass>(), null)).Callback(() => Assert.Fail("Ignore Rule should not have been called")); ;


            var keepRule = new Mock<FilterRule>();
            keepRule.Setup(p => p.Operation()).Returns(FilterOperation.Keep);
            keepRule.Setup(p => p.Matches(It.IsAny<ObjectIdentifier>(), It.IsAny<ModelTypeClass>(), null)).Callback(() => Assert.Fail("Keep Rule should not have been called"));

            var decider = new KeeperDecider(new List<FilterRule>() { ignoreRule.Object, keepRule.Object});
            var result = decider.ShouldRemoveFromPlan(new ObjectIdentifier("aa"), ModelSchema.Aggregate, StepType.Other);

            Assert.IsFalse(result);
        }

    }
}
