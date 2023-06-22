using System;

namespace SnmpSharpNet
{
	public class MsgFlags : AsnType, ICloneable
	{
		public static byte FLAG_AUTH = 1;

		public static byte FLAG_PRIV = 2;

		public static byte FLAG_REPORTABLE = 4;

		protected bool _authenticationFlag;

		protected bool _privacyFlag;

		protected bool _reportableFlag;

		public bool Authentication
		{
			get
			{
				return _authenticationFlag;
			}
			set
			{
				_authenticationFlag = value;
			}
		}

		public bool Privacy
		{
			get
			{
				return _privacyFlag;
			}
			set
			{
				_privacyFlag = value;
			}
		}

		public bool Reportable
		{
			get
			{
				return _reportableFlag;
			}
			set
			{
				_reportableFlag = value;
			}
		}

		public MsgFlags()
		{
			_authenticationFlag = (_privacyFlag = (_reportableFlag = false));
		}

		public MsgFlags(bool authentication, bool privacy, bool reportable)
		{
			_authenticationFlag = authentication;
			_privacyFlag = privacy;
			_reportableFlag = reportable;
		}

		public override void encode(MutableByte buffer)
		{
			byte b = 0;
			if (_authenticationFlag)
			{
				b = (byte)(b | FLAG_AUTH);
			}
			if (_privacyFlag)
			{
				b = (byte)(b | FLAG_PRIV);
			}
			if (_reportableFlag)
			{
				b = (byte)(b | FLAG_REPORTABLE);
			}
			OctetString octetString = new OctetString(b);
			octetString.encode(buffer);
		}

		public override int decode(byte[] buffer, int offset)
		{
			_authenticationFlag = false;
			_privacyFlag = false;
			_reportableFlag = false;
			OctetString octetString = new OctetString();
			offset = octetString.decode(buffer, offset);
			if (octetString.Length > 0)
			{
				if ((octetString[0] & FLAG_AUTH) != 0)
				{
					_authenticationFlag = true;
				}
				if ((octetString[0] & FLAG_PRIV) != 0)
				{
					_privacyFlag = true;
				}
				if ((octetString[0] & FLAG_REPORTABLE) != 0)
				{
					_reportableFlag = true;
				}
				return offset;
			}
			throw new SnmpDecodingException("Invalid SNMPv3 flag field.");
		}

		public override object Clone()
		{
			return new MsgFlags(_authenticationFlag, _privacyFlag, _reportableFlag);
		}

		public override string ToString()
		{
			return $"Reportable {_reportableFlag.ToString()} Authenticated {_authenticationFlag.ToString()} Privacy {_privacyFlag.ToString()}";
		}
	}
}
