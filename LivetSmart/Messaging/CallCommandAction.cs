using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Input;

namespace LivetSmart.Messaging {
	/// <summary>
	/// コマンドを実行するアクションです。<see cref="Message"/>に対応します。
	/// </summary>
	public class CallCommandAction : TriggerAction<DependencyObject> {
		protected override void Invoke( object parameter ) {
			if( UseCommandParameter ) {
				if( Command.CanExecute( CommandParameter ) ) {
					Command.Execute( CommandParameter );
				}
			} else {
				if( Command.CanExecute( parameter ) ) {
					Command.Execute( parameter );
				}
			}
		}

		#region Register Command
		public ICommand Command {
			get => (ICommand)GetValue( CommandProperty );
			set => SetValue( CommandProperty, value );
		}

		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.Register( nameof( Command ), typeof( ICommand ), typeof( CallCommandAction ), new PropertyMetadata( default( ICommand ) ) );
		#endregion

		/// <summary>
		/// 引数として<see cref="CommandParameter"/>を使用するかどうかを、設定または取得します。
		/// </summary>
		public bool UseCommandParameter { get; set; }

		#region Register CommandParameter
		public object CommandParameter {
			get => GetValue( CommandParameterProperty );
			set => SetValue( CommandParameterProperty, value );
		}

		public static readonly DependencyProperty CommandParameterProperty =
			DependencyProperty.Register( nameof( CommandParameter ), typeof( object ), typeof( CallCommandAction ), new PropertyMetadata( default( object ) ) );
		#endregion
	}
}
