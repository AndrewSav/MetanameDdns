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
using System.Text;
using Metaname.Api.Logging;
using Newtonsoft.Json;

namespace Metaname.Api.Models
{
    public class DomainName : CanDumpPropertiesToString
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("when_registered")]
        public DateTime? WhenRegistered { get; set; }
        [JsonProperty("when_paid_up_to")]
        public DateTime? WhenPaidUpTo { get; set; }
        [JsonProperty("when_canceled")]
        public DateTime? WhenCancelled { get; set; }
        [JsonProperty("when_locked")]
        public DateTime? WhenLocked { get; set; }
        [JsonProperty("belongs_to_set")]
        public string BelongsToSet { get; set; }
        [JsonProperty("uses_dns_hosting")]
        public bool UsesDnsHosting { get; set; }
        [JsonProperty("redirected_to")]
        public string RedirectedTo { get; set; }
        [JsonProperty("contacts")]
        public DomainContacts Contacts { get; set; }
        [JsonProperty("name_servers")]
        [SupperssPropertyDump]
        public List<NameServerDetails> NameServers { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(base.ToString());
            sb.AppendLine("NameServers:");
            foreach (NameServerDetails nameServerDetails in NameServers)
            {
                sb.Append(nameServerDetails);
            }
            return sb.ToString();
        }
    }
}
