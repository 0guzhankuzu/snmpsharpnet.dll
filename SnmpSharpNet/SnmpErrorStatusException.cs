using System;

namespace SnmpSharpNet
{
	public class SnmpErrorStatusException : Exception
	{
		protected int _errorStatus;

		protected int _errorIndex;

		public int ErrorStatus
		{
			get
			{
				return _errorStatus;
			}
			set
			{
				_errorStatus = value;
			}
		}

		public int ErrorIndex
		{
			get
			{
				return _errorIndex;
			}
			set
			{
				_errorIndex = value;
			}
		}

		public override string Message => $"{base.Message}> ErrorStatus {_errorStatus} ErrorIndex {_errorIndex}";

		public SnmpErrorStatusException()
		{
			_errorStatus = 0;
			_errorIndex = 0;
		}

		public SnmpErrorStatusException(string msg, int status, int index)
			: base(msg)
		{
			_errorStatus = status;
			_errorIndex = index;
		}
	}
}
