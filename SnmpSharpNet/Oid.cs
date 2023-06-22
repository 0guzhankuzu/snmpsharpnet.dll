using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SnmpSharpNet
{
	[Serializable]
	public class Oid : AsnType, ICloneable, IComparable, IEnumerable<uint>, IEnumerable
	{
		protected uint[] _data;

		public virtual int Length
		{
			get
			{
				if (_data == null)
				{
					return 0;
				}
				return _data.Length;
			}
		}

		public bool IsNull
		{
			get
			{
				if (Length == 0)
				{
					return true;
				}
				if (Length == 2 && _data[0] == 0 && _data[1] == 0)
				{
					return true;
				}
				return false;
			}
		}

		public uint this[int index]
		{
			get
			{
				if (_data == null || index < 0 || index >= _data.Length)
				{
					throw new OverflowException("Requested instance is outside the bounds of the Oid array");
				}
				return _data[index];
			}
		}

		public Oid()
		{
			_asnType = SnmpConstants.SMI_OBJECTID;
			_data = null;
		}

		public Oid(uint[] data)
			: this()
		{
			Set(data);
		}

		public Oid(int[] data)
			: this()
		{
			Set(data);
		}

		public Oid(Oid second)
			: this()
		{
			Set(second);
		}

		public Oid(string value)
			: this()
		{
			Set(value);
		}

		public virtual void Set(int[] value)
		{
			if (value == null)
			{
				_data = null;
				return;
			}
			for (int i = 0; i < value.Length; i++)
			{
				int num = value[i];
				if (num < 0)
				{
					throw new OverflowException("OID instance value cannot be less then zero.");
				}
			}
			_data = new uint[value.Length];
			for (int num = 0; num < value.Length; num++)
			{
				_data[num] = (uint)value[num];
			}
		}

		public virtual void Set(uint[] value)
		{
			if (value == null)
			{
				_data = null;
				return;
			}
			_data = new uint[value.Length];
			Array.Copy(value, 0, _data, 0, value.Length);
		}

		public void Set(Oid value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			Set(value.GetData());
		}

		public void Set(string value)
		{
			if (value == null || value.Length == 0)
			{
				_data = null;
			}
			else
			{
				_data = Parse(value);
			}
		}

		public virtual void Add(int[] ids)
		{
			if (ids == null || ids.Length == 0)
			{
				return;
			}
			if (_data != null)
			{
				if (ids == null || ids.Length == 0)
				{
					return;
				}
				uint[] array = new uint[_data.Length + ids.Length];
				Array.Copy(_data, 0, array, 0, _data.Length);
				for (int i = 0; i < ids.Length; i++)
				{
					if (ids[i] < 0)
					{
						throw new OverflowException("Instance value cannot be less then zero.");
					}
					array[_data.Length + i] = (uint)ids[i];
				}
				_data = array;
				return;
			}
			_data = new uint[ids.Length];
			for (int i = 0; i < ids.Length; i++)
			{
				if (ids[i] < 0)
				{
					throw new OverflowException("Instance value cannot be less then zero.");
				}
				_data[i] = (uint)ids[i];
			}
		}

		public virtual void Add(uint[] ids)
		{
			if (ids == null || ids.Length == 0)
			{
				return;
			}
			if (_data != null)
			{
				if (ids != null && ids.Length != 0)
				{
					uint[] array = new uint[_data.Length + ids.Length];
					Array.Copy(_data, 0, array, 0, _data.Length);
					Array.Copy(ids, 0, array, _data.Length, ids.Length);
					_data = array;
				}
			}
			else
			{
				_data = new uint[ids.Length];
				Array.Copy(ids, _data, ids.Length);
			}
		}

		public virtual void Add(uint id)
		{
			if (_data != null)
			{
				uint[] array = new uint[_data.Length + 1];
				Array.Copy(_data, 0, array, 0, _data.Length);
				array[_data.Length] = id;
				_data = array;
			}
			else
			{
				_data = new uint[1];
				_data[0] = id;
			}
		}

		public virtual void Add(int id)
		{
			if (id < 0)
			{
				throw new OverflowException("Instance id is less then zero.");
			}
			if (_data != null)
			{
				uint[] array = new uint[_data.Length + 1];
				Array.Copy(_data, 0, array, 0, _data.Length);
				array[_data.Length] = (uint)id;
				_data = array;
			}
			else
			{
				_data = new uint[1];
				_data[0] = (uint)id;
			}
		}

		public virtual void Add(string strOids)
		{
			uint[] ids = Parse(strOids);
			Add(ids);
		}

		public virtual void Add(Oid second)
		{
			Add(second.GetData());
		}

		public virtual int Compare(uint[] ids)
		{
			if (ids == null && _data == null)
			{
				return 0;
			}
			if (ids != null && _data == null)
			{
				return 1;
			}
			if (ids == null && _data != null)
			{
				return -1;
			}
			return Compare(ids, (_data.Length > ids.Length) ? ids.Length : _data.Length);
		}

		public virtual int Compare(uint[] ids, int dist)
		{
			if (_data == null)
			{
				if (ids == null)
				{
					return 0;
				}
				return -1;
			}
			if (ids == null)
			{
				return 1;
			}
			if (ids.Length < dist || _data.Length < dist)
			{
				if (_data.Length < ids.Length || _data.Length == ids.Length)
				{
					return -1;
				}
				return 1;
			}
			for (int i = 0; i < dist; i++)
			{
				if (_data[i] < ids[i])
				{
					return -1;
				}
				if (_data[i] > ids[i])
				{
					return 1;
				}
			}
			return 0;
		}

		public int CompareExact(Oid oid)
		{
			return CompareExact(oid.GetData());
		}

		public int CompareExact(uint[] ids)
		{
			int num = Compare(ids);
			if (num == 0)
			{
				if (ids == null)
				{
					if (_data == null)
					{
						return 0;
					}
					return 1;
				}
				if (_data == null)
				{
					return -1;
				}
				if (_data.Length != ids.Length)
				{
					if (_data.Length > ids.Length)
					{
						return 1;
					}
					if (_data.Length < ids.Length)
					{
						return -1;
					}
				}
			}
			return num;
		}

		public virtual int Compare(Oid cmp)
		{
			if ((object)cmp == null)
			{
				return 1;
			}
			if (cmp.GetData() == null && _data == null)
			{
				return 0;
			}
			if (cmp.GetData() != null && _data == null)
			{
				return 1;
			}
			if (cmp.GetData() == null && _data != null)
			{
				return -1;
			}
			return Compare(cmp.GetData());
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is Oid)
			{
				return CompareExact(((Oid)obj)._data) == 0;
			}
			if (obj is string)
			{
				return CompareExact(Parse((string)obj)) == 0;
			}
			if (obj is uint[])
			{
				return CompareExact((uint[])obj) == 0;
			}
			return false;
		}

		public virtual bool IsRootOf(Oid leaf)
		{
			return Compare(leaf._data, (_data != null) ? _data.Length : 0) == 0;
		}

		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			if (obj is Oid)
			{
				return CompareExact((Oid)obj);
			}
			return 1;
		}

		protected uint[] GetData()
		{
			return _data;
		}

		public void Reset()
		{
			if (_data != null)
			{
				_data = null;
			}
		}

		public uint[] ToArray()
		{
			if (_data == null || _data.Length == 0)
			{
				return null;
			}
			uint[] array = new uint[_data.Length];
			Array.Copy(_data, 0, array, 0, _data.Length);
			return array;
		}

		public static uint[] GetChildIdentifiers(Oid root, Oid leaf)
		{
			if ((object)leaf == null || leaf.IsNull)
			{
				return null;
			}
			uint[] array;
			if (((object)root == null || root.IsNull) && (object)leaf != null)
			{
				array = new uint[leaf.Length];
				Array.Copy(leaf.GetData(), array, leaf.Length);
				return array;
			}
			if (!root.IsRootOf(leaf))
			{
				return null;
			}
			if (leaf.Length <= root.Length)
			{
				return null;
			}
			int num = leaf.Length - root.Length;
			array = new uint[num];
			Array.Copy(leaf.GetData(), root.Length, array, 0, num);
			return array;
		}

		public static string ToString(int[] vals)
		{
			string text = "";
			if (vals == null)
			{
				return text;
			}
			for (int i = 0; i < vals.Length; i++)
			{
				text += vals[i].ToString(CultureInfo.CurrentCulture);
				if (i != vals.Length - 1)
				{
					text += ".";
				}
			}
			return text;
		}

		public static string ToString(int[] vals, int startpos)
		{
			string text = "";
			if (vals == null)
			{
				return text;
			}
			if (startpos < 0 || startpos >= vals.Length)
			{
				throw new IndexOutOfRangeException("Requested value is out of range");
			}
			for (int i = startpos; i < vals.Length; i++)
			{
				text += vals[i];
				if (i != vals.Length - 1)
				{
					text += ".";
				}
			}
			return text;
		}

		public static Oid operator +(Oid oid, uint[] ids)
		{
			if ((object)oid == null && ids == null)
			{
				return null;
			}
			if (ids == null)
			{
				return (Oid)oid.Clone();
			}
			Oid oid2 = new Oid(oid);
			oid2.Add(ids);
			return oid2;
		}

		public static Oid operator +(Oid oid, string strOids)
		{
			if ((object)oid == null && (strOids == null || strOids.Length == 0))
			{
				return null;
			}
			if (strOids == null || strOids.Length == 0)
			{
				return (Oid)oid.Clone();
			}
			Oid oid2 = new Oid(oid);
			oid2.Add(strOids);
			return oid2;
		}

		public static Oid operator +(Oid oid1, Oid oid2)
		{
			if ((object)oid1 == null && (object)oid2 == null)
			{
				return null;
			}
			if ((object)oid2 == null || oid2.IsNull)
			{
				return (Oid)oid1.Clone();
			}
			if ((object)oid1 == null)
			{
				return (Oid)oid2.Clone();
			}
			Oid oid3 = new Oid(oid1);
			oid3.Add(oid2);
			return oid3;
		}

		public static Oid operator +(Oid oid1, uint id)
		{
			if ((object)oid1 == null)
			{
				return null;
			}
			Oid oid2 = new Oid(oid1);
			oid2.Add(id);
			return oid2;
		}

		public static explicit operator uint[](Oid oid)
		{
			return oid.ToArray();
		}

		public static bool operator ==(Oid oid1, Oid oid2)
		{
			if ((object)oid1 == null && (object)oid2 == null)
			{
				return true;
			}
			if ((object)oid1 == null || (object)oid2 == null)
			{
				return false;
			}
			return oid1.Equals(oid2);
		}

		public static bool operator !=(Oid oid1, Oid oid2)
		{
			if ((object)oid1 == null && (object)oid2 == null)
			{
				return false;
			}
			if ((object)oid1 == null || (object)oid2 == null)
			{
				return true;
			}
			return !oid1.Equals(oid2);
		}

		public static bool operator >(Oid oid1, Oid oid2)
		{
			if ((object)oid1 == null && (object)oid2 == null)
			{
				return false;
			}
			if ((object)oid1 == null)
			{
				return false;
			}
			if ((object)oid2 == null)
			{
				return true;
			}
			if (oid1.Compare(oid2) > 0)
			{
				return true;
			}
			return false;
		}

		public static bool operator <(Oid oid1, Oid oid2)
		{
			if ((object)oid1 == null && (object)oid2 == null)
			{
				return false;
			}
			if ((object)oid1 == null)
			{
				return true;
			}
			if ((object)oid2 == null)
			{
				return false;
			}
			if (oid1.Compare(oid2) < 0)
			{
				return true;
			}
			return false;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (_data == null)
			{
				stringBuilder.Append("0.0");
			}
			else
			{
				for (int i = 0; i < _data.Length; i++)
				{
					if (i > 0)
					{
						stringBuilder.Append('.');
					}
					stringBuilder.Append(_data[i].ToString());
				}
			}
			return stringBuilder.ToString();
		}

		public override int GetHashCode()
		{
			int num = 0;
			if (_data == null)
			{
				return num;
			}
			for (int i = 0; i < _data.Length; i++)
			{
				num ^= (int)((_data[i] > int.MaxValue) ? int.MaxValue : _data[i]);
			}
			return num;
		}

		public override object Clone()
		{
			return new Oid(this);
		}

		public override void encode(MutableByte buffer)
		{
			MutableByte mutableByte = new MutableByte();
			uint[] array = _data;
			if (array == null || array.Length < 2)
			{
				array = new uint[2];
				array[0] = (array[1] = 0u);
			}
			if (_data.Length < 2)
			{
				array = new uint[2]
				{
					0u,
					0u
				};
			}
			if (array[0] < 0 || array[0] > 2)
			{
				throw new SnmpException("Invalid Object Identifier");
			}
			if (array[1] < 0 || array[1] > 40)
			{
				throw new SnmpException("Invalid Object Identifier");
			}
			mutableByte.Append((byte)(array[0] * 40 + array[1]));
			for (int i = 2; i < array.Length; i++)
			{
				mutableByte.Append(encodeInstance(array[i]));
			}
			AsnType.BuildHeader(buffer, base.Type, mutableByte.Length);
			buffer.Append(mutableByte);
		}

		protected byte[] encodeInstance(uint number)
		{
			MutableByte mutableByte = new MutableByte();
			if (number <= 127)
			{
				mutableByte.Set((byte)number);
			}
			else
			{
				uint num = number;
				MutableByte mutableByte2 = new MutableByte();
				while (num != 0)
				{
					byte[] bytes = BitConverter.GetBytes(num);
					byte b = bytes[0];
					if ((b & 0x80u) != 0)
					{
						b = (byte)(b & ~AsnType.HIGH_BIT);
					}
					num >>= 7;
					mutableByte2.Append(b);
				}
				for (int num2 = mutableByte2.Length - 1; num2 >= 0; num2--)
				{
					if (num2 > 0)
					{
						mutableByte.Append((byte)(mutableByte2[num2] | AsnType.HIGH_BIT));
					}
					else
					{
						mutableByte.Append(mutableByte2[num2]);
					}
				}
			}
			return mutableByte;
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
				throw new OverflowException("Buffer underflow error");
			}
			if (length == 0)
			{
				_data = null;
				return offset;
			}
			List<uint> list = new List<uint>();
			length--;
			uint num = Convert.ToUInt32(buffer[offset++]);
			list.Add(num / 40u);
			list.Add(num % 40u);
			while (length > 0)
			{
				uint num2 = 0u;
				if ((buffer[offset] & AsnType.HIGH_BIT) == 0)
				{
					num2 = buffer[offset];
					offset++;
					length--;
				}
				else
				{
					MutableByte mutableByte = new MutableByte();
					bool flag = false;
					do
					{
						mutableByte.Append((byte)(buffer[offset] & ~AsnType.HIGH_BIT));
						if ((buffer[offset] & AsnType.HIGH_BIT) == 0)
						{
							flag = true;
						}
						offset++;
						length--;
					}
					while (!flag);
					for (int i = 0; i < mutableByte.Length; i++)
					{
						num2 <<= 7;
						num2 |= mutableByte[i];
					}
				}
				list.Add(num2);
			}
			_data = list.ToArray();
			if (_data.Length == 2 && _data[0] == 0 && _data[1] == 0)
			{
				_data = null;
			}
			return offset;
		}

		private static uint[] Parse(string oidStr)
		{
			if (oidStr == null || oidStr.Length <= 0)
			{
				return null;
			}
			char[] array = oidStr.ToCharArray();
			foreach (char c in array)
			{
				if (!char.IsNumber(c) && c != '.')
				{
					return null;
				}
			}
			if (oidStr[0] == '.')
			{
				oidStr = oidStr.Remove(0, 1);
			}
			string[] array2 = oidStr.Split(new char[1]
			{
				'.'
			}, StringSplitOptions.None);
			if (array2.Length < 0)
			{
				return null;
			}
			List<uint> list = new List<uint>();
			string[] array3 = array2;
			foreach (string value in array3)
			{
				list.Add(Convert.ToUInt32(value));
			}
			return list.ToArray();
		}

		public IEnumerator<uint> GetEnumerator()
		{
			if (_data != null)
			{
				return ((IEnumerable<uint>)_data).GetEnumerator();
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

		public static Oid NullOid()
		{
			uint[] data = new uint[2];
			return new Oid(data);
		}
	}
}
