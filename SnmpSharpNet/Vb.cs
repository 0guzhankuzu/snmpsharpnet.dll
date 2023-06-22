using System;

namespace SnmpSharpNet
{
	public class Vb : AsnType, ICloneable
	{
		private Oid _oid;

		private AsnType _value;

		public AsnType Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = (AsnType)value.Clone();
			}
		}

		public Oid Oid
		{
			get
			{
				return _oid;
			}
			set
			{
				_oid = (Oid)value.Clone();
			}
		}

		public Vb()
		{
			_asnType = (byte)(AsnType.SEQUENCE | AsnType.CONSTRUCTOR);
		}

		public Vb(Oid oid)
			: this()
		{
			_oid = (Oid)oid.Clone();
			_value = new Null();
		}

		public Vb(Oid oid, AsnType value)
			: this(oid)
		{
			_value = (AsnType)value.Clone();
		}

		public Vb(string oid)
			: this()
		{
			_oid = new Oid(oid);
			_value = new Null();
		}

		public Vb(Vb second)
			: this()
		{
			Set(second);
		}

		public void Set(Vb value)
		{
			_oid = (Oid)value.Oid.Clone();
			_value = (Oid)value.Value.Clone();
		}

		public void ResetValue()
		{
			_value = new Null();
		}

		public override object Clone()
		{
			return new Vb(_oid, _value);
		}

		public override string ToString()
		{
			return _oid.ToString() + ": (" + SnmpConstants.GetTypeName(_value.Type) + ") " + _value.ToString();
		}

		public override void encode(MutableByte buffer)
		{
			MutableByte mutableByte = new MutableByte();
			_oid.encode(mutableByte);
			MutableByte mutableByte2 = new MutableByte();
			_value.encode(mutableByte2);
			int asnLength = mutableByte.Length + mutableByte2.Length;
			AsnType.BuildHeader(buffer, base.Type, asnLength);
			buffer.Append(mutableByte);
			buffer.Append(mutableByte2);
		}

		public override int decode(byte[] buffer, int offset)
		{
			byte b = AsnType.ParseHeader(buffer, ref offset, out var length);
			if (b != base.Type)
			{
				throw new SnmpException($"Invalid ASN.1 type. Expected 0x{base.Type:x2} received 0x{b:x2}");
			}
			if (buffer.Length - offset < length)
			{
				throw new OverflowException("Buffer underflow error");
			}
			_oid = new Oid();
			offset = _oid.decode(buffer, offset);
			int offset2 = offset;
			b = AsnType.ParseHeader(buffer, ref offset2, out length);
			_value = SnmpConstants.GetSyntaxObject(b);
			if (_value == null)
			{
				throw new SnmpDecodingException($"Invalid ASN.1 type encountered 0x{b:x2}. Unable to continue decoding.");
			}
			offset = _value.decode(buffer, offset);
			return offset;
		}
	}
}
