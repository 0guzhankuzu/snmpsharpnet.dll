using System;

namespace SnmpSharpNet
{
	[Serializable]
	public class EndOfMibView : V2Error, ICloneable
	{
		public EndOfMibView()
		{
			_asnType = SnmpConstants.SMI_ENDOFMIBVIEW;
		}

		public EndOfMibView(EndOfMibView second)
			: base(second)
		{
			_asnType = SnmpConstants.SMI_ENDOFMIBVIEW;
		}

		public override object Clone()
		{
			return new EndOfMibView(this);
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

		public override void encode(MutableByte buffer)
		{
			AsnType.BuildHeader(buffer, base.Type, 0);
		}

		public override string ToString()
		{
			return "SNMP End-of-MIB-View";
		}
	}
}
