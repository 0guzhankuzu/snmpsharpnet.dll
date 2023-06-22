using System;

namespace SnmpSharpNet
{
	public class ScopedPdu : Pdu
	{
		protected OctetString _contextEngineId;

		protected OctetString _contextName;

		public OctetString ContextEngineId => _contextEngineId;

		public OctetString ContextName => _contextName;

		public ScopedPdu()
		{
			_contextEngineId = new OctetString();
			_contextName = new OctetString();
		}

		public ScopedPdu(PduType pduType)
			: base(pduType)
		{
			_contextEngineId = new OctetString();
			_contextName = new OctetString();
		}

		public ScopedPdu(PduType pduType, int requestId)
			: base(pduType)
		{
			_requestId.Value = requestId;
			_contextEngineId = new OctetString();
			_contextName = new OctetString();
		}

		public ScopedPdu(Pdu pdu)
			: base(pdu)
		{
			_contextEngineId = new OctetString();
			_contextName = new OctetString();
		}

		public override void encode(MutableByte buffer)
		{
			MutableByte mutableByte = new MutableByte();
			_contextEngineId.encode(mutableByte);
			_contextName.encode(mutableByte);
			base.encode(mutableByte);
			AsnType.BuildHeader(buffer, SnmpConstants.SMI_SEQUENCE, mutableByte.Length);
			buffer.Append(mutableByte);
		}

		public override int decode(byte[] buffer, int offset)
		{
			int length;
			byte b = AsnType.ParseHeader(buffer, ref offset, out length);
			if (b != SnmpConstants.SMI_SEQUENCE)
			{
				throw new SnmpDecodingException("Invalid ScopedPdu sequence detected. Invalid ScopedPdu encoding.");
			}
			if (length > buffer.Length - offset)
			{
				throw new OverflowException("SNMP packet too short.");
			}
			_contextEngineId.Reset();
			_contextName.Reset();
			offset = _contextEngineId.decode(buffer, offset);
			offset = _contextName.decode(buffer, offset);
			offset = base.decode(buffer, offset);
			return offset;
		}
	}
}
