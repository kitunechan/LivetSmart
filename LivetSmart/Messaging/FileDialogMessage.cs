namespace LivetSmart.Messaging {
	/// <summary>
	/// ファイルを開く・ファイルを保存するアクション用の共通相互作用メッセージ基底抽象クラスです。<br/>
	/// ファイルを開くアクションをViewに行わせたい場合は、<see cref="FileDialogMessage"/>を使用してください。<br/>
	/// ファイルを保存するアクションをViewに行わせたい場合は、<see cref="SaveFileDialogMessage"/>を使用してください。
	/// </summary>
	public abstract class FileDialogMessage : ResponsiveMessage<string[]?> {
		protected FileDialogMessage() {
		}

		protected FileDialogMessage( string? messageKey ) : base( messageKey ) {
		}

		/// <summary>
		/// ダイアログタイトルを指定、または取得します。
		/// </summary>
		public string Title { get; set; } = string.Empty;

		/// <summary>
		/// ファイルの拡張子Filterを指定、または取得します。
		/// </summary>
		public string Filter { get; set; } = string.Empty;

		/// <summary>
		/// ファイル ダイアログで現在選択されているフィルターのインデックスを取得または設定します。 既定値は1です。
		/// </summary>
		public int FilterIndex { get; set; } = 1;

		/// <summary>
		/// 拡張子を指定しなかった場合、自動で拡張子を追加するかどうかを指定、または取得します。デフォルトはtrueです。
		/// </summary>
		public bool AddExtension { get; set; } = true;

		/// <summary>
		/// ダイアログに表示される初期ディレクトリを指定、または取得します。
		/// </summary>
		public string InitialDirectory { get; set; } = string.Empty;

		/// <summary>
		/// ファイルダイアログで指定されたファイルのパスを含む文字列を指定、または取得します。
		/// </summary>
		public string FileName { get; set; } = string.Empty;


		/// <summary>
		/// ユーザーが無効なパスとファイル名を入力した場合に警告を表示するかどうかを指定する値を取得または設定します。
		///  警告を表示する場合は true。それ以外の場合は false。 既定値は、true です。
		/// </summary>
		public bool CheckPathExists { get; set; } = true;

		/// <summary>
		/// 存在しないファイル名をユーザーが指定した場合に、ファイル ダイアログで警告を表示するかどうかを示す値を取得または設定します。
		/// 警告を表示する場合は true。それ以外の場合は false。 この基本クラスの既定値は false です。
		/// </summary>
		public bool CheckFileExists { get; set; } = false;

		/// <summary>
		/// 既定のファイル名の拡張子を取得または設定します。
		/// </summary>
		public string DefaultExt { get; set; } = string.Empty;

	}
}