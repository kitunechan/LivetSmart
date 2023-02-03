namespace LivetSmart.Messaging {
	/// <summary>
	/// ファイルを開く アクション用の相互作用メッセージです。
	/// </summary>
	public class OpenFileDialogMessage : FileDialogMessage {
		public OpenFileDialogMessage() {
		}

		/// <summary>
		/// メッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public OpenFileDialogMessage( string? messageKey ) : base( messageKey ) {
		}

		/// <summary>
		/// 複数ファイルを選択可能かを取得、または設定します。
		/// </summary>
		public bool MultiSelect { get; set; } = false;

		/// <summary>
		/// ファイル ダイアログに表示される初期ディレクトリのグループを取得または設定します。
		/// </summary>
		public string InitialDirectoryGroup { get; set; } = string.Empty;
	}
}