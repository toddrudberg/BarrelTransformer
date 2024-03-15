using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Electroimpact.XmlSerialization
{
	public class SerializableTimeSpan
	{
		private TimeSpan _Value;
		[XmlIgnore]
		public TimeSpan Value
		{
			get
			{
				return _Value;
			}
			set
			{
				_Value = value;
			}

		}

		[Browsable(false)]
		[XmlElement(ElementName = "TimeSpan")]
		public string SerializableValue
		{
			get
			{
				return XmlConvert.ToString(Value);
			}
			set
			{
				Value = string.IsNullOrEmpty(value) ?
						TimeSpan.Zero : XmlConvert.ToTimeSpan(value);
			}
		}

		public static implicit operator TimeSpan(SerializableTimeSpan sts)
		{
			return sts.Value;
		}
		public static implicit operator SerializableTimeSpan(TimeSpan ts)
		{
			var rv = new SerializableTimeSpan();
			rv.Value = ts;
			return rv;
		}
	}

	

}
