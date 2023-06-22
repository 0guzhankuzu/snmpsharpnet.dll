using System;
using System.Net;

namespace SnmpSharpNet
{
	public class UdpTarget : UdpTransport, IDisposable
	{
		protected IPAddress _address;

		protected int _retry;

		protected int _port;

		protected int _timeout;

		protected IAgentParameters _agentParameters;

		public IPAddress Address
		{
			get
			{
				return _address;
			}
			set
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Invalid comparison between Unknown and I4
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Invalid comparison between Unknown and I4
				_address = value;
				if ((int)_address.get_AddressFamily() == 23 && !base.IsIPv6)
				{
					initSocket(useV6: true);
				}
				else if ((int)_address.get_AddressFamily() == 2 && base.IsIPv6)
				{
					initSocket(useV6: false);
				}
			}
		}

		public int Port
		{
			get
			{
				return _port;
			}
			set
			{
				_port = value;
			}
		}

		public int Timeout
		{
			get
			{
				return _timeout;
			}
			set
			{
				_timeout = value;
			}
		}

		public int Retry
		{
			get
			{
				return _retry;
			}
			set
			{
				_retry = value;
			}
		}

		protected event SnmpAsyncResponse _response;

		public UdpTarget(IPAddress peer, int port, int timeout, int retry)
			: base((int)peer.get_AddressFamily() == 23)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Invalid comparison between Unknown and I4
			_address = peer;
			_port = port;
			_timeout = timeout;
			_retry = retry;
		}

		public UdpTarget(IPAddress peer)
			: base((int)peer.get_AddressFamily() == 23)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Invalid comparison between Unknown and I4
			_address = peer;
			_port = 161;
			_timeout = 2000;
			_retry = 2;
		}

		public SnmpPacket Request(Pdu pdu, IAgentParameters agentParameters)
		{
			byte[] array;
			if (agentParameters.Version == SnmpVersion.Ver3)
			{
				SecureAgentParameters secureAgentParameters = (SecureAgentParameters)agentParameters;
				if (secureAgentParameters.Authentication != 0 && secureAgentParameters.AuthenticationSecret.Length <= 0)
				{
					throw new SnmpAuthenticationException("Authentication password not specified.");
				}
				if (secureAgentParameters.Privacy != 0 && secureAgentParameters.PrivacySecret.Length <= 0)
				{
					throw new SnmpPrivacyException("Privacy password not specified.");
				}
				_noSourceCheck = false;
				ScopedPdu pdu2 = new ScopedPdu(pdu);
				SnmpV3Packet snmpV3Packet = new SnmpV3Packet(pdu2);
				secureAgentParameters.InitializePacket(snmpV3Packet);
				array = ((!secureAgentParameters.HasCachedKeys) ? snmpV3Packet.encode() : snmpV3Packet.encode(secureAgentParameters.AuthenticationKey, secureAgentParameters.PrivacyKey));
			}
			else if (agentParameters.Version == SnmpVersion.Ver1)
			{
				AgentParameters agentParameters2 = (AgentParameters)agentParameters;
				if (!agentParameters2.Valid())
				{
					throw new SnmpException(SnmpException.InvalidIAgentParameters, "Invalid AgentParameters. Unable to process request.");
				}
				SnmpV1Packet snmpV1Packet = new SnmpV1Packet();
				snmpV1Packet.Pdu.Set(pdu);
				snmpV1Packet.Community.Set(agentParameters2.Community);
				array = snmpV1Packet.encode();
				_noSourceCheck = agentParameters2.DisableReplySourceCheck;
			}
			else
			{
				if (agentParameters.Version != SnmpVersion.Ver2)
				{
					throw new SnmpInvalidVersionException("Unsupported SNMP version.");
				}
				AgentParameters agentParameters2 = (AgentParameters)agentParameters;
				if (!agentParameters2.Valid())
				{
					throw new SnmpException(SnmpException.InvalidIAgentParameters, "Invalid AgentParameters. Unable to process request.");
				}
				SnmpV2Packet snmpV2Packet = new SnmpV2Packet();
				snmpV2Packet.Pdu.Set(pdu);
				snmpV2Packet.Community.Set(agentParameters2.Community);
				_noSourceCheck = agentParameters2.DisableReplySourceCheck;
				array = snmpV2Packet.encode();
			}
			byte[] array2 = Request(_address, _port, array, array.Length, _timeout, _retry);
			if (array2 == null || array2.Length <= 0)
			{
				throw new SnmpException(SnmpException.NoDataReceived, "No data received on request.");
			}
			if (agentParameters.Version == SnmpVersion.Ver1)
			{
				SnmpV1Packet snmpV1Packet = new SnmpV1Packet();
				AgentParameters agentParameters2 = (AgentParameters)agentParameters;
				snmpV1Packet.decode(array2, array2.Length);
				if (snmpV1Packet.Community != agentParameters2.Community)
				{
					throw new SnmpAuthenticationException("Invalid community name in reply.");
				}
				if (snmpV1Packet.Pdu.RequestId != pdu.RequestId)
				{
					throw new SnmpException(SnmpException.InvalidRequestId, "Invalid request id in reply.");
				}
				return snmpV1Packet;
			}
			if (agentParameters.Version == SnmpVersion.Ver2)
			{
				SnmpV2Packet snmpV2Packet = new SnmpV2Packet();
				AgentParameters agentParameters2 = (AgentParameters)agentParameters;
				snmpV2Packet.decode(array2, array2.Length);
				if (snmpV2Packet.Community != agentParameters2.Community)
				{
					throw new SnmpAuthenticationException("Invalid community name in reply.");
				}
				if (snmpV2Packet.Pdu.RequestId != pdu.RequestId)
				{
					throw new SnmpException(SnmpException.InvalidRequestId, "Invalid request id in reply.");
				}
				return snmpV2Packet;
			}
			if (agentParameters.Version == SnmpVersion.Ver3)
			{
				SnmpV3Packet snmpV3Packet = new SnmpV3Packet();
				SecureAgentParameters secureAgentParameters = (SecureAgentParameters)agentParameters;
				secureAgentParameters.InitializePacket(snmpV3Packet);
				if (secureAgentParameters.HasCachedKeys)
				{
					snmpV3Packet.decode(array2, array2.Length, secureAgentParameters.AuthenticationKey, secureAgentParameters.PrivacyKey);
				}
				else
				{
					snmpV3Packet.decode(array2, array2.Length);
				}
				if (snmpV3Packet.Pdu.Type == PduType.Report && snmpV3Packet.Pdu.VbCount > 0 && snmpV3Packet.Pdu.VbList[0].Oid.Equals(SnmpConstants.usmStatsUnknownEngineIDs))
				{
					secureAgentParameters.UpdateDiscoveryValues(snmpV3Packet);
					return snmpV3Packet;
				}
				if (!secureAgentParameters.ValidateIncomingPacket(snmpV3Packet))
				{
					return null;
				}
				secureAgentParameters.UpdateDiscoveryValues(snmpV3Packet);
				return snmpV3Packet;
			}
			return null;
		}

		public bool RequestAsync(Pdu pdu, IAgentParameters agentParameters, SnmpAsyncResponse responseCallback)
		{
			if (base.IsBusy)
			{
				return false;
			}
			this._response = null;
			_response += responseCallback;
			_agentParameters = agentParameters;
			byte[] array;
			if (agentParameters.Version == SnmpVersion.Ver3)
			{
				SecureAgentParameters secureAgentParameters = (SecureAgentParameters)agentParameters;
				if (secureAgentParameters.Authentication != 0 && secureAgentParameters.AuthenticationSecret.Length <= 0)
				{
					return false;
				}
				if (secureAgentParameters.Privacy != 0 && secureAgentParameters.PrivacySecret.Length <= 0)
				{
					return false;
				}
				_noSourceCheck = false;
				ScopedPdu scopedPdu = new ScopedPdu(pdu);
				scopedPdu.ContextEngineId.Set(secureAgentParameters.EngineId);
				scopedPdu.ContextName.Set(secureAgentParameters.ContextName);
				SnmpV3Packet snmpV3Packet = new SnmpV3Packet(scopedPdu);
				secureAgentParameters.InitializePacket(snmpV3Packet);
				try
				{
					array = ((!secureAgentParameters.HasCachedKeys) ? snmpV3Packet.encode() : snmpV3Packet.encode(secureAgentParameters.AuthenticationKey, secureAgentParameters.PrivacyKey));
				}
				catch (Exception ex)
				{
					ex.GetType();
					this._response(AsyncRequestResult.EncodeError, snmpV3Packet);
					return false;
				}
			}
			else if (agentParameters.Version == SnmpVersion.Ver1)
			{
				AgentParameters agentParameters2 = (AgentParameters)agentParameters;
				_noSourceCheck = agentParameters2.DisableReplySourceCheck;
				SnmpV1Packet snmpV1Packet = new SnmpV1Packet();
				snmpV1Packet.Pdu.Set(pdu);
				snmpV1Packet.Community.Set(agentParameters2.Community);
				try
				{
					array = snmpV1Packet.encode();
				}
				catch (Exception ex)
				{
					ex.GetType();
					this._response(AsyncRequestResult.EncodeError, snmpV1Packet);
					return false;
				}
			}
			else
			{
				if (agentParameters.Version != SnmpVersion.Ver2)
				{
					throw new SnmpInvalidVersionException("Unsupported SNMP version.");
				}
				AgentParameters agentParameters2 = (AgentParameters)agentParameters;
				_noSourceCheck = agentParameters2.DisableReplySourceCheck;
				SnmpV2Packet snmpV2Packet = new SnmpV2Packet();
				snmpV2Packet.Pdu.Set(pdu);
				snmpV2Packet.Community.Set(agentParameters2.Community);
				try
				{
					array = snmpV2Packet.encode();
				}
				catch (Exception ex)
				{
					ex.GetType();
					this._response(AsyncRequestResult.EncodeError, snmpV2Packet);
					return false;
				}
			}
			if (!RequestAsync(_address, _port, array, array.Length, _timeout, _retry, AsyncResponse))
			{
				return false;
			}
			return true;
		}

		public bool Discovery(SecureAgentParameters param)
		{
			param.Reset();
			param.SecurityName.Set("");
			param.Reportable = true;
			Pdu pdu = new Pdu();
			SnmpV3Packet snmpV3Packet = (SnmpV3Packet)Request(pdu, param);
			if (snmpV3Packet != null)
			{
				if (snmpV3Packet.USM.EngineBoots != 0 || snmpV3Packet.USM.EngineTime != 0)
				{
					return true;
				}
				snmpV3Packet = (SnmpV3Packet)Request(pdu, param);
				if (snmpV3Packet != null)
				{
					return true;
				}
			}
			return false;
		}

		public bool DiscoveryAsync(SecureAgentParameters param, SnmpAsyncResponse callback)
		{
			Pdu pdu = new Pdu();
			return RequestAsync(pdu, param, callback);
		}

		internal void AsyncResponse(AsyncRequestResult result, IPEndPoint peer, byte[] buffer, int buflen)
		{
			if (result != 0)
			{
				this._response(result, null);
			}
			else if (buffer == null || buffer.Length <= 0 || buflen <= 0)
			{
				this._response(AsyncRequestResult.NoDataReceived, null);
			}
			else if (_agentParameters.Version == SnmpVersion.Ver1)
			{
				SnmpV1Packet snmpV1Packet = new SnmpV1Packet();
				try
				{
					snmpV1Packet.decode(buffer, buflen);
				}
				catch (Exception ex)
				{
					ex.GetType();
					this._response(AsyncRequestResult.DecodeError, snmpV1Packet);
					return;
				}
				this._response(AsyncRequestResult.NoError, snmpV1Packet);
			}
			else if (_agentParameters.Version == SnmpVersion.Ver2)
			{
				SnmpV2Packet snmpV2Packet = new SnmpV2Packet();
				try
				{
					snmpV2Packet.decode(buffer, buflen);
				}
				catch (Exception ex)
				{
					ex.GetType();
					this._response(AsyncRequestResult.DecodeError, snmpV2Packet);
					return;
				}
				this._response(AsyncRequestResult.NoError, snmpV2Packet);
			}
			else
			{
				if (_agentParameters.Version != SnmpVersion.Ver3)
				{
					return;
				}
				SnmpV3Packet snmpV3Packet = new SnmpV3Packet();
				SecureAgentParameters secureAgentParameters = (SecureAgentParameters)_agentParameters;
				secureAgentParameters.InitializePacket(snmpV3Packet);
				try
				{
					if (secureAgentParameters.HasCachedKeys)
					{
						snmpV3Packet.decode(buffer, buflen, secureAgentParameters.AuthenticationKey, secureAgentParameters.PrivacyKey);
					}
					else
					{
						snmpV3Packet.decode(buffer, buflen);
					}
				}
				catch
				{
					this._response(AsyncRequestResult.DecodeError, snmpV3Packet);
					return;
				}
				if (!secureAgentParameters.ValidateIncomingPacket(snmpV3Packet))
				{
					this._response(AsyncRequestResult.AuthenticationError, snmpV3Packet);
					return;
				}
				secureAgentParameters.UpdateDiscoveryValues(snmpV3Packet);
				this._response(AsyncRequestResult.NoError, snmpV3Packet);
			}
		}
	}
}
