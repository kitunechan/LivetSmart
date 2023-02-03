namespace LivetSmart.Messaging {
	public interface IMessage {
		string? MessageKey { get; }

		bool IsHandled { get; set; }
	}
}