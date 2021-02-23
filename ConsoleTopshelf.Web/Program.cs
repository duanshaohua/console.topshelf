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
            #region ���̼��
            string processName = Process.GetCurrentProcess().ProcessName;
            var processLength = Process.GetProcessesByName(processName).Length;
            //if (processLength > 1)
            //{
            //    Console.WriteLine($"{processName}-������");
            //    return;
            //}
            Log.Information($"��{processName}������������...");
            #endregion
            #region ��־
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
            #region �������
            try
            {
                Log.Information("Starting...");

                HostFactory.Run(configure =>
                {
                    #region ��������
                    //�����������
                    configure.SetDescription("DSH���Է���");
                    //������ʾ����
                    configure.SetDisplayName("DshService");
                    //��������
                    configure.SetServiceName("DshService");
                    #endregion
                    #region �Զ������
                    //�������
                    configure.Service<TopshelfService>(service =>
                    {
                        //ָ����������
                        service.ConstructUsing(hostSettings => new TopshelfService(hostSettings));
                        //������������ִ��ʲô
                        //service.WhenStarted(s => s.Start(args));
                        service.WhenStarted((s, hostControl) => s.Start(hostControl, args));
                        //������ֹͣ��ִ��ʲô
                        service.WhenStopped((s, hostControl) => s.Stop(hostControl));
                        ////������
                        //service.WhenContinued(async _ => await _.ContinueAsync());
                        ////��ͣ������
                        //service.WhenPaused(async _ => await _.PauseAsync());
                        ////���ػ�ʱ
                        //service.WhenShutdown(async _ => await _.ShutdownAsync());
                    });
                    #endregion
                    #region ��������ģʽ
                    ////�Զ���������
                    //configure.StartAutomatically();
                    ////�Զ�(�ӳ�)��������.net 4.0����߰汾�Ͽ���
                    //configure.StartAutomaticallyDelayed();
                    ////�ֶ���������
                    //configure.StartManually();
                    ////������װΪ����״̬
                    //configure.Disabled();
                    #endregion
                    #region ����ָ�
                    //configure.EnableServiceRecovery(r =>
                    //{
                    //    //�ȴ��ӳ�ʱ����������������
                    //    r.RestartComputer(5, "message");
                    //    //�ȴ��ӳ�ʱ���������������
                    //    r.RestartService(0);
                    //    //����ָ��������
                    //    r.RunProgram(7, "ping google.com");
                    //    //ָ�����ڷ������ʱ�Ų�ȡ�ָ���������������Է�����˳����˳����򲻻�������������
                    //    r.OnCrashOnly();
                    //    //�������������õ�����
                    //    r.SetResetPeriod(1);
                    //});
                    #endregion
                    #region �����ʶ
                    //�����������Ϊ�����ͬ��������У�ʹ������ʵ�����ѡ�
                    //configure.RunAs("username", "password");
                    //ʹ��ָ�����û������������з�����Ҳ����ʹ�������н������á�����ؽ������� UPN ��׺�������û���ֵ�У���������/�û�����username@suffix.com��
                    //configure.RunAsPrompt();
                    //��װ����ʱ����װ��Ա����ʾ��������������û���/������ϡ�
                    //configure.RunAsNetworkService();
                    //ʹ��NETWORK_SERVICE�����ʻ����з���
                    configure.RunAsLocalSystem();
                    //ʹ�ñ���ϵͳ�ʻ����з���
                    //configure.RunAsLocalService();
                    #endregion
                    #region �Զ��尲װ����
                    //��װ����֮ǰ
                    configure.BeforeInstall(settings =>
                    {
                        Log.Information("��װ����֮ǰ");
                    });
                    //��װ������
                    configure.AfterInstall(settings =>
                    {
                        Log.Information("��װ������");
                    });
                    //ж�ز���֮ǰ
                    configure.BeforeUninstall(() =>
                    {
                        Log.Information("ж�ز���֮ǰ");
                    });
                    //ж�ز�����
                    configure.AfterUninstall(() =>
                    {
                        Log.Information("ж�ز�����");
                    });
                    #endregion
                    #region ����������
                    ////ָ������������
                    //configure.DependsOn("SomeOtherService");
                    ////֪�������������չ����
                    //configure.DependsOnMsmq(); // ΢����Ϣ�Ŷ�
                    //configure.DependsOnMsSql(); // Microsoft SQL Server
                    //configure.DependsOnEventLog(); // Windows�¼���־
                    //configure.DependsOnIis(); // ��������Ϣ������
                    #endregion
                    #region �߼�����
                    ////������ͣ�ͼ�����ָ������֧����ͣ�ͼ��������������ƹ�����ͨ����ͣ�������������
                    //configure.EnablePauseAndContinue();
                    ////���ùرգ�ָ���÷���֧�ֹرշ���������������ƹ��������ٹرշ���
                    //configure.EnableShutdown();
                    ////�����쳣��Ϊ���������ڼ��׳����쳣����ṩ�ص����˻ص����Ǵ�����򣬲���Ӱ�� Topshelf �Ѿ��ṩ��Ĭ���쳣������ּ��Ϊ�����ⲿ��������¼���ṩ���쳣����ṩ�ɼ��ԡ�
                    configure.OnException(ex =>
                    {
                        // Do something with the exception
                        Log.Fatal(ex, "��������쳣!");
                    });
                    #endregion
                });
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "����������ֹ!");
            }
            finally
            {
                Log.CloseAndFlush();
            }
            #endregion
        }
    }
}
