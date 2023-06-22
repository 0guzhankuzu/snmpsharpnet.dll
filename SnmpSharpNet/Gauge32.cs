using System;

namespace SnmpSharpNet
{
	[Serializable]
	public class Gauge32 : UInteger32, ICloneable
	{
		public Gauge32()
		{
			_asnType = SnmpConstants.SMI_GAUGE32;
		}

		public Gauge32(Gauge32 second)
			: base(second)
		{
			_asnType = SnmpConstants.SMI_GAUGE32;
		}

		public Gauge32(UInteger32 uint32)
			: base(uint32)
		{
			_asnType = SnmpConstants.SMI_GAUGE32;
		}

		public Gauge32(string val)
			: base(val)
		{
			_asnType = SnmpConstants.SMI_GAUGE32;
		}

		public Gauge32(uint val)
			: base(val)
		{
			_asnType = SnmpConstants.SMI_GAUGE32;
		}

		public override object Clone()
		{
			return new Gauge32(this);
		}
	}
}
