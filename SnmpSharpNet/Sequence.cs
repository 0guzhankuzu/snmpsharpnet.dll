using System;

namespace SnmpSharpNet
{
	[Serializable]
	public class Sequence : AsnType, ICloneable
	{
		protected byte[] _data;

		public byte[] Value => _data;

		public Sequence()
		{
			_asnType = SnmpConstants.SMI_SEQUENCE;
			_data = null;
		}

		public Sequence(byte[] value)
			: this()
		{
			if (value != null && value.Length > 0)
			{
				_data = new byte[value.Length];
				Buffer.BlockCopy(value, 0, _data, 0, value.Length);
			}
		}

		public void Set(byte[] value)
		{
			if (value == null || value.Length <= 0)
			{
				_data = null;
				return;
			}
			_data = new byte[value.Length];
			Buffer.BlockCopy(value, 0, _data, 0, value.Length);
		}

		public override void encode(MutableByte buffer)
		{
			int num = 0;
			if (_data != null && _data.Length > 0)
			{
				num = _data.Length;
			}
			AsnType.BuildHeader(buffer, base.Type, num);
			if (num > 0)
			{
				buffer.Append(_data);
			}
		}

		public override int decode(byte[] buffer, int offset)
		{
			_data = null;
			int length = 0;
			int num = AsnType.ParseHeader(buffer, ref offset, out length);
			if (num != base.Type)
			{
				throw new SnmpException("Invalid ASN.1 type.");
			}
			if (offset + length > buffer.Length)
			{
				throw new OverflowException("Sequence longer then packet.");
			}
			if (length > 0)
			{
				_data = new byte[length];
				Buffer.BlockCopy(buffer, offset, _data, 0, length);
			}
			return offset;
		}

		public override object Clone()
		{
			return new Sequence(_data);
		}
	}
}
