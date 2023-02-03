using System.Windows;
using System;

namespace LivetSmart.Messaging {
	/// <summary>
	/// 画面遷移アクション用の相互作用メッセージです。
	/// </summary>
	public class ShowWindowMessage : ResponsiveMessage<bool?> {
		/// <summary>
		/// 相互作用メッセージのインスタンスを生成します。
		/// </summary>
		public ShowWindowMessage() {
		}

		/// <summary>
		/// メッセージキーを指定して相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public ShowWindowMessage( string? messageKey ) : base( messageKey ) { }


		/// <summary>
		/// Windowの型、Windowに設定するViewModel、メッセージキーを指定して相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		/// <param name="windowType">新しいWindowの型</param>
		/// <param name="viewModel">新しいWindowのDataContextに設定するViewModel</param>
		public ShowWindowMessage( string? messageKey, Type windowType, ViewModel viewModel ) : base( messageKey ) {
			ViewModel = viewModel;

			if( !windowType.IsSubclassOf( typeof( Window ) ) ) {
				throw new ArgumentException( "Windowの派生クラスを指定してください。", "windowType" );
			}

			WindowType = windowType;
		}


		/// <summary>
		/// 新しいWindowのDataContextに設定するViewModelを指定、または取得します。
		/// </summary>
		public ViewModel? ViewModel { get; set; }

		/// <summary>
		/// 新しいWindowの表示方法を決定するWindowModeを指定、または取得します。<br/>
		/// </summary>
		public WindowMode? Mode { get; set; }

		/// <summary>
		/// 新しいWindowの型を指定、または取得します。
		/// </summary>
		public Type? WindowType { get; set; }

		/// <summary>
		/// 遷移先ウィンドウがアクションのウィンドウに所有されるかを設定します。
		/// </summary>
		public bool? IsOwned { get; set; }



		public WindowState? WindowState { get; set; }

		public WindowStartupLocation? WindowStartupLocation { get; set; }


		/// <summary>
		/// ウインドウの設定を行う関数
		/// </summary>
		public Action<Window>? WindowSettingAction { get; set; }

		/// <summary>
		/// ウインドウコンテンツがレンダリングされた後に実行する関数
		/// </summary>
		public Action<Window>? InitializeAction { get; set; }

	}
}
