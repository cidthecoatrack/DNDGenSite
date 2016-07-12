﻿using System.Web.Mvc;
using TreasureGen.Common;

namespace DnDGen.Web.Controllers.Treasures
{
    public abstract class TreasuresController : Controller
    {
        protected JsonResult BuildJsonResult(Treasure treasure)
        {
            return Json(new { treasure = treasure }, JsonRequestBehavior.AllowGet);
        }
    }
}