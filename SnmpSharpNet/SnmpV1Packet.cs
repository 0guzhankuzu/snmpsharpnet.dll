using System;
using System.Text;

namespace SnmpSharpNet
{
	public class SnmpV1Packet : SnmpPacket
	{
		protected OctetString _snmpCommunity;

		public Pdu _pdu;

		public OctetString Community => _snmpCommunity;

		public override Pdu Pdu => _pdu;

		public SnmpV1Packet()
			: base(SnmpVersion.Ver1)
		{
			_snmpCommunity = new OctetString();
			_pdu = new Pdu();
		}

		public SnmpV1Packet(string snmpCommunity)
			: this()
		{
			_snmpCommunity.Set(snmpCommunity);
		}

		public override int decode(byte[] buffer, int length)
		{
			MutableByte mutableByte = new MutableByte(buffer, length);
			int num = 0;
			num = base.decode(buffer, buffer.Length);
			if (_protocolVersion.Value != 0)
			{
				throw new SnmpInvalidVersionException("Invalid protocol version");
			}
			num = _snmpCommunity.decode(mutableByte, num);
			int offset = num;
			int length2;
			byte b = AsnType.ParseHeader(mutableByte, ref offset, out length2);
			if (length2 + num > mutableByte.Length)
			{
				throw new OverflowException("Insufficient data in packet");
			}
			if (b != 160 && b != 161 && b != 163 && b != 162)
			{
				throw new SnmpInvalidPduTypeException("Invalid SNMP operation received: " + $"0x{b:x2}");
			}
			num = Pdu.decode(mutableByte, num);
			return length;
		}

		private new void encode(MutableByte buffer)
		{
			throw new NotImplementedException();
		}

		public override byte[] encode()
		{
			if (Pdu.Type != PduType.Get && Pdu.Type != PduType.GetNext && Pdu.Type != PduType.Set && Pdu.Type != PduType.Response)
			{
				throw new SnmpInvalidVersionException("Invalid SNMP PDU type while attempting to encode PDU: " + $"0x{Pdu.Type:x2}");
			}
			if (Pdu.RequestId == 0)
			{
				Random random = new Random((int)DateTime.Now.Ticks);
				Pdu.RequestId = random.Next();
			}
			MutableByte mutableByte = new MutableByte();
			_protocolVersion.encode(mutableByte);
			_snmpCommunity.encode(mutableByte);
			Pdu.encode(mutableByte);
			MutableByte mutableByte2 = new MutableByte();
			AsnType.BuildHeader(mutableByte2, SnmpConstants.SMI_SEQUENCE, mutableByte.Length);
			mutableByte2.Append(mutableByte);
			return mutableByte2;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SnmpV1Packet:\nCommunity: {0}\n{1}\n", Community.ToString(), Pdu.ToString());
			return stringBuilder.ToString();
		}
	}
}
