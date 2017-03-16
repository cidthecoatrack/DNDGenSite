﻿using CharacterGen;
using CharacterGen.Abilities.Feats;
using CharacterGen.Abilities.Skills;
using CharacterGen.Abilities.Stats;
using DnDGen.Web.Controllers;
using DnDGen.Web.Models;
using EncounterGen.Common;
using EncounterGen.Generators;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DnDGen.Web.Tests.Unit.Controllers
{
    [TestFixture]
    public class EncounterControllerTests
    {
        private EncounterController controller;
        private Mock<IEncounterGenerator> mockEncounterGenerator;
        private Mock<IEncounterVerifier> mockEncounterVerifier;
        private List<string> filters;

        [SetUp]
        public void Setup()
        {
            mockEncounterGenerator = new Mock<IEncounterGenerator>();
            mockEncounterVerifier = new Mock<IEncounterVerifier>();
            controller = new EncounterController(mockEncounterGenerator.Object, mockEncounterVerifier.Object);

            filters = new List<string>();
        }

        [TestCase("Index")]
        [TestCase("Generate")]
        [TestCase("Validate")]
        public void ActionHandlesGetVerb(string methodName)
        {
            var attributes = AttributeProvider.GetAttributesFor(controller, methodName);
            Assert.That(attributes, Contains.Item(typeof(HttpGetAttribute)));
        }

        [Test]
        public void IndexReturnsView()
        {
            var result = controller.Index();
            Assert.That(result, Is.InstanceOf<ViewResult>());
        }

        [Test]
        public void IndexViewContainsModel()
        {
            var result = controller.Index() as ViewResult;
            Assert.That(result.Model, Is.InstanceOf<EncounterViewModel>());
        }

        [Test]
        public void IndexModelContainsEnvironments()
        {
            var result = controller.Index() as ViewResult;
            var model = result.Model as EncounterViewModel;
            Assert.That(model.Environments, Contains.Item(EnvironmentConstants.Civilized));
            Assert.That(model.Environments, Contains.Item(EnvironmentConstants.Desert));
            Assert.That(model.Environments, Contains.Item(EnvironmentConstants.Dungeon));
            Assert.That(model.Environments, Contains.Item(EnvironmentConstants.Forest));
            Assert.That(model.Environments, Contains.Item(EnvironmentConstants.Hills));
            Assert.That(model.Environments, Contains.Item(EnvironmentConstants.Marsh));
            Assert.That(model.Environments, Contains.Item(EnvironmentConstants.Mountain));
            Assert.That(model.Environments, Contains.Item(EnvironmentConstants.Plains));
            Assert.That(model.Environments.Count(), Is.EqualTo(8));
        }

        [Test]
        public void IndexModelContainsTemperatures()
        {
            var result = controller.Index() as ViewResult;
            var model = result.Model as EncounterViewModel;
            Assert.That(model.Temperatures, Contains.Item(EnvironmentConstants.Temperatures.Cold));
            Assert.That(model.Temperatures, Contains.Item(EnvironmentConstants.Temperatures.Temperate));
            Assert.That(model.Temperatures, Contains.Item(EnvironmentConstants.Temperatures.Warm));
            Assert.That(model.Temperatures.Count(), Is.EqualTo(3));
        }

        [Test]
        public void IndexModelContainsTimesOfDay()
        {
            var result = controller.Index() as ViewResult;
            var model = result.Model as EncounterViewModel;
            Assert.That(model.TimesOfDay, Contains.Item(EnvironmentConstants.TimesOfDay.Day));
            Assert.That(model.TimesOfDay, Contains.Item(EnvironmentConstants.TimesOfDay.Night));
            Assert.That(model.TimesOfDay.Count(), Is.EqualTo(2));
        }

        [Test]
        public void IndexModelContainsCreatureTypes()
        {
            var result = controller.Index() as ViewResult;
            var model = result.Model as EncounterViewModel;
            Assert.That(model.CreatureTypes, Contains.Item(CreatureConstants.Types.Aberration));
            Assert.That(model.CreatureTypes, Contains.Item(CreatureConstants.Types.Animal));
            Assert.That(model.CreatureTypes, Contains.Item(CreatureConstants.Types.Construct));
            Assert.That(model.CreatureTypes, Contains.Item(CreatureConstants.Types.Dragon));
            Assert.That(model.CreatureTypes, Contains.Item(CreatureConstants.Types.Elemental));
            Assert.That(model.CreatureTypes, Contains.Item(CreatureConstants.Types.Fey));
            Assert.That(model.CreatureTypes, Contains.Item(CreatureConstants.Types.Giant));
            Assert.That(model.CreatureTypes, Contains.Item(CreatureConstants.Types.Humanoid));
            Assert.That(model.CreatureTypes, Contains.Item(CreatureConstants.Types.MagicalBeast));
            Assert.That(model.CreatureTypes, Contains.Item(CreatureConstants.Types.MonstrousHumanoid));
            Assert.That(model.CreatureTypes, Contains.Item(CreatureConstants.Types.Ooze));
            Assert.That(model.CreatureTypes, Contains.Item(CreatureConstants.Types.Outsider));
            Assert.That(model.CreatureTypes, Contains.Item(CreatureConstants.Types.Plant));
            Assert.That(model.CreatureTypes, Contains.Item(CreatureConstants.Types.Undead));
            Assert.That(model.CreatureTypes, Contains.Item(CreatureConstants.Types.Vermin));
            Assert.That(model.CreatureTypes.Count(), Is.EqualTo(15));
        }

        [Test]
        public void GenerateReturnsJsonResult()
        {
            var encounter = new Encounter();
            encounter.Characters = Enumerable.Empty<Character>();
            mockEncounterGenerator.Setup(g => g.Generate("environment", 9266, "temperature", "time of day")).Returns(encounter);

            var result = controller.Generate("environment", "temperature", "time of day", 9266, filters);
            Assert.That(result, Is.InstanceOf<JsonResult>());
        }

        [Test]
        public void GenerateJsonAllowsGet()
        {
            var encounter = new Encounter();
            encounter.Characters = Enumerable.Empty<Character>();
            mockEncounterGenerator.Setup(g => g.Generate("environment", 9266, "temperature", "time of day")).Returns(encounter);

            var result = controller.Generate("environment", "temperature", "time of day", 9266, filters) as JsonResult;
            Assert.That(result.JsonRequestBehavior, Is.EqualTo(JsonRequestBehavior.AllowGet));
        }

        [Test]
        public void GenerateJsonReturnsGeneratedEncounter()
        {
            var encounter = new Encounter();
            encounter.Characters = Enumerable.Empty<Character>();
            mockEncounterGenerator.Setup(g => g.Generate("environment", 9266, "temperature", "time of day")).Returns(encounter);

            var result = controller.Generate("environment", "temperature", "time of day", 9266, filters) as JsonResult;
            dynamic data = result.Data;
            Assert.That(data.encounter, Is.EqualTo(encounter));
        }

        [Test]
        public void GenerateSortsCharacterFeats()
        {
            var character = new Character();
            var otherCharacter = new Character();
            var encounter = new Encounter();

            encounter.Characters = new[] { character, otherCharacter };
            mockEncounterGenerator.Setup(g => g.Generate("environment", 9266, "temperature", "time of day")).Returns(encounter);

            character.Ability.Feats = new[]
            {
                new Feat { Name = "zzzz" },
                new Feat { Name = "aaa" },
                new Feat { Name = "kkkk" }
            };

            otherCharacter.Ability.Feats = new[]
            {
                new Feat { Name = "a" },
                new Feat { Name = "aa" },
                new Feat { Name = "a" }
            };

            var result = controller.Generate("environment", "temperature", "time of day", 9266, filters) as JsonResult;
            dynamic data = result.Data;
            Assert.That(data.encounter, Is.EqualTo(encounter));

            var firstCharacter = encounter.Characters.First();
            var lastCharacter = encounter.Characters.Last();

            Assert.That(encounter.Characters.Count(), Is.EqualTo(2));
            Assert.That(firstCharacter, Is.Not.EqualTo(lastCharacter));
            Assert.That(firstCharacter, Is.EqualTo(character));
            Assert.That(firstCharacter.Ability.Feats, Is.Ordered.By("Name"));
            Assert.That(lastCharacter, Is.EqualTo(otherCharacter));
            Assert.That(lastCharacter.Ability.Feats, Is.Ordered.By("Name"));
        }

        [Test]
        public void GenerateSortsCharacterSkills()
        {
            var character = new Character();
            var otherCharacter = new Character();
            var encounter = new Encounter();

            encounter.Characters = new[] { character, otherCharacter };
            mockEncounterGenerator.Setup(g => g.Generate("environment", 9266, "temperature", "time of day")).Returns(encounter);

            character.Ability.Skills["zzzz"] = new Skill(string.Empty, new Stat(string.Empty), int.MaxValue) { Ranks = 42 };
            character.Ability.Skills["aaaa"] = new Skill(string.Empty, new Stat(string.Empty), int.MaxValue) { Ranks = 600 };
            character.Ability.Skills["kkkk"] = new Skill(string.Empty, new Stat(string.Empty), int.MaxValue) { Ranks = 1337 };

            otherCharacter.Ability.Skills["a"] = new Skill(string.Empty, new Stat(string.Empty), int.MaxValue) { Ranks = 1234 };
            otherCharacter.Ability.Skills["b"] = new Skill(string.Empty, new Stat(string.Empty), int.MaxValue) { Ranks = 2345 };
            otherCharacter.Ability.Skills["aa"] = new Skill(string.Empty, new Stat(string.Empty), int.MaxValue) { Ranks = 3456 };

            var result = controller.Generate("environment", "temperature", "time of day", 9266, filters) as JsonResult;
            dynamic data = result.Data;
            Assert.That(data.encounter, Is.EqualTo(encounter));

            var firstCharacter = encounter.Characters.First();
            var lastCharacter = encounter.Characters.Last();

            Assert.That(encounter.Characters.Count(), Is.EqualTo(2));
            Assert.That(firstCharacter, Is.Not.EqualTo(lastCharacter));
            Assert.That(firstCharacter, Is.EqualTo(character));
            Assert.That(firstCharacter.Ability.Skills, Is.Ordered.By("Key"));
            Assert.That(lastCharacter, Is.EqualTo(otherCharacter));
            Assert.That(lastCharacter.Ability.Skills, Is.Ordered.By("Key"));

            Assert.That(firstCharacter.Ability.Skills["aaaa"].Ranks, Is.EqualTo(600));
            Assert.That(firstCharacter.Ability.Skills["kkkk"].Ranks, Is.EqualTo(1337));
            Assert.That(firstCharacter.Ability.Skills["zzzz"].Ranks, Is.EqualTo(42));
            Assert.That(lastCharacter.Ability.Skills["a"].Ranks, Is.EqualTo(1234));
            Assert.That(lastCharacter.Ability.Skills["aa"].Ranks, Is.EqualTo(3456));
            Assert.That(lastCharacter.Ability.Skills["b"].Ranks, Is.EqualTo(2345));
        }

        [Test]
        public void GenerateJsonUsesFilters()
        {
            var encounter = new Encounter();
            encounter.Characters = Enumerable.Empty<Character>();
            filters.Add("filter 1");
            filters.Add("filter 2");
            mockEncounterGenerator.Setup(g => g.Generate("environment", 9266, "temperature", "time of day",
                It.Is<string[]>(ss => ss.Except(filters).Any() == false && ss.Count() == filters.Count))).Returns(encounter);

            var result = controller.Generate("environment", "temperature", "time of day", 9266, filters) as JsonResult;
            dynamic data = result.Data;
            Assert.That(data.encounter, Is.EqualTo(encounter));
        }

        [Test]
        public void ValidateReturnsJsonResult()
        {
            filters.Add("filter 1");
            filters.Add("filter 2");
            mockEncounterVerifier.Setup(g => g.ValidEncounterExistsAtLevel("environment", 9266, "temperature", "time of day",
                It.Is<string[]>(ss => ss.Except(filters).Any() == false && ss.Count() == filters.Count))).Returns(true);

            var result = controller.Validate("environment", "temperature", "time of day", 9266, filters);
            Assert.That(result, Is.InstanceOf<JsonResult>());
        }

        [Test]
        public void ValidateJsonAllowsGet()
        {
            filters.Add("filter 1");
            filters.Add("filter 2");
            mockEncounterVerifier.Setup(g => g.ValidEncounterExistsAtLevel("environment", 9266, "temperature", "time of day",
                It.Is<string[]>(ss => ss.Except(filters).Any() == false && ss.Count() == filters.Count))).Returns(true);

            var result = controller.Validate("environment", "temperature", "time of day", 9266, filters) as JsonResult;
            Assert.That(result.JsonRequestBehavior, Is.EqualTo(JsonRequestBehavior.AllowGet));
        }

        [Test]
        public void ValidateJsonReturnsValid()
        {
            filters.Add("filter 1");
            filters.Add("filter 2");
            mockEncounterVerifier.Setup(g => g.ValidEncounterExistsAtLevel("environment", 9266, "temperature", "time of day",
                It.Is<string[]>(ss => ss.Except(filters).Any() == false && ss.Count() == filters.Count))).Returns(true);

            var result = controller.Validate("environment", "temperature", "time of day", 9266, filters) as JsonResult;
            dynamic data = result.Data;
            Assert.That(data.isValid, Is.True);
        }

        [Test]
        public void ValidateJsonReturnsInvalid()
        {
            filters.Add("filter 1");
            filters.Add("filter 2");
            mockEncounterVerifier.Setup(g => g.ValidEncounterExistsAtLevel("environment", 9266, "temperature", "time of day",
                It.Is<string[]>(ss => ss.Except(filters).Any() == false && ss.Count() == filters.Count))).Returns(false);

            var result = controller.Validate("environment", "temperature", "time of day", 9266, filters) as JsonResult;
            dynamic data = result.Data;
            Assert.That(data.isValid, Is.False);
        }

        [Test]
        public void CanValidateNullFilters()
        {
            mockEncounterVerifier.Setup(g => g.ValidEncounterExistsAtLevel("environment", 9266, "temperature", "time of day",
                It.Is<string[]>(ss => ss.Any() == false))).Returns(true);

            var result = controller.Validate("environment", "temperature", "time of day", 9266, null) as JsonResult;
            dynamic data = result.Data;
            Assert.That(data.isValid, Is.True);
        }

        [Test]
        public void CanGenerateNullFilters()
        {
            var encounter = new Encounter();
            encounter.Characters = Enumerable.Empty<Character>();
            mockEncounterGenerator.Setup(g => g.Generate("environment", 9266, "temperature", "time of day",
                It.Is<string[]>(ss => ss.Any() == false))).Returns(encounter);

            var result = controller.Generate("environment", "temperature", "time of day", 9266, null) as JsonResult;
            dynamic data = result.Data;
            Assert.That(data.encounter, Is.EqualTo(encounter));
        }
    }
}