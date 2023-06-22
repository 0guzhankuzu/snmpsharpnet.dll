namespace SnmpSharpNet
{
	public sealed class SnmpError
	{
		public static string ErrorMessage(int errorCode)
		{
			return errorCode switch
			{
				0 => "No error", 
				1 => "Request too big", 
				2 => "noSuchName", 
				3 => "badValue", 
				4 => "readOnly", 
				5 => "genericError", 
				6 => "noAccess", 
				7 => "wrongType", 
				8 => "wrongLength", 
				9 => "wrongEncoding", 
				10 => "wrongValue", 
				11 => "noCreation", 
				12 => "inconsistentValue", 
				13 => "resourceUnavailable", 
				14 => "commitFailed", 
				15 => "undoFailed", 
				16 => "authorizationError", 
				17 => "notWritable", 
				18 => "inconsistentName", 
				_ => $"Unknown error ({errorCode})", 
			};
		}

		private SnmpError()
		{
		}
	}
}
