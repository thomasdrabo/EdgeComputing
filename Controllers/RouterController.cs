using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Edge.Models;
using Docker.DotNet;
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

        //[Authorize(Roles = "Manager")]
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

        //public async Task<ActionResult> CreateImageAsync()
        //{
        //    DockerClient client = new DockerClientConfiguration()
        //        .CreateClient();
        //    //Stream stream = await client.Images.CreateImageAsync(
        //    //new ImagesCreateParameters
        //    //{
        //    //    FromImage = "fedora/memcached",
        //    //    Tag = "alpha"
        //    //},
        //    //null);

        //    return View();
        //}

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

            var log = "";
            _sshClient.Connect();
            using (SshCommand cmd = _sshClient.CreateCommand($"show log"))
            {
                cmd.Execute();
                Console.WriteLine("Command>" + cmd.CommandText);
                Console.WriteLine("Return Value = {0}", cmd.ExitStatus);
                log = cmd.Result;
                Console.WriteLine(log);
            }
            _sshClient.Disconnect();
            ViewBag.Logs = (log);


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
            var data = result.Trim().Split().Where(x => x.Length > 0).ToList();
            var model = new ApplicationDetail();
            model.RessourceReservation = new Ressource();
            model.Application = new Details();
            model.AppID = appName;
            model.Owner = data[6];
            model.State = data[9];
            model.Application.Type = data[13];
            model.Application.Name = data[16];
            model.Application.Version = data[19];
            model.Application.Description = data[22]+ " " + data[23] + " " + data[24] + " " + data[25] + " " + data[26];
            model.Application.Path = string.Empty;
            model.Application.URLPath = string.Empty;
            model.RessourceReservation.Memory = data[41]+data[42];
            model.RessourceReservation.Disk = data[45]+data[46];
            model.RessourceReservation.CPU = data[49]+data[50];
            model.RessourceReservation.CPUPercent = data[53]+data[54];
            model.RessourceReservation.VCPU = data[57];
            ViewBag.Name = appName;
            return View("ApplicationDetail", model);
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
            return RedirectToAction("ShowApplicationList");
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
            return RedirectToAction("ShowApplicationList");
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
            return RedirectToAction("ShowApplicationList");
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
            return RedirectToAction("ShowApplicationList");
        }

        public ActionResult GetLogs()
        {
            var result = "";
            _sshClient.Connect();
            using (SshCommand cmd = _sshClient.CreateCommand($"show log"))
            {
                cmd.Execute();
                Console.WriteLine("Command>" + cmd.CommandText);
                Console.WriteLine("Return Value = {0}", cmd.ExitStatus);
                result = cmd.Result;
                Console.WriteLine(result);
            }
            _sshClient.Disconnect();
            ViewBag.Logs = (result);
            return View("RouterLogs");
        }


    }
}
