using System;

namespace SnmpSharpNet
{
	public abstract class AsnType : ICloneable
	{
		public static readonly byte BOOLEAN = 1;

		public static readonly byte INTEGER = 2;

		public static readonly byte BITSTRING = 3;

		public static readonly byte OCTETSTRING = 4;

		public static readonly byte NULL = 5;

		public static readonly byte OBJECTID = 6;

		public static readonly byte SEQUENCE = 16;

		public static readonly byte SET = 17;

		public static readonly byte UNIVERSAL = 0;

		public static readonly byte APPLICATION = 64;

		public static readonly byte CONTEXT = 128;

		public static readonly byte PRIVATE = 192;

		public static readonly byte PRIMITIVE = 0;

		public static readonly byte CONSTRUCTOR = 32;

		protected static readonly byte HIGH_BIT = 128;

		protected static readonly byte EXTENSION_ID = 31;

		protected byte _asnType;

		public byte Type
		{
			get
			{
				return _asnType;
			}
			set
			{
				_asnType = value;
			}
		}

		public abstract void encode(MutableByte buffer);

		public abstract int decode(byte[] buffer, int offset);

		internal static void BuildLength(MutableByte mb, int asnLength)
		{
			if (asnLength < 0)
			{
				throw new ArgumentOutOfRangeException("Length cannot be less then 0.");
			}
			byte[] bytes = BitConverter.GetBytes(asnLength);
			MutableByte mutableByte = new MutableByte();
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
			if (mutableByte.Length == 1 && (mutableByte[0] & HIGH_BIT) == 0)
			{
				mb.Append(mutableByte);
				return;
			}
			byte b = (byte)mutableByte.Length;
			b = (byte)(b | HIGH_BIT);
			mb.Append(b);
			mb.Append(mutableByte);
		}

		internal static int ParseLength(byte[] mb, ref int offset)
		{
			if (offset == mb.Length)
			{
				throw new OverflowException("Buffer is too short.");
			}
			int num = 0;
			if ((mb[offset] & HIGH_BIT) == 0)
			{
				return mb[offset++];
			}
			num = mb[offset++] & ~HIGH_BIT;
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				num2 <<= 8;
				num2 |= mb[offset++];
				if (offset > mb.Length || (i < num - 1 && offset == mb.Length))
				{
					throw new OverflowException("Buffer is too short.");
				}
			}
			return num2;
		}

		internal static void BuildHeader(MutableByte mb, byte asnType, int asnLength)
		{
			mb.Append(asnType);
			BuildLength(mb, asnLength);
		}

		internal static byte ParseHeader(byte[] mb, ref int offset, out int length)
		{
			if (mb.Length - offset < 1)
			{
				throw new OverflowException("Buffer is too short.");
			}
			byte b = mb[offset++];
			if ((b & EXTENSION_ID) == EXTENSION_ID)
			{
				throw new SnmpException("Invalid SNMP header type");
			}
			length = ParseLength(mb, ref offset);
			return b;
		}

		public abstract object Clone();
	}
}
