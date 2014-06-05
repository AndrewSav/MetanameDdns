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
using Metaname.Api.Exceptions;
using Metaname.Api.Models;
using Newtonsoft.Json.Linq;


namespace Metaname.Api
{
    // Please read https://metaname.net/api/1.1/doc for the API documentation
    public class ApiClient
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string _clientId;
        private readonly string _clientSecret;
        private string _apiUrl = @"https://metaname.net/api/1.1";
        JsonRpcClient _client;

        public ApiClient(string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            Logger.Debug(string.Format("ApiClient created, clientId: {0}, clientSecret:{1}",clientId,clientSecret));
        }

        public string Url
        {
            get { return _apiUrl ; }
            set
            {
                _apiUrl = value;
                _client = null;
                Logger.Debug(string.Format("Url set: {0}", _apiUrl));
            }
        }

        private JsonRpcClient Client
        {
            get { return _client ?? (_client = new JsonRpcClient {Url = _apiUrl}); }
        }

        public string Price(string domainName, int termInMonth, bool renewal)
        {
            Logger.Debug(string.Format("Entering Price method. domainName: {0}, ternInMonth: {1}, renewal: {2}", domainName, termInMonth, renewal));            
            JToken result = Client.Invoke("price", _clientId, _clientSecret, domainName, termInMonth, renewal);
            Logger.Debug(string.Format("Price method finished. result: {0}", result));
            return result.ToObject<string>();
        }

        public string CheckAvailability(string domainName, string sourceIp)
        {
            Logger.Debug(string.Format("Entering CheckAvailability method. domainName: {0}, sourceIp: {1}", domainName, sourceIp));            
            JToken result = Client.Invoke("check_availability", _clientId, _clientSecret, domainName, sourceIp);
            Logger.Debug(string.Format("CheckAvailability method finished. result: {0}", result));
            return result.ToObject<string>();
        }

        public string RegisterDomainName(string domainName, int term, DomainContacts contacts, IEnumerable<NameServerDetails> nameServers)
        {
            Logger.Debug(string.Format("Entering RegisterDomainName method. domainName: {0}, term: {1}, contacts: {2}", domainName, term, contacts));
            if (nameServers != null)
            {
                foreach (NameServerDetails nameServerDetails in nameServers)
                {
                    Logger.Debug(string.Format("Name server: {0}", nameServerDetails));
                }
            }            
            JToken result = Client.Invoke("register_domain_name", _clientId, _clientSecret, domainName, term, contacts, nameServers);
            Logger.Debug(string.Format("RegisterDomainName method finished. result: {0}", result));
            return result.ToObject<string>();
        }

        public string ImportNzDomainName(string domainName, string udai)
        {
            Logger.Debug(string.Format("Entering ImportNzDomainName method. domainName: {0}, udai: {1}", domainName, udai));            
            JToken result = Client.Invoke("import_nz_domain_name", _clientId, _clientSecret, domainName, udai);
            Logger.Debug(string.Format("ImportNzDomainName method finished. result: {0}", result));
            return result.ToObject<string>();
            
        }
        
        public void ImportOtherDomainName(string domainName, DomainContacts contacts)
        {
            Logger.Debug(string.Format("Entering ImportOtherDomainName method. domainName: {0}, contacts: {1}", domainName, contacts));            
            JToken result = Client.Invoke("import_other_domain_name", _clientId, _clientSecret, domainName, contacts);
            Logger.Debug(string.Format("ImportOtherDomainName method finished. result: {0}", result));
        }

        public List<DomainName> DomainNames()
        {
            Logger.Debug(string.Format("Entering DomainNames method."));            
            JToken result = Client.Invoke("domain_names", _clientId, _clientSecret);
            JArray array = result as JArray;
            if (array == null)
            {
                Logger.Error(string.Format("Expected array; received result: {0}", result));
                throw new MetanameApiException("Invalid result");
            }
            List<DomainName> recordList = array.ToObject<List<DomainName>>();
            Logger.Debug(string.Format("DomainNames method finished. result: {0}", result));
            return recordList;                                    
        }

        public void UpdateContacts(string domainName, DomainContacts contacts)
        {
            Logger.Debug(string.Format("Entering UpdateContacts method. domainName: {0}, contacts: {1}", domainName, contacts));            
            JToken result = Client.Invoke("update_contacts", _clientId, _clientSecret, domainName, contacts);
            Logger.Debug(string.Format("UpdateContacts method finished. result: {0}", result));
            
        }

        public void UpdateNameServers(string domainName, IEnumerable<NameServerDetails> nameServers)
        {
            Logger.Debug(string.Format("Entering UpdateNameServers method. domainName: {0}", domainName));
            if (nameServers != null)
            {
                foreach (NameServerDetails nameServerDetails in nameServers)
                {
                    Logger.Debug(string.Format("Name server: {0}", nameServerDetails));
                }
            }
            
            JToken result = Client.Invoke("update_name_servers", _clientId, _clientSecret, domainName, nameServers);
            Logger.Debug(string.Format("UpdateNameServers method finished. result: {0}", result));
            
        }
        
        public List<ResourceRecordDetails> DnsZone(string domainName)
        {
            Logger.Debug(string.Format("Entering DnsZone method. domainName: {0}", domainName));            
            JToken result = Client.Invoke("dns_zone", _clientId, _clientSecret, domainName);
            JArray array = result as JArray;
            if (array == null)
            {
                Logger.Error(string.Format("Expected array; received result: {0}", result));
                throw new MetanameApiException("Invalid result");
            }
            List<ResourceRecordDetails> recordList = array.ToObject<List<ResourceRecordDetails>>();
            Logger.Debug(string.Format("DnsZone method finished. result: {0}", result));
            return recordList;
        }

        public string CreateDnsRecord(string domainName, ResourceRecordDetails record)
        {
            Logger.Debug(string.Format("Entering CreateDnsRecord method. domainName: {0}, record: {1}", domainName, record));            
            JToken result = Client.Invoke("create_dns_record", _clientId, _clientSecret, domainName, record);
            Logger.Debug(string.Format("CreateDnsRecord method finished. result: {0}", result));
            return result.ToObject<string>();
            
        }
        
        public void UpdateDnsRecord(string domainName, string reference, ResourceRecordDetails record)
        {
            Logger.Debug(string.Format("Entering UpdateDnsRecord method. domainName: {0}, reference: {1}, record: {2}", domainName, reference, record));            
            JToken result = Client.Invoke("update_dns_record", _clientId, _clientSecret, domainName, reference, record);
            Logger.Debug(string.Format("UpdateDnsRecord method finished. result: {0}", result));

        }
        
        public void DeleteDnsRecord(string domainName, string reference)
        {
            Logger.Debug(string.Format("Entering DeleteDnsRecord method. domainName: {0}, reference: {1}", domainName, reference));            
            JToken result = Client.Invoke("delete_dns_record", _clientId, _clientSecret, domainName, reference);
            Logger.Debug(string.Format("DeleteDnsRecord method finished. result: {0}", result));
        }

        public void UpdateDsRecords(string domainName, IEnumerable<DsRecord> dsRecords)
        {
            Logger.Debug(string.Format("Entering UpdateDsRecords method. domainName: {0}", domainName));
            if (dsRecords != null)
            {
                foreach (DsRecord dsRecord in dsRecords)
                {
                    Logger.Debug(string.Format("DS record: {0}", dsRecord));
                }
            }            
            JToken result = Client.Invoke("update_ds_records", _clientId, _clientSecret, domainName, dsRecords);
            Logger.Debug(string.Format("UpdateDsRecords method finished. result: {0}", result));
        }

        public void SetHttpRedirection(string domainName, string redirectionUri)
        {
            Logger.Debug(string.Format("Entering SetHttpRedirection method. domainName: {0}, redirectionUri: {1}", domainName, redirectionUri));            
            JToken result = Client.Invoke("set_http_redirection", _clientId, _clientSecret, domainName, redirectionUri);
            Logger.Debug(string.Format("SetHttpRedirection method finished. result: {0}", result));
        }

        public void RenewDomainName(string domainName, int term)
        {
            Logger.Debug(string.Format("Entering RenewDomainName method. domainName: {0}, term: {1}", domainName, term));            
            JToken result = Client.Invoke("renew_domain_name", _clientId, _clientSecret, domainName, term);
            Logger.Debug(string.Format("RenewDomainName method finished. result: {0}", result));
        }

        public bool DomainNameIsLocked(string domainName)
        {
            Logger.Debug(string.Format("Entering DomainNameIsLocked method. domainName: {0}", domainName));
            JToken result = Client.Invoke("domain_name_is_locked", _clientId, _clientSecret, domainName);
            Logger.Debug(string.Format("DomainNameIsLocked method finished. result: {0}", result));
            return result.ToObject<bool>();
        }

        public void LockDomainName(string domainName)
        {
            Logger.Debug(string.Format("Entering LockDomainName method. domainName: {0}", domainName));
            JToken result = Client.Invoke("lock_domain_name", _clientId, _clientSecret, domainName);
            Logger.Debug(string.Format("LockDomainName method finished. result: {0}", result));
        }
        
        public void UnlockDomainName(string domainName)
        {
            Logger.Debug(string.Format("Entering UnlockDomainName method. domainName: {0}", domainName));
            JToken result = Client.Invoke("unlock_domain_name", _clientId, _clientSecret, domainName);
            Logger.Debug(string.Format("UnlockDomainName method finished. result: {0}", result));
        }

        public string ResetDomainNameSecret(string domainName)
        {
            Logger.Debug(string.Format("Entering ResetDomainNameSecret method. domainName: {0}", domainName));
            JToken result = Client.Invoke("reset_domain_name_secret", _clientId, _clientSecret, domainName);
            Logger.Debug(string.Format("ResetDomainNameSecret method finished. result: {0}", result));
            return result.ToObject<string>();
        }
        public string AccountBalance()
        {
            Logger.Debug(string.Format("Entering AccountBalance method."));
            JToken result = Client.Invoke("account_balance", _clientId, _clientSecret);
            Logger.Debug(string.Format("AccountBalance method finished. result: {0}", result));
            return result.ToObject<string>();
        }

        public void DropCatchingNames()
        {
            Logger.Debug(string.Format("Entering DropCatchingNames method."));
            JToken result = Client.Invoke("drop_catching_names", _clientId, _clientSecret);
            Logger.Debug(string.Format("DropCatchingNames method finished. result: {0}", result));            
        }
        
        public void PlaceDropCatchingBid(string domainName, decimal bid, bool autoBid)
        {
            Logger.Debug(string.Format("Entering PlaceDropCatchingBid method. domainName: {0}, bid: {1}, autoBid: {2}",domainName, bid, autoBid));
            JToken result = Client.Invoke("place_drop_catching_bid", _clientId, _clientSecret, domainName, bid, autoBid);
            Logger.Debug(string.Format("PlaceDropCatchingBid method finished. result: {0}", result));
        }

        public void ConfigureZone(string zoneName, List<ResourceRecordDetails> resourceRecords, Dictionary<string, string> options)
        {
            Logger.Debug(string.Format("Entering ConfigureZone method. zoneName: {0}", zoneName));
            if (resourceRecords != null)
            {
                foreach (ResourceRecordDetails resourceRecord in resourceRecords)
                {
                    Logger.Debug(string.Format("Resource record: {0}", resourceRecord));
                }
            }
            if (options != null)
            {
                foreach (KeyValuePair<string, string> option in options)
                {
                    Logger.Debug(string.Format("Option {0}: {1}", option.Key, option.Value));
                }
            }            
            JToken result = Client.Invoke("configure_zone", _clientId, _clientSecret, zoneName, resourceRecords, options);
            Logger.Debug(string.Format("ConfigureZone method finished. result: {0}", result));
            
        }
    }
}
