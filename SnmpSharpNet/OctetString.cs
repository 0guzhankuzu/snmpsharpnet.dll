using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SnmpSharpNet
{
	[Serializable]
	public class OctetString : AsnType, ICloneable, IComparable<byte[]>, IComparable<OctetString>, IEnumerable<byte>, IEnumerable
	{
		protected byte[] _data;

		public virtual int Length
		{
			get
			{
				int result = 0;
				if (_data != null)
				{
					result = _data.Length;
				}
				return result;
			}
		}

		public byte this[int index]
		{
			get
			{
				if (index < 0 || index >= Length)
				{
					return 0;
				}
				return _data[index];
			}
			set
			{
				if (index >= 0 && index < Length)
				{
					_data[index] = value;
				}
			}
		}

		public bool IsHex
		{
			get
			{
				if (_data == null || _data.Length < 0)
				{
					return false;
				}
				bool result = false;
				for (int i = 0; i < _data.Length; i++)
				{
					byte b = _data[i];
					if (b < 32)
					{
						int num;
						switch (b)
						{
						case 0:
							num = ((_data.Length - 1 == i) ? 1 : 0);
							break;
						default:
							num = 0;
							break;
						case 10:
						case 13:
							num = 1;
							break;
						}
						if (num == 0)
						{
							result = true;
						}
					}
					else if (b > 127)
					{
						result = true;
					}
				}
				return result;
			}
		}

		public OctetString()
		{
			_asnType = SnmpConstants.SMI_STRING;
		}

		public OctetString(string data)
			: this()
		{
			Set(data);
		}

		public OctetString(byte[] data)
			: this()
		{
			Set(data);
		}

		public OctetString(byte[] data, bool useReference)
			: this()
		{
			if (useReference)
			{
				SetRef(data);
			}
			else
			{
				Set(data);
			}
		}

		public OctetString(OctetString second)
			: this()
		{
			Set(second);
		}

		public OctetString(byte data)
			: this()
		{
			Set(data);
		}

		internal byte[] GetData()
		{
			return _data;
		}

		public void Clear()
		{
			_data = null;
		}

		public byte[] ToArray()
		{
			if (_data == null)
			{
				return null;
			}
			byte[] array = new byte[_data.Length];
			Buffer.BlockCopy(_data, 0, array, 0, _data.Length);
			return array;
		}

		public virtual void Set(string value)
		{
			if (value == null)
			{
				_data = null;
			}
			if (value.Length == 0)
			{
				_data = null;
			}
			else
			{
				_data = Encoding.UTF8.GetBytes(value);
			}
		}

		public virtual void Set(byte[] data)
		{
			if (data == null || data.Length <= 0)
			{
				_data = null;
				return;
			}
			_data = new byte[data.Length];
			Buffer.BlockCopy(data, 0, _data, 0, data.Length);
		}

		public virtual void Set(byte data)
		{
			_data = new byte[1];
			_data[0] = data;
		}

		public virtual void Set(int position, byte value)
		{
			if (position >= 0 && position < Length)
			{
				_data[position] = value;
			}
		}

		public virtual void SetRef(byte[] data)
		{
			_data = data;
		}

		public void Append(string value)
		{
			if (_data == null)
			{
				Set(value);
			}
			else if (value.Length > 0)
			{
				byte[] bytes = Encoding.UTF8.GetBytes(value);
				if (bytes != null && bytes.Length > 0)
				{
					Append(bytes);
				}
			}
		}

		public void Append(byte[] value)
		{
			if (value == null || value.Length == 0)
			{
				throw new ArgumentNullException("value");
			}
			if (_data == null)
			{
				Set(value);
				return;
			}
			byte[] array = new byte[_data.Length + value.Length];
			Buffer.BlockCopy(_data, 0, array, 0, _data.Length);
			Buffer.BlockCopy(value, 0, array, _data.Length, value.Length);
			_data = array;
		}

		public override object Clone()
		{
			return new OctetString(this);
		}

		public string ToMACAddressString()
		{
			if (Length == 6)
			{
				return string.Format(CultureInfo.CurrentCulture, "{0:x2}{1:x2}.{2:x2}{3:x2}.{4:x2}{5:x2}", _data[0], _data[1], _data[2], _data[3], _data[4], _data[5]);
			}
			return "";
		}

		public override string ToString()
		{
			if (_data == null || _data.Length <= 0)
			{
				return "";
			}
			bool isHex = IsHex;
			string text = null;
			return (!isHex) ? new string(Encoding.UTF8.GetChars(_data)) : ToHexString();
		}

		public string ToHexString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < _data.Length; i++)
			{
				int num = _data[i] & 0xFF;
				if (num < 16)
				{
					stringBuilder.Append('0');
				}
				stringBuilder.Append(Convert.ToString(num, 16).ToUpper());
				if (i < _data.Length - 1)
				{
					stringBuilder.Append(' ');
				}
			}
			return stringBuilder.ToString();
		}

		public override bool Equals(object obj)
		{
			byte[] array = null;
			if (obj is OctetString)
			{
				OctetString octetString = obj as OctetString;
				array = octetString.GetData();
			}
			else
			{
				if (!(obj is string))
				{
					return false;
				}
				array = Encoding.UTF8.GetBytes((string)obj);
			}
			if (array == null || _data == null)
			{
				if (array == null && _data == null)
				{
					return true;
				}
				return false;
			}
			if (array.Length != _data.Length)
			{
				return false;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != _data[i])
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(OctetString str1, OctetString str2)
		{
			if ((object)str1 == null && (object)str2 == null)
			{
				return true;
			}
			if ((object)str1 == null || (object)str2 == null)
			{
				return false;
			}
			return str1.Equals(str2);
		}

		public static bool operator !=(OctetString str1, OctetString str2)
		{
			return !(str1 == str2);
		}

		public int CompareTo(byte[] other)
		{
			if (_data == null && other != null && other.Length > 0)
			{
				return 1;
			}
			if ((other == null || other.Length == 0) && _data.Length > 0)
			{
				return -1;
			}
			if (_data == null && other == null)
			{
				return 0;
			}
			if (_data.Length > other.Length)
			{
				return -1;
			}
			if (_data.Length < other.Length)
			{
				return 1;
			}
			for (int i = 0; i < _data.Length; i++)
			{
				if (_data[i] > other[i])
				{
					return -1;
				}
				if (_data[i] < other[i])
				{
					return 1;
				}
			}
			return 0;
		}

		public int CompareTo(OctetString other)
		{
			return CompareTo(other.GetData());
		}

		public static implicit operator byte[](OctetString oStr)
		{
			if (oStr == null)
			{
				return null;
			}
			return oStr.ToArray();
		}

		public void Reset()
		{
			_data = null;
		}

		public override void encode(MutableByte buffer)
		{
			if (_data == null || _data.Length == 0)
			{
				AsnType.BuildHeader(buffer, base.Type, 0);
				return;
			}
			AsnType.BuildHeader(buffer, base.Type, _data.Length);
			buffer.Append(_data);
		}

		public override int decode(byte[] buffer, int offset)
		{
			int length;
			byte b = AsnType.ParseHeader(buffer, ref offset, out length);
			if (b != base.Type)
			{
				throw new SnmpException("Invalid ASN.1 type.");
			}
			if (buffer.Length - offset < length)
			{
				throw new OverflowException("Data buffer is too small");
			}
			if (length == 0)
			{
				_data = null;
			}
			else
			{
				_data = new byte[length];
				Buffer.BlockCopy(buffer, offset, _data, 0, length);
				offset += length;
			}
			return offset;
		}

		public IEnumerator<byte> GetEnumerator()
		{
			if (_data != null)
			{
				return ((IEnumerable<byte>)_data).GetEnumerator();
			}
			return null;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			if (_data != null)
			{
				return _data.GetEnumerator();
			}
			return null;
		}
	}
}
