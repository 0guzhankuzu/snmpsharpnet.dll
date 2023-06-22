using System;
using System.Text;

namespace SnmpSharpNet
{
	public class MutableByte : ICloneable, IComparable<MutableByte>, IComparable<byte[]>
	{
		private byte[] _buffer;

		internal byte[] Value => _buffer;

		public int Length
		{
			get
			{
				if (_buffer == null)
				{
					return 0;
				}
				return _buffer.Length;
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
				return _buffer[index];
			}
			set
			{
				if (index >= 0 && index < Length)
				{
					_buffer[index] = value;
				}
			}
		}

		public MutableByte()
		{
		}

		public MutableByte(byte[] buf)
		{
			if (buf != null)
			{
				_buffer = new byte[buf.Length];
				Buffer.BlockCopy(buf, 0, _buffer, 0, buf.Length);
			}
		}

		public MutableByte(byte[] buf1, byte[] buf2)
		{
			if (buf1 == null || buf1.Length == 0)
			{
				throw new ArgumentNullException("buf1");
			}
			if (buf2 == null || buf2.Length == 0)
			{
				throw new ArgumentNullException("buf2");
			}
			_buffer = new byte[buf1.Length + buf2.Length];
			Buffer.BlockCopy(buf1, 0, _buffer, 0, buf1.Length);
			Buffer.BlockCopy(buf2, 0, _buffer, buf1.Length, buf2.Length);
		}

		public MutableByte(byte[] buf, int buflen)
		{
			Set(buf, buflen);
		}

		public void Set(byte[] buf)
		{
			_buffer = null;
			if (buf != null && buf.Length != 0)
			{
				_buffer = new byte[buf.Length];
				Buffer.BlockCopy(buf, 0, _buffer, 0, buf.Length);
			}
		}

		public void Set(byte[] buf, int length)
		{
			_buffer = null;
			if (buf == null || buf.Length == 0)
			{
				throw new ArgumentNullException("buf", "Byte array is null.");
			}
			_buffer = new byte[length];
			Buffer.BlockCopy(buf, 0, _buffer, 0, length);
		}

		public void Set(byte buf)
		{
			_buffer = new byte[1];
			_buffer[0] = buf;
		}

		public void Set(int position, byte value)
		{
			if (position >= 0 && position < Length)
			{
				_buffer[position] = value;
			}
		}

		public void Set(byte[] value, int offset, int length)
		{
			if (offset < 0 || length < 0 || offset + length > value.Length)
			{
				throw new ArgumentOutOfRangeException();
			}
			_buffer = new byte[length];
			Buffer.BlockCopy(value, offset, _buffer, 0, length);
		}

		public void Set(string value)
		{
			if (value == null || value.Length <= 0)
			{
				_buffer = null;
			}
			else
			{
				Set(Encoding.UTF8.GetBytes(value));
			}
		}

		public void Append(byte[] buf)
		{
			if (buf != null && buf.Length != 0)
			{
				if (_buffer == null)
				{
					Set(buf);
					return;
				}
				int dstOffset = _buffer.Length;
				Array.Resize(ref _buffer, _buffer.Length + buf.Length);
				Buffer.BlockCopy(buf, 0, _buffer, dstOffset, buf.Length);
			}
		}

		public void Append(byte buf)
		{
			if (_buffer == null)
			{
				Set(buf);
				return;
			}
			Array.Resize(ref _buffer, _buffer.Length + 1);
			_buffer[_buffer.Length - 1] = buf;
		}

		public void Insert(int position, byte[] buf)
		{
			if (position < 0 || position >= Length)
			{
				throw new ArgumentOutOfRangeException("position", "Index outside of the buffer scope");
			}
			if (buf == null)
			{
				throw new ArgumentNullException("buf");
			}
			if (position == 0)
			{
				Prepend(buf);
				return;
			}
			byte[] array = new byte[_buffer.Length + buf.Length];
			Buffer.BlockCopy(_buffer, 0, array, 0, position);
			Buffer.BlockCopy(buf, 0, array, position, buf.Length);
			Buffer.BlockCopy(_buffer, position, array, position + buf.Length, _buffer.Length - position);
			_buffer = array;
		}

		public void Insert(int position, byte buf)
		{
			if (position < 0 || position >= Length)
			{
				throw new ArgumentOutOfRangeException("position", "Index outside of the buffer scope");
			}
			if (position == 0)
			{
				Prepend(buf);
				return;
			}
			byte[] array = new byte[_buffer.Length + 1];
			Buffer.BlockCopy(_buffer, 0, array, 0, position);
			array[position] = buf;
			Buffer.BlockCopy(_buffer, position, array, position + 2, _buffer.Length - position);
			_buffer = array;
		}

		public void Prepend(byte[] buf)
		{
			if (Length <= 0)
			{
				Set(buf);
				return;
			}
			byte[] array = new byte[_buffer.Length + buf.Length];
			Buffer.BlockCopy(buf, 0, array, 0, buf.Length);
			Buffer.BlockCopy(_buffer, 0, array, buf.Length, _buffer.Length);
			_buffer = array;
		}

		public void Prepend(byte buf)
		{
			if (Length <= 0)
			{
				Set(buf);
				return;
			}
			byte[] array = new byte[_buffer.Length + 1];
			array[0] = buf;
			Buffer.BlockCopy(_buffer, 0, array, 1, _buffer.Length);
			_buffer = array;
		}

		public void RemoveBeginning(int count)
		{
			if (Length == 0)
			{
				throw new ArgumentOutOfRangeException("count", "Buffer is length 0. Unable to remove members.");
			}
			if (count > _buffer.Length)
			{
				throw new ArgumentOutOfRangeException("count", "Byte count is greater then the length of the array");
			}
			if (count == Length)
			{
				Reset();
				return;
			}
			byte[] array = new byte[_buffer.Length - count];
			Buffer.BlockCopy(_buffer, count, array, 0, _buffer.Length - count);
			_buffer = array;
		}

		public void RemoveEnd(int count)
		{
			if (Length == 0)
			{
				throw new ArgumentOutOfRangeException("count", "Buffer is length 0. Unable to remove members.");
			}
			if (count > _buffer.Length)
			{
				throw new ArgumentOutOfRangeException("count", "Byte count is greater then the length of the array");
			}
			if (count == Length)
			{
				Reset();
			}
			else
			{
				Array.Resize(ref _buffer, _buffer.Length - count);
			}
		}

		public void Remove(int start, int count)
		{
			if (_buffer.Length == 0)
			{
				throw new ArgumentOutOfRangeException("start", "Byte array is empty. Unable to remove members.");
			}
			if (start < 0 || start >= Length)
			{
				throw new ArgumentOutOfRangeException("start", "Start argument is beyond the bounds of the array.");
			}
			if (count > Length || start + count > Length || count < 1)
			{
				throw new ArgumentOutOfRangeException("count", "Length argument is beyond the bounds of the array.");
			}
			if (start == 0)
			{
				RemoveBeginning(count);
				return;
			}
			if (start + count == Length)
			{
				RemoveEnd(count);
				return;
			}
			byte[] array = new byte[_buffer.Length - count];
			Buffer.BlockCopy(_buffer, 0, array, 0, start);
			Buffer.BlockCopy(_buffer, start + count, array, start, _buffer.Length - start - count);
			_buffer = array;
		}

		public MutableByte Get(int position, int length)
		{
			if (_buffer.Length <= position || _buffer.Length < position + length)
			{
				throw new OverflowException("Buffer is too small to extract sub-array.\r\n" + $"buffer length: {_buffer.Length} offset: {position} length: {length}");
			}
			byte[] array = new byte[length];
			Buffer.BlockCopy(_buffer, position, array, 0, length);
			return new MutableByte(array);
		}

		public static MutableByte operator +(MutableByte buf1, byte[] buf2)
		{
			return new MutableByte(buf1.Value, buf2);
		}

		public static MutableByte operator +(MutableByte buf1, MutableByte buf2)
		{
			return new MutableByte(buf1, buf2);
		}

		public static MutableByte operator +(MutableByte buf1, byte b)
		{
			MutableByte mutableByte = new MutableByte(buf1.Value);
			mutableByte.Append(b);
			return mutableByte;
		}

		public static bool operator ==(MutableByte buf1, MutableByte buf2)
		{
			if ((object)buf1 == null && (object)buf2 == null)
			{
				return true;
			}
			if ((object)buf1 == null || (object)buf2 == null)
			{
				return false;
			}
			return buf1.Equals(buf2);
		}

		public static bool operator !=(MutableByte buf1, MutableByte buf2)
		{
			if ((object)buf1 == null && (object)buf2 == null)
			{
				return false;
			}
			if ((object)buf1 == null || (object)buf2 == null)
			{
				return true;
			}
			return !buf1.Equals(buf2);
		}

		public static implicit operator byte[](MutableByte obj)
		{
			return obj.Value;
		}

		public static bool operator <(MutableByte firstClass, MutableByte secondClass)
		{
			if (firstClass == null && secondClass == null)
			{
				return false;
			}
			if (firstClass == null && secondClass != null)
			{
				return true;
			}
			if (firstClass != null && secondClass == null)
			{
				return false;
			}
			if (firstClass.CompareTo(secondClass) < 0)
			{
				return true;
			}
			return false;
		}

		public static bool operator >(MutableByte firstClass, MutableByte secondClass)
		{
			if (firstClass == null && secondClass == null)
			{
				return false;
			}
			if (firstClass == null && secondClass != null)
			{
				return false;
			}
			if (firstClass != null && secondClass == null)
			{
				return true;
			}
			if (firstClass.CompareTo(secondClass) < 0)
			{
				return false;
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is MutableByte)
			{
				MutableByte other = obj as MutableByte;
				if (CompareTo(other) == 0)
				{
					return true;
				}
			}
			else if (obj is byte[])
			{
				byte[] other2 = obj as byte[];
				if (CompareTo(other2) == 0)
				{
					return true;
				}
			}
			return false;
		}

		public static bool Equals(byte[] buf1, byte[] buf2)
		{
			if (buf1.Length != buf2.Length)
			{
				return false;
			}
			for (int i = 0; i < buf1.Length; i++)
			{
				if (buf1[i] != buf2[i])
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

		public override string ToString()
		{
			if (_buffer == null)
			{
				return "";
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < _buffer.Length; i++)
			{
				stringBuilder.Append($"{_buffer[i]:x02} ");
				if (i > 0 && i < _buffer.Length - 1 && i % 16 == 0)
				{
					stringBuilder.Append("\n");
				}
				else if (i < _buffer.Length - 1)
				{
					stringBuilder.Append(" ");
				}
			}
			return stringBuilder.ToString();
		}

		public string ToString(int start, int length)
		{
			if (_buffer == null)
			{
				return "";
			}
			if (_buffer.Length <= start || _buffer.Length < start + length)
			{
				throw new ArgumentOutOfRangeException("start", "Range specification past boundaries of the buffer.");
			}
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			int num = 0;
			stringBuilder.AppendFormat("{0:d03}  ", start);
			for (int i = start; i < start + length; i++)
			{
				stringBuilder.AppendFormat("{0:x2}", _buffer[i]);
				if (_buffer[i] > 31 && _buffer[i] < 128)
				{
					stringBuilder2.Append(Convert.ToChar(_buffer[i]));
				}
				num++;
				if (num == 16)
				{
					stringBuilder.Append("    ");
					stringBuilder.Append(stringBuilder2.ToString());
					stringBuilder.Append("\n");
					stringBuilder.AppendFormat("{0:d03}  ", i + 1);
					stringBuilder2.Remove(0, stringBuilder2.Length);
					num = 0;
				}
				else
				{
					stringBuilder.Append(" ");
				}
			}
			return stringBuilder.ToString();
		}

		public object Clone()
		{
			return new MutableByte(_buffer);
		}

		public void Reset()
		{
			_buffer = null;
		}

		public void Clear()
		{
			_buffer = null;
		}

		public int CompareTo(MutableByte other)
		{
			if (other.Length > Length)
			{
				return -1;
			}
			if (other.Length < Length)
			{
				return 1;
			}
			for (int i = 0; i < Length; i++)
			{
				if (other.Value[i] > Value[i])
				{
					return -1;
				}
				if (other.Value[i] < Value[i])
				{
					return 1;
				}
			}
			return 0;
		}

		public int CompareTo(byte[] other)
		{
			if (other.Length > Length)
			{
				return -1;
			}
			if (other.Length < Length)
			{
				return 1;
			}
			for (int i = 0; i < Length; i++)
			{
				if (other[i] > Value[i])
				{
					return -1;
				}
				if (other[i] < Value[i])
				{
					return 1;
				}
			}
			return 0;
		}
	}
}
