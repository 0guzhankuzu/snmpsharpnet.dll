using System;
using System.Text;

namespace SnmpSharpNet
{
	public class V2PartyClock : UInteger32, ICloneable
	{
		public V2PartyClock()
		{
			_asnType = SnmpConstants.SMI_PARTY_CLOCK;
		}

		public V2PartyClock(V2PartyClock second)
			: base(second)
		{
			_asnType = SnmpConstants.SMI_PARTY_CLOCK;
		}

		public V2PartyClock(UInteger32 uint32)
			: base(uint32)
		{
			_asnType = SnmpConstants.SMI_PARTY_CLOCK;
		}

		public override object Clone()
		{
			return new V2PartyClock(this);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			long num = base.Value;
			long num2 = 0L;
			if ((num2 = num / 8640000) > 0)
			{
				stringBuilder.Append(num2).Append("d ");
				num %= 8640000;
			}
			else
			{
				stringBuilder.Append("0d ");
			}
			if ((num2 = num / 360000) > 0)
			{
				stringBuilder.Append(num2).Append("h ");
				num %= 360000;
			}
			else
			{
				stringBuilder.Append("0h ");
			}
			if ((num2 = num / 6000) > 0)
			{
				stringBuilder.Append(num2).Append("m ");
				num %= 6000;
			}
			else
			{
				stringBuilder.Append("0m ");
			}
			if ((num2 = num / 100) > 0)
			{
				stringBuilder.Append(num2).Append("s ");
				num %= 100;
			}
			else
			{
				stringBuilder.Append("0s ");
			}
			stringBuilder.Append(num2 * 10).Append("ms");
			return stringBuilder.ToString();
		}
	}
}
