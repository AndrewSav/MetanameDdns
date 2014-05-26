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
//
// This code is loosly based on https://github.com/dreamins/Route53DDNS/blob/master/Route53DDNSLib/accessor/ExternalIPAccessor.cs
using log4net;
using System.Net;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MetanameDdns.Configuration.Data;

namespace MetanameDdns.Ddns
{
    internal static class IpDetector
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static string DetectIp(string url, string pattern)
        {
            try
            {
                return Regex.Match((new WebClient()).DownloadString(url), pattern).ToString();
            }
            catch (WebException e)
            {
                Logger.Warn(string.Format("Failed calling url {0}", url), e);
                return null;
            }
        }

        public static string DetectIp(IEnumerable<IpDetectionParameters> parameters)
        {
            foreach (IpDetectionParameters p in parameters)
            {
                if (!p.Enabled)
                {
                    continue;
                }
                Logger.Info(string.Format("Retreiving external IP from {0}", p.Url));
                string ip = DetectIp(p.Url, p.Pattern);
                if (string.IsNullOrEmpty(ip))
                {
                    Logger.Warn(string.Format("Could not retreive external IP from {0}", p.Url));
                    continue;
                }
                IPAddress tmp;
                if (!IPAddress.TryParse(ip, out tmp))
                {
                    Logger.Warn(string.Format("Retreived IP is invalid: {0}", ip));
                    continue;
                }
                return ip;
            }
            return null;
        }
    }
}
