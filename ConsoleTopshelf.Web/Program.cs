using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Topshelf;

namespace ConsoleTopshelf.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
#if DEBUG
        .MinimumLevel.Debug()
#else
        .MinimumLevel.Information()
#endif
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Async(c => c.File("Logs/logs.txt"))
#if DEBUG
        .WriteTo.Async(c => c.Console())
#endif
        .CreateLogger();

            try
            {
                Log.Information("Starting...");

                HostFactory.Run(configure =>
                {
                    //定义服务描述
                    configure.SetDescription("DSH测试服务");
                    //服务显示名称
                    configure.SetDisplayName("DshService");
                    //服务名称
                    configure.SetServiceName("DshService");

                    //服务用本地系统账号来运行
                    configure.RunAsLocalSystem();

                    //定义操作
                    configure.Service<Service>(service =>
                    {
                        //指定服务类型
                        service.ConstructUsing(_ => new Service());
                        //当服务启动后执行什么
                        service.WhenStarted(async _ => await _.StartAsync(args));
                        //当服务停止后执行什么
                        service.WhenStopped(async _ => await _.StopAsync());
                        //当继续
                        service.WhenContinued(async _ => await _.ContinueAsync());
                        //当停了下来
                        service.WhenPaused(async _ => await _.PauseAsync());
                    });
                });
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Service terminated unexpectedly!");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
