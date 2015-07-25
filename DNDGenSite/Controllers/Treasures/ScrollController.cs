﻿using System;
using System.Web.Mvc;
using TreasureGen.Common;
using TreasureGen.Common.Items;
using TreasureGen.Generators.Items.Magical;

namespace DNDGenSite.Controllers.Treasures
{
    public class ScrollController : TreasuresController
    {
        private IMagicalItemGenerator scrollGenerator;

        public ScrollController(IMagicalItemGenerator scrollGenerator)
        {
            this.scrollGenerator = scrollGenerator;
        }

        [HttpGet]
        public JsonResult Generate(String power)
        {
            var treasure = new Treasure();
            var item = GetScroll(power);
            treasure.Items = new[] { item };

            return BuildJsonResult(treasure);
        }

        private Item GetScroll(String power)
        {
            var item = scrollGenerator.GenerateAtPower(power);

            while (item.ItemType != ItemTypeConstants.Scroll)
                item = scrollGenerator.GenerateAtPower(power);

            return item;
        }
    }
}