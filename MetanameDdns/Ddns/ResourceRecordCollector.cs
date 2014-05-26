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
using System.Collections.Generic;
using System.Linq;
using Metaname.Api.Models;
using MetanameDdns.Configuration.Data;

namespace MetanameDdns.Ddns
{
    internal class ResourceRecordCollector
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Config _config;
        private readonly ApiWrapper _wrapper;
        private readonly Dictionary<string, ResourceRecordDetails> _records = new Dictionary<string, ResourceRecordDetails>();

        public ResourceRecordCollector(Config config, ApiWrapper wrapper)
        {
            _config = config;
            _wrapper = wrapper;
        }

        public bool HasResourceRecords
        {
            get { return _records.Any(); }
        }

        public void CollectResourceRecord()
        {
            _records.Clear();
            foreach (DdnsTarget zone in _config.DdnsTargets)
            {
                Logger.Info(string.Format("Accessing url {0}...", _wrapper.Url));
                IEnumerable<ResourceRecordDetails> records = _wrapper.GetDnsRecordsChecked(zone.Zone);
                if (records == null)
                {
                    return;
                }
                foreach (ResourceRecordDetails recordInZone in records)
                {
                    Logger.Info(string.Format(
                        "Found ref: {0}, name: {1}, type: {2}, data: {3}, ttl: {4}, aux: {5}",
                        recordInZone.Reference, recordInZone.Name, recordInZone.Type, recordInZone.Data,
                        recordInZone.Ttl, recordInZone.Aux));
                    if (recordInZone.Type.ToUpperInvariant() == "A")
                    {
                        _records.Add(recordInZone.Name + zone.Zone, recordInZone);
                    }
                }
            }
        }

        public ResourceRecordDetails GetResourceRecord(string zone, string record)
        {
            return _records.ContainsKey(record + zone) ? _records[record + zone] : null;
        }
    }
}
