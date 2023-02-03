using System.Windows.Input;
using System;
using System.ComponentModel;

namespace LivetSmart.Commands {
	/// <summary>
	/// 汎用的コマンドを表します。
	/// </summary>
	public sealed class DelegateCommand : Command, ICommand, INotifyPropertyChanged {
		readonly Action _execute;
		readonly Func<bool>? _canExecute;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="execute">コマンドが実行するAction</param>
		public DelegateCommand( Action execute ) : this( execute, null ) { }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="execute">コマンドが実行するAction</param>
		/// <param name="canExecute">コマンドが実行可能かどうかをあらわすFunc&lt;bool&gt;</param>
		public DelegateCommand( Action execute, Func<bool>? canExecute ) {
			_execute = execute ?? throw new ArgumentNullException( "execute" );
			_canExecute = canExecute;
		}

		/// <summary>
		/// コマンドが実行可能かどうかを取得します。
		/// </summary>
		public bool CanExecute => _canExecute?.Invoke() ?? true;

		/// <summary>
		/// コマンドを実行します。
		/// </summary>
		public void Execute() {
			_execute();
		}

		/// <summary>
		/// コマンドを試行します。
		/// </summary>
		public void TryExecute() {
			if( CanExecute ) {
				_execute();
			}
		}

		void ICommand.Execute( object? parameter ) => Execute();

		bool ICommand.CanExecute( object? parameter ) => CanExecute;

		/// <summary>
		/// コマンドが実行可能かどうかが変化した時に発生します。
		/// </summary>
		public event PropertyChangedEventHandler? PropertyChanged;

		private void OnPropertyChanged() {
			PropertyChanged?.Invoke( this, EventArgsFactory.GetPropertyChangedEventArgs( nameof( CanExecute ) ) );
		}

		/// <summary>
		/// コマンドが実行可能かどうかが変化したことを通知します。
		/// </summary>
		public void RaiseCanExecuteChanged() {
			OnPropertyChanged();
			OnCanExecuteChanged();
		}
	}
}
