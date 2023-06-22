using System;
using System.Globalization;

namespace SnmpSharpNet
{
	[Serializable]
	public class EthernetAddress : OctetString, ICloneable
	{
		public EthernetAddress()
			: base(new byte[6])
		{
		}

		public EthernetAddress(byte[] data)
			: base(data)
		{
			if (data.Length < 6)
			{
				throw new ArgumentException("Buffer underflow error converting IP address");
			}
			if (data.Length > 6)
			{
				throw new ArgumentException("Buffer overflow error converting IP address");
			}
			base.Set(data);
		}

		public EthernetAddress(EthernetAddress second)
		{
			base.Set(second.ToArray());
		}

		public EthernetAddress(OctetString second)
			: this()
		{
			if (second.Length < 6)
			{
				throw new ArgumentException("Buffer underflow error converting IP address");
			}
			if (Length > 6)
			{
				throw new ArgumentException("Buffer overflow error converting IP address");
			}
			base.Set(second);
		}

		public override object Clone()
		{
			return new EthernetAddress(this);
		}

		public override void Set(string value)
		{
			if (value == null || value.Length <= 0)
			{
				throw new ArgumentException("Invalid argument. String is empty.");
			}
			string text = (string)value.Clone();
			for (int i = 0; i < value.Length; i++)
			{
				if (!char.IsNumber(text[i]) && char.ToUpper(text[i]) != 'A' && char.ToUpper(text[i]) != 'B' && char.ToUpper(text[i]) != 'C' && char.ToUpper(text[i]) != 'D' && char.ToUpper(text[i]) != 'E' && char.ToUpper(text[i]) != 'F')
				{
					text.Remove(i, 1);
					i--;
				}
			}
			if (text.Length != 12)
			{
				throw new ArgumentException("Invalid Ethernet address format.");
			}
			int j = 0;
			int num = 0;
			for (; j + 2 < text.Length; j += 2)
			{
				string s = text.Substring(j, 2);
				byte b = byte.Parse(s, NumberStyles.HexNumber);
				_data[num++] = b;
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "{0:x2}{1:x2}.{2:x2}{3:x2}.{4:x2}{5:x2}", _data[0], _data[1], _data[2], _data[3], _data[4], _data[5]);
		}
	}
}
