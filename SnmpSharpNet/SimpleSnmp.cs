using System;
using System.Collections.Generic;
using System.Net;

namespace SnmpSharpNet
{
	public class SimpleSnmp
	{
		protected IPAddress _peerIP;

		protected string _peerName;

		protected int _peerPort;

		protected UdpTarget _target;

		protected int _timeout;

		protected int _retry;

		protected string _community;

		protected int _nonRepeaters = 0;

		protected int _maxRepetitions = 50;

		protected bool _suppressExceptions = true;

		public bool Valid
		{
			get
			{
				if (_peerIP == IPAddress.None || _peerIP == IPAddress.Any)
				{
					return false;
				}
				if (_community.Length < 1 || _community.Length > 50)
				{
					return false;
				}
				return true;
			}
		}

		public IPAddress PeerIP
		{
			get
			{
				return _peerIP;
			}
			set
			{
				_peerIP = value;
			}
		}

		public string PeerName
		{
			get
			{
				return _peerName;
			}
			set
			{
				_peerName = value;
				Resolve();
			}
		}

		public int PeerPort
		{
			get
			{
				return _peerPort;
			}
			set
			{
				_peerPort = value;
			}
		}

		public int Timeout
		{
			get
			{
				return _timeout;
			}
			set
			{
				_timeout = value;
			}
		}

		public int Retry
		{
			get
			{
				return _retry;
			}
			set
			{
				_retry = value;
			}
		}

		public string Community
		{
			get
			{
				return _community;
			}
			set
			{
				_community = value;
			}
		}

		public int NonRepeaters
		{
			get
			{
				return _nonRepeaters;
			}
			set
			{
				_nonRepeaters = value;
			}
		}

		public int MaxRepetitions
		{
			get
			{
				return _maxRepetitions;
			}
			set
			{
				_maxRepetitions = value;
			}
		}

		public bool SuppressExceptions
		{
			get
			{
				return _suppressExceptions;
			}
			set
			{
				_suppressExceptions = value;
			}
		}

		public SimpleSnmp()
		{
			_peerIP = IPAddress.Loopback;
			_peerPort = 161;
			_timeout = 2000;
			_retry = 2;
			_community = "public";
			_target = null;
			_peerName = "localhost";
		}

		public SimpleSnmp(string peerName)
			: this()
		{
			_peerName = peerName;
			Resolve();
		}

		public SimpleSnmp(string peerName, string community)
			: this(peerName)
		{
			_community = community;
		}

		public SimpleSnmp(string peerName, int peerPort, string community)
			: this(peerName, community)
		{
			_peerPort = peerPort;
		}

		public SimpleSnmp(string peerName, int peerPort, string community, int timeout, int retry)
			: this(peerName, peerPort, community)
		{
			_timeout = timeout;
			_retry = retry;
		}

		public Dictionary<Oid, AsnType> Get(SnmpVersion version, Pdu pdu)
		{
			if (!Valid)
			{
				if (!_suppressExceptions)
				{
					throw new SnmpException("SimpleSnmp class is not valid.");
				}
				return null;
			}
			if (version != 0 && version != SnmpVersion.Ver2)
			{
				if (!_suppressExceptions)
				{
					throw new SnmpInvalidVersionException("SimpleSnmp support SNMP version 1 and 2 only.");
				}
				return null;
			}
			try
			{
				_target = new UdpTarget(_peerIP, _peerPort, _timeout, _retry);
			}
			catch (Exception ex)
			{
				_target = null;
				if (!_suppressExceptions)
				{
					throw ex;
				}
			}
			if (_target == null)
			{
				return null;
			}
			try
			{
				AgentParameters agentParameters = new AgentParameters(version, new OctetString(_community));
				SnmpPacket snmpPacket = _target.Request(pdu, agentParameters);
				if (snmpPacket != null)
				{
					if (snmpPacket.Pdu.ErrorStatus == 0)
					{
						Dictionary<Oid, AsnType> dictionary = new Dictionary<Oid, AsnType>();
						foreach (Vb vb in snmpPacket.Pdu.VbList)
						{
							if (version == SnmpVersion.Ver2 && (vb.Value.Type == SnmpConstants.SMI_NOSUCHINSTANCE || vb.Value.Type == SnmpConstants.SMI_NOSUCHOBJECT))
							{
								if (!dictionary.ContainsKey(vb.Oid))
								{
									dictionary.Add(vb.Oid, new Null());
								}
								else
								{
									dictionary.Add(Oid.NullOid(), vb.Value);
								}
								continue;
							}
							if (!dictionary.ContainsKey(vb.Oid))
							{
								dictionary.Add(vb.Oid, vb.Value);
								continue;
							}
							if (dictionary[vb.Oid].Type == vb.Value.Type)
							{
								dictionary[vb.Oid] = vb.Value;
								continue;
							}
							throw new SnmpException(SnmpException.OidValueTypeChanged, $"Value type changed from {dictionary[vb.Oid].Type} to {vb.Value.Type}");
						}
						_target.Close();
						_target = null;
						return dictionary;
					}
					if (!_suppressExceptions)
					{
						throw new SnmpErrorStatusException("Agent responded with an error", snmpPacket.Pdu.ErrorStatus, snmpPacket.Pdu.ErrorIndex);
					}
				}
			}
			catch (Exception ex)
			{
				if (!_suppressExceptions)
				{
					_target.Close();
					_target = null;
					throw ex;
				}
			}
			_target.Close();
			_target = null;
			return null;
		}

		public Dictionary<Oid, AsnType> Get(SnmpVersion version, string[] oidList)
		{
			if (!Valid)
			{
				if (!_suppressExceptions)
				{
					throw new SnmpException("SimpleSnmp class is not valid.");
				}
				return null;
			}
			if (version != 0 && version != SnmpVersion.Ver2)
			{
				if (!_suppressExceptions)
				{
					throw new SnmpInvalidVersionException("SimpleSnmp support SNMP version 1 and 2 only.");
				}
				return null;
			}
			Pdu pdu = new Pdu(PduType.Get);
			foreach (string oid in oidList)
			{
				pdu.VbList.Add(oid);
			}
			return Get(version, pdu);
		}

		public Dictionary<Oid, AsnType> GetNext(SnmpVersion version, Pdu pdu)
		{
			if (!Valid)
			{
				if (!_suppressExceptions)
				{
					throw new SnmpException("SimpleSnmp class is not valid.");
				}
				return null;
			}
			if (version != 0 && version != SnmpVersion.Ver2)
			{
				if (!_suppressExceptions)
				{
					throw new SnmpInvalidVersionException("SimpleSnmp support SNMP version 1 and 2 only.");
				}
				return null;
			}
			try
			{
				_target = new UdpTarget(_peerIP, _peerPort, _timeout, _retry);
			}
			catch
			{
				_target = null;
			}
			if (_target == null)
			{
				return null;
			}
			try
			{
				AgentParameters agentParameters = new AgentParameters(version, new OctetString(_community));
				SnmpPacket snmpPacket = _target.Request(pdu, agentParameters);
				if (snmpPacket != null)
				{
					if (snmpPacket.Pdu.ErrorStatus == 0)
					{
						Dictionary<Oid, AsnType> dictionary = new Dictionary<Oid, AsnType>();
						foreach (Vb vb in snmpPacket.Pdu.VbList)
						{
							if ((version == SnmpVersion.Ver2 && vb.Value.Type == SnmpConstants.SMI_ENDOFMIBVIEW) || vb.Value.Type == SnmpConstants.SMI_NOSUCHINSTANCE || vb.Value.Type == SnmpConstants.SMI_NOSUCHOBJECT)
							{
								break;
							}
							if (dictionary.ContainsKey(vb.Oid))
							{
								if (dictionary[vb.Oid].Type != vb.Value.Type)
								{
									throw new SnmpException(SnmpException.OidValueTypeChanged, "OID value type changed for OID: " + vb.Oid.ToString());
								}
								dictionary[vb.Oid] = vb.Value;
							}
							else
							{
								dictionary.Add(vb.Oid, vb.Value);
							}
						}
						_target.Close();
						_target = null;
						return dictionary;
					}
					if (!_suppressExceptions)
					{
						throw new SnmpErrorStatusException("Agent responded with an error", snmpPacket.Pdu.ErrorStatus, snmpPacket.Pdu.ErrorIndex);
					}
				}
			}
			catch (Exception ex)
			{
				if (!_suppressExceptions)
				{
					_target.Close();
					_target = null;
					throw ex;
				}
			}
			_target.Close();
			_target = null;
			return null;
		}

		public Dictionary<Oid, AsnType> GetNext(SnmpVersion version, string[] oidList)
		{
			if (!Valid)
			{
				if (!_suppressExceptions)
				{
					throw new SnmpException("SimpleSnmp class is not valid.");
				}
				return null;
			}
			if (version != 0 && version != SnmpVersion.Ver2)
			{
				if (!_suppressExceptions)
				{
					throw new SnmpInvalidVersionException("SimpleSnmp support SNMP version 1 and 2 only.");
				}
				return null;
			}
			Pdu pdu = new Pdu(PduType.GetNext);
			foreach (string oid in oidList)
			{
				pdu.VbList.Add(oid);
			}
			return GetNext(version, pdu);
		}

		public Dictionary<Oid, AsnType> GetBulk(Pdu pdu)
		{
			if (!Valid)
			{
				if (!_suppressExceptions)
				{
					throw new SnmpException("SimpleSnmp class is not valid.");
				}
				return null;
			}
			try
			{
				pdu.NonRepeaters = _nonRepeaters;
				pdu.MaxRepetitions = _maxRepetitions;
				_target = new UdpTarget(_peerIP, _peerPort, _timeout, _retry);
			}
			catch (Exception ex)
			{
				_target = null;
				if (!_suppressExceptions)
				{
					throw ex;
				}
			}
			if (_target == null)
			{
				return null;
			}
			try
			{
				AgentParameters agentParameters = new AgentParameters(SnmpVersion.Ver2, new OctetString(_community));
				SnmpPacket snmpPacket = _target.Request(pdu, agentParameters);
				if (snmpPacket != null)
				{
					if (snmpPacket.Pdu.ErrorStatus == 0)
					{
						Dictionary<Oid, AsnType> dictionary = new Dictionary<Oid, AsnType>();
						foreach (Vb vb in snmpPacket.Pdu.VbList)
						{
							if (vb.Value.Type == SnmpConstants.SMI_ENDOFMIBVIEW || vb.Value.Type == SnmpConstants.SMI_NOSUCHINSTANCE || vb.Value.Type == SnmpConstants.SMI_NOSUCHOBJECT)
							{
								break;
							}
							if (dictionary.ContainsKey(vb.Oid))
							{
								if (dictionary[vb.Oid].Type != vb.Value.Type)
								{
									throw new SnmpException(SnmpException.OidValueTypeChanged, "OID value type changed for OID: " + vb.Oid.ToString());
								}
								dictionary[vb.Oid] = vb.Value;
							}
							else
							{
								dictionary.Add(vb.Oid, vb.Value);
							}
						}
						_target.Close();
						_target = null;
						return dictionary;
					}
					if (!_suppressExceptions)
					{
						throw new SnmpErrorStatusException("Agent responded with an error", snmpPacket.Pdu.ErrorStatus, snmpPacket.Pdu.ErrorIndex);
					}
				}
			}
			catch (Exception ex)
			{
				if (!_suppressExceptions)
				{
					_target.Close();
					_target = null;
					throw ex;
				}
			}
			_target.Close();
			_target = null;
			return null;
		}

		public Dictionary<Oid, AsnType> GetBulk(string[] oidList)
		{
			if (!Valid)
			{
				if (!_suppressExceptions)
				{
					throw new SnmpException("SimpleSnmp class is not valid.");
				}
				return null;
			}
			Pdu pdu = new Pdu(PduType.GetBulk);
			pdu.MaxRepetitions = _maxRepetitions;
			pdu.NonRepeaters = _nonRepeaters;
			foreach (string oid in oidList)
			{
				pdu.VbList.Add(oid);
			}
			return GetBulk(pdu);
		}

		public Dictionary<Oid, AsnType> Set(SnmpVersion version, Pdu pdu)
		{
			if (!Valid)
			{
				if (!_suppressExceptions)
				{
					throw new SnmpException("SimpleSnmp class is not valid.");
				}
				return null;
			}
			if (version != 0 && version != SnmpVersion.Ver2)
			{
				if (!_suppressExceptions)
				{
					throw new SnmpInvalidVersionException("SimpleSnmp support SNMP version 1 and 2 only.");
				}
				return null;
			}
			try
			{
				_target = new UdpTarget(_peerIP, _peerPort, _timeout, _retry);
			}
			catch (Exception ex)
			{
				_target = null;
				if (!_suppressExceptions)
				{
					throw ex;
				}
			}
			if (_target == null)
			{
				return null;
			}
			try
			{
				AgentParameters agentParameters = new AgentParameters(version, new OctetString(_community));
				SnmpPacket snmpPacket = _target.Request(pdu, agentParameters);
				if (snmpPacket != null)
				{
					if (snmpPacket.Pdu.ErrorStatus == 0)
					{
						Dictionary<Oid, AsnType> dictionary = new Dictionary<Oid, AsnType>();
						foreach (Vb vb in snmpPacket.Pdu.VbList)
						{
							if (dictionary.ContainsKey(vb.Oid))
							{
								if (dictionary[vb.Oid].Type != vb.Value.Type)
								{
									throw new SnmpException(SnmpException.OidValueTypeChanged, "OID value type changed for OID: " + vb.Oid.ToString());
								}
								dictionary[vb.Oid] = vb.Value;
							}
							else
							{
								dictionary.Add(vb.Oid, vb.Value);
							}
						}
						_target.Close();
						_target = null;
						return dictionary;
					}
					if (!_suppressExceptions)
					{
						throw new SnmpErrorStatusException("Agent responded with an error", snmpPacket.Pdu.ErrorStatus, snmpPacket.Pdu.ErrorIndex);
					}
				}
			}
			catch (Exception ex)
			{
				if (!_suppressExceptions)
				{
					_target.Close();
					_target = null;
					throw ex;
				}
			}
			_target.Close();
			_target = null;
			return null;
		}

		public Dictionary<Oid, AsnType> Set(SnmpVersion version, Vb[] vbs)
		{
			if (!Valid)
			{
				if (!_suppressExceptions)
				{
					throw new SnmpException("SimpleSnmp class is not valid.");
				}
				return null;
			}
			if (version != 0 && version != SnmpVersion.Ver2)
			{
				if (!_suppressExceptions)
				{
					throw new SnmpInvalidVersionException("SimpleSnmp support SNMP version 1 and 2 only.");
				}
				return null;
			}
			Pdu pdu = new Pdu(PduType.Set);
			foreach (Vb vb in vbs)
			{
				pdu.VbList.Add(vb);
			}
			return Set(version, pdu);
		}

		public Dictionary<Oid, AsnType> Walk(SnmpVersion version, string rootOid)
		{
			if (!Valid)
			{
				if (!_suppressExceptions)
				{
					throw new SnmpException("SimpleSnmp class is not valid.");
				}
				return null;
			}
			if (version != 0 && version != SnmpVersion.Ver2)
			{
				if (!_suppressExceptions)
				{
					throw new SnmpInvalidVersionException("SimpleSnmp support SNMP version 1 and 2 only.");
				}
				return null;
			}
			if (rootOid.Length < 2)
			{
				if (!_suppressExceptions)
				{
					throw new SnmpException(SnmpException.InvalidOid, "RootOid is not a valid Oid");
				}
				return null;
			}
			Oid oid = new Oid(rootOid);
			if (oid.Length <= 0)
			{
				return null;
			}
			Oid oid2 = (Oid)oid.Clone();
			Dictionary<Oid, AsnType> dictionary = new Dictionary<Oid, AsnType>();
			while (oid2 != null && oid.IsRootOf(oid2))
			{
				Dictionary<Oid, AsnType> dictionary2 = null;
				dictionary2 = ((version != 0) ? GetBulk(new string[1]
				{
					oid2.ToString()
				}) : GetNext(version, new string[1]
				{
					oid2.ToString()
				}));
				if (dictionary2 == null)
				{
					return null;
				}
				foreach (KeyValuePair<Oid, AsnType> item in dictionary2)
				{
					if (oid.IsRootOf(item.Key))
					{
						if (dictionary.ContainsKey(item.Key))
						{
							if (dictionary[item.Key].Type != item.Value.Type)
							{
								throw new SnmpException(SnmpException.OidValueTypeChanged, "OID value type changed for OID: " + item.Key.ToString());
							}
							dictionary[item.Key] = item.Value;
						}
						else
						{
							dictionary.Add(item.Key, item.Value);
						}
						oid2 = (Oid)item.Key.Clone();
						continue;
					}
					oid2 = null;
					break;
				}
			}
			return dictionary;
		}

		internal bool Resolve()
		{
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Invalid comparison between Unknown and I4
			_peerIP = IPAddress.None;
			if (_peerName.Length > 0)
			{
				if (IPAddress.TryParse(_peerName, ref _peerIP))
				{
					return true;
				}
				_peerIP = IPAddress.None;
				IPHostEntry val = null;
				try
				{
					val = Dns.GetHostEntry(_peerName);
				}
				catch (Exception ex)
				{
					if (!_suppressExceptions)
					{
						throw ex;
					}
					val = null;
				}
				if (val == null)
				{
					return false;
				}
				IPAddress[] addressList = val.get_AddressList();
				foreach (IPAddress val2 in addressList)
				{
					if ((int)val2.get_AddressFamily() == 2)
					{
						_peerIP = val2;
						break;
					}
				}
				if (_peerIP != IPAddress.None)
				{
					return true;
				}
			}
			return false;
		}
	}
}
