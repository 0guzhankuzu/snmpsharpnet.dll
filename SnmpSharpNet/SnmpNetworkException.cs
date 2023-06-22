using System;

namespace SnmpSharpNet
{
	public class SnmpNetworkException : SnmpException
	{
		private Exception _systemException;

		public Exception SystemException => _systemException;

		public SnmpNetworkException(Exception sysException, string msg)
			: base(msg)
		{
			_systemException = sysException;
		}

		public SnmpNetworkException(string msg)
			: base(msg)
		{
			_systemException = null;
		}
	}
}
