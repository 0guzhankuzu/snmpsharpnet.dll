namespace SnmpSharpNet
{
	public class AgentParameters : IAgentParameters
	{
		protected Integer32 _version;

		protected OctetString _community;

		protected bool _disableReplySourceCheck;

		public virtual SnmpVersion Version
		{
			get
			{
				return (SnmpVersion)_version.Value;
			}
			set
			{
				if (value != 0 && value != SnmpVersion.Ver2)
				{
					throw new SnmpInvalidVersionException("Valid SNMP versions are 1 or 2");
				}
				_version.Value = (int)value;
			}
		}

		public virtual OctetString Community => _community;

		public bool DisableReplySourceCheck
		{
			get
			{
				return _disableReplySourceCheck;
			}
			set
			{
				_disableReplySourceCheck = value;
			}
		}

		public AgentParameters()
		{
			_version = new Integer32(0);
			_community = new OctetString("public");
			_disableReplySourceCheck = false;
		}

		public AgentParameters(AgentParameters second)
		{
			_version.Value = (int)second.Version;
			_community.Set(second.Community);
			_disableReplySourceCheck = second.DisableReplySourceCheck;
		}

		public AgentParameters(SnmpVersion version)
			: this()
		{
			_version.Value = (int)version;
		}

		public AgentParameters(OctetString community)
			: this()
		{
			_community.Set(community);
		}

		public AgentParameters(SnmpVersion version, OctetString community)
			: this(version)
		{
			_community.Set(community);
		}

		public AgentParameters(SnmpVersion version, OctetString community, bool disableReplySourceCheck)
			: this(version, community)
		{
			_disableReplySourceCheck = disableReplySourceCheck;
		}

		public Integer32 GetVersion()
		{
			return _version;
		}

		public bool Valid()
		{
			if (_community != null && _community.Length > 0 && _version != null && (_version.Value == 0 || _version.Value == 1))
			{
				return true;
			}
			return false;
		}

		public void InitializePacket(SnmpPacket packet)
		{
			if (packet is SnmpV1Packet)
			{
				SnmpV1Packet snmpV1Packet = (SnmpV1Packet)packet;
				snmpV1Packet.Community.Set(_community);
				return;
			}
			if (packet is SnmpV2Packet)
			{
				SnmpV2Packet snmpV2Packet = (SnmpV2Packet)packet;
				snmpV2Packet.Community.Set(_community);
				return;
			}
			throw new SnmpInvalidVersionException("Invalid SNMP version.");
		}

		public object Clone()
		{
			return new AgentParameters(Version, Community, DisableReplySourceCheck);
		}
	}
}
