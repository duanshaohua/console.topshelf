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
                    //�����������
                    configure.SetDescription("DSH���Է���");
                    //������ʾ����
                    configure.SetDisplayName("DshService");
                    //��������
                    configure.SetServiceName("DshService");

                    //�����ñ���ϵͳ�˺�������
                    configure.RunAsLocalSystem();

                    //�������
                    configure.Service<Service>(service =>
                    {
                        //ָ����������
                        service.ConstructUsing(_ => new Service());
                        //������������ִ��ʲô
                        service.WhenStarted(async _ => await _.StartAsync(args));
                        //������ֹͣ��ִ��ʲô
                        service.WhenStopped(async _ => await _.StopAsync());
                        //������
                        service.WhenContinued(async _ => await _.ContinueAsync());
                        //��ͣ������
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
