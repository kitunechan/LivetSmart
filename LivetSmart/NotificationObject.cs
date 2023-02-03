using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;

namespace LivetSmart {
	/// <summary>
	/// 変更通知オブジェクトの基底クラスです。
	/// </summary>
	[Serializable]
	public class NotificationObject : INotifyPropertyChanged {
		/// <summary>
		/// プロパティ変更通知イベントです。
		/// </summary>
		[field: NonSerialized]
		public event PropertyChangedEventHandler? PropertyChanged;

		/// <summary>
		/// プロパティ変更通知イベントを発生させます。
		/// </summary>
		/// <param name="propertyExpression">() => プロパティ形式のラムダ式</param>
		/// <exception cref="NotSupportedException">() => プロパティ 以外の形式のラムダ式が指定されました。</exception>
		[Obsolete( "RaisePropertyChanged( nameof(propertyName) )の仕様を検討してください。" )]
		protected virtual void RaisePropertyChanged<T>( Expression<Func<T>> propertyExpression ) {
			var memberExpression = propertyExpression.Body as MemberExpression ?? throw new NotSupportedException( "このメソッドでは ()=>プロパティ の形式のラムダ式以外許可されません" );
			RaisePropertyChanged( memberExpression.Member.Name );
		}

		/// <summary>
		/// プロパティ変更通知イベントを発生させます
		/// </summary>
		/// <param name="propertyName">プロパティ名</param>
		protected virtual void RaisePropertyChanged( [CallerMemberName] string propertyName = "" ) {
			var threadSafeHandler = Interlocked.CompareExchange( ref PropertyChanged, null, null );
			threadSafeHandler?.Invoke( this, EventArgsFactory.GetPropertyChangedEventArgs( propertyName ) );
		}

	}

}
