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
using log4net;
using System.Threading;
using System.ServiceProcess;
using MetanameDdns.Configuration;
using MetanameDdns.Configuration.Data;
using MetanameDdns.Ddns;

namespace MetanameDdns.Infrastructure
{
    [System.ComponentModel.DesignerCategory("Code")]
    internal class Service : ServiceBase
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Updater _updater;

        internal Service()
        {
            ServiceName = "Metaname DDNS Updater Service";
            _updater = new Updater();
        }

        protected override void OnStart(string[] args)
        {
            Logger.Info("Starting...");
            new Thread(Update).Start();
        }

        internal void Start()
        {
            OnStart(null);
        }

        protected override void OnStop()
        {
            Logger.Info("Stopping...");
            base.OnStop();
        }

        // ReSharper disable once FunctionNeverReturns
        private void Update()
        {
            for (;;)
            {
                Logger.Info("Waking up...");
                Config config = Settings.ReadConfig();
                _updater.Update(config);
                Logger.Info(string.Format("Going to sleep for {0} minute(s)...", config.Service.CheckLocalIpEveryMinutes));
                Thread.Sleep(config.Service.CheckLocalIpEveryMinutes*60*1000);
            }
        }
    }
}
