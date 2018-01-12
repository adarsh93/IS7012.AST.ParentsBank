using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IS7012.AST.ParentsBank.Controllers
{
    public class HelpController : Controller
    {
        // GET: Help
        [ActionName("financial-resources")]
        public ActionResult Index()
        {
            return View();
        }
    }
}