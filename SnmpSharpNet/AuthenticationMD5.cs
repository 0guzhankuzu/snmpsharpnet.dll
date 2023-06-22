using System;
using System.Security.Cryptography;

namespace SnmpSharpNet
{
	public class AuthenticationMD5 : IAuthenticationDigest
	{
		public int DigestLength => 16;

		public string Name => "HMAC-MD5";

		public byte[] authenticate(byte[] authenticationSecret, byte[] engineId, byte[] wholeMessage)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			byte[] array = new byte[12];
			byte[] array2 = PasswordToKey(authenticationSecret, engineId);
			HMACMD5 val = new HMACMD5(array2);
			byte[] src = ((HashAlgorithm)val).ComputeHash(wholeMessage);
			Buffer.BlockCopy(src, 0, array, 0, 12);
			return array;
		}

		public byte[] authenticate(byte[] authKey, byte[] wholeMessage)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Expected O, but got Unknown
			byte[] array = new byte[12];
			HMACMD5 val = new HMACMD5(authKey);
			byte[] src = ((HashAlgorithm)val).ComputeHash(wholeMessage);
			Buffer.BlockCopy(src, 0, array, 0, 12);
			return array;
		}

		public bool authenticateIncomingMsg(byte[] userPassword, byte[] engineId, byte[] authenticationParameters, MutableByte wholeMessage)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			byte[] array = PasswordToKey(userPassword, engineId);
			HMACMD5 val = new HMACMD5(array);
			byte[] buf = ((HashAlgorithm)val).ComputeHash((byte[])wholeMessage, 0, wholeMessage.Length);
			MutableByte mutableByte = new MutableByte(buf, 12);
			if (mutableByte.Equals(authenticationParameters))
			{
				return true;
			}
			return false;
		}

		public bool authenticateIncomingMsg(byte[] authKey, byte[] authenticationParameters, MutableByte wholeMessage)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Expected O, but got Unknown
			HMACMD5 val = new HMACMD5(authKey);
			byte[] buf = ((HashAlgorithm)val).ComputeHash((byte[])wholeMessage, 0, wholeMessage.Length);
			MutableByte mutableByte = new MutableByte(buf, 12);
			if (mutableByte.Equals(authenticationParameters))
			{
				return true;
			}
			return false;
		}

		public byte[] PasswordToKey(byte[] userPassword, byte[] engineID)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Expected O, but got Unknown
			if (userPassword == null || userPassword.Length < 8)
			{
				throw new SnmpAuthenticationException("Secret key is too short.");
			}
			int num = 0;
			int i = 0;
			MD5 val = (MD5)new MD5CryptoServiceProvider();
			byte[] array = new byte[1048576];
			byte[] array2 = new byte[64];
			for (; i < 1048576; i += 64)
			{
				for (int j = 0; j < 64; j++)
				{
					array2[j] = userPassword[num++ % userPassword.Length];
				}
				Buffer.BlockCopy(array2, 0, array, i, array2.Length);
			}
			byte[] buf = ((HashAlgorithm)val).ComputeHash(array);
			MutableByte mutableByte = new MutableByte();
			mutableByte.Append(buf);
			mutableByte.Append(engineID);
			mutableByte.Append(buf);
			return ((HashAlgorithm)val).ComputeHash((byte[])mutableByte);
		}

		public byte[] ComputeHash(byte[] data, int offset, int count)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			MD5 val = (MD5)new MD5CryptoServiceProvider();
			byte[] result = ((HashAlgorithm)val).ComputeHash(data, offset, count);
			((HashAlgorithm)val).Clear();
			return result;
		}
	}
}
