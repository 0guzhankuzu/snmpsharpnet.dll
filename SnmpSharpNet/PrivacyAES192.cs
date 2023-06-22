namespace SnmpSharpNet
{
	public class PrivacyAES192 : PrivacyAES
	{
		public override string Name => "AES192";

		public PrivacyAES192()
			: base(24)
		{
		}
	}
}
