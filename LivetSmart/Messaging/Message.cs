namespace LivetSmart.Messaging {
	/// <summary>
	/// 相互作用メッセージの基底クラスです。<br/>
	/// Viewからのアクション実行後、戻り値情報が必要ない相互作用メッセージを作成する場合はこのクラスを継承して相互作用メッセージを作成します。
	/// </summary>
	public class Message : IMessage {
		public Message() {
		}

		/// <summary>
		/// メッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public Message( string? messageKey ) {
			this.MessageKey = messageKey;
		}

		/// <summary>
		/// メッセージキーを指定、または取得します。
		/// </summary>
		public string? MessageKey { get; init; }

		public bool IsHandled { get; set; }

	}
}