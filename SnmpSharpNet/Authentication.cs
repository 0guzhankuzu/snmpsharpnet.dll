namespace SnmpSharpNet
{
	public sealed class Authentication
	{
		public static IAuthenticationDigest GetInstance(AuthenticationDigests authProtocol)
		{
			return authProtocol switch
			{
				AuthenticationDigests.MD5 => new AuthenticationMD5(), 
				AuthenticationDigests.SHA1 => new AuthenticationSHA1(), 
				_ => null, 
			};
		}

		private Authentication()
		{
		}
	}
}
