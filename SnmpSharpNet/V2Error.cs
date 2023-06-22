using System;

namespace SnmpSharpNet
{
	public class V2Error : AsnType, ICloneable
	{
		public V2Error()
		{
		}

		public V2Error(V2Error second)
		{
		}

		public override void encode(MutableByte buffer)
		{
			AsnType.BuildHeader(buffer, base.Type, 0);
		}

		public override int decode(byte[] buffer, int offset)
		{
			int length;
			byte b = AsnType.ParseHeader(buffer, ref offset, out length);
			if (b != base.Type)
			{
				throw new SnmpException("Invalid ASN.1 type");
			}
			if (length != 0)
			{
				throw new SnmpException("Invalid ASN.1 length");
			}
			return offset;
		}

		public override object Clone()
		{
			return new V2Error(this);
		}
	}
}
