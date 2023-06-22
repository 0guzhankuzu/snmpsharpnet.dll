namespace SnmpSharpNet
{
	public interface IAuthenticationDigest
	{
		int DigestLength
		{
			get;
		}

		string Name
		{
			get;
		}

		byte[] authenticate(byte[] userPassword, byte[] engineId, byte[] wholeMessage);

		byte[] authenticate(byte[] authKey, byte[] wholeMessage);

		bool authenticateIncomingMsg(byte[] authentiationSecret, byte[] engineId, byte[] authenticationParameters, MutableByte wholeMessage);

		bool authenticateIncomingMsg(byte[] authKey, byte[] authenticationParameters, MutableByte wholeMessage);

		byte[] PasswordToKey(byte[] passwordString, byte[] engineID);

		byte[] ComputeHash(byte[] data, int offset, int count);
	}
}
