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
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace Metaname.Api.Exceptions
{
    public sealed class JsonClientException : Exception
    {
        public JsonClientException()
        {
        }

        public JsonClientException(string message) : base (message)
        {
            
        }

        public JsonClientException(string message, JToken data, int? code, string jsonrpcResponse) : base (message)
        {
            if (data != null)
            {
                Data.Add("data", data.ToString());
            }
            if (jsonrpcResponse != null)
            {
                Data.Add("jsonrpcResponse", jsonrpcResponse);
            }
            if (code.HasValue)
            {
                Data.Add("code", code.Value);
            }
        }
        public JsonClientException(string message, Exception inner) : base(message, inner)
        {
        }

        public JsonClientException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
