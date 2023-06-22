using System.Net;
using System.Threading;

namespace SnmpSharpNet
{
	internal class AsyncRequestState
	{
		protected IPEndPoint _endPoint;

		protected byte[] _packet;

		protected int _packetLength;

		protected int _maxRetries;

		protected int _timeout;

		protected Timer _timer;

		protected int _currentRetry;

		public IPEndPoint EndPoint
		{
			get
			{
				return _endPoint;
			}
			set
			{
				_endPoint = value;
			}
		}

		public byte[] Packet
		{
			get
			{
				return _packet;
			}
			set
			{
				_packet = value;
			}
		}

		public int PacketLength
		{
			get
			{
				return _packetLength;
			}
			set
			{
				_packetLength = value;
			}
		}

		public int MaxRetries
		{
			get
			{
				return _maxRetries;
			}
			set
			{
				_maxRetries = value;
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

		public Timer Timer
		{
			get
			{
				return _timer;
			}
			set
			{
				_timer = value;
			}
		}

		public int CurrentRetry
		{
			get
			{
				return _currentRetry;
			}
			set
			{
				_currentRetry = value;
			}
		}

		public AsyncRequestState(IPAddress peerIP, int peerPort, int maxretries, int timeout)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Expected O, but got Unknown
			_endPoint = new IPEndPoint(peerIP, peerPort);
			_maxRetries = maxretries;
			_timeout = timeout;
			_currentRetry = -1;
			_timer = null;
		}
	}
}
