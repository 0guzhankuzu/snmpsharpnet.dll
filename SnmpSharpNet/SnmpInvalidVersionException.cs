namespace SnmpSharpNet
{
	public class SnmpInvalidVersionException : SnmpException
	{
		public SnmpInvalidVersionException(string msg)
			: base(msg)
		{
		}
	}
}
