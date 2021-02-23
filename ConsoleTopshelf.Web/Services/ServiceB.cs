using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ConsoleTopshelf.Web.Services
{
    public class ServiceB : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information("ServiceB正在开始。");

            stoppingToken.Register(() => Log.Information("ServiceB停止。"));

            while (!stoppingToken.IsCancellationRequested)
            {
                Log.Information("ServiceB正在做后台工作。");

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }

            Log.Information("ServiceB已经停止。");
        }
    }
}
