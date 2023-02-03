using LivetSmart.EventListeners;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace LivetSmart.WeakEventListeners {
	/// <summary>
	/// INotifyPropertyChanged.PropertyChangedを受信するためのWeakイベントリスナです。
	/// </summary>
	public sealed class LivetPropertyChangedWeakEventListener : LivetWeakEventListener<PropertyChangedEventHandler, PropertyChangedEventArgs>, IEnumerable<KeyValuePair<string, List<PropertyChangedEventHandler>>> {
		private PropertyChangedEventHandlerBag _bag;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">INotifyPropertyChangedオブジェクト</param>
		public LivetPropertyChangedWeakEventListener( INotifyPropertyChanged source ) {
			_bag = new PropertyChangedEventHandlerBag( source );
			Initialize(
				h => new PropertyChangedEventHandler( h ),
				h => source.PropertyChanged += h,
				h => source.PropertyChanged -= h,
				( sender, e ) => _bag.ExecuteHandler( e ) );
		}

		/// <summary>
		/// コンストラクタ。リスナのインスタンスの作成と同時にハンドラを一つ登録します。
		/// </summary>
		/// <param name="source">INotifyPropertyChangedオブジェクト</param>
		/// <param name="handler">PropertyChangedイベントハンドラ</param>
		public LivetPropertyChangedWeakEventListener( INotifyPropertyChanged source, PropertyChangedEventHandler handler ) {
			_bag = new PropertyChangedEventHandlerBag( source, handler );
			Initialize(
				 h => new PropertyChangedEventHandler( h ),
				 h => source.PropertyChanged += h,
				 h => source.PropertyChanged -= h,
				 ( sender, e ) => _bag.ExecuteHandler( e ) );
		}

		/// <summary>
		/// このリスナインスタンスに新たなハンドラを追加します。
		/// </summary>
		/// <param name="handler">PropertyChangedイベントハンドラ</param>
		public void RegisterHandler( PropertyChangedEventHandler handler ) {
			ThrowExceptionIfDisposed();
			_bag.RegisterHandler( handler );
		}

		/// <summary>
		/// このリスナインスタンスにプロパティ名でフィルタリング済のハンドラを追加します。
		/// </summary>
		/// <param name="propertyName">ハンドラを登録したいPropertyChagedEventArgs.PropertyNameの名前</param>
		/// <param name="handler">propertyNameで指定されたプロパティ用のPropertyChangedイベントハンドラ</param>
		public void RegisterHandler( string propertyName, PropertyChangedEventHandler handler ) {
			ThrowExceptionIfDisposed();
			_bag.RegisterHandler( propertyName, handler );
		}

		/// <summary>
		/// このリスナインスタンスにプロパティ名でフィルタリング済のハンドラを追加します。
		/// </summary>
		/// <param name="propertyExpression">() => プロパティ形式のラムダ式</param>
		/// <param name="handler">propertyExpressionで指定されたプロパティ用のPropertyChangedイベントハンドラ</param>
		[Obsolete( "nameof() の仕様を検討してください。" )]
		public void RegisterHandler<T>( Expression<Func<T>> propertyExpression, PropertyChangedEventHandler handler ) {
			ThrowExceptionIfDisposed();
			_bag.RegisterHandler( propertyExpression, handler );
		}

		IEnumerator<KeyValuePair<string, List<PropertyChangedEventHandler>>> IEnumerable<KeyValuePair<string, List<PropertyChangedEventHandler>>>.GetEnumerator() {
			ThrowExceptionIfDisposed();
			return
				( (IEnumerable<KeyValuePair<string, List<PropertyChangedEventHandler>>>)_bag )
					.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			ThrowExceptionIfDisposed();
			return ( (IEnumerable<KeyValuePair<string, List<PropertyChangedEventHandler>>>)_bag ).GetEnumerator();
		}

		public void Add( PropertyChangedEventHandler handler ) {
			ThrowExceptionIfDisposed();
			_bag.Add( handler );
		}

		public void Add( string propertyName, PropertyChangedEventHandler handler ) {
			ThrowExceptionIfDisposed();
			_bag.Add( propertyName, handler );
		}

		[Obsolete( "記述方法が間違っています。", true )]
		public void Add( string propertyName ) {
			throw new NotSupportedException( "記述方法が間違っています。" );
		}

		public void Add( string propertyName, params PropertyChangedEventHandler[] handlers ) {
			ThrowExceptionIfDisposed();
			_bag.Add( propertyName, handlers );
		}

		[Obsolete( "nameof() の仕様を検討してください。" )]
		public void Add<T>( Expression<Func<T>> propertyExpression, PropertyChangedEventHandler handler ) {
			ThrowExceptionIfDisposed();
			_bag.Add( propertyExpression, handler );
		}

		[Obsolete( "記述方法が間違っています。", true )]
		public void Add<T>( Expression<Func<T>> propertyExpression ) {
			throw new NotSupportedException( "記述方法が間違っています。" );
		}

		[Obsolete( "nameof() の仕様を検討してください。" )]
		public void Add<T>( Expression<Func<T>> propertyExpression, params PropertyChangedEventHandler[] handlers ) {
			ThrowExceptionIfDisposed();
			_bag.Add( propertyExpression, handlers );
		}
	}
}
