namespace SnmpSharpNet
{
	public class SnmpV1TrapPacket : SnmpPacket
	{
		protected TrapPdu _pdu;

		protected OctetString _snmpCommunity;

		public new TrapPdu Pdu => _pdu;

		public TrapPdu TrapPdu => _pdu;

		public OctetString Community => _snmpCommunity;

		public SnmpV1TrapPacket()
			: base(SnmpVersion.Ver1)
		{
			_snmpCommunity = new OctetString();
			_pdu = new TrapPdu();
		}

		public SnmpV1TrapPacket(string snmpCommunity)
			: this()
		{
			_snmpCommunity.Set(snmpCommunity);
		}

		public override int decode(byte[] buffer, int length)
		{
			int num = 0;
			MutableByte obj = new MutableByte(buffer, length);
			num = base.decode(buffer, length);
			num = _snmpCommunity.decode(obj, num);
			int offset = num;
			int length2;
			byte b = AsnType.ParseHeader(buffer, ref offset, out length2);
			if (b != 164)
			{
				throw new SnmpException($"Invalid SNMP ASN.1 type. Received: {b:x2}");
			}
			return Pdu.decode(obj, num);
		}

		public override byte[] encode()
		{
			MutableByte mutableByte = new MutableByte();
			_snmpCommunity.encode(mutableByte);
			Pdu.encode(mutableByte);
			base.encode(mutableByte);
			return mutableByte;
		}
	}
}
