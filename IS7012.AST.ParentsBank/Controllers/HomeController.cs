using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IS7012.AST.ParentsBank.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        
        public ActionResult FinancialResources()
        {
            ViewBag.Message = "Your Financial Resources Page.";
            //bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            //if (val1)
            //{
            //    return View();
            //}
            //else
            //    return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest); ;
            return View();
        }
        [Authorize]
        public ActionResult Wishlist()
        {
            ViewBag.Message = "Your Wishlist.";
            return View();
        }
    }
}