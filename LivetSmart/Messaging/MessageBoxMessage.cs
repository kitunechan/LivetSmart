using System.Windows;

namespace LivetSmart.Messaging {
	/// <summary>
	/// メッセージボックスを表示ズルメッセージです。
	/// </summary>
	public class MessageBoxMessage : ResponsiveMessage<MessageBoxResult> {

		public MessageBoxMessage() { }

		/// <summary>
		/// メッセージキーを指定して、新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public MessageBoxMessage( string? messageKey ) : base( messageKey ) { }

		/// <summary>
		/// メッセージボックスがアクションの親ウインドウに所有されるかを設定します。
		/// </summary>
		public bool? IsOwned { get; set; }

		/// <summary>
		/// 表示するメッセージを指定、または取得します。
		/// </summary>
		public string? Text { get; set; }

		/// <summary>
		/// キャプション（タイトル部分）を指定、または取得します。
		/// </summary>
		public string? Caption { get; set; }


		/// <summary>
		/// メッセージボックスイメージを指定、または取得します。
		/// </summary>
		public MessageBoxImage Image { get; set; }

		/// <summary>
		/// メッセージボックスボタンを指定、または取得します。
		/// </summary>
		public MessageBoxButton Button { get; set; }

		/// <summary>
		/// メッセージボックスの既定の結果を指定、または取得します。
		/// </summary>
		public MessageBoxResult DefaultResult { get; set; }
	}
}