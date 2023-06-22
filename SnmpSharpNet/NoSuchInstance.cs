using System;

namespace SnmpSharpNet
{
	[Serializable]
	public class NoSuchInstance : V2Error, ICloneable
	{
		public NoSuchInstance()
		{
			_asnType = SnmpConstants.SMI_NOSUCHINSTANCE;
		}

		public NoSuchInstance(NoSuchInstance second)
			: base(second)
		{
			_asnType = SnmpConstants.SMI_NOSUCHINSTANCE;
		}

		public override object Clone()
		{
			return new NoSuchInstance(this);
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
				throw new SnmpDecodingException("Invalid ASN.1 length");
			}
			return offset;
		}

		public override void encode(MutableByte buffer)
		{
			AsnType.BuildHeader(buffer, base.Type, 0);
		}

		public override string ToString()
		{
			return "SNMP No-Such-Instance";
		}
	}
}
