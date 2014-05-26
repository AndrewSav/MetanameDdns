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
using System.Linq;
using System.Reflection;
using System.Text;

namespace Metaname.Api.Logging
{
    public abstract class CanDumpPropertiesToString
    {
        private PropertyInfo[] _propertyInfos;

        public override string ToString()
        {
            return ToString(0);
        }

        public virtual string ToString(int ident)
        {
            string prefix = string.Empty.PadLeft(ident);
            if (_propertyInfos == null)
            {
                _propertyInfos = GetType().GetProperties();
            }

            StringBuilder sb = new StringBuilder();

            foreach (PropertyInfo info in _propertyInfos)
            {
                if (info.GetCustomAttributes(typeof(SupperssPropertyDumpAttribute)).Any())
                {
                    continue;
                }
                object value = info.GetValue(this, null) ?? "(null)";
                sb.Append(prefix + info.Name + ": ");
                
                if (value is CanDumpPropertiesToString)
                {
                    sb.AppendLine();
                    sb.AppendLine((value as CanDumpPropertiesToString).ToString(ident + 2));
                }
                else
                {
                    sb.AppendLine(value.ToString());
                }
            }

            if (ident > 0 && _propertyInfos.Length > 0)
            {
                sb.Remove(sb.Length - 2,2);
            }

            return sb.ToString();            
        }
    }
}
