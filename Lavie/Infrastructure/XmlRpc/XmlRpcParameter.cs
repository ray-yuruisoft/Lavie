using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Lavie.Infrastructure.XmlRpc
{
    public class XmlRpcParameter
    {
        private readonly XElement _parameterElement;
        private readonly XmlRpcValue _value;
        private readonly XElement _valueElement;

        public XmlRpcParameter(XElement parameterElement)
        {
            this._parameterElement = parameterElement;
            _valueElement = this._parameterElement.Element("value");
            _value = new XmlRpcValue(_valueElement);
        }

        public string AsString()
        {
            return _value.AsString();
        }

        public int? AsInt()
        {
            return _value.AsInt();
        }

        public bool? AsBool()
        {
            return _value.AsBool();
        }

        public double? AsDouble()
        {
            return _value.AsDouble();
        }

        public DateTime? AsDateTime()
        {
            return _value.AsDateTime();
        }

        public byte[] AsBytes()
        {
            return _value.AsBytes();
        }

        public object[] AsArray()
        {
            return _value.AsArray();
        }

        public IDictionary<string, object> AsDictionary()
        {
            return _value.AsDictionary();
        }

        public Type ComputeType()
        {
            return _value.ComputeType();
        }

        public object AsType(Type type)
        {
            return _value.AsType(type);
        }
    }
}
