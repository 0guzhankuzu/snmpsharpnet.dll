using System;

namespace SnmpSharpNet
{
	public class UserSecurityModel : AsnType, ICloneable
	{
		protected OctetString _engineId;

		protected Integer32 _engineBoots;

		protected Integer32 _engineTime;

		protected OctetString _securityName;

		protected AuthenticationDigests _authentication;

		protected MutableByte _authenticationSecret;

		private OctetString _authenticationParameters;

		protected PrivacyProtocols _privacy;

		protected MutableByte _privacySecret;

		protected OctetString _privacyParameters;

		public OctetString EngineId => _engineId;

		public int EngineBoots
		{
			get
			{
				return _engineBoots.Value;
			}
			set
			{
				_engineBoots.Value = value;
			}
		}

		public int EngineTime
		{
			get
			{
				return _engineTime.Value;
			}
			set
			{
				_engineTime.Value = value;
			}
		}

		public OctetString AuthenticationParameters => _authenticationParameters;

		public AuthenticationDigests Authentication
		{
			get
			{
				return _authentication;
			}
			set
			{
				_authentication = value;
			}
		}

		public OctetString SecurityName => _securityName;

		public MutableByte AuthenticationSecret => _authenticationSecret;

		public MutableByte PrivacySecret => _privacySecret;

		public PrivacyProtocols Privacy
		{
			get
			{
				return _privacy;
			}
			set
			{
				_privacy = value;
			}
		}

		public OctetString PrivacyParameters => _privacyParameters;

		public void SetEngineTime(int engineTime, int engineBoots)
		{
			_engineTime.Value = engineTime;
			_engineBoots.Value = engineBoots;
		}

		public UserSecurityModel()
		{
			_asnType = 3;
			_engineId = new OctetString();
			_engineBoots = new Integer32();
			_engineTime = new Integer32();
			_authentication = AuthenticationDigests.None;
			_securityName = new OctetString();
			_authenticationSecret = new MutableByte();
			_authenticationParameters = new OctetString();
			_privacySecret = new MutableByte();
			_privacy = PrivacyProtocols.None;
			_privacyParameters = new OctetString();
		}

		public UserSecurityModel(UserSecurityModel value)
			: this()
		{
			_engineId.Set(value.EngineId);
			_engineBoots.Value = value.EngineBoots;
			_engineTime.Value = value.EngineTime;
			_securityName.Set(value.SecurityName);
			_authenticationParameters = new OctetString();
			_privacySecret = new MutableByte();
			_privacy = PrivacyProtocols.None;
			_privacyParameters = new OctetString();
		}

		public void Authenticate(ref MutableByte wholePacket)
		{
			if (_authentication != 0)
			{
				IAuthenticationDigest instance = SnmpSharpNet.Authentication.GetInstance(_authentication);
				byte[] data = instance.authenticate(AuthenticationSecret, EngineId.ToArray(), wholePacket);
				_authenticationParameters = new OctetString(data);
			}
		}

		public void Authenticate(byte[] authKey, ref MutableByte wholePacket)
		{
			IAuthenticationDigest instance = SnmpSharpNet.Authentication.GetInstance(_authentication);
			byte[] data = instance.authenticate(authKey, wholePacket);
			_authenticationParameters = new OctetString(data);
		}

		public bool IsAuthentic(MutableByte wholePacket)
		{
			if (_authentication != 0)
			{
				IAuthenticationDigest instance = SnmpSharpNet.Authentication.GetInstance(_authentication);
				if (instance != null)
				{
					return instance.authenticateIncomingMsg(AuthenticationSecret, _engineId, _authenticationParameters, wholePacket);
				}
			}
			return false;
		}

		public bool IsAuthentic(byte[] authKey, MutableByte wholePacket)
		{
			if (_authentication != 0)
			{
				IAuthenticationDigest instance = SnmpSharpNet.Authentication.GetInstance(_authentication);
				if (instance != null)
				{
					return instance.authenticateIncomingMsg(authKey, _authenticationParameters, wholePacket);
				}
			}
			return false;
		}

		public override void encode(MutableByte buffer)
		{
			MutableByte mutableByte = new MutableByte();
			_engineId.encode(mutableByte);
			_engineBoots.encode(mutableByte);
			_engineTime.encode(mutableByte);
			_securityName.encode(mutableByte);
			if (_authentication != 0)
			{
				if (_authenticationParameters.Length <= 0)
				{
					OctetString authenticationParameters = _authenticationParameters;
					byte[] data = new byte[12];
					authenticationParameters.Set(data);
				}
			}
			else
			{
				_authenticationParameters.Reset();
			}
			_authenticationParameters.encode(mutableByte);
			if (_privacy != 0)
			{
				if (_privacyParameters.Length <= 0)
				{
					IPrivacyProtocol instance = PrivacyProtocol.GetInstance(_privacy);
					if (instance == null)
					{
						throw new SnmpException(SnmpException.UnsupportedPrivacyProtocol, "Unrecognized privacy protocol specified.");
					}
					byte[] array = new byte[instance.PrivacyParametersLength];
					for (int i = 0; i < instance.PrivacyParametersLength; i++)
					{
						array[i] = 0;
					}
					_privacyParameters.Set(array);
				}
			}
			else
			{
				_privacyParameters.Reset();
			}
			_privacyParameters.encode(mutableByte);
			MutableByte mutableByte2 = new MutableByte();
			AsnType.BuildHeader(mutableByte2, SnmpConstants.SMI_SEQUENCE, mutableByte.Length);
			mutableByte2.Append(mutableByte);
			AsnType.BuildHeader(buffer, AsnType.OCTETSTRING, mutableByte2.Length);
			buffer.Append(mutableByte2);
		}

		public override int decode(byte[] buffer, int offset)
		{
			byte b = AsnType.ParseHeader(buffer, ref offset, out var length);
			if (b != AsnType.OCTETSTRING)
			{
				throw new SnmpDecodingException("Invalid value type found while looking for USM header.");
			}
			if (length > buffer.Length - offset)
			{
				throw new OverflowException("Packet too small");
			}
			b = AsnType.ParseHeader(buffer, ref offset, out length);
			if (b != SnmpConstants.SMI_SEQUENCE)
			{
				throw new SnmpDecodingException("Sequence missing from USM header.");
			}
			if (length > buffer.Length - offset)
			{
				throw new OverflowException("Packet too small");
			}
			offset = _engineId.decode(buffer, offset);
			offset = _engineBoots.decode(buffer, offset);
			offset = _engineTime.decode(buffer, offset);
			offset = _securityName.decode(buffer, offset);
			int num = offset;
			offset = _authenticationParameters.decode(buffer, offset);
			if (_authenticationParameters.Length > 0)
			{
				num += 2;
				for (int i = 0; i < _authenticationParameters.Length; i++)
				{
					buffer[num + i] = 0;
				}
			}
			offset = _privacyParameters.decode(buffer, offset);
			return offset;
		}

		public override object Clone()
		{
			return new UserSecurityModel(this);
		}

		public bool Valid()
		{
			if ((_authentication != 0 || _privacy != 0) && _securityName.Length <= 0)
			{
				return false;
			}
			if (_authentication == AuthenticationDigests.None && _privacy != 0)
			{
				return false;
			}
			if (_authentication != 0 && _authenticationSecret.Length <= 0)
			{
				return false;
			}
			if (_privacy != 0 && _privacySecret.Length <= 0)
			{
				return false;
			}
			return true;
		}

		public void Reset()
		{
			_asnType = 3;
			_engineId = new OctetString();
			_engineBoots = new Integer32();
			_engineTime = new Integer32();
			_authentication = AuthenticationDigests.None;
			_securityName = new OctetString();
			_authenticationSecret = new MutableByte();
			_authenticationParameters = new OctetString();
			_privacySecret = new MutableByte();
			_privacy = PrivacyProtocols.None;
			_privacyParameters = new OctetString();
		}
	}
}
