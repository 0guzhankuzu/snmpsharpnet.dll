using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SnmpSharpNet
{
	public class Pdu : AsnType, ICloneable, IEnumerable<Vb>, IEnumerable
	{
		protected VbCollection _vbs;

		protected Integer32 _errorStatus;

		protected Integer32 _errorIndex;

		protected Integer32 _requestId;

		protected TimeTicks _trapTimeStamp;

		protected Oid _trapObjectID;

		public int ErrorStatus
		{
			get
			{
				if (Type == PduType.GetBulk)
				{
					throw new SnmpInvalidPduTypeException("ErrorStatus property is not valid for GetBulk packets.");
				}
				return _errorStatus.Value;
			}
			set
			{
				_errorStatus.Value = value;
			}
		}

		public int ErrorIndex
		{
			get
			{
				if (Type == PduType.GetBulk)
				{
					throw new SnmpInvalidPduTypeException("ErrorStatus property is not valid for GetBulk packets.");
				}
				return _errorIndex.Value;
			}
			set
			{
				_errorIndex.Value = value;
			}
		}

		public int RequestId
		{
			get
			{
				return _requestId.Value;
			}
			set
			{
				_requestId.Value = value;
			}
		}

		public new PduType Type
		{
			get
			{
				return (PduType)_asnType;
			}
			set
			{
				if ((uint)_asnType != (uint)value)
				{
					if (value != PduType.GetBulk)
					{
						_errorIndex.Value = 0;
						_errorStatus.Value = 0;
					}
					else
					{
						_errorStatus.Value = 0;
						_errorIndex.Value = 100;
					}
					_asnType = (byte)value;
				}
			}
		}

		public int MaxRepetitions
		{
			get
			{
				if (Type == PduType.GetBulk)
				{
					return _errorIndex.Value;
				}
				throw new SnmpInvalidPduTypeException("NonRepeaters property is only available in GET-BULK PDU type.");
			}
			set
			{
				if (Type == PduType.GetBulk)
				{
					_errorIndex.Value = value;
					return;
				}
				throw new SnmpInvalidPduTypeException("NonRepeaters property is only available in GET-BULK PDU type.");
			}
		}

		public int NonRepeaters
		{
			get
			{
				if (Type == PduType.GetBulk)
				{
					return _errorStatus.Value;
				}
				throw new SnmpInvalidPduTypeException("NonRepeaters property is only available in GET-BULK PDU type.");
			}
			set
			{
				if (_asnType == 165)
				{
					_errorStatus.Value = value;
					return;
				}
				throw new SnmpInvalidPduTypeException("NonRepeaters property is only available in GET-BULK PDU type.");
			}
		}

		public VbCollection VbList => _vbs;

		public TimeTicks TrapSysUpTime => _trapTimeStamp;

		public Oid TrapObjectID
		{
			get
			{
				if (Type != PduType.V2Trap && Type != PduType.Inform && Type != PduType.Response)
				{
					throw new SnmpInvalidPduTypeException("TrapObjectID value can only be accessed in V2TRAP, INFORM and RESPONSE PDUs");
				}
				return _trapObjectID;
			}
			set
			{
				_trapObjectID.Set(value);
			}
		}

		public int VbCount => _vbs.Count;

		public Vb this[int index] => _vbs[index];

		public Vb this[Oid oid]
		{
			get
			{
				if (!_vbs.ContainsOid(oid))
				{
					return null;
				}
				foreach (Vb vb in _vbs)
				{
					if (vb.Oid.Equals(oid))
					{
						return vb;
					}
				}
				return null;
			}
		}

		public Vb this[string oid]
		{
			get
			{
				foreach (Vb vb in _vbs)
				{
					if (vb.Oid.Equals(oid))
					{
						return vb;
					}
				}
				return null;
			}
		}

		public Pdu()
		{
			_vbs = null;
			_errorIndex = new Integer32();
			_errorStatus = new Integer32();
			_requestId = new Integer32();
			_requestId.SetRandom();
			_asnType = 160;
			_vbs = new VbCollection();
			_trapTimeStamp = new TimeTicks();
			_trapObjectID = new Oid();
		}

		public Pdu(PduType pduType)
			: this()
		{
			_asnType = (byte)pduType;
			if (_asnType == 165)
			{
				_errorStatus.Value = 0;
				_errorIndex.Value = 100;
			}
		}

		public Pdu(VbCollection vbs)
			: this()
		{
			_vbs = (VbCollection)vbs.Clone();
		}

		public Pdu(VbCollection vbs, PduType type, int requestId)
			: this(vbs)
		{
			_requestId.Value = requestId;
			_asnType = (byte)type;
		}

		public Pdu(Pdu pdu)
			: this(pdu.VbList, pdu.Type, pdu.RequestId)
		{
			if (pdu.Type == PduType.GetBulk)
			{
				NonRepeaters = pdu.NonRepeaters;
				MaxRepetitions = pdu.MaxRepetitions;
			}
			else
			{
				ErrorStatus = pdu.ErrorStatus;
				ErrorIndex = pdu.ErrorIndex;
			}
		}

		public void Set(AsnType value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			Pdu pdu = value as Pdu;
			if (pdu != null)
			{
				Type = pdu.Type;
				_requestId.Value = pdu.RequestId;
				if (Type == PduType.GetBulk)
				{
					NonRepeaters = pdu.NonRepeaters;
					MaxRepetitions = pdu.MaxRepetitions;
				}
				else
				{
					ErrorStatus = pdu.ErrorStatus;
					ErrorIndex = pdu.ErrorIndex;
				}
				_vbs.Clear();
				foreach (Vb vb in pdu.VbList)
				{
					_vbs.Add((Vb)vb.Clone());
				}
				return;
			}
			throw new ArgumentNullException("value", "Argument is not an Oid class");
		}

		public void SetVbList(VbCollection value)
		{
			_vbs.Clear();
			foreach (Vb item in value)
			{
				_vbs.Add(item);
			}
		}

		public void Reset()
		{
			_vbs.Clear();
			_errorStatus.Value = 0;
			_errorIndex.Value = 0;
			if (_requestId.Value == int.MaxValue)
			{
				_requestId.Value = 1;
			}
			else
			{
				_requestId.Value += 1;
			}
			_trapObjectID.Reset();
			_trapTimeStamp.Value = 0u;
		}

		public static Pdu GetPdu(VbCollection vbs)
		{
			Pdu pdu = new Pdu(vbs);
			pdu.Type = PduType.Get;
			pdu.ErrorIndex = 0;
			pdu.ErrorStatus = 0;
			return pdu;
		}

		public static Pdu GetPdu()
		{
			return new Pdu(PduType.Get);
		}

		public static Pdu SetPdu(VbCollection vbs)
		{
			Pdu pdu = new Pdu(vbs);
			pdu.Type = PduType.Set;
			pdu.ErrorIndex = 0;
			pdu.ErrorStatus = 0;
			return pdu;
		}

		public static Pdu SetPdu()
		{
			return new Pdu(PduType.Set);
		}

		public static Pdu GetNextPdu(VbCollection vbs)
		{
			Pdu pdu = new Pdu(vbs);
			pdu.Type = PduType.GetNext;
			pdu.ErrorIndex = 0;
			pdu.ErrorStatus = 0;
			return pdu;
		}

		public static Pdu GetNextPdu()
		{
			return new Pdu(PduType.GetNext);
		}

		public static Pdu GetBulkPdu(VbCollection vbs)
		{
			Pdu pdu = new Pdu(vbs);
			pdu.Type = PduType.GetBulk;
			pdu.MaxRepetitions = 100;
			pdu.NonRepeaters = 0;
			return pdu;
		}

		public static Pdu GetBulkPdu()
		{
			return new Pdu(PduType.GetBulk);
		}

		public Vb GetVb(int index)
		{
			return _vbs[index];
		}

		public void DeleteVb(int pos)
		{
			if (pos >= 0 && pos <= _vbs.Count)
			{
				_vbs.RemoveAt(pos);
			}
		}

		public override void encode(MutableByte buffer)
		{
			MutableByte mutableByte = new MutableByte();
			if (_requestId.Value == 0)
			{
				_requestId.SetRandom();
			}
			_requestId.encode(mutableByte);
			_errorStatus.encode(mutableByte);
			_errorIndex.encode(mutableByte);
			if (Type == PduType.V2Trap || Type == PduType.Inform)
			{
				if (_vbs.Count == 0)
				{
					_vbs.Add(SnmpConstants.SysUpTime, _trapTimeStamp);
					_vbs.Add(SnmpConstants.TrapObjectId, _trapObjectID);
				}
				else
				{
					if (_vbs.Count > 0 && !_vbs[0].Oid.Equals(SnmpConstants.SysUpTime))
					{
						Vb item = new Vb(SnmpConstants.SysUpTime, _trapTimeStamp);
						_vbs.Insert(0, item);
					}
					if (_vbs.Count > 1 && !_vbs[1].Oid.Equals(SnmpConstants.TrapObjectId))
					{
						Vb item2 = new Vb(SnmpConstants.TrapObjectId, _trapObjectID);
						_vbs.Insert(1, item2);
					}
				}
			}
			_vbs.encode(mutableByte);
			AsnType.BuildHeader(buffer, (byte)Type, mutableByte.Length);
			buffer.Append(mutableByte);
		}

		public override int decode(byte[] buffer, int offset)
		{
			int length;
			byte asnType = AsnType.ParseHeader(buffer, ref offset, out length);
			if (offset + length > buffer.Length)
			{
				throw new OverflowException("Insufficient data in packet");
			}
			_asnType = asnType;
			offset = _requestId.decode(buffer, offset);
			offset = _errorStatus.decode(buffer, offset);
			offset = _errorIndex.decode(buffer, offset);
			_vbs.Clear();
			offset = _vbs.decode(buffer, offset);
			if (Type == PduType.V2Trap || Type == PduType.Inform)
			{
				if (_vbs.Count > 0 && _vbs[0].Oid.Equals(SnmpConstants.SysUpTime))
				{
					_trapTimeStamp.Set(_vbs[0].Value);
					_vbs.RemoveAt(0);
				}
				if (_vbs.Count > 0 && _vbs[0].Oid.Equals(SnmpConstants.TrapObjectId))
				{
					_trapObjectID.Set((Oid)_vbs[0].Value);
					_vbs.RemoveAt(0);
				}
			}
			return offset;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("PDU-");
			switch (_asnType)
			{
			case 160:
				stringBuilder.Append("Get");
				break;
			case 161:
				stringBuilder.Append("GetNext");
				break;
			case 165:
				stringBuilder.Append("GetBulk");
				break;
			case 167:
				stringBuilder.Append("V2Trap");
				break;
			case 162:
				stringBuilder.Append("Response");
				break;
			case 166:
				stringBuilder.Append("Inform");
				break;
			default:
				stringBuilder.Append("Unknown");
				break;
			}
			stringBuilder.Append("\n");
			stringBuilder.AppendFormat("RequestId: {0}\n", RequestId);
			if (Type != PduType.GetBulk)
			{
				stringBuilder.AppendFormat("ErrorStatus: {0}\nError Index: {1}\n", ErrorStatus, ErrorIndex);
			}
			else
			{
				stringBuilder.AppendFormat("MaxRepeaters: {0}\nNonRepeaters: {1}\n", MaxRepetitions, NonRepeaters);
			}
			if (Type == PduType.V2Trap || Type == PduType.Inform)
			{
				stringBuilder.AppendFormat("TimeStamp: {0}\nTrapOID: {1}\n", TrapSysUpTime.ToString(), TrapObjectID.ToString());
			}
			stringBuilder.AppendFormat("VbList entries: {0}\n", VbCount);
			if (VbCount > 0)
			{
				foreach (Vb vb in VbList)
				{
					stringBuilder.AppendFormat("Vb: {0}\n", vb.ToString());
				}
			}
			return stringBuilder.ToString();
		}

		public override object Clone()
		{
			Pdu pdu = new Pdu(_vbs, Type, _requestId);
			if (Type == PduType.GetBulk)
			{
				pdu.NonRepeaters = _errorStatus;
				pdu.MaxRepetitions = _errorIndex;
			}
			else
			{
				pdu.ErrorIndex = ErrorIndex;
				pdu.ErrorStatus = ErrorStatus;
			}
			if (Type == PduType.V2Trap)
			{
				pdu.TrapObjectID.Set(TrapObjectID);
				pdu.TrapSysUpTime.Value = TrapSysUpTime.Value;
			}
			return pdu;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is Integer32)
			{
				if ((Integer32)obj == _requestId)
				{
					return true;
				}
				return false;
			}
			if (obj is Pdu)
			{
				Pdu pdu = (Pdu)obj;
				if (pdu.Type != Type)
				{
					return false;
				}
				if (pdu.RequestId != RequestId)
				{
					return false;
				}
				if (pdu.VbCount != VbCount)
				{
					return false;
				}
				foreach (Vb vb in VbList)
				{
					if (!pdu.VbList.ContainsOid(vb.Oid))
					{
						return false;
					}
				}
				foreach (Vb vb2 in pdu.VbList)
				{
					if (!VbList.ContainsOid(vb2.Oid))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (int)Type | RequestId;
		}

		public IEnumerator<Vb> GetEnumerator()
		{
			return _vbs.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)_vbs).GetEnumerator();
		}
	}
}
