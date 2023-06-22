using System;

namespace SnmpSharpNet
{
	[Serializable]
	public class Integer32 : AsnType, IComparable<Integer32>, IComparable<int>, ICloneable
	{
		protected int _value;

		public int Value
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

		public Integer32()
		{
			_asnType = SnmpConstants.SMI_INTEGER;
		}

		public Integer32(int val)
			: this()
		{
			_value = val;
		}

		public Integer32(Integer32 second)
			: this()
		{
			Set(second);
		}

		public Integer32(string val)
			: this()
		{
			Set(val);
		}

		public void Set(AsnType value)
		{
			Integer32 integer = value as Integer32;
			if (integer != null)
			{
				_value = integer.Value;
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
				_value = int.Parse(value);
			}
			catch
			{
				throw new ArgumentException("Invalid argument format.");
			}
		}

		public override object Clone()
		{
			return new Integer32(this);
		}

		public override string ToString()
		{
			return _value.ToString();
		}

		public static implicit operator int(Integer32 value)
		{
			if (value == null)
			{
				return 0;
			}
			return value.Value;
		}

		public override int GetHashCode()
		{
			return _value.GetHashCode();
		}

		public void SetRandom()
		{
			Random random = new Random();
			_value = random.Next();
		}

		public override void encode(MutableByte buffer)
		{
			int value = _value;
			byte[] bytes = BitConverter.GetBytes(_value);
			MutableByte mutableByte = new MutableByte();
			if (value < 0)
			{
				for (int num = 3; num >= 0; num--)
				{
					if (mutableByte.Length > 0 || bytes[num] != byte.MaxValue)
					{
						mutableByte.Append(bytes[num]);
					}
				}
				if (mutableByte.Length == 0)
				{
					mutableByte.Append(byte.MaxValue);
				}
				if ((mutableByte[0] & 0x80) == 0)
				{
					mutableByte.Prepend(byte.MaxValue);
				}
			}
			else if (value == 0)
			{
				mutableByte.Append(0);
			}
			else
			{
				for (int num = 3; num >= 0; num--)
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
				else if ((mutableByte[0] & 0x80u) != 0)
				{
					mutableByte.Prepend(0);
				}
			}
			if (mutableByte.Length > 1 && mutableByte[0] == byte.MaxValue && (mutableByte[1] & 0x80u) != 0)
			{
				mutableByte.Prepend(0);
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
				throw new SnmpException("Invalid ASN.1 type");
			}
			if (buffer.Length - offset < length)
			{
				throw new OverflowException("Buffer underflow error");
			}
			bool flag = false;
			if (length > 5)
			{
				throw new OverflowException("Integer size is invalid. Unable to decode.");
			}
			if ((buffer[offset] & AsnType.HIGH_BIT) != 0)
			{
				flag = true;
			}
			if (buffer[offset] == 128 && length > 2 && buffer[offset + 1] == byte.MaxValue && (buffer[offset + 2] & 0x80u) != 0)
			{
				offset++;
				length--;
			}
			if (flag)
			{
				_value = -1;
			}
			else
			{
				_value = 0;
			}
			for (int i = 0; i < length; i++)
			{
				_value <<= 8;
				_value |= buffer[offset++];
			}
			return offset;
		}

		public int CompareTo(Integer32 other)
		{
			if ((object)other == null)
			{
				return 1;
			}
			return _value.CompareTo(other.Value);
		}

		public int CompareTo(int other)
		{
			return _value.CompareTo(other);
		}

		public override bool Equals(object obj)
		{
			if (obj is Integer32)
			{
				Integer32 integer = (Integer32)obj;
				return _value.Equals(integer.Value);
			}
			if (obj is int)
			{
				int obj2 = (int)obj;
				return _value.Equals(obj2);
			}
			return false;
		}

		public static bool operator ==(Integer32 first, Integer32 second)
		{
			if ((object)first == null && (object)second == null)
			{
				return true;
			}
			if ((object)first == null && (object)second != null)
			{
				return false;
			}
			return first.Equals(second);
		}

		public static bool operator !=(Integer32 first, Integer32 second)
		{
			return !(first == second);
		}

		public static Integer32 operator +(Integer32 first, Integer32 second)
		{
			if (first == null && second == null)
			{
				return null;
			}
			if (first == null)
			{
				return new Integer32(second);
			}
			if (second == null)
			{
				return new Integer32(first);
			}
			return new Integer32(first.Value + second.Value);
		}

		public static Integer32 operator -(Integer32 first, Integer32 second)
		{
			if (first == null && second == null)
			{
				return null;
			}
			if (first == null)
			{
				return new Integer32(second);
			}
			if (second == null)
			{
				return new Integer32(first);
			}
			return new Integer32(first.Value - second.Value);
		}
	}
}
