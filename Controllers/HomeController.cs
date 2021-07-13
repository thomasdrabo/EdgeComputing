using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Edge.Models;
using Docker.DotNet;
using Docker.DotNet.Models;
using System.IO;
using System.Runtime.InteropServices;
using Renci.SshNet;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace Edge.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
       

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
    }
}
