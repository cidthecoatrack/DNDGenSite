﻿using CharacterGen.Abilities;
using CharacterGen.Characters;
using CharacterGen.Leaders;
using CharacterGen.Skills;
using DnDGen.Web.Controllers.Characters;
using EventGen;
using Moq;
using NUnit.Framework;
using System;
using System.Web.Mvc;

namespace DnDGen.Web.Tests.Unit.Controllers.Characters
{
    [TestFixture]
    public class LeadershipControllerTests
    {
        private LeadershipController controller;
        private Mock<ILeadershipGenerator> mockLeadershipGenerator;
        private Mock<ClientIDManager> mockClientIdManager;
        private Guid clientId;

        [SetUp]
        public void Setup()
        {
            mockLeadershipGenerator = new Mock<ILeadershipGenerator>();
            mockClientIdManager = new Mock<ClientIDManager>();
            controller = new LeadershipController(mockLeadershipGenerator.Object, mockClientIdManager.Object);

            clientId = Guid.NewGuid();
        }

        [TestCase("Generate")]
        [TestCase("Cohort")]
        [TestCase("Follower")]
        public void ActionHandlesGetVerb(string action)
        {
            var attributes = AttributeProvider.GetAttributesFor(controller, action);
            Assert.That(attributes, Contains.Item(typeof(HttpGetAttribute)));
        }

        [Test]
        public void GenerateReturnsJsonResult()
        {
            var result = controller.Generate(clientId, 9266, 90210, "leader animal");
            Assert.That(result, Is.InstanceOf<JsonResult>());
        }

        [Test]
        public void GenerateJsonResultAllowsGet()
        {
            var result = controller.Generate(clientId, 9266, 90210, "leader animal") as JsonResult;
            Assert.That(result.JsonRequestBehavior, Is.EqualTo(JsonRequestBehavior.AllowGet));
        }

        [Test]
        public void GenerateSetsClientId()
        {
            var result = controller.Generate(clientId, 9266, 90210, "leader animal");
            Assert.That(result, Is.InstanceOf<JsonResult>());

            mockClientIdManager.Verify(m => m.SetClientID(It.IsAny<Guid>()), Times.Once);
            mockClientIdManager.Verify(m => m.SetClientID(clientId), Times.Once);
        }

        [Test]
        public void GenerateReturnsLeadershipFromGenerator()
        {
            var leadership = new Leadership();
            mockLeadershipGenerator.Setup(g => g.GenerateLeadership(9266, 90210, "leader animal")).Returns(leadership);

            var result = controller.Generate(clientId, 9266, 90210, "leader animal") as JsonResult;
            dynamic data = result.Data;
            Assert.That(data.leadership, Is.EqualTo(leadership));
        }

        [Test]
        public void CohortReturnsJsonResult()
        {
            var cohort = new Character();
            mockLeadershipGenerator.Setup(g => g.GenerateCohort(9266, 90210, "leader alignment", "leader class")).Returns(cohort);

            var result = controller.Cohort(clientId, 9266, 90210, "leader alignment", "leader class");
            Assert.That(result, Is.InstanceOf<JsonResult>());
        }

        [Test]
        public void CohortJsonResultAllowsGet()
        {
            var cohort = new Character();
            mockLeadershipGenerator.Setup(g => g.GenerateCohort(9266, 90210, "leader alignment", "leader class")).Returns(cohort);

            var result = controller.Cohort(clientId, 9266, 90210, "leader alignment", "leader class") as JsonResult;
            Assert.That(result.JsonRequestBehavior, Is.EqualTo(JsonRequestBehavior.AllowGet));
        }

        [Test]
        public void CohortSetsClientId()
        {
            var cohort = new Character();
            mockLeadershipGenerator.Setup(g => g.GenerateCohort(9266, 90210, "leader alignment", "leader class")).Returns(cohort);

            var result = controller.Cohort(clientId, 9266, 90210, "leader alignment", "leader class");
            Assert.That(result, Is.InstanceOf<JsonResult>());

            mockClientIdManager.Verify(m => m.SetClientID(It.IsAny<Guid>()), Times.Once);
            mockClientIdManager.Verify(m => m.SetClientID(clientId), Times.Once);
        }

        [Test]
        public void CohortReturnsCohortFromGenerator()
        {
            var cohort = new Character();
            mockLeadershipGenerator.Setup(g => g.GenerateCohort(9266, 90210, "leader alignment", "leader class")).Returns(cohort);

            var result = controller.Cohort(clientId, 9266, 90210, "leader alignment", "leader class") as JsonResult;
            dynamic data = result.Data;
            Assert.That(data.cohort, Is.EqualTo(cohort));
        }

        [Test]
        public void CohortSortsCharacterSkills()
        {
            var cohort = new Character();
            mockLeadershipGenerator.Setup(g => g.GenerateCohort(9266, 90210, "leader alignment", "leader class")).Returns(cohort);

            cohort.Skills = new[]
            {
                new Skill("zzzz", new Ability(string.Empty), 123456) { Ranks = 42 },
                new Skill("aaaa", new Ability(string.Empty), 123456, "ccccc") { Ranks = 600 },
                new Skill("aaaa", new Ability(string.Empty), 123456, "bbbbb") { Ranks = 1234 },
                new Skill("kkkk", new Ability(string.Empty), 123456) { Ranks = 1337 },
            };

            var result = controller.Cohort(clientId, 9266, 90210, "leader alignment", "leader class") as JsonResult;
            dynamic data = result.Data;
            Assert.That(data.cohort, Is.EqualTo(cohort));
            Assert.That(cohort.Skills, Is.Ordered.By("Name").Then.By("Focus"));
        }

        [Test]
        public void FollowerReturnsJsonResult()
        {
            var follower = new Character();
            mockLeadershipGenerator.Setup(g => g.GenerateFollower(9266, "leader alignment", "leader class")).Returns(follower);

            var result = controller.Follower(clientId, 9266, "leader alignment", "leader class");
            Assert.That(result, Is.InstanceOf<JsonResult>());
        }

        [Test]
        public void FollowerJsonResultAllowsGet()
        {
            var follower = new Character();
            mockLeadershipGenerator.Setup(g => g.GenerateFollower(9266, "leader alignment", "leader class")).Returns(follower);

            var result = controller.Follower(clientId, 9266, "leader alignment", "leader class") as JsonResult;
            Assert.That(result.JsonRequestBehavior, Is.EqualTo(JsonRequestBehavior.AllowGet));
        }

        [Test]
        public void FollowerSetsClientId()
        {
            var follower = new Character();
            mockLeadershipGenerator.Setup(g => g.GenerateFollower(9266, "leader alignment", "leader class")).Returns(follower);

            var result = controller.Follower(clientId, 9266, "leader alignment", "leader class");
            Assert.That(result, Is.InstanceOf<JsonResult>());

            mockClientIdManager.Verify(m => m.SetClientID(It.IsAny<Guid>()), Times.Once);
            mockClientIdManager.Verify(m => m.SetClientID(clientId), Times.Once);
        }

        [Test]
        public void FollowerReturnsFollowerFromGenerator()
        {
            var follower = new Character();
            mockLeadershipGenerator.Setup(g => g.GenerateFollower(9266, "leader alignment", "leader class")).Returns(follower);

            var result = controller.Follower(clientId, 9266, "leader alignment", "leader class") as JsonResult;
            dynamic data = result.Data;
            Assert.That(data.follower, Is.EqualTo(follower));
        }

        [Test]
        public void FollowerSortsCharacterSkills()
        {
            var follower = new Character();
            mockLeadershipGenerator.Setup(g => g.GenerateFollower(9266, "leader alignment", "leader class")).Returns(follower);

            follower.Skills = new[]
            {
                new Skill("zzzz", new Ability(string.Empty), 123456) { Ranks = 42 },
                new Skill("aaaa", new Ability(string.Empty), 123456, "ccccc") { Ranks = 600 },
                new Skill("aaaa", new Ability(string.Empty), 123456, "bbbbb") { Ranks = 1234 },
                new Skill("kkkk", new Ability(string.Empty), 123456) { Ranks = 1337 },
            };

            var result = controller.Follower(clientId, 9266, "leader alignment", "leader class") as JsonResult;
            dynamic data = result.Data;
            Assert.That(data.follower, Is.EqualTo(follower));
            Assert.That(follower.Skills, Is.Ordered.By("Name").Then.By("Focus"));
        }
    }
}
