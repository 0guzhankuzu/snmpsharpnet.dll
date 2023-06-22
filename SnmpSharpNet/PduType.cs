namespace SnmpSharpNet
{
	public enum PduType : byte
	{
		Get = 160,
		GetNext,
		Response,
		Set,
		Trap,
		GetBulk,
		Inform,
		V2Trap,
		Report
	}
}
