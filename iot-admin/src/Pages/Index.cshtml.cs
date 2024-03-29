﻿using System;
using System.Collections.Generic;
using io = System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using IotStarterKit.Docker;
using IotStarterKit.Docker.Commands;

namespace IotStarterKit.Pages
{
    public class IndexModel : PageModel
    {        
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public string JenkinsUrl {get; private set;}
        public string JenkinsUsername {get; private set;}
        public string JenkinsPassword {get; private set;}
        public string GitHubUrl {get; private set;}

        public IndexModel(ILogger<IndexModel> logger, IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;

            JenkinsUrl = Secret.Read("jenkins-url");
            JenkinsUsername = Secret.Read("jenkins-username");
            JenkinsPassword = Secret.Read("jenkins-password");

            var githubRepo = Secret.Read("github-repo");
            var githubUser = Secret.Read("github-username");
            GitHubUrl = $"https://github.com/{githubUser}/{githubRepo}.git";
        }      

        public void OnGet()
        {
        }

        
        public void OnPostTest()
        {
            var composeFilePath = io.Path.Combine(_hostingEnvironment.WebRootPath, "stacks", "service.yml");
            _logger.LogInformation($"Deploying stack from: {composeFilePath}, on: Test Server");
            var cmd = new StackDeploy(composeFilePath, Target.TestServer);
            var output = cmd.Run();
            _logger.LogDebug(output);
        }

        public void OnPostLocal()
        {           
            var composeFilePath = io.Path.Combine(io.Directory.GetCurrentDirectory(), "stacks", "device.yml");
            _logger.LogInformation($"Deploying stack from: {composeFilePath}, on: Local Device");
            var cmd = new StackDeploy(composeFilePath, Target.LocalDevice);
            var output = cmd.Run();
            _logger.LogDebug(output);
        }
    }
}
