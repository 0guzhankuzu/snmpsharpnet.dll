using System.Net;

namespace SnmpSharpNet
{
	internal delegate void SnmpAsyncCallback(AsyncRequestResult status, IPEndPoint peer, byte[] buffer, int length);
}
