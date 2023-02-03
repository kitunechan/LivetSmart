namespace LivetSmart.Messaging {

	public abstract class ResponsiveMessage<T> : IMessage {
		public ResponsiveMessage() {
		}

		/// <summary>
		/// メッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public ResponsiveMessage( string? messageKey ) {
			this.MessageKey = messageKey;
		}

		/// <summary>
		/// メッセージキーを指定、または取得します。
		/// </summary>
		public string? MessageKey { get; }

		public bool IsHandled { get; set; }

		public T? Response { get; set; }
	}

}