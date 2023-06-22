namespace SnmpSharpNet
{
	public interface IAgentParameters
	{
		SnmpVersion Version
		{
			get;
		}

		bool Valid();

		void InitializePacket(SnmpPacket packet);

		object Clone();
	}
}
