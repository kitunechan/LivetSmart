using System.Windows.Threading;
using System.ComponentModel;
using System.Windows;

namespace LivetSmart {
	/// <summary>
	/// UIDispatcherへのアクセスを簡易化します。
	/// </summary>
	public static class LivetDispatcherHelper {

		/// <summary>
		/// UIDispatcherを指定、または取得します。通常このプロパティはApp_StartUpで指定されます。
		/// </summary>
		public static Dispatcher? UIDispatcher {
			get {
				if( (bool)( DesignerProperties.IsInDesignModeProperty.GetMetadata( typeof( DependencyObject ) ).DefaultValue ) ) {
					_uiDispatcher = Dispatcher.CurrentDispatcher;
				}
				return _uiDispatcher;
			}
			set {
				_uiDispatcher = value;
			}
		}
		private static Dispatcher? _uiDispatcher;
	}
}
