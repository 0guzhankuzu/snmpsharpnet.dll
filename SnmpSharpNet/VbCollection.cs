using System;
using System.Collections;
using System.Collections.Generic;

namespace SnmpSharpNet
{
	public class VbCollection : AsnType, IEnumerable<Vb>, IEnumerable
	{
		private List<Vb> _vbs;

		public int Count => _vbs.Count;

		public Vb this[int index]
		{
			get
			{
				if (index < 0 && index >= _vbs.Count)
				{
					throw new IndexOutOfRangeException("Requested VarBind entry is outside the collection range.");
				}
				return _vbs[index];
			}
		}

		public Vb this[Oid oid]
		{
			get
			{
				if (!ContainsOid(oid))
				{
					return null;
				}
				foreach (Vb vb in _vbs)
				{
					if (vb.Oid.Equals(oid))
					{
						return vb;
					}
				}
				return null;
			}
		}

		public Vb this[string oid]
		{
			get
			{
				foreach (Vb vb in _vbs)
				{
					if (vb.Oid.Equals(oid))
					{
						return vb;
					}
				}
				return null;
			}
		}

		public VbCollection()
		{
			base.Type = SnmpConstants.SMI_SEQUENCE;
			_vbs = new List<Vb>();
		}

		public VbCollection(IEnumerable<Vb> second)
		{
			base.Type = SnmpConstants.SMI_SEQUENCE;
			_vbs = new List<Vb>();
			foreach (Vb item in second)
			{
				_vbs.Add(item);
			}
		}

		public void Clear()
		{
			_vbs.Clear();
		}

		public void RemoveAt(int pos)
		{
			if (pos < 0 && pos >= _vbs.Count)
			{
				throw new IndexOutOfRangeException("Requested VarBind entry is outside the collection range.");
			}
			_vbs.RemoveAt(pos);
		}

		public void Insert(int pos, Vb item)
		{
			if (pos < 0 && pos >= _vbs.Count)
			{
				throw new IndexOutOfRangeException("Requested VarBind position is outside the collection range.");
			}
			_vbs.Insert(pos, item);
		}

		public void Add(Vb vb)
		{
			_vbs.Add(vb);
		}

		public void Add(string oid)
		{
			Oid oid2 = new Oid(oid);
			Vb vb = new Vb(oid2);
			Add(vb);
		}

		public void Add(Oid oid)
		{
			if (oid == null)
			{
				throw new ArgumentNullException("oid", "Can't create vb entry with null Oid.");
			}
			Vb vb = new Vb(oid);
			Add(vb);
		}

		public void Add(Oid oid, AsnType value)
		{
			Vb vb = new Vb(oid, value);
			Add(vb);
		}

		public void Add(IEnumerable<Vb> varList)
		{
			if (varList == null)
			{
				return;
			}
			foreach (Vb var in varList)
			{
				Add(var);
			}
		}

		public void Add(IEnumerable<Oid> oidList)
		{
			if (oidList == null)
			{
				return;
			}
			foreach (Oid oid in oidList)
			{
				Vb vb = new Vb(oid);
				Add(vb);
			}
		}

		public bool ContainsOid(Oid oid)
		{
			if (oid == null)
			{
				return false;
			}
			foreach (Vb vb in _vbs)
			{
				if (vb.Oid.Equals(oid))
				{
					return true;
				}
			}
			return false;
		}

		public Oid[] OidArray()
		{
			List<Oid> list = new List<Oid>();
			foreach (Vb vb in _vbs)
			{
				list.Add((Oid)vb.Oid.Clone());
			}
			return list.ToArray();
		}

		public IEnumerator<Vb> GetEnumerator()
		{
			return _vbs.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _vbs.GetEnumerator();
		}

		public override object Clone()
		{
			return new VbCollection(this);
		}

		public override void encode(MutableByte buffer)
		{
			MutableByte mutableByte = new MutableByte();
			foreach (Vb vb in _vbs)
			{
				vb.encode(mutableByte);
			}
			AsnType.BuildHeader(buffer, base.Type, mutableByte.Length);
			buffer.Append(mutableByte);
		}

		public override int decode(byte[] buffer, int offset)
		{
			int length;
			byte b = AsnType.ParseHeader(buffer, ref offset, out length);
			if (b != base.Type)
			{
				throw new SnmpException("Invalid ASN.1 encoding for variable binding list.");
			}
			_vbs.Clear();
			int num = offset;
			while (length > 0)
			{
				Vb vb = new Vb();
				offset = vb.decode(buffer, offset);
				length -= offset - num;
				num = offset;
				_vbs.Add(vb);
			}
			return offset;
		}
	}
}
