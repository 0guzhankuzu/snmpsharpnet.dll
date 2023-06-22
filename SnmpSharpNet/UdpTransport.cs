using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;

namespace SnmpSharpNet
{
	public class UdpTransport : IDisposable
	{
		protected Socket _socket;

		protected bool _isIPv6;

		protected bool _noSourceCheck;

		internal bool _busy;

		internal AsyncRequestState _requestState;

		internal byte[] _inBuffer;

		internal IPEndPoint _receivePeer;

		public bool IsIPv6 => _isIPv6;

		public bool IsBusy => _busy;

		internal event SnmpAsyncCallback _asyncCallback;

		public UdpTransport(bool useV6)
		{
			_isIPv6 = useV6;
			_socket = null;
			initSocket(_isIPv6);
		}

		~UdpTransport()
		{
			if (_socket != null)
			{
				_socket.Close();
				_socket = null;
			}
		}

		protected void initSocket(bool useV6)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Expected O, but got Unknown
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Expected O, but got Unknown
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Invalid comparison between Unknown and I4
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Expected O, but got Unknown
			if (_socket != null)
			{
				Close();
			}
			if (useV6)
			{
				_socket = new Socket((AddressFamily)23, (SocketType)2, (ProtocolType)17);
			}
			else
			{
				_socket = new Socket((AddressFamily)2, (SocketType)2, (ProtocolType)17);
			}
			IPEndPoint val = new IPEndPoint(((int)_socket.get_AddressFamily() == 2) ? IPAddress.Any : IPAddress.IPv6Any, 0);
			EndPoint val2 = (EndPoint)(object)val;
			_socket.Bind(val2);
		}

		public byte[] Request(IPAddress peer, int port, byte[] buffer, int bufferLength, int timeout, int retries)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Expected O, but got Unknown
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Invalid comparison between Unknown and I4
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Expected O, but got Unknown
			//IL_00bc: Expected O, but got Unknown
			if (_socket == null)
			{
				return null;
			}
			if (_socket.get_AddressFamily() != peer.get_AddressFamily())
			{
				throw new SnmpException("Invalid address protocol version.");
			}
			IPEndPoint val = new IPEndPoint(peer, port);
			_socket.SetSocketOption((SocketOptionLevel)65535, (SocketOptionName)4102, timeout);
			int num = 0;
			int num2 = 0;
			byte[] array = new byte[65536];
			EndPoint val2 = (EndPoint)new IPEndPoint(((int)peer.get_AddressFamily() == 2) ? IPAddress.Any : IPAddress.IPv6Any, 0);
			while (true)
			{
				bool flag = true;
				try
				{
					_socket.SendTo(buffer, bufferLength, (SocketFlags)0, (EndPoint)(object)val);
					num = _socket.ReceiveFrom(array, ref val2);
				}
				catch (SocketException val3)
				{
					SocketException val4 = val3;
					if (((ExternalException)(object)val4).ErrorCode == 10040)
					{
						num = 0;
					}
					else
					{
						if (((ExternalException)(object)val4).ErrorCode == 10050)
						{
							throw new SnmpNetworkException((Exception)(object)val4, "Network error: Destination network is down.");
						}
						if (((ExternalException)(object)val4).ErrorCode == 10051)
						{
							throw new SnmpNetworkException((Exception)(object)val4, "Network error: destination network is unreachable.");
						}
						if (((ExternalException)(object)val4).ErrorCode == 10054)
						{
							throw new SnmpNetworkException((Exception)(object)val4, "Network error: connection reset by peer.");
						}
						if (((ExternalException)(object)val4).ErrorCode == 10064)
						{
							throw new SnmpNetworkException((Exception)(object)val4, "Network error: remote host is down.");
						}
						if (((ExternalException)(object)val4).ErrorCode == 10065)
						{
							throw new SnmpNetworkException((Exception)(object)val4, "Network error: remote host is unreachable.");
						}
						if (((ExternalException)(object)val4).ErrorCode == 10061)
						{
							throw new SnmpNetworkException((Exception)(object)val4, "Network error: connection refused.");
						}
						if (((ExternalException)(object)val4).ErrorCode == 10060)
						{
							num = 0;
						}
					}
				}
				if (num > 0)
				{
					IPEndPoint val5 = val2 as IPEndPoint;
					if (_noSourceCheck || ((object)val5).Equals((object?)val))
					{
						MutableByte obj = new MutableByte(array, num);
						return obj;
					}
					if (val5.get_Address() != val.get_Address())
					{
						Console.WriteLine("Address miss-match {0} != {1}", (object)val5.get_Address(), (object)val.get_Address());
					}
					if (val5.get_Port() != val.get_Port())
					{
						Console.WriteLine("Port # miss-match {0} != {1}", (object)val5.get_Port(), (object)val.get_Port());
					}
					num2++;
					if (num2 > retries)
					{
						throw new SnmpException(SnmpException.RequestTimedOut, "Request has reached maximum retries.");
					}
				}
				else
				{
					num2++;
					if (num2 > retries)
					{
						break;
					}
				}
			}
			throw new SnmpException(SnmpException.RequestTimedOut, "Request has reached maximum retries.");
		}

		internal bool RequestAsync(IPAddress peer, int port, byte[] buffer, int bufferLength, int timeout, int retries, SnmpAsyncCallback asyncCallback)
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			if (_busy)
			{
				return false;
			}
			if (_socket == null)
			{
				return false;
			}
			if (_socket.get_AddressFamily() != peer.get_AddressFamily())
			{
				throw new SnmpException("Invalid address protocol version.");
			}
			_busy = true;
			this._asyncCallback = null;
			_asyncCallback += asyncCallback;
			_requestState = new AsyncRequestState(peer, port, retries, timeout);
			_requestState.Packet = buffer;
			_requestState.PacketLength = bufferLength;
			_inBuffer = new byte[65536];
			SendToBegin();
			return true;
		}

		internal void SendToBegin()
		{
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Expected O, but got Unknown
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Invalid comparison between Unknown and I4
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Expected O, but got Unknown
			if (_requestState == null)
			{
				_busy = false;
				return;
			}
			if (_requestState.Timer != null)
			{
				_requestState.Timer.Dispose();
				_requestState.Timer = null;
			}
			if (_socket == null)
			{
				_busy = false;
				_requestState = null;
				this._asyncCallback(AsyncRequestResult.Terminated, new IPEndPoint(IPAddress.Any, 0), null, 0);
				return;
			}
			try
			{
				_socket.BeginSendTo(_requestState.Packet, 0, _requestState.PacketLength, (SocketFlags)0, (EndPoint)(object)_requestState.EndPoint, (AsyncCallback)SendToCallback, (object)null);
			}
			catch
			{
				_busy = false;
				_requestState = null;
				this._asyncCallback(AsyncRequestResult.SocketSendError, new IPEndPoint(((int)_socket.get_AddressFamily() == 2) ? IPAddress.Any : IPAddress.IPv6Any, 0), null, 0);
			}
		}

		internal void SendToCallback(IAsyncResult ar)
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Expected O, but got Unknown
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Invalid comparison between Unknown and I4
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Expected O, but got Unknown
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Invalid comparison between Unknown and I4
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Expected O, but got Unknown
			if (_socket == null || !_busy || _requestState == null)
			{
				_busy = false;
				_requestState = null;
				this._asyncCallback(AsyncRequestResult.Terminated, new IPEndPoint(IPAddress.Any, 0), null, 0);
				return;
			}
			int num = 0;
			try
			{
				num = _socket.EndSendTo(ar);
			}
			catch (NullReferenceException ex)
			{
				ex.GetType();
				_busy = false;
				_requestState = null;
				this._asyncCallback(AsyncRequestResult.Terminated, new IPEndPoint(((int)_socket.get_AddressFamily() == 2) ? IPAddress.Any : IPAddress.IPv6Any, 0), null, 0);
				return;
			}
			catch
			{
				num = 0;
			}
			if (num != _requestState.PacketLength)
			{
				_busy = false;
				_requestState = null;
				this._asyncCallback(AsyncRequestResult.SocketSendError, new IPEndPoint(((int)_socket.get_AddressFamily() == 2) ? IPAddress.Any : IPAddress.IPv6Any, 0), null, 0);
			}
			else
			{
				ReceiveBegin();
			}
		}

		internal void ReceiveBegin()
		{
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Invalid comparison between Unknown and I4
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Expected O, but got Unknown
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Expected O, but got Unknown
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Invalid comparison between Unknown and I4
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Expected O, but got Unknown
			if (_requestState.Timer != null)
			{
				_requestState.Timer.Dispose();
				_requestState.Timer = null;
			}
			if (_socket == null || !_busy || _requestState == null)
			{
				_busy = false;
				_requestState = null;
				if (_socket != null)
				{
					this._asyncCallback(AsyncRequestResult.Terminated, new IPEndPoint(((int)_socket.get_AddressFamily() == 2) ? IPAddress.Any : IPAddress.IPv6Any, 0), null, 0);
				}
				else
				{
					this._asyncCallback(AsyncRequestResult.Terminated, new IPEndPoint(IPAddress.Any, 0), null, 0);
				}
				return;
			}
			_receivePeer = new IPEndPoint(((int)_socket.get_AddressFamily() == 2) ? IPAddress.Any : IPAddress.IPv6Any, 0);
			EndPoint receivePeer = (EndPoint)(object)_receivePeer;
			try
			{
				_socket.BeginReceiveFrom(_inBuffer, 0, _inBuffer.Length, (SocketFlags)0, ref receivePeer, (AsyncCallback)ReceiveFromCallback, (object)null);
			}
			catch
			{
				RetryAsyncRequest();
				return;
			}
			_requestState.Timer = new Timer(new TimerCallback(AsyncRequestTimerCallback), null, _requestState.Timeout, -1);
		}

		internal void RetryAsyncRequest()
		{
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Invalid comparison between Unknown and I4
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Expected O, but got Unknown
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Expected O, but got Unknown
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Invalid comparison between Unknown and I4
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Expected O, but got Unknown
			if (_requestState.Timer != null)
			{
				_requestState.Timer.Dispose();
				_requestState.Timer = null;
			}
			if (_socket == null || !_busy || _requestState == null)
			{
				_busy = false;
				_requestState = null;
				if (_socket != null)
				{
					this._asyncCallback(AsyncRequestResult.Terminated, new IPEndPoint(((int)_socket.get_AddressFamily() == 2) ? IPAddress.Any : IPAddress.IPv6Any, 0), null, 0);
				}
				else
				{
					this._asyncCallback(AsyncRequestResult.Terminated, new IPEndPoint(IPAddress.Any, 0), null, 0);
				}
				return;
			}
			_requestState.CurrentRetry++;
			if (_requestState.CurrentRetry >= _requestState.MaxRetries)
			{
				_busy = false;
				_requestState = null;
				this._asyncCallback(AsyncRequestResult.Timeout, new IPEndPoint(((int)_socket.get_AddressFamily() == 2) ? IPAddress.Any : IPAddress.IPv6Any, 0), null, 0);
			}
			else
			{
				SendToBegin();
			}
		}

		internal void ReceiveFromCallback(IAsyncResult ar)
		{
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Invalid comparison between Unknown and I4
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Expected O, but got Unknown
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Expected O, but got Unknown
			//IL_00ed: Expected O, but got Unknown
			if (_requestState.Timer != null)
			{
				_requestState.Timer.Dispose();
				_requestState.Timer = null;
			}
			if (_socket == null || !_busy || _requestState == null)
			{
				_busy = false;
				_requestState = null;
				if (_socket == null)
				{
					this._asyncCallback(AsyncRequestResult.Terminated, new IPEndPoint(((int)_socket.get_AddressFamily() == 2) ? IPAddress.Any : IPAddress.IPv6Any, 0), null, 0);
				}
				else
				{
					this._asyncCallback(AsyncRequestResult.Terminated, new IPEndPoint(IPAddress.Any, 0), null, 0);
				}
				return;
			}
			int num = 0;
			EndPoint receivePeer = (EndPoint)(object)_receivePeer;
			try
			{
				num = _socket.EndReceiveFrom(ar, ref receivePeer);
			}
			catch (SocketException val)
			{
				SocketException val2 = val;
				if (((ExternalException)(object)val2).ErrorCode == 10040)
				{
					num = 0;
				}
				else
				{
					if (((ExternalException)(object)val2).ErrorCode == 10050)
					{
						_busy = false;
						_requestState = null;
						this._asyncCallback(AsyncRequestResult.SocketReceiveError, null, null, -1);
						return;
					}
					if (((ExternalException)(object)val2).ErrorCode == 10051)
					{
						_busy = false;
						_requestState = null;
						this._asyncCallback(AsyncRequestResult.SocketReceiveError, null, null, -1);
						return;
					}
					if (((ExternalException)(object)val2).ErrorCode == 10054)
					{
						_busy = false;
						_requestState = null;
						this._asyncCallback(AsyncRequestResult.SocketReceiveError, null, null, -1);
						return;
					}
					if (((ExternalException)(object)val2).ErrorCode == 10064)
					{
						_busy = false;
						_requestState = null;
						this._asyncCallback(AsyncRequestResult.SocketReceiveError, null, null, -1);
						return;
					}
					if (((ExternalException)(object)val2).ErrorCode == 10065)
					{
						_busy = false;
						_requestState = null;
						this._asyncCallback(AsyncRequestResult.SocketReceiveError, null, null, -1);
						return;
					}
					if (((ExternalException)(object)val2).ErrorCode == 10061)
					{
						_busy = false;
						_requestState = null;
						this._asyncCallback(AsyncRequestResult.SocketReceiveError, null, null, -1);
						return;
					}
					if (((ExternalException)(object)val2).ErrorCode == 10060)
					{
						num = 0;
					}
				}
			}
			catch (ObjectDisposedException ex)
			{
				ex.GetType();
				this._asyncCallback(AsyncRequestResult.Terminated, null, null, -1);
				return;
			}
			catch (NullReferenceException ex2)
			{
				ex2.GetType();
				this._asyncCallback(AsyncRequestResult.Terminated, null, null, -1);
				return;
			}
			catch (Exception ex3)
			{
				ex3.GetType();
				num = 0;
			}
			if (num == 0)
			{
				RetryAsyncRequest();
				return;
			}
			byte[] array = new byte[num];
			Buffer.BlockCopy(_inBuffer, 0, array, 0, num);
			_busy = false;
			_requestState = null;
			this._asyncCallback(AsyncRequestResult.NoError, _receivePeer, array, array.Length);
		}

		internal void AsyncRequestTimerCallback(object stateInfo)
		{
			if (_socket != null || (_requestState != null && _busy))
			{
				RetryAsyncRequest();
			}
		}

		public void Dispose()
		{
			Close();
		}

		public void Close()
		{
			if (_socket != null)
			{
				try
				{
					_socket.Close();
				}
				catch
				{
				}
				_socket = null;
			}
		}
	}
}
