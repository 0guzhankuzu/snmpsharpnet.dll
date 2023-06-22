using System;
using System.Security.Cryptography;

namespace SnmpSharpNet
{
	public class Privacy3DES : IPrivacyProtocol
	{
		protected int _salt;

		public int MinimumKeyLength => 32;

		public int MaximumKeyLength => 32;

		public int PrivacyParametersLength => 8;

		public string Name => "TripleDES";

		public bool CanExtendShortKey => true;

		public Privacy3DES()
		{
			Random random = new Random();
			_salt = random.Next();
		}

		public int NextSalt()
		{
			if (_salt == int.MaxValue)
			{
				_salt = 1;
			}
			else
			{
				_salt++;
			}
			return _salt;
		}

		public byte[] Encrypt(byte[] unencryptedData, int offset, int length, byte[] key, int engineBoots, int engineTime, out byte[] privacyParameters, IAuthenticationDigest authDigest)
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Expected O, but got Unknown
			privacyParameters = GetSalt(engineBoots);
			byte[] src = authDigest.ComputeHash(privacyParameters, 0, privacyParameters.Length);
			privacyParameters = new byte[8];
			Buffer.BlockCopy(src, 0, privacyParameters, 0, 8);
			byte[] iV = GetIV(key, privacyParameters);
			byte[] array = null;
			try
			{
				TripleDES val = (TripleDES)new TripleDESCryptoServiceProvider();
				((SymmetricAlgorithm)val).set_Mode((CipherMode)1);
				((SymmetricAlgorithm)val).set_Padding((PaddingMode)1);
				byte[] array2 = new byte[24];
				Buffer.BlockCopy(key, 0, array2, 0, array2.Length);
				ICryptoTransform val2 = ((SymmetricAlgorithm)val).CreateEncryptor(array2, iV);
				if (length % 8 == 0)
				{
					return val2.TransformFinalBlock(unencryptedData, offset, length);
				}
				byte[] array3 = new byte[8 * (length / 8 + 1)];
				Buffer.BlockCopy(unencryptedData, offset, array3, 0, length);
				return val2.TransformFinalBlock(array3, 0, array3.Length);
			}
			catch (Exception ex)
			{
				throw new SnmpPrivacyException(ex, "Exception was thrown while TripleDES privacy protocol was encrypting data\r\n" + ex.ToString());
			}
		}

		public byte[] Decrypt(byte[] encryptedData, int offset, int length, byte[] key, int engineBoots, int engineTime, byte[] privacyParameters)
		{
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Expected O, but got Unknown
			if (length % 8 != 0)
			{
				throw new ArgumentOutOfRangeException("encryptedData", "Encrypted data buffer has to be divisable by 8.");
			}
			if (encryptedData == null || encryptedData.Length < 8)
			{
				throw new ArgumentOutOfRangeException("encryptedData", "Encrypted data buffer is null or smaller then 8 bytes in length.");
			}
			if (offset > encryptedData.Length || offset + length > encryptedData.Length)
			{
				throw new ArgumentOutOfRangeException("offset", "Offset and length arguments point beyond the bounds of the encryptedData array.");
			}
			if (key == null || key.Length < 32)
			{
				throw new ArgumentOutOfRangeException("decryptionKey", "Minimum acceptable length of the decryption key is 32 bytes.");
			}
			if (privacyParameters == null || privacyParameters.Length != 8)
			{
				throw new ArgumentOutOfRangeException("privacyParameters", "Privacy parameters field is not 8 bytes long.");
			}
			byte[] iV = GetIV(key, privacyParameters);
			byte[] array = null;
			try
			{
				TripleDES val = (TripleDES)new TripleDESCryptoServiceProvider();
				((SymmetricAlgorithm)val).set_Mode((CipherMode)1);
				((SymmetricAlgorithm)val).set_Padding((PaddingMode)1);
				byte[] array2 = new byte[24];
				Buffer.BlockCopy(key, 0, array2, 0, array2.Length);
				ICryptoTransform val2 = ((SymmetricAlgorithm)val).CreateDecryptor(array2, iV);
				return val2.TransformFinalBlock(encryptedData, offset, length);
			}
			catch (Exception ex)
			{
				throw new SnmpPrivacyException(ex, "Exception was thrown while TripleDES privacy protocol was decrypting data.");
			}
		}

		public int GetEncryptedLength(int scopedPduLength)
		{
			if (scopedPduLength % 8 == 0)
			{
				return scopedPduLength;
			}
			return 8 * (scopedPduLength / 8 + 1);
		}

		private byte[] GetSalt(int engineBoots)
		{
			byte[] array = new byte[8];
			int value = NextSalt();
			byte[] bytes = BitConverter.GetBytes(engineBoots);
			array[3] = bytes[0];
			array[2] = bytes[1];
			array[1] = bytes[2];
			array[0] = bytes[3];
			byte[] bytes2 = BitConverter.GetBytes(value);
			array[7] = bytes2[0];
			array[6] = bytes2[1];
			array[5] = bytes2[2];
			array[4] = bytes2[3];
			return array;
		}

		private byte[] GetIV(byte[] privacyKey, byte[] salt)
		{
			if (privacyKey.Length < 32)
			{
				throw new SnmpPrivacyException("Invalid privacy key length");
			}
			byte[] array = new byte[8];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (byte)(salt[i] ^ privacyKey[24 + i]);
			}
			return array;
		}

		public byte[] ExtendShortKey(byte[] shortKey, byte[] password, byte[] engineID, IAuthenticationDigest authProtocol)
		{
			int i = shortKey.Length;
			byte[] array = new byte[MinimumKeyLength];
			Buffer.BlockCopy(shortKey, 0, array, 0, shortKey.Length);
			int num;
			for (; i < MinimumKeyLength; i += num)
			{
				byte[] src = authProtocol.PasswordToKey(shortKey, engineID);
				num = Math.Min(MaximumKeyLength - i, authProtocol.DigestLength);
				Buffer.BlockCopy(src, 0, array, i, num);
			}
			return array;
		}

		public byte[] PasswordToKey(byte[] secret, byte[] engineId, IAuthenticationDigest authProtocol)
		{
			if (secret == null || secret.Length < 8)
			{
				throw new SnmpPrivacyException("Invalid privacy secret length.");
			}
			byte[] array = authProtocol.PasswordToKey(secret, engineId);
			if (array.Length < MinimumKeyLength)
			{
				array = ExtendShortKey(array, secret, engineId, authProtocol);
			}
			return array;
		}
	}
}
