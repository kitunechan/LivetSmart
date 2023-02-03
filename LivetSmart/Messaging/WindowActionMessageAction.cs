using Microsoft.Xaml.Behaviors;
using System.Windows;

namespace LivetSmart.Messaging {
	/// <summary>
	/// Windowの最小化・最大化・閉じる・通常化・ダイアログ結果を行うアクションです。<see cref="WindowCommandMessage">に対応します。
	/// </summary>
	public class WindowActionMessageAction : TriggerAction<FrameworkElement> {
		protected override void Invoke( object parameter ) {
			if( parameter is not WindowCommandMessage message ) {
				return;
			}

			Action( AssociatedObject, message );
		}

		public static void Action( FrameworkElement element, WindowCommandMessage message ) {
			var window = Window.GetWindow( element );
			if( window != null ) {
				message.IsHandled = true;
				switch( message.Command ) {
					case WindowCommand.Close: {
						window.Close();
						break;
					}
					case WindowCommand.Maximize: {
						window.WindowState = WindowState.Maximized;
						break;
					}
					case WindowCommand.Minimize: {
						window.WindowState = WindowState.Minimized;
						break;
					}
					case WindowCommand.Normal: {
						window.WindowState = WindowState.Normal;
						break;
					}
					case WindowCommand.Active: {
						window.Activate();
						break;
					}
					case WindowCommand.ResultOK: {
						window.DialogResult = true;
						break;
					}
					case WindowCommand.ResultCancel: {
						window.DialogResult = false;
						break;
					}

					default: {
						break;
					}
				}
			}
		}
	}
}
