namespace SnmpSharpNet
{
	public sealed class SnmpVariableType
	{
		public const byte Counter32 = 64;

		public const byte Counter64 = 102;

		public const byte EndOfMibView = 130;

		public const byte Gauge32 = 65;

		public const byte Integer = 2;

		public const byte IPAddress = 64;

		public const byte NoSuchInstance = 129;

		public const byte NoSuchObject = 128;

		public const byte Null = 5;

		public const byte OctetString = 4;

		public const byte Oid = 6;

		public const byte Opaque = 68;

		public const byte Sequence = 48;

		public const byte TimeTicks = 67;

		public const byte Unsigned32 = 66;

		public const byte VarBind = 48;

		private SnmpVariableType()
		{
		}
	}
}
