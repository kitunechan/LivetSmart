using System;
using System.Linq;
using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace LivetSmart.Messaging {
	/// <summary>
	/// 画面遷移(Window)を行うアクションです。<see cref="ShowWindowMessage"/>に対応します。
	/// </summary>
	public class ShowWindowMessageAction : TriggerAction<FrameworkElement> {

		#region Register WindowType

		/// <summary>
		/// 遷移するウインドウの型を指定、または取得します。
		/// </summary>
		public Type? WindowType {
			get { return (Type?)GetValue( WindowTypeProperty ); }
			set { SetValue( WindowTypeProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for WindowType.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty WindowTypeProperty =
			DependencyProperty.Register( "WindowType", typeof( Type ), typeof( ShowWindowMessageAction ), new PropertyMetadata() );

		#endregion

		#region Register Mode
		/// <summary>
		/// 画面遷移の種類を指定するTransitionMode列挙体を指定、または取得します。<br/>
		/// MessageでModeがUnKnown以外に指定されていた場合、そちらが優先されます。
		/// </summary>
		public WindowMode Mode {
			get { return (WindowMode)GetValue( ModeProperty ); }
			set { SetValue( ModeProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for Mode.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ModeProperty =
			DependencyProperty.Register( "Mode", typeof( WindowMode ), typeof( ShowWindowMessageAction ), new PropertyMetadata( WindowMode.Modal ) );
		#endregion

		#region Register IsOwned
		/// <summary>
		/// 遷移先ウィンドウがこのウィンドウに所有されるかを設定します。
		/// </summary>
		public bool IsOwned {
			get { return (bool)GetValue( OwnedFromThisProperty ); }
			set { SetValue( OwnedFromThisProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for OwnedFromThis.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty OwnedFromThisProperty =
			DependencyProperty.Register( "IsOwned", typeof( bool ), typeof( ShowWindowMessageAction ), new PropertyMetadata( true ) );
		#endregion

		#region Register WindowState
		public WindowState WindowState {
			get { return (WindowState)GetValue( WindowStateProperty ); }
			set { SetValue( WindowStateProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for WindowState.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty WindowStateProperty =
			DependencyProperty.Register( nameof( WindowState ), typeof( WindowState ), typeof( ShowWindowMessageAction ), new PropertyMetadata( WindowState.Normal ) );
		#endregion

		protected override void Invoke( object parameter ) {
			if( parameter is not ShowWindowMessage message ) {
				return;
			}

			message.WindowType ??= WindowType ?? throw new ArgumentNullException();
			message.Mode ??= Mode;
			message.IsOwned ??= IsOwned;
			message.WindowState ??= WindowState;

			Action( AssociatedObject, message );
		}

		private static bool IsValidWindowType( Type value ) {
			if( value is not null ) {
				if( value.IsSubclassOf( typeof( Window ) ) ) {
					return value.GetConstructor( Type.EmptyTypes ) is not null;
				}
			}

			return false;
		}


		public static void Action( FrameworkElement element, ShowWindowMessage message ) {
			var targetType = message.WindowType!;

			if( !IsValidWindowType( targetType ) ) {
				return;
			}
			message.IsHandled = true;

			var defaultConstructor = targetType.GetConstructor( Type.EmptyTypes ) ?? throw new ArgumentNullException();

			var mode = message.Mode;

			switch( mode ) {
				case WindowMode.Modeless:
				case WindowMode.Modal: {
					var targetWindow = (Window)defaultConstructor.Invoke( null );
					if( message.ViewModel != null ) {
						targetWindow.DataContext = message.ViewModel;
					}

					if( message.IsOwned == true ) {
						targetWindow.Owner = Window.GetWindow( element );
					}

					if( message.WindowStartupLocation.HasValue ) {
						targetWindow.WindowStartupLocation = message.WindowStartupLocation.Value;
					}

					targetWindow.WindowState = message.WindowState ?? WindowState.Normal;

					message.WindowSettingAction?.Invoke( targetWindow );
					targetWindow.ContentRendered += ( x, e ) => {
						message.InitializeAction?.Invoke( targetWindow );
					};

					if( mode == WindowMode.Modeless ) {
						targetWindow.Show();
						message.Response = null;

					} else {
						targetWindow.StateChanged += ( s, e ) => {
							if( s is Window window ) {
								if( window.WindowState == WindowState.Minimized ) {
									if( window.Owner != null ) {
										window.Owner.WindowState = WindowState.Minimized;
									}
								}
							}
						};

						if( !( targetWindow is IWindowClosed isClosedWindow && isClosedWindow.IsClosed ) ) {
							message.Response = targetWindow.ShowDialog();
						}
					}

					message.ViewModel ??= targetWindow.DataContext as ViewModel;

					break;
				}

				case WindowMode.NewOrActive: {
					var window = Application.Current.Windows
						.OfType<Window>()
						.FirstOrDefault( w => w.GetType() == targetType );


					if( window == null ) {
						window = (Window)defaultConstructor.Invoke( null );

						if( message.ViewModel != null ) {
							window.DataContext = message.ViewModel;
						}
						if( message.WindowStartupLocation.HasValue ) {
							window.WindowStartupLocation = message.WindowStartupLocation.Value;
						}
						if( message.IsOwned == true ) {
							window.Owner = Window.GetWindow( element );
						}

						window.WindowState = message.WindowState ?? WindowState.Normal;

						message.WindowSettingAction?.Invoke( window );
						window.ContentRendered += ( x, e ) => {
							message.InitializeAction?.Invoke( window );
						};

						window.Show();
						message.Response = null;
					} else {
						if( message.ViewModel != null ) {
							window.DataContext = message.ViewModel;
						}
						if( message.WindowStartupLocation.HasValue ) {
							window.WindowStartupLocation = message.WindowStartupLocation.Value;
						}
						if( message.IsOwned == true ) {
							window.Owner = Window.GetWindow( element );
						}
						window.Activate();
						window.WindowState = message.WindowState ?? WindowState.Normal;

						message.Response = null;
					}

					break;
				}
			}

		}

	}

	public interface IWindowClosed {
		bool IsClosed { get; }
	}
}
