﻿using System;
using System.Web.Mvc;
using TreasureGen.Common;
using TreasureGen.Common.Items;
using TreasureGen.Generators.Items.Magical;

namespace DnDGen.Web.Controllers.Treasures
{
    public class WondrousItemController : TreasuresController
    {
        private MagicalItemGenerator wondrousItemGenerator;

        public WondrousItemController(MagicalItemGenerator wondrousItemGenerator)
        {
            this.wondrousItemGenerator = wondrousItemGenerator;
        }

        [HttpGet]
        public JsonResult Generate(String power)
        {
            var treasure = new Treasure();
            var item = GetWondrousItem(power);
            treasure.Items = new[] { item };

            return BuildJsonResult(treasure);
        }

        private Item GetWondrousItem(String power)
        {
            var item = wondrousItemGenerator.GenerateAtPower(power);

            while (item.ItemType != ItemTypeConstants.WondrousItem)
                item = wondrousItemGenerator.GenerateAtPower(power);

            return item;
        }
    }
}