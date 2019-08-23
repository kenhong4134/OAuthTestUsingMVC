using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OAuth_WebApp.Controllers
{
    public class OAuthController : Controller
    {
        // GET: OAuth
        public ActionResult Index()
        {
            return View();
        }
    }
}