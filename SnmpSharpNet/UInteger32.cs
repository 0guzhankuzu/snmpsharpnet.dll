using System;
using System.Globalization;

namespace SnmpSharpNet
{
	[Serializable]
	public class UInteger32 : AsnType, IComparable<UInteger32>, IComparable<uint>
	{
		protected uint _value;

		public uint Value
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

		public UInteger32()
		{
			_asnType = SnmpConstants.SMI_UNSIGNED32;
		}

		public UInteger32(uint val)
			: this()
		{
			_value = val;
		}

		public UInteger32(UInteger32 second)
			: this(second.Value)
		{
		}

		public UInteger32(string val)
			: this()
		{
			Set(val);
		}

		public void Set(string value)
		{
			if (value.Length == 0)
			{
				throw new ArgumentException("value", "String has to be length greater then 0");
			}
			_value = uint.Parse(value);
		}

		public void Set(AsnType value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value", "Parameter is null");
			}
			if (value is UInteger32)
			{
				_value = ((UInteger32)value).Value;
			}
			else if (value is Integer32)
			{
				_value = (uint)((Integer32)value).Value;
			}
		}

		public override string ToString()
		{
			return Convert.ToString(_value, CultureInfo.CurrentCulture);
		}

		public override object Clone()
		{
			return new UInteger32(this);
		}

		public static implicit operator uint(UInteger32 value)
		{
			if (value == null)
			{
				return 0u;
			}
			return value.Value;
		}

		public override void encode(MutableByte buffer)
		{
			MutableByte mutableByte = new MutableByte();
			byte[] bytes = BitConverter.GetBytes(_value);
			for (int num = 3; num >= 0; num--)
			{
				if (bytes[num] != 0 || mutableByte.Length > 0)
				{
					mutableByte.Append(bytes[num]);
				}
			}
			if (mutableByte.Length > 0 && (mutableByte[0] & 0x80u) != 0)
			{
				mutableByte.Prepend(0);
			}
			else if (mutableByte.Length == 0)
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
			if (length > 5)
			{
				throw new OverflowException("Integer too large: cannot decode");
			}
			_value = 0u;
			for (int i = 0; i < length; i++)
			{
				_value <<= 8;
				_value |= buffer[offset++];
			}
			return offset;
		}

		public int CompareTo(UInteger32 other)
		{
			return _value.CompareTo(other.Value);
		}

		public int CompareTo(uint other)
		{
			return _value.CompareTo(other);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is UInteger32)
			{
				UInteger32 uInteger = (UInteger32)obj;
				return _value.Equals(uInteger.Value);
			}
			if (obj is uint)
			{
				uint obj2 = (uint)obj;
				return _value.Equals(obj2);
			}
			return false;
		}

		public static bool operator ==(UInteger32 first, UInteger32 second)
		{
			if ((object)first == null && (object)second == null)
			{
				return true;
			}
			return first?.Equals(second) ?? false;
		}

		public static bool operator !=(UInteger32 first, UInteger32 second)
		{
			return !(first == second);
		}

		public static bool operator >(UInteger32 first, UInteger32 second)
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

		public static bool operator <(UInteger32 first, UInteger32 second)
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

		public override int GetHashCode()
		{
			return _value.GetHashCode();
		}

		public static UInteger32 operator +(UInteger32 first, UInteger32 second)
		{
			if (first == null && second == null)
			{
				return null;
			}
			if (first == null)
			{
				return new UInteger32(second);
			}
			if (second == null)
			{
				return new UInteger32(first);
			}
			return new UInteger32(first.Value + second.Value);
		}

		public static UInteger32 operator -(UInteger32 first, UInteger32 second)
		{
			if (first == null && second == null)
			{
				return null;
			}
			if (first == null)
			{
				return new UInteger32(second);
			}
			if (second == null)
			{
				return new UInteger32(first);
			}
			return new UInteger32(first.Value - second.Value);
		}
	}
}
