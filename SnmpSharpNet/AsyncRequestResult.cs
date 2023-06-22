namespace SnmpSharpNet
{
	public enum AsyncRequestResult
	{
		NoError,
		RequestInProgress,
		Timeout,
		SocketSendError,
		SocketReceiveError,
		Terminated,
		NoDataReceived,
		AuthenticationError,
		PrivacyError,
		EncodeError,
		DecodeError
	}
}
