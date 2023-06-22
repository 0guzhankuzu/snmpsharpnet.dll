using System;
using System.Text;

namespace SnmpSharpNet
{
	[Serializable]
	public class TimeTicks : UInteger32, ICloneable
	{
		public long Milliseconds
		{
			get
			{
				long num = Convert.ToInt64(_value);
				return num * 10;
			}
		}

		public TimeTicks()
		{
			_asnType = SnmpConstants.SMI_TIMETICKS;
		}

		public TimeTicks(TimeTicks second)
			: base(second)
		{
			_asnType = SnmpConstants.SMI_TIMETICKS;
		}

		public TimeTicks(UInteger32 uint32)
			: base(uint32)
		{
			_asnType = SnmpConstants.SMI_TIMETICKS;
		}

		public TimeTicks(uint value)
			: base(value)
		{
			_asnType = SnmpConstants.SMI_TIMETICKS;
		}

		public TimeTicks(string val)
			: base(val)
		{
			_asnType = SnmpConstants.SMI_TIMETICKS;
		}

		public override object Clone()
		{
			return new TimeTicks(this);
		}

		public static explicit operator TimeSpan(TimeTicks value)
		{
			long num = value.Value;
			num *= 10;
			num *= 10000;
			return new TimeSpan(num);
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
			stringBuilder.Append(num * 10).Append("ms");
			return stringBuilder.ToString();
		}

		public override int GetHashCode()
		{
			return _value.GetHashCode();
		}
	}
}
