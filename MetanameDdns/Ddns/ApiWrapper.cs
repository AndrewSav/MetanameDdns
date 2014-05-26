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
using System.Collections.Generic;
using log4net;
using Metaname.Api;
using Metaname.Api.Exceptions;
using Metaname.Api.Models;

namespace MetanameDdns.Ddns
{
    internal class ApiWrapper
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ApiClient _client;

        public string Url
        {
            get { return _client.Url; }
        }
        public ApiWrapper(string clientId, string clientSecret, string url)
        {
            _client = new ApiClient(clientId, clientSecret)
            {
                Url = url
            };
        }
        public bool UpdateDnsRecordChecked(string zone, string reference, ResourceRecordDetails record)
        {
            try
            {
                _client.UpdateDnsRecord(zone, reference, record);
                return true;
            }
            catch (Exception e)
            {
                Logger.Warn("Could not update DNS record", e);
                return false;
            }
        }

        public IEnumerable<ResourceRecordDetails> GetDnsRecordsChecked(string zone)
        {
            try
            {
                return _client.DnsZone(zone);
            }
            catch (JsonClientException e)
            {
                if (Equals(e.Data["code"], -5))
                {
                    Logger.Warn("Server does not know this zone", e);
                    return new List<ResourceRecordDetails>();
                }
                throw;
            }
            catch (Exception e)
            {
                Logger.Warn("Could not retreive zone data", e);
                return null;
            }
        }
    }
}
