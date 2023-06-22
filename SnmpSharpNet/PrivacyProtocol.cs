namespace SnmpSharpNet
{
	public sealed class PrivacyProtocol
	{
		public static IPrivacyProtocol GetInstance(PrivacyProtocols privProtocol)
		{
			return privProtocol switch
			{
				PrivacyProtocols.None => null, 
				PrivacyProtocols.DES => new PrivacyDES(), 
				PrivacyProtocols.AES128 => new PrivacyAES128(), 
				PrivacyProtocols.AES192 => new PrivacyAES192(), 
				PrivacyProtocols.AES256 => new PrivacyAES256(), 
				PrivacyProtocols.TripleDES => new Privacy3DES(), 
				_ => null, 
			};
		}

		private PrivacyProtocol()
		{
		}
	}
}
