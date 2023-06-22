using System;

namespace SnmpSharpNet
{
	public class SnmpPrivacyException : SnmpException
	{
		private Exception _parentException;

		public Exception ParentException => _parentException;

		public SnmpPrivacyException(string msg)
			: base(msg)
		{
		}

		public SnmpPrivacyException(Exception ex, string msg)
			: base(msg)
		{
			_parentException = ex;
		}
	}
}
