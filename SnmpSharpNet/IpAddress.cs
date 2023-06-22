using System;
using System.Net;
using System.Text;

namespace SnmpSharpNet
{
	[Serializable]
	public class IpAddress : OctetString, IComparable, ICloneable
	{
		public static int ClassA = 1;

		public static int ClassB = 2;

		public static int ClassC = 3;

		public static int ClassD = 4;

		public static int ClassE = 5;

		public static int InvalidClass = 0;

		public bool Valid
		{
			get
			{
				if (_data != null && _data.Length == 4 && (_data[0] != 0 || _data[1] != 0 || _data[2] != 0 || _data[3] != 0))
				{
					return true;
				}
				return false;
			}
		}

		public IpAddress()
		{
			_asnType = SnmpConstants.SMI_IPADDRESS;
			byte[] array = (_data = new byte[4]);
		}

		public IpAddress(byte[] data)
			: this()
		{
			_asnType = SnmpConstants.SMI_IPADDRESS;
			if (data.Length != 4)
			{
				throw new OverflowException("Too much data passed to constructor: " + data.Length);
			}
			((OctetString)this).Set(data);
		}

		public IpAddress(IpAddress second)
			: this()
		{
			if (second == null)
			{
				throw new ArgumentException("Constructor argument cannot be null.");
			}
			if (!second.Valid)
			{
				byte[] array = (_data = new byte[4]);
			}
			else
			{
				((OctetString)this).Set(second.GetData());
			}
		}

		public IpAddress(OctetString second)
			: this(second.GetData())
		{
		}

		public IpAddress(IPAddress inetAddr)
			: this(inetAddr.GetAddressBytes())
		{
			_asnType = SnmpConstants.SMI_IPADDRESS;
		}

		public IpAddress(string inetAddr)
			: this()
		{
			((OctetString)this).Set(inetAddr);
		}

		public IpAddress(uint inetAddr)
			: this()
		{
			Set(inetAddr);
		}

		public override object Clone()
		{
			return new IpAddress(this);
		}

		public override void Set(string value)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Invalid comparison between Unknown and I4
			IPAddress val = default(IPAddress);
			if (!IPAddress.TryParse(value, ref val))
			{
				try
				{
					IPHostEntry hostEntry = Dns.GetHostEntry(value);
					IPAddress[] addressList = hostEntry.get_AddressList();
					foreach (IPAddress val2 in addressList)
					{
						if ((int)val2.get_AddressFamily() == 2)
						{
							_data = val2.GetAddressBytes();
							break;
						}
					}
				}
				catch
				{
					throw new ArgumentException("Unable to parse or resolve supplied value to an IP address.", "value");
				}
			}
			else
			{
				_data = val.GetAddressBytes();
			}
		}

		public void Set(uint ipvalue)
		{
			_data = new byte[4];
			_data[0] = (byte)ipvalue;
			_data[1] = (byte)(ipvalue >> 8);
			_data[2] = (byte)(ipvalue >> 16);
			_data[3] = (byte)(ipvalue >> 24);
		}

		public void Set(IPAddress ipaddr)
		{
			base.Set(ipaddr.GetAddressBytes());
		}

		public static explicit operator IPAddress(IpAddress ipaddr)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Expected O, but got Unknown
			if (ipaddr.Length != 4)
			{
				return IPAddress.Any;
			}
			return new IPAddress(ipaddr.GetData());
		}

		public override string ToString()
		{
			if (_data == null)
			{
				return "";
			}
			byte[] data = _data;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append((data[0] < 0) ? (256 + data[0]) : data[0]).Append('.');
			stringBuilder.Append((data[1] < 0) ? (256 + data[1]) : data[1]).Append('.');
			stringBuilder.Append((data[2] < 0) ? (256 + data[2]) : data[2]).Append('.');
			stringBuilder.Append((data[3] < 0) ? (256 + data[3]) : data[3]);
			return stringBuilder.ToString();
		}

		public int CompareTo(object obj)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected O, but got Unknown
			byte[] array = null;
			if (obj == null)
			{
				return -1;
			}
			if (obj is IPAddress)
			{
				IPAddress val = (IPAddress)obj;
				array = val.GetAddressBytes();
			}
			else if (obj is byte[])
			{
				array = (byte[])obj;
			}
			else if (obj is uint)
			{
				IpAddress ipAddress = new IpAddress((uint)obj);
				array = ipAddress.ToArray();
			}
			else if (obj is IpAddress)
			{
				array = ((IpAddress)obj).ToArray();
			}
			else if (obj is OctetString)
			{
				array = ((OctetString)obj).ToArray();
			}
			if (_data == null)
			{
				return -1;
			}
			if (array.Length != _data.Length)
			{
				if (_data.Length < array.Length)
				{
					return -1;
				}
				return 1;
			}
			for (int i = 0; i < _data.Length; i++)
			{
				if (_data[i] < array[i])
				{
					return -1;
				}
				if (_data[i] > array[i])
				{
					return 1;
				}
			}
			return 0;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			return (CompareTo(obj) == 0) ? true : false;
		}

		public override int GetHashCode()
		{
			if (_data == null || _data.Length != 4)
			{
				return 0;
			}
			return Convert.ToInt32(_data[0]) + Convert.ToInt32(_data[1]) + Convert.ToInt32(_data[2]) + Convert.ToInt32(_data[3]);
		}

		public override int decode(byte[] buffer, int offset)
		{
			offset = base.decode(buffer, offset);
			if (_data.Length != 4)
			{
				_data = null;
				throw new OverflowException("ASN.1 decoding error. Invalid data length.");
			}
			return offset;
		}

		public int GetClass()
		{
			byte b = _data[0];
			if ((b & 0x80) == 0)
			{
				return ClassA;
			}
			if ((b & 0x80u) != 0 && (b & 0x40) == 0)
			{
				return ClassB;
			}
			if ((b & 0x80u) != 0 && (b & 0x40u) != 0 && (b & 0x20) == 0)
			{
				return ClassC;
			}
			if ((b & 0x80u) != 0 && (b & 0x40u) != 0 && (b & 0x20u) != 0 && (b & 0x10) == 0)
			{
				return ClassD;
			}
			if ((b & 0x80u) != 0 && (b & 0x40u) != 0 && (b & 0x20u) != 0 && (b & 0x10u) != 0)
			{
				return ClassE;
			}
			return InvalidClass;
		}

		public uint ToUInt32()
		{
			uint num = (uint)(_data[3] << 24);
			num += (uint)(_data[2] << 16);
			num += (uint)(_data[1] << 8);
			return num + _data[0];
		}

		public IpAddress GetSubnetAddress(IpAddress mask)
		{
			byte[] data = _data;
			byte[] array = mask.ToArray();
			byte[] array2 = new byte[4];
			for (int i = 0; i < 4; i++)
			{
				array2[i] = (byte)(data[i] & array[i]);
			}
			return new IpAddress(array2);
		}

		public IpAddress Invert()
		{
			byte[] array = new byte[8]
			{
				128,
				64,
				32,
				16,
				8,
				4,
				2,
				1
			};
			byte[] data = _data;
			byte[] array2 = new byte[4];
			byte[] array3 = array2;
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < array.Length; j++)
				{
					if ((data[i] & array[j]) == 0)
					{
						array3[i] |= array[j];
					}
				}
			}
			return new IpAddress(array3);
		}

		public IpAddress GetBroadcastAddress(IpAddress mask)
		{
			IpAddress ipAddress = mask.Invert();
			byte[] data = _data;
			byte[] data2 = ipAddress.GetData();
			byte[] array = new byte[4];
			for (int i = 0; i < 4; i++)
			{
				array[i] = (byte)(data[i] | data2[i]);
			}
			return new IpAddress(array);
		}

		public IpAddress NetworkMask()
		{
			return GetClass() switch
			{
				1 => new IpAddress(new byte[4]
				{
					255,
					0,
					0,
					0
				}), 
				2 => new IpAddress(new byte[4]
				{
					255,
					255,
					0,
					0
				}), 
				3 => new IpAddress(new byte[4]
				{
					255,
					255,
					255,
					0
				}), 
				_ => null, 
			};
		}

		public bool IsValidMask()
		{
			byte[] array = new byte[8]
			{
				128,
				64,
				32,
				16,
				8,
				4,
				2,
				1
			};
			byte[] data = _data;
			bool flag = false;
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < array.Length; j++)
				{
					if ((data[i] & array[j]) == 0 && !flag)
					{
						flag = true;
					}
					if ((data[i] & array[j]) != 0 && flag)
					{
						return false;
					}
				}
			}
			return true;
		}

		public int GetMaskBits()
		{
			if (!IsValidMask())
			{
				return 0;
			}
			byte[] array = new byte[8]
			{
				128,
				64,
				32,
				16,
				8,
				4,
				2,
				1
			};
			byte[] data = _data;
			int num = 0;
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < array.Length; j++)
				{
					if ((data[i] & array[j]) != 0)
					{
						num++;
					}
				}
			}
			return num;
		}

		public static IpAddress BuildMaskFromBits(int bits)
		{
			byte[] array = new byte[8]
			{
				128,
				64,
				32,
				16,
				8,
				4,
				2,
				1
			};
			byte[] array2 = new byte[4];
			byte[] array3 = array2;
			int num = 0;
			for (int i = 0; i <= 3; i++)
			{
				for (int j = 0; j < array.Length; j++)
				{
					if (num < bits)
					{
						array3[i] |= array[j];
						num++;
					}
				}
			}
			return new IpAddress(array3);
		}

		public static uint ReverseByteOrder(uint val)
		{
			byte[] bytes = BitConverter.GetBytes(val);
			return BitConverter.ToUInt32(new byte[4]
			{
				bytes[3],
				bytes[2],
				bytes[1],
				bytes[0]
			}, 0);
		}

		public IpAddress Increment(uint count)
		{
			uint val = ToUInt32();
			uint num = ReverseByteOrder(val);
			num += count;
			return new IpAddress(ReverseByteOrder(num));
		}

		public static bool IsIP(string val)
		{
			if (val.Length == 0 || val.Length < 7 || val.Length > 15)
			{
				return false;
			}
			bool flag = true;
			int num = 0;
			foreach (char c in val)
			{
				if (!char.IsDigit(c) && c != '.')
				{
					flag = false;
					break;
				}
				if (c == '.')
				{
					num++;
				}
			}
			if (!flag)
			{
				return false;
			}
			if (num != 3)
			{
				return false;
			}
			string[] array = val.Split(new char[1]
			{
				'.'
			});
			if (array.Length != 4)
			{
				return false;
			}
			for (int j = 0; j < 4; j++)
			{
				int num2 = Convert.ToInt32(array[j]);
				if (num2 < 0 || num2 > 255)
				{
					return false;
				}
			}
			return true;
		}
	}
}
