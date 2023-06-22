using System;

namespace SnmpSharpNet
{
	public abstract class SnmpPacket
	{
		protected Integer32 _protocolVersion;

		public SnmpVersion Version => (SnmpVersion)_protocolVersion.Value;

		public virtual Pdu Pdu => null;

		public bool IsReport
		{
			get
			{
				if (Pdu.Type == PduType.Response)
				{
					return true;
				}
				return false;
			}
		}

		public bool IsRequest
		{
			get
			{
				if (Pdu.Type == PduType.Get || Pdu.Type == PduType.GetNext || Pdu.Type == PduType.GetBulk || Pdu.Type == PduType.Set)
				{
					return true;
				}
				return false;
			}
		}

		public bool IsResponse
		{
			get
			{
				if (Pdu.Type == PduType.Response)
				{
					return true;
				}
				return false;
			}
		}

		public bool IsNotification
		{
			get
			{
				if (Pdu.Type == PduType.Trap || Pdu.Type == PduType.V2Trap || Pdu.Type == PduType.Inform)
				{
					return true;
				}
				return false;
			}
		}

		public SnmpPacket()
		{
			_protocolVersion = new Integer32(0);
		}

		public SnmpPacket(SnmpVersion protocolVersion)
		{
			_protocolVersion = new Integer32((int)protocolVersion);
		}

		public virtual int decode(byte[] buffer, int length)
		{
			int offset = 0;
			if (length < 2)
			{
				throw new OverflowException("Packet too small.");
			}
			MutableByte obj = new MutableByte(buffer, length);
			Sequence sequence = new Sequence();
			offset = sequence.decode(obj, offset);
			if (sequence.Type != SnmpConstants.SMI_SEQUENCE)
			{
				throw new SnmpDecodingException("Invalid sequence type at the start of the SNMP packet.");
			}
			return _protocolVersion.decode(obj, offset);
		}

		public abstract byte[] encode();

		public virtual void encode(MutableByte buffer)
		{
			MutableByte mutableByte = new MutableByte();
			_protocolVersion.encode(mutableByte);
			buffer.Prepend(mutableByte);
			mutableByte.Reset();
			AsnType.BuildHeader(mutableByte, SnmpConstants.SMI_SEQUENCE, buffer.Length);
			buffer.Prepend(mutableByte);
		}

		public static int GetProtocolVersion(byte[] buffer, int bufferLength)
		{
			int offset = 0;
			int length = 0;
			byte b = AsnType.ParseHeader(buffer, ref offset, out length);
			if (offset + length > bufferLength)
			{
				return -1;
			}
			if (b != SnmpConstants.SMI_SEQUENCE)
			{
				throw new SnmpDecodingException("Invalid sequence type at the start of the SNMP packet.");
			}
			Integer32 integer = new Integer32();
			offset = integer.decode(buffer, offset);
			return integer.Value;
		}
	}
}
