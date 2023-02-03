using Microsoft.Xaml.Behaviors;
using System.Windows;

namespace LivetSmart.Messaging {
	/// <summary>
	/// メッセージボックスを表示するアクションです。<see cref="MessageBoxMessage"/>に対応します。
	/// </summary>
	public class MessageBoxMessageAction : TriggerAction<FrameworkElement> {

		protected override void Invoke( object parameter ) {
			if( parameter is not MessageBoxMessage messageBoxMessage ) {
				return;
			}

			messageBoxMessage.IsOwned ??= IsOwned;

			Action( AssociatedObject, messageBoxMessage );
		}

		#region Register IsOwned
		/// <summary>
		/// メッセージボックスがこのウィンドウに所有されるかを設定します。
		/// </summary>
		public bool IsOwned {
			get { return (bool)GetValue( OwnedFromThisProperty ); }
			set { SetValue( OwnedFromThisProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for OwnedFromThis.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty OwnedFromThisProperty =
			DependencyProperty.Register( "IsOwned", typeof( bool ), typeof( MessageBoxMessageAction ), new PropertyMetadata( true ) );
		#endregion

		public static void Action( FrameworkElement element, MessageBoxMessage message ) {
			message.IsHandled = true;

			var window = message.IsOwned == true ? Window.GetWindow( element ) : null;

			if( window is null ) {
				message.Response = MessageBox.Show(
					message.Text,
					message.Caption,
					message.Button,
					message.Image,
					message.DefaultResult
					);
			} else {
				message.Response = MessageBox.Show(
					window,
					message.Text,
					message.Caption,
					message.Button,
					message.Image,
					message.DefaultResult
					);
			}
		}
	}
}
