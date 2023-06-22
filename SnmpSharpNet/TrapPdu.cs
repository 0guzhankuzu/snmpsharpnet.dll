using System;

namespace SnmpSharpNet
{
	public class TrapPdu : AsnType, ICloneable
	{
		protected Oid _enterprise;

		protected IpAddress _agentAddr;

		protected Integer32 _generic;

		protected Integer32 _specific;

		protected TimeTicks _timeStamp;

		private VbCollection _variables;

		public virtual IpAddress AgentAddress => _agentAddr;

		public virtual int Generic
		{
			get
			{
				return _generic.Value;
			}
			set
			{
				_generic.Value = value;
			}
		}

		public virtual int Specific
		{
			get
			{
				return _specific.Value;
			}
			set
			{
				_specific.Value = value;
			}
		}

		public virtual uint TimeStamp
		{
			get
			{
				return _timeStamp.Value;
			}
			set
			{
				_timeStamp.Value = value;
			}
		}

		public virtual int Count => _variables.Count;

		public new PduType Type => (PduType)_asnType;

		public Oid Enterprise => _enterprise;

		public VbCollection VbList => _variables;

		public int VbCount => _variables.Count;

		public TrapPdu()
		{
			_asnType = 164;
			_enterprise = new Oid();
			_agentAddr = new IpAddress();
			_generic = new Integer32();
			_specific = new Integer32();
			_timeStamp = new TimeTicks();
			_variables = new VbCollection();
		}

		public TrapPdu(TrapPdu second)
			: this()
		{
			_enterprise.Set(second._enterprise);
			((OctetString)_agentAddr).Set((byte[])second._agentAddr);
			_generic.Value = second.Generic;
			_specific.Value = second.Specific;
			_timeStamp.Value = second.TimeStamp;
			for (int i = 0; i < second._variables.Count; i++)
			{
				_variables = (VbCollection)second.VbList.Clone();
			}
		}

		public void Set(string value)
		{
			throw new NotImplementedException();
		}

		public void Set(TrapPdu second)
		{
			if (second != null)
			{
				_enterprise.Set(second._enterprise);
				((OctetString)_agentAddr).Set((byte[])second._agentAddr);
				_generic.Value = second.Generic;
				_specific.Value = second.Specific;
				_timeStamp.Value = second.TimeStamp;
				_variables.Clear();
				for (int i = 0; i < second._variables.Count; i++)
				{
					_variables = (VbCollection)second.VbList.Clone();
				}
				return;
			}
			throw new ArgumentException("Invalid argument type.", "value");
		}

		public override void encode(MutableByte buffer)
		{
			MutableByte mutableByte = new MutableByte();
			_enterprise.encode(mutableByte);
			_agentAddr.encode(mutableByte);
			_generic.encode(mutableByte);
			_specific.encode(mutableByte);
			_timeStamp.encode(mutableByte);
			_variables.encode(mutableByte);
			MutableByte mutableByte2 = new MutableByte();
			AsnType.BuildHeader(mutableByte2, 164, mutableByte.Length);
			mutableByte.Prepend(mutableByte2);
			buffer.Append(mutableByte);
		}

		public override int decode(byte[] buffer, int offset)
		{
			int length;
			byte b = AsnType.ParseHeader(buffer, ref offset, out length);
			if (b != 164)
			{
				throw new SnmpException("Invalid PDU type.");
			}
			if (length > buffer.Length - offset)
			{
				throw new OverflowException("Packet is too short.");
			}
			offset = _enterprise.decode(buffer, offset);
			offset = _agentAddr.decode(buffer, offset);
			offset = _generic.decode(buffer, offset);
			offset = _specific.decode(buffer, offset);
			offset = _timeStamp.decode(buffer, offset);
			_variables.Clear();
			offset = _variables.decode(buffer, offset);
			return offset;
		}

		public override object Clone()
		{
			return new TrapPdu(this);
		}
	}
}
