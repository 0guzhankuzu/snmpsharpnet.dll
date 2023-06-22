using System;

namespace SnmpSharpNet
{
	[Serializable]
	public class Opaque : OctetString, ICloneable
	{
		public Opaque()
		{
			_asnType = SnmpConstants.SMI_OPAQUE;
		}

		public Opaque(byte[] data)
			: base(data)
		{
			_asnType = SnmpConstants.SMI_OPAQUE;
		}

		public Opaque(Opaque second)
			: base(second)
		{
			_asnType = SnmpConstants.SMI_OPAQUE;
		}

		public Opaque(OctetString second)
			: base(second)
		{
			_asnType = SnmpConstants.SMI_OPAQUE;
		}

		public Opaque(string value)
			: base(value)
		{
			_asnType = SnmpConstants.SMI_OPAQUE;
		}

		public override object Clone()
		{
			return new Opaque(this);
		}
	}
}
