using System.Windows;
using System;

namespace LivetSmart.Messaging {
	/// <summary>
	/// 画面遷移アクション用の相互作用メッセージです。
	/// </summary>
	public class ShowWindowMessage<TWindow> : ShowWindowMessage where TWindow : Window {
		public ShowWindowMessage() : base() {
			this.WindowType = typeof( TWindow );
		}

		/// <summary>
		/// メッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public ShowWindowMessage( string messageKey ) : base( messageKey ) {
			this.WindowType = typeof( TWindow );
		}

		/// <summary>
		/// ウインドウの設定を行う関数
		/// </summary>
		public new Action<TWindow>? WindowSettingAction {
			get => base.WindowSettingAction;
			set => base.WindowSettingAction = window => value?.Invoke( (TWindow)window );
		}

		/// <summary>
		/// ウインドウコンテンツがレンダリングされた後に実行する関数
		/// </summary>
		public new Action<TWindow>? InitializeAction {
			get => base.InitializeAction;
			set => base.InitializeAction = window => value?.Invoke( (TWindow)window );
		}

	}

	/// <summary>
	/// 画面遷移アクション用の相互作用メッセージです。
	/// </summary>
	public class ShowWindowMessage<TWindow, TViewModel> : ShowWindowMessage<TWindow> where TWindow : Window where TViewModel : ViewModel {
		public ShowWindowMessage() : base() {

		}

		/// <summary>
		/// メッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="messageKey">メッセージキー</param>
		public ShowWindowMessage( string messageKey ) : base( messageKey ) { }


		/// <summary>
		/// 新しいWindowのDataContextに設定するViewModelとメッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="viewModel">新しいWindowのDataContextに設定するViewModel</param>
		/// <param name="messageKey">メッセージキー</param>
		public ShowWindowMessage( string messageKey, TViewModel viewModel ) : base( messageKey ) {
			this.ViewModel = viewModel;
		}

		/// <summary>
		/// 新しいWindowのDataContextに設定するViewModelとメッセージキーを指定して新しい相互作用メッセージのインスタンスを生成します。
		/// </summary>
		/// <param name="ViewModel">新しいWindowのDataContextに設定するViewModel</param>
		public ShowWindowMessage( TViewModel viewModel ) : this() {
			this.ViewModel = viewModel;
		}


		/// <summary>
		/// 新しいWindowのDataContextに設定するViewModelを指定、または取得します。
		/// </summary>
		public new TViewModel? ViewModel {
			get { return (TViewModel?)base.ViewModel; }
			set { base.ViewModel = value; }
		}
	}

	public static class ShowWindowMessageGenerator<TWindow> where TWindow : Window {
		public static ShowWindowMessage<TWindow, TViewModel> Create<TViewModel>( TViewModel viewModel ) where TViewModel : ViewModel {
			return new ShowWindowMessage<TWindow, TViewModel>( viewModel ) {
				WindowType = typeof( TWindow )

			};
		}

		public static ShowWindowMessage<TWindow, TViewModel> Create<TViewModel>( string messageKey, TViewModel viewModel ) where TViewModel : ViewModel {
			return new ShowWindowMessage<TWindow, TViewModel>( messageKey, viewModel ) {
				WindowType = typeof( TWindow )

			};
		}
	}
}
