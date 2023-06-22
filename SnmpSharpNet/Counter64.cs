using System;
using System.Globalization;

namespace SnmpSharpNet
{
	[Serializable]
	public class Counter64 : AsnType, IComparable<ulong>, IComparable<Counter64>, ICloneable
	{
		protected ulong _value;

		public virtual ulong Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		public Counter64()
		{
			_asnType = SnmpConstants.SMI_COUNTER64;
		}

		public Counter64(long value)
			: this()
		{
			_value = Convert.ToUInt64(value);
		}

		public Counter64(Counter64 value)
			: this()
		{
			_value = value.Value;
		}

		public Counter64(ulong value)
			: this()
		{
			_value = value;
		}

		public Counter64(string value)
			: this()
		{
			Set(value);
		}

		public void Set(AsnType value)
		{
			Counter64 counter = value as Counter64;
			if (counter != null)
			{
				_value = counter.Value;
				return;
			}
			throw new ArgumentException("Invalid argument type.");
		}

		public void Set(string value)
		{
			if (value.Length <= 0)
			{
				throw new ArgumentOutOfRangeException(value, "String has to be length greater then 0");
			}
			try
			{
				_value = Convert.ToUInt64(value, CultureInfo.CurrentCulture);
			}
			catch
			{
				throw new ArgumentException("Invalid argument format.");
			}
		}

		public override object Clone()
		{
			return new Counter64(this);
		}

		public override int GetHashCode()
		{
			return _value.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is Counter64)
			{
				return _value.Equals(((Counter64)obj).Value);
			}
			if (obj is ulong)
			{
				return _value.Equals((ulong)obj);
			}
			return false;
		}

		public override string ToString()
		{
			return Value.ToString(CultureInfo.CurrentCulture);
		}

		public static implicit operator ulong(Counter64 value)
		{
			return value.Value;
		}

		public override void encode(MutableByte buffer)
		{
			byte[] bytes = BitConverter.GetBytes(_value);
			MutableByte mutableByte = new MutableByte();
			for (int num = bytes.Length - 1; num >= 0; num--)
			{
				if (bytes[num] != 0 || mutableByte.Length > 0)
				{
					mutableByte.Append(bytes[num]);
				}
			}
			if (mutableByte.Length == 0)
			{
				mutableByte.Append(0);
			}
			AsnType.BuildHeader(buffer, base.Type, mutableByte.Length);
			buffer.Append(mutableByte);
		}

		public override int decode(byte[] buffer, int offset)
		{
			int length;
			byte b = AsnType.ParseHeader(buffer, ref offset, out length);
			if (b != base.Type)
			{
				throw new SnmpException("Invalid ASN.1 type.");
			}
			if (buffer.Length - offset < length)
			{
				throw new OverflowException("Buffer underflow error");
			}
			if (length > 9)
			{
				throw new OverflowException("Integer too large: cannot decode");
			}
			byte[] array = new byte[8];
			if (length == 9)
			{
				offset++;
				length--;
			}
			while (length > 0)
			{
				array[length - 1] = buffer[offset];
				offset++;
				length--;
			}
			_value = BitConverter.ToUInt64(array, 0);
			return offset;
		}

		public int CompareTo(ulong other)
		{
			return _value.CompareTo(other);
		}

		public int CompareTo(Counter64 other)
		{
			if (other == null)
			{
				return -1;
			}
			return _value.CompareTo(other.Value);
		}

		public static bool operator ==(Counter64 first, Counter64 second)
		{
			if ((object)first == null && (object)second == null)
			{
				return true;
			}
			if ((object)first == null || (object)second == null)
			{
				return false;
			}
			return first.Equals(second);
		}

		public static bool operator !=(Counter64 first, Counter64 second)
		{
			if ((object)first == null && (object)second == null)
			{
				return false;
			}
			if ((object)first == null || (object)second == null)
			{
				return true;
			}
			return !first.Equals(second);
		}

		public static bool operator >(Counter64 first, Counter64 second)
		{
			if ((object)first == null && (object)second == null)
			{
				return false;
			}
			if ((object)first == null)
			{
				return false;
			}
			if ((object)second == null)
			{
				return true;
			}
			if (first.Value > second.Value)
			{
				return true;
			}
			return false;
		}

		public static bool operator <(Counter64 first, Counter64 second)
		{
			if ((object)first == null && (object)second == null)
			{
				return false;
			}
			if ((object)first == null)
			{
				return true;
			}
			if ((object)second == null)
			{
				return false;
			}
			if (first.Value < second.Value)
			{
				return true;
			}
			return false;
		}

		public static Counter64 operator +(Counter64 first, Counter64 second)
		{
			if (first == null && second == null)
			{
				return null;
			}
			if (first == null)
			{
				return new Counter64(second);
			}
			if (second == null)
			{
				return new Counter64(first);
			}
			return new Counter64(first.Value + second.Value);
		}

		public static Counter64 operator -(Counter64 first, Counter64 second)
		{
			if (first == null && second == null)
			{
				return null;
			}
			if (first == null)
			{
				return new Counter64(second);
			}
			if (second == null)
			{
				return new Counter64(first);
			}
			return new Counter64(first.Value - second.Value);
		}

		public static ulong Diff(Counter64 first, Counter64 second)
		{
			ulong value = first.Value;
			ulong value2 = second.Value;
			ulong num = 0uL;
			if (value2 > value)
			{
				return (ulong)(-1L - (long)value) + value2;
			}
			return value2 - value;
		}
	}
}
