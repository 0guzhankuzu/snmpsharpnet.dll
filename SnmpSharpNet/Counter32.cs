using System;

namespace SnmpSharpNet
{
	[Serializable]
	public class Counter32 : UInteger32, ICloneable
	{
		public Counter32()
		{
			_asnType = SnmpConstants.SMI_COUNTER32;
		}

		public Counter32(Counter32 second)
			: base(second)
		{
			_asnType = SnmpConstants.SMI_COUNTER32;
		}

		public Counter32(UInteger32 uint32)
			: base(uint32)
		{
			_asnType = SnmpConstants.SMI_COUNTER32;
		}

		public Counter32(string val)
			: base(val)
		{
			_asnType = SnmpConstants.SMI_COUNTER32;
		}

		public Counter32(uint val)
			: base(val)
		{
			_asnType = SnmpConstants.SMI_COUNTER32;
		}

		public override object Clone()
		{
			return new Counter32(this);
		}

		public static uint Diff(Counter32 first, Counter32 second)
		{
			uint value = first.Value;
			uint value2 = second.Value;
			uint num = 0u;
			if (value2 > value)
			{
				return (uint)(-1 - (int)value) + value2;
			}
			return value2 - value;
		}
	}
}
