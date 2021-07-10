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

namespace Edge.Controllers
{
    public class RouterController : Controller
    {
        private readonly ILogger<RouterController> _logger;
        private readonly DockerClient _dockerClient;
        private readonly SshClient _sshClient;

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
       

        public RouterController(ILogger<RouterController> logger)
        {
            _logger = logger;
            _dockerClient = new DockerClientConfiguration(new Uri(DockerApiUri())).CreateClient();
            _sshClient = new SshClient("192.168.40.40", "cisco", "cisco");
        }

        public IActionResult Index()
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

        public ActionResult CiscoSwitchConnectionSSH()
        {

            using (SshClient ssh = new SshClient("192.168.40.40", "cisco", "cisco"))
            {
                ssh.Connect();
                using (SshCommand cmd = ssh.CreateCommand("show vlan"))
                {
                    cmd.Execute();
                    Console.WriteLine("Command>" + cmd.CommandText);
                    Console.WriteLine("Return Value = {0}", cmd.ExitStatus);
                    var toto = cmd.Result;
                    Console.WriteLine(toto);
                }
                ssh.Disconnect();
            }
            return View("Router");
    
        }

        public ActionResult ShowApplicationList()
        {

            var result = "";
            _sshClient.Connect();
            using (SshCommand cmd = _sshClient.CreateCommand("show app-hosting list"))
            {
                cmd.Execute();
                Console.WriteLine("Command>" + cmd.CommandText);
                Console.WriteLine("Return Value = {0}", cmd.ExitStatus);
                result = cmd.Result;
                Console.WriteLine(result);
            }
            _sshClient.Disconnect();
            var data = result.Split().Where(x => x.Length > 0).Skip(4).ToList();
            var i = 0;
            var list = new List<ApplicationList>();
            while (i < data.Count)
            {
                var model = new ApplicationList();
                model.AppID = data[i];
                model.State = data[i+1];
                i += 2;
                list.Add(model);
            }
            ViewBag.Result = (list);
            return View("ApplicationList", list);
        }

        public ActionResult ShowApplicationDetail(string appName)
        {
            var result = "";
            _sshClient.Connect();
            using (SshCommand cmd = _sshClient.CreateCommand($"show app-hosting detail appid {appName}"))
            {
                cmd.Execute();
                Console.WriteLine("Command>" + cmd.CommandText);
                Console.WriteLine("Return Value = {0}", cmd.ExitStatus);
                result = cmd.Result;
                Console.WriteLine(result);
            }
            _sshClient.Disconnect();
            ViewBag.Result = (result);
            return View("Index");
        }

        public ActionResult StopApplication(string appName)
        {
            var result = "";
            _sshClient.Connect();
            using (SshCommand cmd = _sshClient.CreateCommand($"app-hosting stop appid {appName}"))
            {
                cmd.Execute();
                Console.WriteLine("Command>" + cmd.CommandText);
                Console.WriteLine("Return Value = {0}", cmd.ExitStatus);
                result = cmd.Result;
                Console.WriteLine(result);
            }
            _sshClient.Disconnect();
            ViewBag.Result = (result);
            return View("/Router/ApplicationList");
        }

        public ActionResult StartApplication(string appName)
        {
            var result = "";
            _sshClient.Connect();
            using (SshCommand cmd = _sshClient.CreateCommand($"app-hosting start appid {appName}"))
            {
                cmd.Execute();
                Console.WriteLine("Command>" + cmd.CommandText);
                Console.WriteLine("Return Value = {0}", cmd.ExitStatus);
                result = cmd.Result;
                Console.WriteLine(result);
            }
            _sshClient.Disconnect();
            ViewBag.Result = (result);
            return View("/Router/ApplicationList");
        }

        public ActionResult ActivateApplication(string appName)
        {
            var result = "";
            _sshClient.Connect();
            using (SshCommand cmd = _sshClient.CreateCommand($"app-hosting activate appid {appName}"))
            {
                cmd.Execute();
                Console.WriteLine("Command>" + cmd.CommandText);
                Console.WriteLine("Return Value = {0}", cmd.ExitStatus);
                result = cmd.Result;
                Console.WriteLine(result);
            }
            _sshClient.Disconnect();
            ViewBag.Result = (result);
            return View("/Router/ApplicationList");
        }

        public ActionResult DeactivateApplication(string appName)
        {
            var result = "";
            _sshClient.Connect();
            using (SshCommand cmd = _sshClient.CreateCommand($"app-hosting deactivate appid {appName}"))
            {
                cmd.Execute();
                Console.WriteLine("Command>" + cmd.CommandText);
                Console.WriteLine("Return Value = {0}", cmd.ExitStatus);
                result = cmd.Result;
                Console.WriteLine(result);
            }
            _sshClient.Disconnect();
            ViewBag.Result = (result);
            return View("~/Views/Router/ApplicationList");
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

            return View("Router");
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

        public async Task<ActionResult> StopContainer(string containerID)
        {
            try
            {
                await _dockerClient.Containers.KillContainerAsync(containerID, new ContainerKillParameters());
                await ContainerListAsync();
                return View("ContainerList");
            }
            catch(Exception e)
            {
                _logger.LogError("Une erreur est survenue : " + e.Message);
                await ContainerListAsync();
                return View("ContainerList");
            }
        }

        public async Task<ActionResult> StartContainer(string containerID)
        {
            try
            {
                await _dockerClient.Containers.StartContainerAsync(containerID, new ContainerStartParameters());
                await ContainerListAsync();
                return View("ContainerList");
            }
            catch (Exception e)
            {
                _logger.LogError("Une erreur est survenue : " + e.Message);
                await ContainerListAsync();
                return View("ContainerList");
            }

        }
    }
}
