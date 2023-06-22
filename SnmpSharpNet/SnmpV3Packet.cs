using System;

namespace SnmpSharpNet
{
	public class SnmpV3Packet : SnmpPacket
	{
		private Integer32 _messageId;

		private Integer32 _maxMessageSize;

		private MsgFlags _msgFlags;

		protected Integer32 _securityModel;

		private UserSecurityModel _userSecurityModel;

		private ScopedPdu _scopedPdu;

		public int MessageId
		{
			get
			{
				return _messageId.Value;
			}
			set
			{
				_messageId.Value = value;
			}
		}

		public int MaxMessageSize
		{
			get
			{
				return _maxMessageSize.Value;
			}
			set
			{
				_maxMessageSize.Value = value;
			}
		}

		public MsgFlags MsgFlags => _msgFlags;

		public UserSecurityModel USM => _userSecurityModel;

		public override Pdu Pdu => _scopedPdu;

		public ScopedPdu ScopedPdu => _scopedPdu;

		public bool IsReportable
		{
			get
			{
				return _msgFlags.Reportable;
			}
			set
			{
				_msgFlags.Reportable = value;
			}
		}

		public bool IsDiscoveryPacket
		{
			get
			{
				if (USM.EngineId.Length == 0 && USM.EngineTime == 0 && USM.EngineBoots == 0)
				{
					return true;
				}
				return false;
			}
		}

		public SnmpV3Packet()
			: base(SnmpVersion.Ver3)
		{
			_messageId = new Integer32();
			_maxMessageSize = new Integer32(65536);
			_msgFlags = new MsgFlags();
			_msgFlags.Reportable = true;
			_securityModel = new Integer32();
			_userSecurityModel = new UserSecurityModel();
			_scopedPdu = new ScopedPdu();
		}

		public SnmpV3Packet(ScopedPdu pdu)
			: this()
		{
			if (pdu != null)
			{
				_scopedPdu = pdu;
			}
		}

		public SnmpV3Packet(SecureAgentParameters param)
			: this()
		{
			param?.InitializePacket(this);
		}

		public SnmpV3Packet(SecureAgentParameters param, ScopedPdu pdu)
			: this(param)
		{
			if (pdu != null)
			{
				_scopedPdu = pdu;
			}
		}

		public void NoAuthNoPriv()
		{
			_msgFlags.Authentication = false;
			_msgFlags.Privacy = false;
			_userSecurityModel.SecurityName.Set("initial");
		}

		public void NoAuthNoPriv(byte[] userName)
		{
			_msgFlags.Authentication = false;
			_msgFlags.Privacy = false;
			_userSecurityModel.SecurityName.Set(userName);
		}

		public void authNoPriv(byte[] userName, byte[] authenticationPassword, AuthenticationDigests authenticationProtocol)
		{
			NoAuthNoPriv(userName);
			_msgFlags.Authentication = true;
			_userSecurityModel.Authentication = authenticationProtocol;
			_userSecurityModel.AuthenticationSecret.Set(authenticationPassword);
			_msgFlags.Privacy = false;
		}

		public void authPriv(byte[] userName, byte[] authenticationPassword, AuthenticationDigests authenticationProtocol, byte[] privacyPassword, PrivacyProtocols privacyProtocol)
		{
			NoAuthNoPriv(userName);
			_msgFlags.Authentication = true;
			_userSecurityModel.AuthenticationSecret.Set(authenticationPassword);
			_userSecurityModel.Authentication = authenticationProtocol;
			_msgFlags.Privacy = true;
			_userSecurityModel.PrivacySecret.Set(privacyPassword);
			_userSecurityModel.Privacy = privacyProtocol;
		}

		public void SetEngineTime(int engineBoots, int engineTime)
		{
			_userSecurityModel.EngineBoots = engineBoots;
			_userSecurityModel.EngineTime = engineTime;
		}

		public void SetEngineId(byte[] engineId)
		{
			_userSecurityModel.EngineId.Set(engineId);
		}

		public UserSecurityModel GetUSM(byte[] berBuffer, int length)
		{
			MutableByte mutableByte = new MutableByte(berBuffer, length);
			int num = 0;
			num = base.decode(mutableByte, length);
			if ((int)_protocolVersion != 3)
			{
				throw new SnmpInvalidVersionException("Expecting SNMP version 3.");
			}
			int length2 = 0;
			byte b = AsnType.ParseHeader(mutableByte, ref num, out length2);
			if (b != SnmpConstants.SMI_SEQUENCE)
			{
				throw new SnmpDecodingException("Invalid sequence type when decoding global message data sequence.");
			}
			if (length2 > mutableByte.Length - num)
			{
				throw new OverflowException("Packet is too small to contain the data described in the header.");
			}
			num = _messageId.decode(mutableByte, num);
			num = _maxMessageSize.decode(mutableByte, num);
			num = _msgFlags.decode(mutableByte, num);
			if (!_msgFlags.Authentication && _msgFlags.Privacy)
			{
				throw new SnmpException(SnmpException.UnsupportedNoAuthPriv, "SNMP version 3 noAuthPriv security combination is not supported.");
			}
			num = _securityModel.decode(mutableByte, num);
			if (_securityModel.Value != _userSecurityModel.Type)
			{
				throw new SnmpException(SnmpException.UnsupportedSecurityModel, "Class only support SNMP Version 3 User Security Model.");
			}
			num = _userSecurityModel.decode(mutableByte, num);
			return _userSecurityModel;
		}

		public override int decode(byte[] berBuffer, int length)
		{
			byte[] privKey = null;
			byte[] authKey = null;
			if (_msgFlags.Authentication && _userSecurityModel.EngineId.Length > 0)
			{
				IAuthenticationDigest instance = Authentication.GetInstance(_userSecurityModel.Authentication);
				if (instance == null)
				{
					throw new SnmpException(SnmpException.UnsupportedNoAuthPriv, "Invalid authentication protocol.");
				}
				authKey = instance.PasswordToKey(_userSecurityModel.AuthenticationSecret, _userSecurityModel.EngineId);
				if (_msgFlags.Privacy && _userSecurityModel.EngineId.Length > 0)
				{
					IPrivacyProtocol instance2 = PrivacyProtocol.GetInstance(_userSecurityModel.Privacy);
					if (instance2 == null)
					{
						throw new SnmpException(SnmpException.UnsupportedPrivacyProtocol, "Specified privacy protocol is not supported.");
					}
					privKey = instance2.PasswordToKey(_userSecurityModel.PrivacySecret, _userSecurityModel.EngineId, instance);
				}
			}
			return decode(berBuffer, length, authKey, privKey);
		}

		public int decode(byte[] berBuffer, int length, byte[] authKey, byte[] privKey)
		{
			MutableByte mutableByte = new MutableByte(berBuffer, length);
			int num = 0;
			num = base.decode(mutableByte, length);
			if ((int)_protocolVersion != 3)
			{
				throw new SnmpInvalidVersionException("Expecting SNMP version 3.");
			}
			int length2 = 0;
			byte b = AsnType.ParseHeader(mutableByte, ref num, out length2);
			if (b != SnmpConstants.SMI_SEQUENCE)
			{
				throw new SnmpDecodingException("Invalid sequence type in global message data sequence.");
			}
			if (length2 > mutableByte.Length - num)
			{
				throw new OverflowException("Packet is too small to contain the data described in the header.");
			}
			num = _messageId.decode(mutableByte, num);
			num = _maxMessageSize.decode(mutableByte, num);
			num = _msgFlags.decode(mutableByte, num);
			if (!_msgFlags.Authentication && _msgFlags.Privacy)
			{
				throw new SnmpException(SnmpException.UnsupportedNoAuthPriv, "SNMP version 3 noAuthPriv security combination is not supported.");
			}
			num = _securityModel.decode(mutableByte, num);
			if (_securityModel.Value != _userSecurityModel.Type)
			{
				throw new SnmpException(SnmpException.UnsupportedSecurityModel, "Class only support SNMP Version 3 User Security Model.");
			}
			num = _userSecurityModel.decode(mutableByte, num);
			if (_msgFlags.Authentication && _userSecurityModel.EngineId.Length > 0)
			{
				if (_userSecurityModel.AuthenticationParameters.Length != 12)
				{
					throw new SnmpAuthenticationException("Invalid authentication parameter field length.");
				}
				if (!_userSecurityModel.IsAuthentic(authKey, mutableByte))
				{
					throw new SnmpAuthenticationException("Authentication of the incoming packet failed.");
				}
			}
			if (_msgFlags.Privacy && _userSecurityModel.EngineId.Length > 0)
			{
				IPrivacyProtocol instance = PrivacyProtocol.GetInstance(_userSecurityModel.Privacy);
				if (instance == null)
				{
					throw new SnmpException(SnmpException.UnsupportedPrivacyProtocol, "Privacy protocol requested is not supported.");
				}
				if (_userSecurityModel.PrivacyParameters.Length != instance.PrivacyParametersLength)
				{
					throw new SnmpException(SnmpException.InvalidPrivacyParameterLength, "Invalid privacy parameters field length.");
				}
				OctetString octetString = new OctetString();
				num = octetString.decode(mutableByte, num);
				byte[] buffer = instance.Decrypt(octetString, 0, octetString.Length, privKey, _userSecurityModel.EngineBoots, _userSecurityModel.EngineTime, _userSecurityModel.PrivacyParameters);
				int offset = 0;
				return _scopedPdu.decode(buffer, offset);
			}
			return _scopedPdu.decode(mutableByte, num);
		}

		public override byte[] encode()
		{
			byte[] privKey = null;
			byte[] authKey = null;
			if (_msgFlags.Authentication && _userSecurityModel.EngineId.Length > 0)
			{
				IAuthenticationDigest instance = Authentication.GetInstance(_userSecurityModel.Authentication);
				if (instance == null)
				{
					throw new SnmpException(SnmpException.UnsupportedNoAuthPriv, "Invalid authentication protocol.");
				}
				authKey = instance.PasswordToKey(_userSecurityModel.AuthenticationSecret, _userSecurityModel.EngineId);
				if (_msgFlags.Privacy && _userSecurityModel.EngineId.Length > 0)
				{
					IPrivacyProtocol instance2 = PrivacyProtocol.GetInstance(_userSecurityModel.Privacy);
					if (instance2 == null)
					{
						throw new SnmpException(SnmpException.UnsupportedPrivacyProtocol, "Specified privacy protocol is not supported.");
					}
					privKey = instance2.PasswordToKey(_userSecurityModel.PrivacySecret, _userSecurityModel.EngineId, instance);
				}
			}
			return encode(authKey, privKey);
		}

		public byte[] encode(byte[] authKey, byte[] privKey)
		{
			MutableByte wholePacket = new MutableByte();
			MutableByte mutableByte = new MutableByte();
			if (_messageId.Value == 0)
			{
				Random random = new Random();
				_messageId.Value = random.Next(1, int.MaxValue);
			}
			_messageId.encode(mutableByte);
			_maxMessageSize.encode(mutableByte);
			_msgFlags.encode(mutableByte);
			_securityModel.Value = _userSecurityModel.Type;
			_securityModel.encode(mutableByte);
			AsnType.BuildHeader(wholePacket, SnmpConstants.SMI_SEQUENCE, mutableByte.Length);
			wholePacket.Append(mutableByte);
			MutableByte mutableByte2 = new MutableByte(wholePacket);
			OctetString octetString = new OctetString();
			bool privacy = _msgFlags.Privacy;
			bool authentication = _msgFlags.Authentication;
			bool reportable = _msgFlags.Reportable;
			if (_userSecurityModel.EngineId.Length <= 0)
			{
				octetString.Set(_userSecurityModel.SecurityName);
				_userSecurityModel.SecurityName.Reset();
				_msgFlags.Authentication = false;
				_msgFlags.Privacy = false;
				_msgFlags.Reportable = true;
			}
			_userSecurityModel.encode(wholePacket);
			if (_userSecurityModel.EngineId.Length <= 0)
			{
				_userSecurityModel.SecurityName.Set(octetString);
				_msgFlags.Authentication = authentication;
				_msgFlags.Privacy = privacy;
				_msgFlags.Reportable = reportable;
			}
			MutableByte mutableByte3 = new MutableByte();
			if (_msgFlags.Privacy && _userSecurityModel.EngineId.Length > 0)
			{
				IPrivacyProtocol instance = PrivacyProtocol.GetInstance(_userSecurityModel.Privacy);
				if (instance == null)
				{
					throw new SnmpException(SnmpException.UnsupportedPrivacyProtocol, "Specified privacy protocol is not supported.");
				}
				MutableByte mutableByte4 = new MutableByte();
				_scopedPdu.encode(mutableByte4);
				byte[] privacyParameters = null;
				IAuthenticationDigest instance2 = Authentication.GetInstance(_userSecurityModel.Authentication);
				if (instance2 == null)
				{
					throw new SnmpException(SnmpException.UnsupportedNoAuthPriv, "Invalid authentication protocol. noAuthPriv mode not supported.");
				}
				byte[] data = instance.Encrypt(mutableByte4, 0, mutableByte4.Length, privKey, _userSecurityModel.EngineBoots, _userSecurityModel.EngineTime, out privacyParameters, instance2);
				_userSecurityModel.PrivacyParameters.Set(privacyParameters);
				OctetString octetString2 = new OctetString(data);
				octetString2.encode(mutableByte3);
				wholePacket.Reset();
				wholePacket.Set(mutableByte2);
				_userSecurityModel.encode(wholePacket);
				int length = mutableByte3.Length;
				wholePacket.Append(mutableByte3);
				if (_maxMessageSize.Value != 0 && mutableByte3.Length - length > (int)_maxMessageSize)
				{
					throw new SnmpException(SnmpException.MaximumMessageSizeExceeded, "ScopedPdu exceeds maximum message size.");
				}
			}
			else
			{
				_scopedPdu.encode(mutableByte3);
				wholePacket.Append(mutableByte3);
			}
			base.encode(wholePacket);
			if (_msgFlags.Authentication && _userSecurityModel.EngineId.Length > 0)
			{
				_userSecurityModel.Authenticate(authKey, ref wholePacket);
				_userSecurityModel.encode(mutableByte2);
				mutableByte2.Append(mutableByte3);
				base.encode(mutableByte2);
				wholePacket = mutableByte2;
			}
			return wholePacket;
		}

		public byte[] GenerateAuthenticationKey()
		{
			if (_userSecurityModel.EngineId == null || _userSecurityModel.EngineId.Length <= 0)
			{
				return null;
			}
			if (_userSecurityModel.AuthenticationSecret == null || _userSecurityModel.AuthenticationSecret.Length <= 0)
			{
				return null;
			}
			if (_userSecurityModel.Authentication != 0)
			{
				IAuthenticationDigest instance = Authentication.GetInstance(_userSecurityModel.Authentication);
				if (instance != null)
				{
					return instance.PasswordToKey(_userSecurityModel.AuthenticationSecret, _userSecurityModel.EngineId);
				}
			}
			return null;
		}

		public byte[] GeneratePrivacyKey()
		{
			if (_userSecurityModel.Authentication == AuthenticationDigests.None)
			{
				return null;
			}
			if (_userSecurityModel.Privacy == PrivacyProtocols.None)
			{
				return null;
			}
			if (_userSecurityModel.PrivacySecret == null || _userSecurityModel.PrivacySecret.Length <= 0)
			{
				return null;
			}
			IAuthenticationDigest instance = Authentication.GetInstance(_userSecurityModel.Authentication);
			if (instance != null)
			{
				IPrivacyProtocol instance2 = PrivacyProtocol.GetInstance(_userSecurityModel.Privacy);
				if (instance2 != null)
				{
					return instance2.PasswordToKey(_userSecurityModel.PrivacySecret, _userSecurityModel.EngineId, instance);
				}
			}
			return null;
		}

		public static SnmpV3Packet DiscoveryRequest()
		{
			return new SnmpV3Packet(new ScopedPdu());
		}

		public static SnmpV3Packet DiscoveryResponse(int messageId, int requestId, OctetString engineId, int engineBoots, int engineTime, int unknownEngineIdCount)
		{
			SnmpV3Packet snmpV3Packet = new SnmpV3Packet();
			snmpV3Packet.Pdu.Type = PduType.Report;
			snmpV3Packet.Pdu.RequestId = requestId;
			snmpV3Packet.Pdu.VbList.Add(SnmpConstants.usmStatsUnknownEngineIDs, new Integer32(unknownEngineIdCount));
			snmpV3Packet.MsgFlags.Reportable = false;
			snmpV3Packet.SetEngineId(engineId);
			snmpV3Packet.MessageId = messageId;
			snmpV3Packet.USM.EngineBoots = engineBoots;
			snmpV3Packet.USM.EngineTime = engineTime;
			return snmpV3Packet;
		}

		public SnmpV3Packet BuildInformResponse()
		{
			return BuildInformResponse(this);
		}

		public static SnmpV3Packet BuildInformResponse(SnmpV3Packet informPacket)
		{
			if (informPacket.Version != SnmpVersion.Ver3)
			{
				throw new SnmpInvalidVersionException("INFORM packet can only be parsed from an SNMP version 3 packet.");
			}
			if (informPacket.Pdu.Type != PduType.Inform)
			{
				throw new SnmpInvalidPduTypeException("Inform response can only be built for INFORM packets.");
			}
			SnmpV3Packet snmpV3Packet = new SnmpV3Packet(informPacket.ScopedPdu);
			snmpV3Packet.MessageId = informPacket.MessageId;
			snmpV3Packet.USM.SecurityName.Set(informPacket.USM.SecurityName);
			snmpV3Packet.USM.EngineTime = informPacket.USM.EngineTime;
			snmpV3Packet.USM.EngineBoots = informPacket.USM.EngineBoots;
			snmpV3Packet.USM.EngineId.Set(informPacket.USM.EngineId);
			snmpV3Packet.USM.Authentication = informPacket.USM.Authentication;
			if (snmpV3Packet.USM.Authentication != 0)
			{
				snmpV3Packet.USM.AuthenticationSecret.Set(informPacket.USM.AuthenticationSecret);
			}
			else
			{
				snmpV3Packet.USM.AuthenticationSecret.Reset();
			}
			snmpV3Packet.USM.Privacy = informPacket.USM.Privacy;
			if (snmpV3Packet.USM.Privacy != 0)
			{
				snmpV3Packet.USM.PrivacySecret.Set(informPacket.USM.PrivacySecret);
			}
			else
			{
				snmpV3Packet.USM.PrivacySecret.Reset();
			}
			snmpV3Packet.MsgFlags.Authentication = informPacket.MsgFlags.Authentication;
			snmpV3Packet.MsgFlags.Privacy = informPacket.MsgFlags.Privacy;
			snmpV3Packet.MsgFlags.Reportable = informPacket.MsgFlags.Reportable;
			snmpV3Packet.ScopedPdu.ContextEngineId.Set(informPacket.ScopedPdu.ContextEngineId);
			snmpV3Packet.ScopedPdu.ContextName.Set(informPacket.ScopedPdu.ContextName);
			snmpV3Packet.Pdu.Type = PduType.Response;
			snmpV3Packet.Pdu.TrapObjectID.Set(informPacket.Pdu.TrapObjectID);
			snmpV3Packet.Pdu.TrapSysUpTime.Value = informPacket.Pdu.TrapSysUpTime.Value;
			snmpV3Packet.Pdu.RequestId = informPacket.Pdu.RequestId;
			return snmpV3Packet;
		}
	}
}
