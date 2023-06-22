using System;

namespace SnmpSharpNet
{
	[Serializable]
	public class SnmpException : Exception
	{
		public static int None = 0;

		public static int UnsupportedSecurityModel = 1;

		public static int UnsupportedNoAuthPriv = 2;

		public static int InvalidAuthenticationParameterLength = 3;

		public static int AuthenticationFailed = 4;

		public static int UnsupportedPrivacyProtocol = 5;

		public static int InvalidPrivacyParameterLength = 6;

		public static int InvalidAuthoritativeEngineId = 7;

		public static int InvalidEngineBoots = 8;

		public static int PacketOutsideTimeWindow = 9;

		public static int InvalidRequestId = 10;

		public static int MaximumMessageSizeExceeded = 11;

		public static int InvalidIAgentParameters = 12;

		public static int RequestTimedOut = 12;

		public static int NoDataReceived = 13;

		public static int InvalidSecurityName = 14;

		public static int ReportOnNoReports = 15;

		public static int OidValueTypeChanged = 16;

		public static int InvalidOid = 17;

		protected int _errorCode;

		public int ErrorCode
		{
			get
			{
				return _errorCode;
			}
			set
			{
				_errorCode = value;
			}
		}

		public SnmpException()
		{
		}

		public SnmpException(string msg)
			: base(msg)
		{
		}

		public SnmpException(int errorCode, string msg)
			: base(msg)
		{
			_errorCode = errorCode;
		}
	}
}
