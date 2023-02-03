namespace LivetSmart.Messaging {
	/// <summary>
	/// Windowを最大化・最小化・閉じる・通常化・ダイアログの結果の相互作用メッセージです。
	/// </summary>
	public class WindowCommandMessage : Message {

		#region StaticMessage

		/// <summary>
		/// メッセージキーの無い WindowCommand.Close のメッセージを取得します。
		/// </summary>
		public static WindowCommandMessage Close => _Close ??= new( WindowCommand.Close );
		static WindowCommandMessage? _Close;

		/// <summary>
		/// メッセージキーの無い WindowCommand.Minimize のメッセージを取得します。
		/// </summary>
		public static WindowCommandMessage Minimize => _Minimize ??= new( WindowCommand.Minimize );
		static WindowCommandMessage? _Minimize;

		/// <summary>
		/// メッセージキーの無い WindowCommand.Maximize のメッセージを取得します。
		/// </summary>
		public static WindowCommandMessage Maximize => _Maximize ??= new( WindowCommand.Maximize );
		static WindowCommandMessage? _Maximize;

		/// <summary>
		/// メッセージキーの無い WindowCommand.Normal のメッセージを取得します。
		/// </summary>
		public static WindowCommandMessage Normal => _Normal ??= new( WindowCommand.Normal );
		static WindowCommandMessage? _Normal;

		/// <summary>
		/// メッセージキーの無い WindowCommand.Active のメッセージを取得します。
		/// </summary>
		public static WindowCommandMessage Active => _Active ??= new( WindowCommand.Active );
		static WindowCommandMessage? _Active;

		/// <summary>
		/// メッセージキーの無い WindowCommand.ResultOK のメッセージを取得します。
		/// </summary>
		public static WindowCommandMessage ResultOK => _ResultOK ??= new( WindowCommand.ResultOK );
		static WindowCommandMessage? _ResultOK;

		/// <summary>
		/// メッセージキーの無い WindowCommand.ResultCancel のメッセージを取得します。
		/// </summary>
		public static WindowCommandMessage ResultCancel => _ResultCancel ??= new( WindowCommand.ResultCancel );
		static WindowCommandMessage? _ResultCancel;

		#endregion

		public WindowCommandMessage() {
		}

		/// <summary>
		/// メッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public WindowCommandMessage( string messageKey ) : base( messageKey ) { }

		/// <summary>
		/// Windowが遷移すべき状態を定義して、新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="command">Windowが遷移すべき状態を表す<see cref="WindowCommand"/>列挙体</param>
		public WindowCommandMessage( WindowCommand command ) : this( string.Empty, command ) { }

		/// <summary>
		/// メッセージキーとWindowが遷移すべき状態を定義して、新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="command">Windowが遷移すべき状態を表す<see cref="WindowCommand"/>列挙体</param>
		/// <param name="messageKey">メッセージキー</param>
		public WindowCommandMessage( string messageKey, WindowCommand command ) : this( messageKey ) {
			Command = command;
		}

		/// <summary>
		/// Windowが遷移すべき状態を表す<see cref="WindowCommand"/>列挙体を指定、または取得します。
		/// </summary>
		public WindowCommand Command { get; set; }

	}
}
