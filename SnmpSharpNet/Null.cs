using System;

namespace SnmpSharpNet
{
	[Serializable]
	public class Null : AsnType, ICloneable
	{
		public Null()
		{
			_asnType = SnmpConstants.SMI_NULL;
		}

		public Null(Null second)
			: this()
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
				throw new SnmpException("Invalid ASN.1 Type");
			}
			if (length != 0)
			{
				throw new SnmpException("Malformed ASN.1 Type");
			}
			return offset;
		}

		public override object Clone()
		{
			return new Null(this);
		}

		public override string ToString()
		{
			return "Null";
		}
	}
}
