using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Tutorial_4_Business_Tier.Controllers
{
    // Controller for views in Model folder under Views
    public class MainController : Controller
    {
        public ActionResult MyIndex()
        {
            return View();
        }

        // default wep api controller methods altered superficially to reflect bankdb functionality
        public ActionResult Accounts()
        {
            ViewBag.Message = "Your accounts page.";

            return PartialView("Accounts");
        }

        public ActionResult Users()
        {
            ViewBag.Message = "Your users page.";

            return PartialView("Users");
        }

        public ActionResult Transaction()
        {
            ViewBag.Message = "Your transactions page.";

            return PartialView("Transaction");
        }
    }
}