namespace SnmpSharpNet
{
	public class SnmpInvalidPduTypeException : SnmpException
	{
		public SnmpInvalidPduTypeException(string msg)
			: base(msg)
		{
		}
	}
}
