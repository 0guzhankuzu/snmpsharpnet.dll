namespace SnmpSharpNet
{
	public sealed class SNMPV3ReportError
	{
		private SNMPV3ReportError()
		{
		}

		public static string TranslateError(SnmpV3Packet packet)
		{
			foreach (Vb vb in packet.Pdu.VbList)
			{
				if (vb.Oid.Compare(SnmpConstants.usmStatsUnsupportedSecLevels) == 0)
				{
					return $"usmStatsUnsupportedSecLevels: {vb.Value.ToString()}";
				}
				if (vb.Oid.Compare(SnmpConstants.usmStatsNotInTimeWindows) == 0)
				{
					return $"usmStatsNotInTimeWindows: {vb.Value.ToString()}";
				}
				if (vb.Oid.Compare(SnmpConstants.usmStatsUnknownSecurityNames) == 0)
				{
					return $"usmStatsUnknownSecurityNames: {vb.Value.ToString()}";
				}
				if (vb.Oid.Compare(SnmpConstants.usmStatsUnknownEngineIDs) == 0)
				{
					return $"usmStatsUnknownEngineIDs: {vb.Value.ToString()}";
				}
				if (vb.Oid.Compare(SnmpConstants.usmStatsWrongDigests) == 0)
				{
					return $"usmStatsWrongDigests: {vb.Value.ToString()}";
				}
				if (vb.Oid.Compare(SnmpConstants.usmStatsDecryptionErrors) == 0)
				{
					return $"usmStatsDecryptionErrors: {vb.Value.ToString()}";
				}
				if (vb.Oid.Compare(SnmpConstants.snmpUnknownSecurityModels) == 0)
				{
					return $"snmpUnknownSecurityModels: {vb.Value.ToString()}";
				}
				if (vb.Oid.Compare(SnmpConstants.snmpInvalidMsgs) == 0)
				{
					return $"snmpInvalidMsgs: {vb.Value.ToString()}";
				}
				if (vb.Oid.Compare(SnmpConstants.snmpUnknownPDUHandlers) == 0)
				{
					return $"snmpUnknownPDUHandlers: {vb.Value.ToString()}";
				}
			}
			return "";
		}
	}
}
