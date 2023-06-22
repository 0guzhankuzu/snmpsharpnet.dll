using System;

namespace SnmpSharpNet
{
	public sealed class SnmpConstants
	{
		public const int ErrNoError = 0;

		public const int ErrTooBig = 1;

		public const int ErrNoSuchName = 2;

		public const int ErrBadValue = 3;

		public const int ErrReadOnly = 4;

		public const int ErrGenError = 5;

		public const int enterpriseSpecific = 6;

		public const int ErrNoAccess = 6;

		public const int ErrWrongType = 7;

		public const int ErrWrongLength = 8;

		public const int ErrWrongEncoding = 9;

		public const int ErrWrongValue = 10;

		public const int ErrNoCreation = 11;

		public const int ErrInconsistentValue = 12;

		public const int ErrResourceUnavailable = 13;

		public const int ErrCommitFailed = 14;

		public const int ErrUndoFailed = 15;

		public const int ErrAuthorizationError = 16;

		public const int ErrNotWritable = 17;

		public const int ErrInconsistentName = 18;

		public const int ColdStart = 0;

		public const int WarmStart = 1;

		public const int LinkDown = 2;

		public const int LinkUp = 3;

		public const int AuthenticationFailure = 4;

		public const int EgpNeighborLoss = 5;

		public const int EnterpriseSpecific = 6;

		public static readonly byte SMI_INTEGER = (byte)(AsnType.UNIVERSAL | AsnType.INTEGER);

		public static readonly string SMI_INTEGER_STR = "Integer32";

		public static readonly byte SMI_STRING = (byte)(AsnType.UNIVERSAL | AsnType.OCTETSTRING);

		public static readonly string SMI_STRING_STR = "OctetString";

		public static readonly byte SMI_OBJECTID = (byte)(AsnType.UNIVERSAL | AsnType.OBJECTID);

		public static readonly string SMI_OBJECTID_STR = "ObjectId";

		public static readonly byte SMI_NULL = (byte)(AsnType.UNIVERSAL | AsnType.NULL);

		public static readonly string SMI_NULL_STR = "NULL";

		public static readonly byte SMI_APPSTRING = AsnType.APPLICATION;

		public static readonly string SMI_APPSTRING_STR = "AppString";

		public static readonly byte SMI_IPADDRESS = AsnType.APPLICATION;

		public static readonly string SMI_IPADDRESS_STR = "IPAddress";

		public static readonly byte SMI_COUNTER32 = (byte)(AsnType.APPLICATION | 1u);

		public static readonly string SMI_COUNTER32_STR = "Counter32";

		public static readonly byte SMI_GAUGE32 = (byte)(AsnType.APPLICATION | 2u);

		public static readonly string SMI_GAUGE32_STR = "Gauge32";

		public static readonly byte SMI_UNSIGNED32 = (byte)(AsnType.APPLICATION | 2u);

		public static readonly string SMI_UNSIGNED32_STR = "Unsigned32";

		public static readonly byte SMI_TIMETICKS = (byte)(AsnType.APPLICATION | 3u);

		public static readonly string SMI_TIMETICKS_STR = "TimeTicks";

		public static readonly byte SMI_OPAQUE = (byte)(AsnType.APPLICATION | 4u);

		public static readonly string SMI_OPAQUE_STR = "Opaque";

		public static readonly byte SMI_COUNTER64 = (byte)(AsnType.APPLICATION | 6u);

		public static readonly string SMI_COUNTER64_STR = "Counter64";

		public static readonly string SMI_UNKNOWN_STR = "Unknown";

		public static readonly byte SMI_NOSUCHOBJECT = (byte)(AsnType.CONTEXT | AsnType.PRIMITIVE);

		public static readonly byte SMI_NOSUCHINSTANCE = (byte)((uint)(AsnType.CONTEXT | AsnType.PRIMITIVE) | 1u);

		public static readonly byte SMI_ENDOFMIBVIEW = (byte)((uint)(AsnType.CONTEXT | AsnType.PRIMITIVE) | 2u);

		public static readonly byte SMI_SEQUENCE = (byte)(AsnType.SEQUENCE | AsnType.CONSTRUCTOR);

		public static readonly byte SMI_PARTY_CLOCK = (byte)(AsnType.APPLICATION | 7u);

		public static Oid SysUpTime = new Oid(new uint[9]
		{
			1u,
			3u,
			6u,
			1u,
			2u,
			1u,
			1u,
			3u,
			0u
		});

		public static Oid TrapObjectId = new Oid(new uint[11]
		{
			1u,
			3u,
			6u,
			1u,
			6u,
			3u,
			1u,
			1u,
			4u,
			1u,
			0u
		});

		public static Oid usmStatsUnsupportedSecLevels = new Oid(new uint[11]
		{
			1u,
			3u,
			6u,
			1u,
			6u,
			3u,
			15u,
			1u,
			1u,
			1u,
			0u
		});

		public static Oid usmStatsNotInTimeWindows = new Oid(new uint[11]
		{
			1u,
			3u,
			6u,
			1u,
			6u,
			3u,
			15u,
			1u,
			1u,
			2u,
			0u
		});

		public static Oid usmStatsUnknownSecurityNames = new Oid(new uint[11]
		{
			1u,
			3u,
			6u,
			1u,
			6u,
			3u,
			15u,
			1u,
			1u,
			3u,
			0u
		});

		public static Oid usmStatsUnknownEngineIDs = new Oid(new uint[11]
		{
			1u,
			3u,
			6u,
			1u,
			6u,
			3u,
			15u,
			1u,
			1u,
			4u,
			0u
		});

		public static Oid usmStatsWrongDigests = new Oid(new uint[11]
		{
			1u,
			3u,
			6u,
			1u,
			6u,
			3u,
			15u,
			1u,
			1u,
			5u,
			0u
		});

		public static Oid usmStatsDecryptionErrors = new Oid(new uint[11]
		{
			1u,
			3u,
			6u,
			1u,
			6u,
			3u,
			15u,
			1u,
			1u,
			6u,
			0u
		});

		public static Oid snmpUnknownSecurityModels = new Oid(new uint[11]
		{
			1u,
			3u,
			6u,
			1u,
			6u,
			3u,
			11u,
			2u,
			1u,
			1u,
			0u
		});

		public static Oid snmpInvalidMsgs = new Oid(new uint[11]
		{
			1u,
			3u,
			6u,
			1u,
			6u,
			3u,
			11u,
			2u,
			1u,
			2u,
			0u
		});

		public static Oid snmpUnknownPDUHandlers = new Oid(new uint[11]
		{
			1u,
			3u,
			6u,
			1u,
			6u,
			3u,
			11u,
			2u,
			1u,
			3u,
			0u
		});

		public static Oid[] v3ErrorOids = new Oid[8]
		{
			usmStatsUnsupportedSecLevels,
			usmStatsNotInTimeWindows,
			usmStatsUnknownSecurityNames,
			usmStatsUnknownEngineIDs,
			usmStatsWrongDigests,
			usmStatsDecryptionErrors,
			snmpUnknownSecurityModels,
			snmpUnknownPDUHandlers
		};

		public static AsnType GetSyntaxObject(byte asnType)
		{
			AsnType result = null;
			if (asnType == SMI_INTEGER)
			{
				result = new Integer32();
			}
			else if (asnType == SMI_COUNTER32)
			{
				result = new Counter32();
			}
			else if (asnType == SMI_GAUGE32)
			{
				result = new Gauge32();
			}
			else if (asnType == SMI_COUNTER64)
			{
				result = new Counter64();
			}
			else if (asnType == SMI_TIMETICKS)
			{
				result = new TimeTicks();
			}
			else if (asnType == SMI_STRING)
			{
				result = new OctetString();
			}
			else if (asnType == SMI_OPAQUE)
			{
				result = new Opaque();
			}
			else if (asnType == SMI_IPADDRESS)
			{
				result = new IpAddress();
			}
			else if (asnType == SMI_OBJECTID)
			{
				result = new Oid();
			}
			else if (asnType == SMI_PARTY_CLOCK)
			{
				result = new V2PartyClock();
			}
			else if (asnType == SMI_NOSUCHINSTANCE)
			{
				result = new NoSuchInstance();
			}
			else if (asnType == SMI_NOSUCHOBJECT)
			{
				result = new NoSuchObject();
			}
			else if (asnType == SMI_ENDOFMIBVIEW)
			{
				result = new EndOfMibView();
			}
			else if (asnType == SMI_NULL)
			{
				result = new Null();
			}
			return result;
		}

		public static AsnType GetSyntaxObject(string name)
		{
			AsnType asnType = null;
			return name switch
			{
				"Integer32" => new Integer32(), 
				"Counter32" => new Counter32(), 
				"Gauge32" => new Gauge32(), 
				"Counter64" => new Counter64(), 
				"TimeTicks" => new TimeTicks(), 
				"OctetString" => new OctetString(), 
				"IpAddress" => new IpAddress(), 
				"Oid" => new Oid(), 
				"Null" => new Null(), 
				_ => throw new ArgumentException("Invalid value type name"), 
			};
		}

		public static string GetTypeName(byte type)
		{
			if (type == SMI_IPADDRESS)
			{
				return SMI_IPADDRESS_STR;
			}
			if (type == SMI_APPSTRING)
			{
				return SMI_APPSTRING_STR;
			}
			if (type == SMI_COUNTER32)
			{
				return SMI_COUNTER32_STR;
			}
			if (type == SMI_COUNTER64)
			{
				return SMI_COUNTER64_STR;
			}
			if (type == SMI_GAUGE32)
			{
				return SMI_GAUGE32_STR;
			}
			if (type == SMI_INTEGER)
			{
				return SMI_INTEGER_STR;
			}
			if (type == SMI_NULL)
			{
				return SMI_NULL_STR;
			}
			if (type == SMI_OBJECTID)
			{
				return SMI_OBJECTID_STR;
			}
			if (type == SMI_OPAQUE)
			{
				return SMI_OPAQUE_STR;
			}
			if (type == SMI_STRING)
			{
				return SMI_STRING_STR;
			}
			if (type == SMI_TIMETICKS)
			{
				return SMI_TIMETICKS_STR;
			}
			if (type == SMI_UNSIGNED32)
			{
				return SMI_UNSIGNED32_STR;
			}
			return SMI_UNKNOWN_STR;
		}

		public static void DumpHex(byte[] data)
		{
			int num = 0;
			for (int i = 0; i < data.Length; i++)
			{
				if (num == 0)
				{
					Console.Write("{0:d04} ", (object)i);
				}
				Console.Write("{0:x2}", (object)data[i]);
				num++;
				if (num == 16)
				{
					num = 0;
					Console.Write("\n");
				}
				else
				{
					Console.Write(" ");
				}
			}
			if (num != 0)
			{
				Console.WriteLine("\n");
			}
		}

		public static bool IsValidVersion(int version)
		{
			if (version == 0 || version == 1 || version == 3)
			{
				return true;
			}
			return false;
		}

		private SnmpConstants()
		{
		}
	}
}
