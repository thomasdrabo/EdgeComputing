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

namespace Edge.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DockerClient _dockerClient;
        private string DockerApiUri()
        {
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            if (isWindows)
            {
                return "npipe://./pipe/docker_engine";
            }

            var isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

            if (isLinux)
            {
                return "unix:/var/run/docker.sock";
            }

            throw new Exception("Was unable to determine what OS this is running on, does not appear to be Windows or Linux!?");
        }
       

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _dockerClient = new DockerClientConfiguration(new Uri(DockerApiUri())).CreateClient();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public ActionResult LoadPackage()
        {
            return View();
        }

        public async Task<ActionResult> CreateImageAsync()
        {
            DockerClient client = new DockerClientConfiguration()
                .CreateClient();
            //Stream stream = await client.Images.CreateImageAsync(
            //new ImagesCreateParameters
            //{
            //    FromImage = "fedora/memcached",
            //    Tag = "alpha"
            //},
            //null);

            return View();
        }

        public async Task<ActionResult> PullImage()
        {
            await _dockerClient.Images
                .CreateImageAsync(new ImagesCreateParameters
                {
                    FromImage = "amazon/dynamodb-local",
                    Tag = "latest"
                },
                    new AuthConfig(),
                    new Progress<JSONMessage>());

            return View("Home");
        }

        public async Task<ActionResult> ContainerListAsync()
        {

            DockerClient client = new DockerClientConfiguration()
                .CreateClient();
            IList<ContainerListResponse> containers = await client.Containers.ListContainersAsync(
            new ContainersListParameters()
            {
                Limit = 10,
            });
            
            return PartialView(containers);
        }

        [HttpPost]
        [Route("StopContainer/{containerID}")]
        public async Task<ActionResult> StopContainer(string containerID)
        {
            await _dockerClient.Containers.KillContainerAsync(containerID, new ContainerKillParameters());
            return View("ContainerList");
        }

        public async Task<ActionResult> StartContainer(string containerID)
        {
            await _dockerClient.Containers.StartContainerAsync(containerID, new ContainerStartParameters());
            return View("ContainerList");
        }
    }
}
