//Copyright (c) 2014 Andrew Savinykh
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using log4net;
using MetanameDdns.ServiceTools;
using NDesk.Options;

namespace MetanameDdns.Infrastructure
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += UnhandledExceptionEventHandler;
            Directory.SetCurrentDirectory(currentDomain.BaseDirectory);

            Version v = Assembly.GetExecutingAssembly().GetName().Version;
            Console.WriteLine("MetanameDdns " + v);

            string serviceName = new Service().ServiceName;

            if (Environment.UserInteractive)
            {

                if (!UacChecker.IsProcessElevated)
                {
                    Console.WriteLine("Please be an administrator and run \"as administrator\"");
                    Console.WriteLine("Exiting...");
                    return;
                }
                if (args.Length > 0)
                {
                    bool used = false;
                    OptionSet o = new OptionSet
                    {
                        {"h|help|?", i => { used = true; ShowHelp();}},
                        {"с|console", i => { used = true; RunInConsole();}},
                        {"i|install", i => { used = true; Install(serviceName);}},
                        {"u|uninstall", i => { used = true; Unisntall(serviceName);}}
                    };
                    o.Parse(new[] { args[0] });
                    if (!used)
                    {
                        ShowHelp();
                    }
                }
                else
                {
                    if (ServiceTools.ServiceInstaller.ServiceIsInstalled(serviceName))
                    {
                        Console.WriteLine("Starting MetanameDdns service...");
                        ServiceTools.ServiceInstaller.StartService(serviceName);
                    }
                    else
                    {
                        Install(serviceName);
                    }
                }
            }
            else
            {
                ServiceBase.Run(new ServiceBase[] { new Service() });
            }
        }

        private static void Unisntall(string name)
        {
            if (ServiceTools.ServiceInstaller.ServiceIsInstalled(name))
            {
                Console.WriteLine("Uninstalling MetanameDdns service...");
                ServiceTools.ServiceInstaller.Uninstall(name);
            }
            else
            {
                Console.WriteLine("MetanameDdns service does not appear to be installed");
            }
        }

        private static void Install(string name)
        {
            if (!ServiceTools.ServiceInstaller.ServiceIsInstalled(name))
            {
                Console.WriteLine("Installing and starting MetanameDdns service...");
                ServiceTools.ServiceInstaller.InstallAndStart(name, name, Assembly.GetExecutingAssembly().Location);
            }
            else
            {
                Console.WriteLine("MetanameDdns service appears to be already installed");
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("/install - to install service");
            Console.WriteLine("/uninstall - to uninstall service");
            Console.WriteLine("/console - to run in a console");
        }

        private static void RunInConsole()
        {
            Console.WriteLine("Press enter key to stop program");

            using (var service = new Service())
            {
                service.Start();
                Console.ReadLine();
                service.Stop();
            }
        }

        private static void UnhandledExceptionEventHandler(object sender, UnhandledExceptionEventArgs e)
        {
            ILog logger = LogManager.GetLogger("Unhandled");
            Exception ex = e.ExceptionObject as Exception;
            logger.Fatal("unhandled exception", ex);
            Environment.FailFast("unhandled exception", ex);
        }
    }
}
