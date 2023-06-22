using System;
using System.Security.Cryptography;

namespace SnmpSharpNet
{
	public class PrivacyAES : IPrivacyProtocol
	{
		protected long _salt = 0L;

		protected int _keyBytes = 16;

		public int MinimumKeyLength => _keyBytes;

		public int MaximumKeyLength => _keyBytes;

		public int PrivacyParametersLength => 8;

		public virtual string Name => "AES";

		public bool CanExtendShortKey => true;

		public PrivacyAES(int keyBytes)
		{
			if (keyBytes != 16 && keyBytes != 24 && keyBytes != 32)
			{
				throw new ArgumentOutOfRangeException("keyBytes", "Valid key sizes are 16, 24 and 32 bytes.");
			}
			_keyBytes = keyBytes;
			Random random = new Random();
			_salt = Convert.ToInt64(random.Next(1, int.MaxValue));
		}

		protected long NextSalt()
		{
			if (_salt == long.MaxValue)
			{
				_salt = 1L;
			}
			else
			{
				_salt++;
			}
			return _salt;
		}

		public byte[] Encrypt(byte[] unencryptedData, int offset, int length, byte[] key, int engineBoots, int engineTime, out byte[] privacyParameters, IAuthenticationDigest authDigest)
		{
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Expected O, but got Unknown
			if (key == null || key.Length < _keyBytes)
			{
				throw new ArgumentOutOfRangeException("encryptionKey", "Invalid key length");
			}
			byte[] array = new byte[16];
			long value = NextSalt();
			privacyParameters = new byte[PrivacyParametersLength];
			byte[] bytes = BitConverter.GetBytes(engineBoots);
			array[0] = bytes[3];
			array[1] = bytes[2];
			array[2] = bytes[1];
			array[3] = bytes[0];
			byte[] bytes2 = BitConverter.GetBytes(engineTime);
			array[4] = bytes2[3];
			array[5] = bytes2[2];
			array[6] = bytes2[1];
			array[7] = bytes2[0];
			byte[] bytes3 = BitConverter.GetBytes(value);
			privacyParameters[0] = bytes3[7];
			privacyParameters[1] = bytes3[6];
			privacyParameters[2] = bytes3[5];
			privacyParameters[3] = bytes3[4];
			privacyParameters[4] = bytes3[3];
			privacyParameters[5] = bytes3[2];
			privacyParameters[6] = bytes3[1];
			privacyParameters[7] = bytes3[0];
			Buffer.BlockCopy(privacyParameters, 0, array, 8, 8);
			Rijndael val = (Rijndael)new RijndaelManaged();
			((SymmetricAlgorithm)val).set_KeySize(_keyBytes * 8);
			((SymmetricAlgorithm)val).set_FeedbackSize(128);
			((SymmetricAlgorithm)val).set_BlockSize(128);
			((SymmetricAlgorithm)val).set_Padding((PaddingMode)3);
			((SymmetricAlgorithm)val).set_Mode((CipherMode)4);
			byte[] array2 = new byte[MinimumKeyLength];
			Buffer.BlockCopy(key, 0, array2, 0, MinimumKeyLength);
			((SymmetricAlgorithm)val).set_Key(array2);
			((SymmetricAlgorithm)val).set_IV(array);
			ICryptoTransform val2 = ((SymmetricAlgorithm)val).CreateEncryptor();
			byte[] array3 = val2.TransformFinalBlock(unencryptedData, offset, length);
			if (array3.Length != unencryptedData.Length)
			{
				byte[] array4 = new byte[unencryptedData.Length];
				Buffer.BlockCopy(array3, 0, array4, 0, unencryptedData.Length);
				return array4;
			}
			return array3;
		}

		public byte[] Decrypt(byte[] cryptedData, int offset, int length, byte[] key, int engineBoots, int engineTime, byte[] privacyParameters)
		{
			if (key == null || key.Length < _keyBytes)
			{
				throw new ArgumentOutOfRangeException("decryptionKey", "Invalid key length");
			}
			byte[] array = new byte[16];
			byte[] bytes = BitConverter.GetBytes(engineBoots);
			array[0] = bytes[3];
			array[1] = bytes[2];
			array[2] = bytes[1];
			array[3] = bytes[0];
			byte[] bytes2 = BitConverter.GetBytes(engineTime);
			array[4] = bytes2[3];
			array[5] = bytes2[2];
			array[6] = bytes2[1];
			array[7] = bytes2[0];
			Buffer.BlockCopy(privacyParameters, 0, array, 8, 8);
			byte[] array2 = null;
			Rijndael val = Rijndael.Create();
			((SymmetricAlgorithm)val).set_KeySize(_keyBytes * 8);
			((SymmetricAlgorithm)val).set_FeedbackSize(128);
			((SymmetricAlgorithm)val).set_BlockSize(128);
			((SymmetricAlgorithm)val).set_Padding((PaddingMode)3);
			((SymmetricAlgorithm)val).set_Mode((CipherMode)4);
			if (key.Length > MinimumKeyLength)
			{
				byte[] array3 = new byte[MinimumKeyLength];
				Buffer.BlockCopy(key, 0, array3, 0, MinimumKeyLength);
				((SymmetricAlgorithm)val).set_Key(array3);
			}
			else
			{
				((SymmetricAlgorithm)val).set_Key(key);
			}
			((SymmetricAlgorithm)val).set_IV(array);
			ICryptoTransform val2 = ((SymmetricAlgorithm)val).CreateDecryptor();
			if (cryptedData.Length % _keyBytes != 0)
			{
				byte[] array4 = new byte[length];
				Buffer.BlockCopy(cryptedData, offset, array4, 0, length);
				int num = (int)Math.Floor((double)array4.Length / 16.0);
				int num2 = (num + 1) * 16;
				byte[] array5 = new byte[num2];
				Buffer.BlockCopy(array4, 0, array5, 0, array4.Length);
				array2 = val2.TransformFinalBlock(array5, 0, array5.Length);
				Buffer.BlockCopy(array2, 0, array4, 0, length);
				return array4;
			}
			return val2.TransformFinalBlock(cryptedData, offset, length);
		}

		public int GetEncryptedLength(int scopedPduLength)
		{
			return scopedPduLength;
		}

		public byte[] ExtendShortKey(byte[] shortKey, byte[] password, byte[] engineID, IAuthenticationDigest authProtocol)
		{
			byte[] array = new byte[MinimumKeyLength];
			byte[] array2 = new byte[shortKey.Length];
			Array.Copy(shortKey, array2, shortKey.Length);
			int num = ((shortKey.Length > MinimumKeyLength) ? MinimumKeyLength : shortKey.Length);
			Array.Copy(shortKey, array, num);
			while (num < MinimumKeyLength)
			{
				byte[] array3 = authProtocol.PasswordToKey(array2, engineID);
				if (array3 == null)
				{
					return null;
				}
				if (array3.Length <= MinimumKeyLength - num)
				{
					Array.Copy(array3, 0, array, num, array3.Length);
					num += array3.Length;
				}
				else
				{
					Array.Copy(array3, 0, array, num, MinimumKeyLength - num);
					num += MinimumKeyLength - num;
				}
				array2 = new byte[array3.Length];
				Array.Copy(array3, array2, array3.Length);
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
