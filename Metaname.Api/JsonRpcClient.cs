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
// This code is loosely based on https://github.com/luckyrat/KeeFox/blob/master/Jayrock/src/Jayrock.Sandbox/JsonRpcClient.cs
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Services.Protocols;
using log4net;
using Metaname.Api.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Metaname.Api
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class JsonRpcClient : HttpWebClientProtocol
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private int _id;
        public JToken Invoke(string method, params object[] args)
        {
            Logger.Debug(string.Format("Entering Invoke method. method: {0}", method));
            if (args != null)
            {
                foreach (object arg in args)
                {
                    Logger.Debug(string.Format("argument: {0}", arg));
                }
            }

            WebRequest request = GetWebRequest(new Uri(Url));
            request.Method = "POST";
            Encoding requestEncoding = RequestEncoding ?? new UTF8Encoding(false);
            request.ContentType = "application/json; charset=" + requestEncoding.HeaderName;

            using (Stream stream = request.GetRequestStream())
            using (StreamWriter writer = new StreamWriter(stream, requestEncoding))
            {
                JObject call = new JObject();
                call["jsonrpc"] = "2.0";
                call["id"] = ++_id;
                call["method"] = method;
                call["params"] = JToken.FromObject(args ?? new object[0]);
                string jsonrpcRequest = JsonConvert.SerializeObject(call);
                Logger.Debug(string.Format("Sending jsonrpc request: {0}", jsonrpcRequest));
                writer.Write(jsonrpcRequest);
            }

            string textResponse = ReadResponse(request);
            Logger.Debug(string.Format("Received jsonrpc response: {0}", textResponse));
            JObject responseJson = JObject.Parse(textResponse);
            if (responseJson["error"] != null)
            {
                throw new JsonClientException(
                    responseJson["error"]["message"].Value<string>(), 
                    responseJson["error"]["data"], 
                    responseJson["error"]["code"].Value<int>(), 
                    responseJson.ToString());
            }

            if (responseJson["result"] == null)
            {
                throw new JsonClientException("Invalid jsonrpc response", null, null, textResponse);
            }
            return responseJson["result"];
        }

        private string ReadResponse(WebRequest request)
        {
            using (WebResponse response = GetWebResponse(request))
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
