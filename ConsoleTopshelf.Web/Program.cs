using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            #region 进程检测
            string processName = Process.GetCurrentProcess().ProcessName;
            var processLength = Process.GetProcessesByName(processName).Length;
            //if (processLength > 1)
            //{
            //    Console.WriteLine($"{processName}-已运行");
            //    return;
            //}
            Log.Information($"【{processName}】进程启动中...");
            #endregion
            #region 日志
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
            #endregion
            #region 进入程序
            try
            {
                Log.Information("Starting...");

                HostFactory.Run(configure =>
                {
                    #region 基础配置
                    //定义服务描述
                    configure.SetDescription("DSH测试服务");
                    //服务显示名称
                    configure.SetDisplayName("DshService");
                    //服务名称
                    configure.SetServiceName("DshService");
                    #endregion
                    #region 自定义服务
                    //定义操作
                    configure.Service<TopshelfService>(service =>
                    {
                        //指定服务类型
                        service.ConstructUsing(hostSettings => new TopshelfService(hostSettings));
                        //当服务启动后执行什么
                        //service.WhenStarted(s => s.Start(args));
                        service.WhenStarted((s, hostControl) => s.Start(hostControl, args));
                        //当服务停止后执行什么
                        service.WhenStopped((s, hostControl) => s.Stop(hostControl));
                        ////当继续
                        //service.WhenContinued(async _ => await _.ContinueAsync());
                        ////当停了下来
                        //service.WhenPaused(async _ => await _.PauseAsync());
                        ////当关机时
                        //service.WhenShutdown(async _ => await _.ShutdownAsync());
                    });
                    #endregion
                    #region 服务启动模式
                    ////自动启动服务
                    //configure.StartAutomatically();
                    ////自动(延迟)——仅在.net 4.0或更高版本上可用
                    //configure.StartAutomaticallyDelayed();
                    ////手动启动服务
                    //configure.StartManually();
                    ////将服务安装为禁用状态
                    //configure.Disabled();
                    #endregion
                    #region 服务恢复
                    //configure.EnableServiceRecovery(r =>
                    //{
                    //    //等待延迟时间后，重新启动计算机
                    //    r.RestartComputer(5, "message");
                    //    //等待延迟时间后，重新启动服务
                    //    r.RestartService(0);
                    //    //运行指定的命令
                    //    r.RunProgram(7, "ping google.com");
                    //    //指定仅在服务崩溃时才采取恢复操作。如果服务以非零的退出码退出，则不会重新启动它。
                    //    r.OnCrashOnly();
                    //    //距离错误计数重置的天数
                    //    r.SetResetPeriod(1);
                    //});
                    #endregion
                    #region 服务标识
                    //服务可以配置为多个不同的身份运行，使用最合适的配置选项。
                    //configure.RunAs("username", "password");
                    //使用指定的用户名和密码运行服务。这也可以使用命令行进行配置。请务必将域名或 UPN 后缀包含在用户名值中，例如域名/用户名或username@suffix.com。
                    //configure.RunAsPrompt();
                    //安装服务时，安装人员将提示用于启动服务的用户名/密码组合。
                    //configure.RunAsNetworkService();
                    //使用NETWORK_SERVICE内置帐户运行服务。
                    configure.RunAsLocalSystem();
                    //使用本地系统帐户运行服务。
                    //configure.RunAsLocalService();
                    #endregion
                    #region 自定义安装操作
                    //安装操作之前
                    configure.BeforeInstall(settings =>
                    {
                        Log.Information("安装操作之前");
                    });
                    //安装操作后
                    configure.AfterInstall(settings =>
                    {
                        Log.Information("安装操作后");
                    });
                    //卸载操作之前
                    configure.BeforeUninstall(() =>
                    {
                        Log.Information("卸载操作之前");
                    });
                    //卸载操作后
                    configure.AfterUninstall(() =>
                    {
                        Log.Information("卸载操作后");
                    });
                    #endregion
                    #region 服务依赖项
                    ////指定服务依赖项
                    //configure.DependsOn("SomeOtherService");
                    ////知名服务的内置扩展方法
                    //configure.DependsOnMsmq(); // 微软消息排队
                    //configure.DependsOnMsSql(); // Microsoft SQL Server
                    //configure.DependsOnEventLog(); // Windows事件日志
                    //configure.DependsOnIis(); // 互联网信息服务器
                    #endregion
                    #region 高级设置
                    ////启用暂停和继续，指定服务支持暂停和继续，允许服务控制管理器通过暂停并继续命令服务。
                    //configure.EnablePauseAndContinue();
                    ////启用关闭，指定该服务支持关闭服务命令，允许服务控制管理器快速关闭服务。
                    //configure.EnableShutdown();
                    ////处理异常，为服务运行期间抛出的异常情况提供回调。此回调不是处理程序，不会影响 Topshelf 已经提供的默认异常处理。它旨在为触发外部操作、记录等提供的异常情况提供可见性。
                    configure.OnException(ex =>
                    {
                        // Do something with the exception
                        Log.Fatal(ex, "服务出现异常!");
                    });
                    #endregion
                });
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "服务意外终止!");
            }
            finally
            {
                Log.CloseAndFlush();
            }
            #endregion
        }
    }
}
