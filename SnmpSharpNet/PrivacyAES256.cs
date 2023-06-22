namespace SnmpSharpNet
{
	public class PrivacyAES256 : PrivacyAES
	{
		public override string Name => "AES256";

		public PrivacyAES256()
			: base(32)
		{
		}
	}
}
