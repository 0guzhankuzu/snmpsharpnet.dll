using System;
using System.Text;

namespace SnmpSharpNet
{
	public class SnmpV2Packet : SnmpPacket
	{
		protected OctetString _snmpCommunity;

		public Pdu _pdu;

		public OctetString Community => _snmpCommunity;

		public override Pdu Pdu => _pdu;

		public SnmpV2Packet()
			: base(SnmpVersion.Ver2)
		{
			_protocolVersion.Value = 1;
			_pdu = new Pdu();
			_snmpCommunity = new OctetString();
		}

		public SnmpV2Packet(string snmpCommunity)
			: this()
		{
			_snmpCommunity.Set(snmpCommunity);
		}

		public override int decode(byte[] buffer, int length)
		{
			MutableByte mutableByte = new MutableByte(buffer, length);
			int num = 0;
			num = base.decode(buffer, buffer.Length);
			if (base.Version != SnmpVersion.Ver2)
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
			if (b != 160 && b != 161 && b != 163 && b != 165 && b != 162 && b != 167 && b != 166)
			{
				throw new SnmpInvalidPduTypeException("Invalid SNMP operation received: " + $"0x{b:x2}");
			}
			num = Pdu.decode(mutableByte, num);
			return length;
		}

		public override byte[] encode()
		{
			MutableByte mutableByte = new MutableByte();
			if (Pdu.Type != PduType.Get && Pdu.Type != PduType.GetNext && Pdu.Type != PduType.Set && Pdu.Type != PduType.V2Trap && Pdu.Type != PduType.Response && Pdu.Type != PduType.GetBulk && Pdu.Type != PduType.Inform)
			{
				throw new SnmpInvalidPduTypeException("Invalid SNMP PDU type while attempting to encode PDU: " + $"0x{Pdu.Type:x2}");
			}
			_protocolVersion.encode(mutableByte);
			_snmpCommunity.encode(mutableByte);
			_pdu.encode(mutableByte);
			MutableByte mutableByte2 = new MutableByte();
			AsnType.BuildHeader(mutableByte2, SnmpConstants.SMI_SEQUENCE, mutableByte.Length);
			mutableByte.Prepend(mutableByte2);
			return mutableByte;
		}

		public SnmpV2Packet BuildInformResponse()
		{
			return BuildInformResponse(this);
		}

		public static SnmpV2Packet BuildInformResponse(SnmpV2Packet informPacket)
		{
			if (informPacket.Version != SnmpVersion.Ver2)
			{
				throw new SnmpInvalidVersionException("INFORM packet can only be parsed from an SNMP version 2 packet.");
			}
			if (informPacket.Pdu.Type != PduType.Inform)
			{
				throw new SnmpInvalidPduTypeException("Inform response can only be built for INFORM packets.");
			}
			SnmpV2Packet snmpV2Packet = new SnmpV2Packet(informPacket.Community.ToString());
			snmpV2Packet.Pdu.Type = PduType.Response;
			snmpV2Packet.Pdu.TrapObjectID.Set(informPacket.Pdu.TrapObjectID);
			snmpV2Packet.Pdu.TrapSysUpTime.Value = informPacket.Pdu.TrapSysUpTime.Value;
			foreach (Vb vb in informPacket.Pdu.VbList)
			{
				snmpV2Packet.Pdu.VbList.Add(vb.Oid, vb.Value);
			}
			snmpV2Packet.Pdu.RequestId = informPacket.Pdu.RequestId;
			return snmpV2Packet;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SnmpV2Packet:\nCommunity: {0}\n{1}\n", Community.ToString(), Pdu.ToString());
			return stringBuilder.ToString();
		}
	}
}
