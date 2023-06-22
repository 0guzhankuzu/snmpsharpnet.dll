namespace SnmpSharpNet
{
	public enum PduErrorStatus
	{
		noError,
		tooBig,
		noSuchName,
		badValue,
		readOnly,
		genErr,
		noAccess,
		wrongType,
		wrongLength,
		wrongEncoding,
		wrongValue,
		noCreation,
		inconsistentValue,
		resourceUnavailable,
		commitFailed,
		undoFailed,
		authorizationError,
		notWritable,
		inconsistentName
	}
}
