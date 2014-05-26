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
using log4net;
using Metaname.Api.Models;
using MetanameDdns.Configuration;
using MetanameDdns.Configuration.Data;

namespace MetanameDdns.Ddns
{
    internal class Updater
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Config _config;

        public void Update(Config config)
        {
            lock (this)
            {
                _config = config;
                DdnsPayloadLocked();
            }
        }

        private void DdnsPayloadLocked()
        {
            string ip = IpDetector.DetectIp(_config.ExternalIpDetection);
            if (string.IsNullOrEmpty(ip))
            {
                Logger.Info("Could not find external ip address.");
                return;
            }
            Logger.Info(string.Format("Got current IP: {0}", ip));
            State state = Settings.ReadState();
            string lastIp = state == null ? null : state.Ip;
            DateTime? lastUpdate = state == null ? null : (DateTime?)state.LastUpdate;
            bool ipChanged = lastIp != ip;
            Logger.Info(string.Format("Last IP: {0}", lastIp));
            Logger.Info(ipChanged ? "IP changed. Need to updpate" : "IP did not change");
            if (!lastUpdate.HasValue)
            {
                Logger.Info("Last DNS update time is not known. Need to update.");
            }
            if (lastUpdate.HasValue && !ipChanged)
            {
                Logger.Info(string.Format("Last DNS update time is {0}", lastUpdate.Value));
                Logger.Info(string.Format("Set to update every {0} minutes", _config.Service.CheckDnsEvenIfLocalIpNotChangedEveryMinutes));
                int difference = (int)(DateTime.Now - lastUpdate.Value).TotalMinutes;
                Logger.Info(string.Format("{0} minutes elapsed", difference));
                if (difference < _config.Service.CheckDnsEvenIfLocalIpNotChangedEveryMinutes)
                {
                    Logger.Info(string.Format("Update is due in {0} minutes", _config.Service.CheckDnsEvenIfLocalIpNotChangedEveryMinutes - difference));
                    return;
                }
                Logger.Info("Update is past due time. Need to update.");
            }

            if (!UpdateDdns(ip))
            {
                return;
            }
            DateTime now = DateTime.Now;
            Logger.Info(string.Format("Saving state ip:{0} update:{1}", ip, now));
            Settings.SaveState(new State { Ip = ip, LastUpdate = now });
        }

        private bool UpdateDdns(string ip)
        {
            Logger.Info("Stating update...");
            ApiWrapper wrapper = new ApiWrapper(_config.Connection.ClientId, _config.Connection.ClientSecret, _config.Connection.Url);

            ResourceRecordCollector recordCollector = new ResourceRecordCollector(_config, wrapper);
            recordCollector.CollectResourceRecord();

            if (!recordCollector.HasResourceRecords)
            {
                return false;
            }

            bool success = true;
            foreach (DdnsTarget zone in _config.DdnsTargets)
            {
                foreach (string record in zone.Records)
                {
                    ResourceRecordDetails rrd = recordCollector.GetResourceRecord(zone.Zone, record);
                    if (rrd == null)
                    {
                        Logger.Warn(
                            string.Format(
                                "No record found for {0} in {1}. Please make sure you have created an A record in the DNS",
                                record, zone.Zone));                        
                    }
                    else
                    {
                        if (rrd.Data == ip)
                        {
                            Logger.Info(string.Format("Dns data is already up to date for {0},{1}. Update is not required.",record, zone.Zone));
                        }
                        else
                        {
                            Logger.Info(string.Format("Dns data changed for {0},{1}. Updating...",record, zone.Zone));
                            rrd.Data = ip;
                            string reference = rrd.Reference;
                            rrd.Reference = null;
                            bool res = wrapper.UpdateDnsRecordChecked(zone.Zone, reference, rrd);
                            if (!res)
                            {
                                success = false;
                            }
                        }                        
                    }
                }
            }
            return success;
        }
    }
}