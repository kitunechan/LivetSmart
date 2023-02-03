using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace LivetSmart.Messaging {
	/// <summary>
	/// 複数のアクションに対応します。
	/// </summary>
	public class MultiMessageAction : TriggerAction<FrameworkElement> {
		protected override void Invoke( object parameter ) {
			if( parameter is not IMessage message ) {
				return;
			}

			switch( message ) {
				case MessageBoxMessage _message: {
					MessageBoxMessageAction.Action( AssociatedObject, _message );
					return;
				}

				case WindowCommandMessage _message: {
					WindowActionMessageAction.Action( AssociatedObject, _message );
					return;
				}

				case ShowWindowMessage _message: {
					ShowWindowMessageAction.Action( AssociatedObject, _message );
					return;
				}

				case OpenFileDialogMessage _message: {
					OpenFileDialogMessageAction.Action( AssociatedObject, _message );
					return;
				}

				case SaveFileDialogMessage _message: {
					SaveFileDialogMessageAction.Action( AssociatedObject, _message );
					return;
				}

				case IMethodMessage: {
					CallMethodAction.Action( AssociatedObject, message );
					return;
				}
			}
		}
	}
}
