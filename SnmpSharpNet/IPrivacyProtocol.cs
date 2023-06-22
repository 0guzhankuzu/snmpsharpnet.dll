namespace SnmpSharpNet
{
	public interface IPrivacyProtocol
	{
		int MinimumKeyLength
		{
			get;
		}

		int MaximumKeyLength
		{
			get;
		}

		int PrivacyParametersLength
		{
			get;
		}

		string Name
		{
			get;
		}

		bool CanExtendShortKey
		{
			get;
		}

		byte[] Encrypt(byte[] unencryptedData, int offset, int length, byte[] encryptionKey, int engineBoots, int engineTime, out byte[] privacyParameters, IAuthenticationDigest authDigest);

		byte[] Decrypt(byte[] cryptedData, int offset, int length, byte[] key, int engineBoots, int engineTime, byte[] privacyParameters);

		int GetEncryptedLength(int scopedPduLength);

		byte[] ExtendShortKey(byte[] shortKey, byte[] password, byte[] engineID, IAuthenticationDigest authProtocol);

		byte[] PasswordToKey(byte[] secret, byte[] engineId, IAuthenticationDigest authProtocol);
	}
}
