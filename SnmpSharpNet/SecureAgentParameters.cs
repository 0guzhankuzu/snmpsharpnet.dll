using System;

namespace SnmpSharpNet
{
	public class SecureAgentParameters : IAgentParameters
	{
		protected OctetString _engineId;

		protected Integer32 _engineBoots;

		protected Integer32 _engineTime;

		protected DateTime _engineTimeStamp;

		protected OctetString _securityName;

		protected PrivacyProtocols _privacyProtocol;

		protected AuthenticationDigests _authenticationProtocol;

		protected MutableByte _privacySecret;

		protected MutableByte _authenticationSecret;

		protected OctetString _contextEngineId;

		protected OctetString _contextName;

		protected Integer32 _maxMessageSize;

		protected bool _reportable;

		protected byte[] _privacyKey;

		protected byte[] _authenticationKey;

		public OctetString EngineId => _engineId;

		public Integer32 EngineBoots => _engineBoots;

		public Integer32 EngineTime => _engineTime;

		public OctetString SecurityName => _securityName;

		public PrivacyProtocols Privacy
		{
			get
			{
				return _privacyProtocol;
			}
			set
			{
				if (value != 0 && PrivacyProtocol.GetInstance(value) == null)
				{
					throw new SnmpPrivacyException("Invalid privacy protocol");
				}
				_privacyProtocol = value;
			}
		}

		public MutableByte PrivacySecret => _privacySecret;

		public AuthenticationDigests Authentication
		{
			get
			{
				return _authenticationProtocol;
			}
			set
			{
				if (value != 0 && SnmpSharpNet.Authentication.GetInstance(value) == null)
				{
					throw new SnmpAuthenticationException("Invalid authentication protocol.");
				}
				_authenticationProtocol = value;
			}
		}

		public MutableByte AuthenticationSecret => _authenticationSecret;

		public SnmpVersion Version => SnmpVersion.Ver3;

		public OctetString ContextEngineId => _contextEngineId;

		public OctetString ContextName => _contextName;

		public Integer32 MaxMessageSize => _maxMessageSize;

		public bool Reportable
		{
			get
			{
				return _reportable;
			}
			set
			{
				_reportable = value;
			}
		}

		public byte[] PrivacyKey
		{
			get
			{
				return _privacyKey;
			}
			set
			{
				_privacyKey = value;
			}
		}

		public byte[] AuthenticationKey
		{
			get
			{
				return _authenticationKey;
			}
			set
			{
				_authenticationKey = value;
			}
		}

		public bool HasCachedKeys
		{
			get
			{
				if (_authenticationProtocol != 0)
				{
					if (_authenticationKey != null && _authenticationKey.Length > 0)
					{
						if (_privacyProtocol == PrivacyProtocols.None)
						{
							return true;
						}
						if (_privacyKey != null && _privacyKey.Length > 0)
						{
							return true;
						}
					}
					return false;
				}
				return false;
			}
		}

		public SecureAgentParameters()
		{
			Reset();
		}

		public SecureAgentParameters(SecureAgentParameters second)
			: this()
		{
			_contextEngineId.Set(second.ContextEngineId);
			_contextName.Set(second.ContextName);
			_engineBoots.Value = second.EngineBoots.Value;
			_engineId.Set(second.EngineId);
			_engineTime.Value = second.EngineTime.Value;
			_engineTimeStamp = second.EngineTimeStamp();
			_maxMessageSize.Value = second.MaxMessageSize.Value;
			_privacyProtocol = second.Privacy;
			_privacySecret.Set(second.PrivacySecret);
			_authenticationProtocol = second.Authentication;
			_authenticationSecret.Set(second.AuthenticationSecret);
			_reportable = second.Reportable;
			_securityName.Set(second.SecurityName);
			if (second.AuthenticationKey != null)
			{
				_authenticationKey = (byte[])second.AuthenticationKey.Clone();
			}
			if (second.PrivacyKey != null)
			{
				_privacyKey = (byte[])second.PrivacyKey.Clone();
			}
		}

		internal DateTime EngineTimeStamp()
		{
			return _engineTimeStamp;
		}

		public void noAuthNoPriv(string securityName)
		{
			_securityName.Set(securityName);
			_authenticationProtocol = AuthenticationDigests.None;
			_authenticationSecret.Clear();
			_privacyProtocol = PrivacyProtocols.None;
			_privacySecret.Clear();
		}

		public void authNoPriv(string securityName, AuthenticationDigests authDigest, string authSecret)
		{
			_securityName.Set(securityName);
			_authenticationProtocol = authDigest;
			_authenticationSecret.Set(authSecret);
			_privacyProtocol = PrivacyProtocols.None;
			_privacySecret.Clear();
		}

		public void authPriv(string securityName, AuthenticationDigests authDigest, string authSecret, PrivacyProtocols privProtocol, string privSecret)
		{
			_securityName.Set(securityName);
			_authenticationProtocol = authDigest;
			_authenticationSecret.Set(authSecret);
			_privacyProtocol = privProtocol;
			_privacySecret.Set(privSecret);
		}

		public bool Valid()
		{
			if (SecurityName.Length <= 0 && (_authenticationProtocol != 0 || _privacyProtocol != 0))
			{
				return false;
			}
			if (_authenticationProtocol == AuthenticationDigests.None && _privacyProtocol != 0)
			{
				return false;
			}
			if (_authenticationProtocol != 0 && _authenticationSecret.Length <= 0)
			{
				return false;
			}
			if (_privacyProtocol != 0 && _privacySecret.Length <= 0)
			{
				return false;
			}
			if (_engineTimeStamp != DateTime.MinValue && !ValidateEngineTime())
			{
				return false;
			}
			return true;
		}

		public void InitializePacket(SnmpPacket packet)
		{
			if (packet is SnmpV3Packet)
			{
				SnmpV3Packet snmpV3Packet = (SnmpV3Packet)packet;
				bool flag = ((_authenticationProtocol != 0) ? true : false);
				bool flag2 = ((_privacyProtocol != 0) ? true : false);
				if (flag && flag2)
				{
					snmpV3Packet.authPriv(_securityName, _authenticationSecret, _authenticationProtocol, _privacySecret, _privacyProtocol);
				}
				else if (flag && !flag2)
				{
					snmpV3Packet.authNoPriv(_securityName, _authenticationSecret, _authenticationProtocol);
				}
				else
				{
					snmpV3Packet.NoAuthNoPriv(_securityName);
				}
				snmpV3Packet.USM.EngineId.Set(_engineId);
				snmpV3Packet.USM.EngineBoots = _engineBoots.Value;
				snmpV3Packet.USM.EngineTime = GetCurrentEngineTime();
				snmpV3Packet.MaxMessageSize = _maxMessageSize.Value;
				snmpV3Packet.MsgFlags.Reportable = _reportable;
				if (_contextEngineId.Length > 0)
				{
					snmpV3Packet.ScopedPdu.ContextEngineId.Set(_contextEngineId);
				}
				else
				{
					snmpV3Packet.ScopedPdu.ContextEngineId.Set(_engineId);
				}
				if (_contextName.Length > 0)
				{
					snmpV3Packet.ScopedPdu.ContextName.Set(_contextName);
				}
				else
				{
					snmpV3Packet.ScopedPdu.ContextName.Reset();
				}
				return;
			}
			throw new SnmpInvalidVersionException("Invalid SNMP version.");
		}

		public void UpdateValues(SnmpPacket packet)
		{
			if (packet is SnmpV3Packet)
			{
				SnmpV3Packet snmpV3Packet = (SnmpV3Packet)packet;
				_authenticationProtocol = snmpV3Packet.USM.Authentication;
				_privacyProtocol = snmpV3Packet.USM.Privacy;
				_authenticationSecret.Set(snmpV3Packet.USM.AuthenticationSecret);
				_privacySecret.Set(snmpV3Packet.USM.PrivacySecret);
				_securityName.Set(snmpV3Packet.USM.SecurityName);
				if (snmpV3Packet.MaxMessageSize < _maxMessageSize.Value)
				{
					_maxMessageSize.Value = snmpV3Packet.MaxMessageSize;
				}
				UpdateDiscoveryValues(snmpV3Packet);
				return;
			}
			throw new SnmpInvalidVersionException("Invalid SNMP version.");
		}

		public void UpdateDiscoveryValues(SnmpPacket packet)
		{
			if (packet is SnmpV3Packet)
			{
				SnmpV3Packet snmpV3Packet = (SnmpV3Packet)packet;
				_engineId.Set(snmpV3Packet.USM.EngineId);
				_engineTime.Value = snmpV3Packet.USM.EngineTime;
				_engineBoots.Value = snmpV3Packet.USM.EngineBoots;
				UpdateTimeStamp();
				_contextEngineId.Set(snmpV3Packet.ScopedPdu.ContextEngineId);
				_contextName.Set(snmpV3Packet.ScopedPdu.ContextName);
				return;
			}
			throw new SnmpInvalidVersionException("Invalid SNMP version.");
		}

		public void UpdateTimeStamp()
		{
			_engineTimeStamp = DateTime.UtcNow;
		}

		public bool ValidateEngineTime()
		{
			if (_engineTimeStamp == DateTime.MinValue)
			{
				return false;
			}
			if (DateTime.UtcNow.Subtract(_engineTimeStamp).TotalSeconds >= 1500.0)
			{
				return false;
			}
			return true;
		}

		public int GetCurrentEngineTime()
		{
			if (!ValidateEngineTime())
			{
				return 0;
			}
			TimeSpan timeSpan = DateTime.UtcNow.Subtract(_engineTimeStamp);
			return Convert.ToInt32((double)_engineTime.Value + timeSpan.TotalSeconds + 1.0);
		}

		public bool ValidateIncomingPacket(SnmpV3Packet packet)
		{
			if (packet.Pdu.Type == PduType.Report)
			{
				if (!_reportable)
				{
					throw new SnmpException(SnmpException.ReportOnNoReports, "Unexpected report packet received.");
				}
				if (!packet.MsgFlags.Authentication && packet.MsgFlags.Privacy)
				{
					throw new SnmpException(SnmpException.UnsupportedNoAuthPriv, "Authentication and privacy combination is not supported.");
				}
			}
			else
			{
				if (packet.USM.EngineId != _engineId)
				{
					throw new SnmpException(SnmpException.InvalidAuthoritativeEngineId, "EngineId mismatch.");
				}
				if (packet.USM.Authentication != _authenticationProtocol || packet.USM.Privacy != _privacyProtocol)
				{
					throw new SnmpException("Agent parameters updated after request was made.");
				}
				if (packet.USM.Authentication != 0 && packet.USM.AuthenticationSecret != _authenticationSecret)
				{
					throw new SnmpAuthenticationException("Authentication secret in the packet class does not match the IAgentParameter secret.");
				}
				if (packet.USM.Privacy != 0 && packet.USM.PrivacySecret != _privacySecret)
				{
					throw new SnmpPrivacyException("Privacy secret in the packet class does not match the IAgentParameters secret.");
				}
				if (packet.USM.SecurityName != _securityName)
				{
					throw new SnmpException(SnmpException.InvalidSecurityName, "Security name mismatch.");
				}
			}
			return true;
		}

		public void ResetKeys()
		{
			_privacyKey = null;
			_authenticationKey = null;
		}

		public void Reset()
		{
			_engineId = new OctetString();
			_engineBoots = new Integer32();
			_engineTime = new Integer32();
			_engineTimeStamp = DateTime.MinValue;
			_privacyProtocol = PrivacyProtocols.None;
			_authenticationProtocol = AuthenticationDigests.None;
			_privacySecret = new MutableByte();
			_authenticationSecret = new MutableByte();
			_contextEngineId = new OctetString();
			_contextName = new OctetString();
			_securityName = new OctetString();
			_maxMessageSize = new Integer32(65536);
			_reportable = true;
			_privacyKey = null;
			_authenticationKey = null;
		}

		public object Clone()
		{
			return new SecureAgentParameters(this);
		}

		public void BuildCachedSecurityKeys()
		{
			_authenticationKey = (_privacyKey = null);
			if (_engineId == null || _engineId.Length <= 0 || _authenticationSecret == null || _authenticationSecret.Length <= 0 || _authenticationProtocol == AuthenticationDigests.None)
			{
				return;
			}
			IAuthenticationDigest instance = SnmpSharpNet.Authentication.GetInstance(_authenticationProtocol);
			if (instance == null)
			{
				return;
			}
			_authenticationKey = instance.PasswordToKey(_authenticationSecret, _engineId);
			if (_privacyProtocol != 0 && _privacySecret != null && _privacySecret.Length > 0)
			{
				IPrivacyProtocol instance2 = PrivacyProtocol.GetInstance(_privacyProtocol);
				if (instance2 != null)
				{
					_privacyKey = instance2.PasswordToKey(_privacySecret, _engineId, instance);
				}
			}
		}
	}
}
