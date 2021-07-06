using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Docker.DotNet;
using Docker.DotNet.Models;
using System.IO;
using System.Diagnostics;

namespace Edge
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        

        public static bool LoadOnSwitch(string path)
        {
            bool response;
            var package = File.Open(path, FileMode.Open);
            var processInfo = new ProcessStartInfo("docker", $"run -it --rm blahblahblah");

            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;

            int exitCode;
            using (var process = new Process())
            {
                process.StartInfo = processInfo;
                //process.OutputDataReceived += new DataReceivedEventHandler();
                //process.ErrorDataReceived += new DataReceivedEventHandler(;

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit(1200000);
                if (!process.HasExited)
                {
                    process.Kill();
                }

                exitCode = process.ExitCode;
                process.Close();
            }
            //if ()
            //{
                response = true;
            //}
            //else
            //{
            //    response = false;
            //}

            return response;
        }
}
}
