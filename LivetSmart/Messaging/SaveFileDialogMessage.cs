namespace LivetSmart.Messaging {
	/// <summary>
	/// ファイルを保存する 用の相互作用メッセージです。
	/// </summary>
	public class SaveFileDialogMessage : FileDialogMessage {
		public SaveFileDialogMessage() {
		}

		/// <summary>
		/// メッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public SaveFileDialogMessage( string messageKey ) : base( messageKey ) {
		}

		/// <summary>
		/// ユーザーが存在しないファイルを指定した場合に、ファイルを作成することを確認するメッセージを表示するかどうかを指定、または取得します。デフォルトはfalseです。
		/// </summary>
		public bool CreatePrompt { get; set; }

		/// <summary>
		/// ユーザーが指定したファイルが存在する場合、上書き確認メッセージを表示するかどうかを指定、または取得します。デフォルトはtrueです。
		/// </summary>
		public bool OverwritePrompt { get; set; }
	}
}