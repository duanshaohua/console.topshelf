using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ConsoleTopshelf.Web
{
    public class Service
    {
        public async Task StartAsync(string[] args)
        {
            //操作逻辑
            Console.WriteLine("StartAsync");
            CreateHostBuilder(args).Build().Run();
        }

        public async Task StopAsync()
        {
            //操作逻辑
            Console.WriteLine("StopAsync");
        }

        public async Task ContinueAsync()
        {
            //操作逻辑
            Console.WriteLine("ContinueAsync");
        }

        public async Task PauseAsync()
        {
            //操作逻辑
            Console.WriteLine("PauseAsync");
        }
        internal static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //webBuilder.UseConfiguration(
                    //    new ConfigurationBuilder()
                    //    .SetBasePath(Environment.CurrentDirectory)
                    //    .AddJsonFile("host.json")
                    //    .Build()
                    //);

                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog();
        internal static IHostBuilder CreateServicesHostBuilder(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                host = host.UseWindowsService();
            }
            return host.ConfigureWebHostDefaults(webBuilder =>
            {
                //webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
                //{
                //    config.AddJsonFile("custom_settings.json");
                //});
                var port = 37488;//设置服务端口
                webBuilder.ConfigureKestrel(serverOptions =>
                {
                    serverOptions.Listen(IPAddress.Any, port);
                    serverOptions.Limits.MaxRequestBodySize = null;
                });
                webBuilder.UseStartup<Startup>();
            });
        }
    }
}
