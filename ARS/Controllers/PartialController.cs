using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ARS.Controllers
{
    public class PartialController : Controller
    {
        [ChildActionOnly]
        public ActionResult RenderLogin()
        {
            return PartialView("_LoginPartial");
        }

        [ChildActionOnly]
        public ActionResult RenderRegister()
        {
            return PartialView("_RegisterPartial");
        }
    }
}