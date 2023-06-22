using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SnmpSharpNet
{
	public class TrapAgent
	{
		protected Socket _sock;

		public TrapAgent()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Expected O, but got Unknown
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Expected O, but got Unknown
			_sock = new Socket((AddressFamily)2, (SocketType)2, (ProtocolType)17);
			_sock.Bind((EndPoint)new IPEndPoint(IPAddress.Any, 0));
		}

		~TrapAgent()
		{
			_sock.Close();
		}

		public void SendV1Trap(SnmpV1TrapPacket packet, IpAddress peer, int port)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			byte[] array = packet.encode();
			_sock.SendTo(array, (EndPoint)new IPEndPoint((IPAddress)peer, port));
		}

		public void SendV1Trap(IpAddress receiver, int receiverPort, string community, Oid senderSysObjectID, IpAddress senderIpAdress, int genericTrap, int specificTrap, uint senderUpTime, VbCollection varList)
		{
			SnmpV1TrapPacket snmpV1TrapPacket = new SnmpV1TrapPacket(community);
			snmpV1TrapPacket.Pdu.Generic = genericTrap;
			snmpV1TrapPacket.Pdu.Specific = specificTrap;
			((OctetString)snmpV1TrapPacket.Pdu.AgentAddress).Set((byte[])senderIpAdress);
			snmpV1TrapPacket.Pdu.TimeStamp = senderUpTime;
			snmpV1TrapPacket.Pdu.VbList.Add(varList);
			snmpV1TrapPacket.Pdu.Enterprise.Set(senderSysObjectID);
			SendV1Trap(snmpV1TrapPacket, receiver, receiverPort);
		}

		public void SendV2Trap(SnmpV2Packet packet, IpAddress peer, int port)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Expected O, but got Unknown
			if (packet.Pdu.Type != PduType.V2Trap)
			{
				throw new SnmpInvalidPduTypeException("Invalid Pdu type.");
			}
			byte[] array = packet.encode();
			_sock.SendTo(array, (EndPoint)new IPEndPoint((IPAddress)peer, port));
		}

		public void SendV2Trap(IpAddress receiver, int receiverPort, string community, uint senderUpTime, Oid trapObjectID, VbCollection varList)
		{
			SnmpV2Packet snmpV2Packet = new SnmpV2Packet(community);
			snmpV2Packet.Pdu.Type = PduType.V2Trap;
			snmpV2Packet.Pdu.TrapObjectID = trapObjectID;
			snmpV2Packet.Pdu.TrapSysUpTime.Value = senderUpTime;
			snmpV2Packet.Pdu.SetVbList(varList);
			SendV2Trap(snmpV2Packet, receiver, receiverPort);
		}

		public void SendV3Trap(SnmpV3Packet packet, IpAddress peer, int port)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Expected O, but got Unknown
			if (packet.Pdu.Type != PduType.V2Trap)
			{
				throw new SnmpInvalidPduTypeException("Invalid Pdu type.");
			}
			byte[] array = packet.encode();
			_sock.SendTo(array, (EndPoint)new IPEndPoint((IPAddress)peer, port));
		}

		public void SendV3Trap(IpAddress receiver, int receiverPort, byte[] engineId, int senderEngineBoots, int senderEngineTime, string senderUserName, uint senderUpTime, Oid trapObjectID, VbCollection varList)
		{
			SnmpV3Packet snmpV3Packet = new SnmpV3Packet();
			snmpV3Packet.Pdu.Type = PduType.V2Trap;
			snmpV3Packet.NoAuthNoPriv(Encoding.UTF8.GetBytes(senderUserName));
			snmpV3Packet.SetEngineId(engineId);
			snmpV3Packet.SetEngineTime(senderEngineBoots, senderEngineTime);
			snmpV3Packet.ScopedPdu.TrapObjectID.Set(trapObjectID);
			snmpV3Packet.ScopedPdu.TrapSysUpTime.Value = senderUpTime;
			snmpV3Packet.ScopedPdu.VbList.Add(varList);
			snmpV3Packet.MsgFlags.Reportable = false;
			SendV3Trap(snmpV3Packet, receiver, receiverPort);
		}

		public void SendV3Trap(IpAddress receiver, int receiverPort, byte[] engineId, int senderEngineBoots, int senderEngineTime, string senderUserName, uint senderUpTime, Oid trapObjectID, VbCollection varList, AuthenticationDigests authDigest, byte[] authSecret)
		{
			SnmpV3Packet snmpV3Packet = new SnmpV3Packet();
			snmpV3Packet.Pdu.Type = PduType.V2Trap;
			snmpV3Packet.authNoPriv(Encoding.UTF8.GetBytes(senderUserName), authSecret, authDigest);
			snmpV3Packet.SetEngineId(engineId);
			snmpV3Packet.SetEngineTime(senderEngineBoots, senderEngineTime);
			snmpV3Packet.ScopedPdu.TrapObjectID.Set(trapObjectID);
			snmpV3Packet.ScopedPdu.TrapSysUpTime.Value = senderUpTime;
			snmpV3Packet.ScopedPdu.VbList.Add(varList);
			snmpV3Packet.MsgFlags.Reportable = false;
			SendV3Trap(snmpV3Packet, receiver, receiverPort);
		}

		public void SendV3Trap(IpAddress receiver, int receiverPort, byte[] engineId, int senderEngineBoots, int senderEngineTime, string senderUserName, uint senderUpTime, Oid trapObjectID, VbCollection varList, AuthenticationDigests authDigest, byte[] authSecret, PrivacyProtocols privProtocol, byte[] privSecret)
		{
			SnmpV3Packet snmpV3Packet = new SnmpV3Packet();
			snmpV3Packet.Pdu.Type = PduType.V2Trap;
			snmpV3Packet.authPriv(Encoding.UTF8.GetBytes(senderUserName), authSecret, authDigest, privSecret, privProtocol);
			snmpV3Packet.SetEngineId(engineId);
			snmpV3Packet.SetEngineTime(senderEngineBoots, senderEngineTime);
			snmpV3Packet.ScopedPdu.TrapObjectID.Set(trapObjectID);
			snmpV3Packet.ScopedPdu.TrapSysUpTime.Value = senderUpTime;
			snmpV3Packet.ScopedPdu.VbList.Add(varList);
			snmpV3Packet.MsgFlags.Reportable = false;
			SendV3Trap(snmpV3Packet, receiver, receiverPort);
		}

		public static void SendTrap(SnmpPacket packet, IpAddress peer, int port)
		{
			TrapAgent trapAgent = new TrapAgent();
			if (packet is SnmpV1TrapPacket)
			{
				trapAgent.SendV1Trap((SnmpV1TrapPacket)packet, peer, port);
				return;
			}
			if (packet is SnmpV2Packet)
			{
				trapAgent.SendV2Trap((SnmpV2Packet)packet, peer, port);
				return;
			}
			if (packet is SnmpV3Packet)
			{
				trapAgent.SendV3Trap((SnmpV3Packet)packet, peer, port);
				return;
			}
			throw new SnmpException("Invalid SNMP packet type.");
		}
	}
}
