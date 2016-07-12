﻿using DnDGen.Web.Controllers.Treasures;
using Moq;
using NUnit.Framework;
using System.Web.Mvc;
using TreasureGen.Common.Items;
using TreasureGen.Generators.Items.Magical;

namespace DnDGen.Web.Tests.Unit.Controllers.Treasures
{
    [TestFixture]
    public class PotionControllerTests
    {
        private PotionController controller;
        private Mock<MagicalItemGenerator> mockPotionGenerator;
        private Item potion;

        [SetUp]
        public void Setup()
        {
            mockPotionGenerator = new Mock<MagicalItemGenerator>();
            controller = new PotionController(mockPotionGenerator.Object);

            potion = new Item { ItemType = ItemTypeConstants.Potion };
            mockPotionGenerator.Setup(g => g.GenerateAtPower("power")).Returns(potion);
        }

        [Test]
        public void GenerateHandlesGetVerb()
        {
            var attributes = AttributeProvider.GetAttributesFor(controller, "Generate");
            Assert.That(attributes, Contains.Item(typeof(HttpGetAttribute)));
        }

        [Test]
        public void GenerateReturnsJsonResult()
        {
            var result = controller.Generate("power");
            Assert.That(result, Is.InstanceOf<JsonResult>());
        }

        [Test]
        public void GenerateJsonResultAllowsGet()
        {
            var result = controller.Generate("power") as JsonResult;
            Assert.That(result.JsonRequestBehavior, Is.EqualTo(JsonRequestBehavior.AllowGet));
        }

        [Test]
        public void GenerateReturnsPotionFromGenerator()
        {
            var result = controller.Generate("power") as JsonResult;
            dynamic data = result.Data;

            Assert.That(data.treasure.Items[0], Is.EqualTo(potion));
            Assert.That(data.treasure.Items.Length, Is.EqualTo(1));
        }

        [Test]
        public void ItemTypeMustBePotion()
        {
            var otherItem = new Item { ItemType = "other" };
            mockPotionGenerator.SetupSequence(g => g.GenerateAtPower("power"))
                .Returns(otherItem)
                .Returns(potion);

            var result = controller.Generate("power") as JsonResult;
            dynamic data = result.Data;

            Assert.That(data.treasure.Items[0], Is.EqualTo(potion));
            Assert.That(data.treasure.Items.Length, Is.EqualTo(1));
        }
    }
}