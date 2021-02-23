using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ConsoleTopshelf.Web.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Topshelf;
using Topshelf.Runtime;
using Host = Microsoft.Extensions.Hosting.Host;

namespace ConsoleTopshelf.Web
{
    public class TopshelfService
    {
        private readonly HostSettings _settings;
        public TopshelfService(HostSettings settings)
        {
            _settings = settings;
            Log.Information("TopshelfService被调用");
        }

        public bool Start(HostControl hostControl, params string[] args)
        {
            Log.Information("服务启动");
            #region 环境检测
            //获取当前工作目录的完全限定路径。
            //string currentDirectory = Environment.CurrentDirectory;
            //string baseDirectory = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            //Log.Information($"CurrentDirectory:{currentDirectory};BaseDirectory:{baseDirectory}");
            //bool isRunWinService = (currentDirectory != baseDirectory);
            //Log.Information($"isRunWinService:{isRunWinService}");
            #endregion
            //if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && isRunWinService)
            //{
            //    CreateServicesHostBuilder(args).Build().RunAsync();
            //}
            //else
            //{
            //    CreateHostBuilder(args).Build().RunAsync();
            //}

            CreateHostBuilder(args).Build().RunAsync();
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            Log.Information("服务停止");
            return true;
        }

        public void ContinueAsync()
        {
            //操作逻辑
            Log.Information("ContinueAsync");
        }

        public void PauseAsync()
        {
            //操作逻辑
            Log.Information("PauseAsync");
        }

        public void ShutdownAsync()
        {
            //操作逻辑
            Log.Information("ShutdownAsync");
        }
        /// <summary>
        /// 创建主机
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        internal static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseConfiguration(
                        new ConfigurationBuilder()
                        .SetBasePath(Environment.CurrentDirectory)
                        .AddJsonFile("host.json")
                        .Build()
                    );
                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog();
        /// <summary>
        /// 创建Windows 服务中的主机（未调试成功）
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        internal static IHostBuilder CreateServicesHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseWindowsService()//config => { }
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("host.json");
                })
                //.ConfigureServices((hostContext, services) =>
                //{
                //    services.AddHostedService<ServiceA>();
                //    services.AddHostedService<ServiceB>();
                //})
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //webBuilder.UseConfiguration(
                    //    new ConfigurationBuilder()
                    //    .SetBasePath(Environment.CurrentDirectory)
                    //    .AddJsonFile("host.json")
                    //    .Build()
                    //);
                    webBuilder
                    //.UseUrls("http://*:37488")
                              .UseStartup<Startup>()
                              .UseKestrel();
                });
        }
    }
}
