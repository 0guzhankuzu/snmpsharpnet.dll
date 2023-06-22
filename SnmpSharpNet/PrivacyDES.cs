using System;
using System.Security.Cryptography;

namespace SnmpSharpNet
{
	public class PrivacyDES : IPrivacyProtocol
	{
		protected int _salt = 0;

		public int MinimumKeyLength => 16;

		public int MaximumKeyLength => 16;

		public int PrivacyParametersLength => 8;

		public string Name => "DES";

		public bool CanExtendShortKey => false;

		public PrivacyDES()
		{
			Random random = new Random();
			_salt = random.Next(1, int.MaxValue);
		}

		protected int NextSalt()
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
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Expected O, but got Unknown
			if (key == null || key.Length < MinimumKeyLength)
			{
				throw new ArgumentOutOfRangeException("encryptionKey", "Encryption key length has to 32 bytes or more.");
			}
			privacyParameters = GetSalt(engineBoots);
			byte[] iV = GetIV(key, privacyParameters);
			byte[] key2 = GetKey(key);
			int num = (int)Math.Floor((double)length / 8.0);
			if (length % 8 != 0)
			{
				num++;
			}
			int num2 = num * 8;
			byte[] array = new byte[num2];
			byte[] array2 = new byte[num2];
			byte[] array3 = new byte[8];
			byte[] array4 = iV;
			int num3 = 0;
			int num4 = 0;
			Buffer.BlockCopy(unencryptedData, offset, array2, 0, length);
			DES val = (DES)new DESCryptoServiceProvider();
			((SymmetricAlgorithm)val).set_Mode((CipherMode)2);
			((SymmetricAlgorithm)val).set_Padding((PaddingMode)1);
			ICryptoTransform val2 = ((SymmetricAlgorithm)val).CreateEncryptor(key2, (byte[])null);
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					array3[j] = (byte)(array2[num3] ^ array4[j]);
					num3++;
				}
				val2.TransformBlock(array3, 0, array3.Length, array4, 0);
				Buffer.BlockCopy(array4, 0, array, num4, array4.Length);
				num4 += array4.Length;
			}
			((SymmetricAlgorithm)val).Clear();
			return array;
		}

		public byte[] Decrypt(byte[] encryptedData, int offset, int length, byte[] key, int engineBoots, int engineTime, byte[] privacyParameters)
		{
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Expected O, but got Unknown
			if (length % 8 != 0)
			{
				throw new ArgumentOutOfRangeException("encryptedData", "Encrypted data buffer has to be divisible by 8.");
			}
			if (encryptedData == null || encryptedData.Length == 0)
			{
				throw new ArgumentNullException("cryptedData");
			}
			if (privacyParameters == null || privacyParameters.Length != PrivacyParametersLength)
			{
				throw new ArgumentOutOfRangeException("privacyParameters", "Privacy parameters argument has to be 8 bytes long");
			}
			if (key == null || key.Length < MinimumKeyLength)
			{
				throw new ArgumentOutOfRangeException("decryptionKey", "Decryption key has to be at least 16 bytes long.");
			}
			byte[] array = new byte[8];
			for (int i = 0; i < 8; i++)
			{
				array[i] = (byte)(key[8 + i] ^ privacyParameters[i]);
			}
			DES val = (DES)new DESCryptoServiceProvider();
			((SymmetricAlgorithm)val).set_Mode((CipherMode)1);
			((SymmetricAlgorithm)val).set_Padding((PaddingMode)3);
			byte[] array2 = new byte[8];
			Buffer.BlockCopy(key, 0, array2, 0, 8);
			((SymmetricAlgorithm)val).set_Key(array2);
			((SymmetricAlgorithm)val).set_IV(array);
			ICryptoTransform val2 = ((SymmetricAlgorithm)val).CreateDecryptor();
			byte[] result = val2.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
			((SymmetricAlgorithm)val).Clear();
			return result;
		}

		public int GetEncryptedLength(int scopedPduLength)
		{
			if (scopedPduLength % 8 == 0)
			{
				return scopedPduLength;
			}
			return 8 * (scopedPduLength / 8 + 1);
		}

		public byte[] ExtendShortKey(byte[] shortKey, byte[] password, byte[] engineID, IAuthenticationDigest authProtocol)
		{
			return shortKey;
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

		private byte[] GetKey(byte[] privacyPassword)
		{
			if (privacyPassword == null || privacyPassword.Length < 16)
			{
				throw new SnmpPrivacyException("Invalid privacy key length.");
			}
			byte[] array = new byte[8];
			Buffer.BlockCopy(privacyPassword, 0, array, 0, 8);
			return array;
		}

		private byte[] GetIV(byte[] privacyKey, byte[] salt)
		{
			if (privacyKey.Length < 16)
			{
				throw new SnmpPrivacyException("Invalid privacy key length");
			}
			byte[] array = new byte[8];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (byte)(salt[i] ^ privacyKey[8 + i]);
			}
			return array;
		}

		public byte[] PasswordToKey(byte[] secret, byte[] engineId, IAuthenticationDigest authProtocol)
		{
			if (secret == null || secret.Length < 8)
			{
				throw new SnmpPrivacyException("Invalid privacy secret length.");
			}
			return authProtocol.PasswordToKey(secret, engineId);
		}
	}
}
