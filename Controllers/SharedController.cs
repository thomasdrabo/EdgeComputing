using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edge.Controllers
{
    public class SharedController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public ActionResult RedirectLogin()
        {
            return View("Shared_LoginPartial");
        }
    }
}
