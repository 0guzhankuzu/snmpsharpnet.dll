namespace SnmpSharpNet
{
	public class PrivacyAES128 : PrivacyAES
	{
		public override string Name => "AES128";

		public PrivacyAES128()
			: base(16)
		{
		}
	}
}
