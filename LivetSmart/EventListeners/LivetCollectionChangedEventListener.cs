using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace LivetSmart.EventListeners {
	/// <summary>
	/// INotifyCollectionChanged.NotifyCollectionChangedを受信するためのイベントリスナです。
	/// </summary>
	public sealed class LivetCollectionChangedEventListener : LivetEventListener<NotifyCollectionChangedEventHandler>, IEnumerable<KeyValuePair<NotifyCollectionChangedAction, List<NotifyCollectionChangedEventHandler>>> {
		private readonly CollectionChangedEventHandlerBag _bag;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">INotifyCollectionChangedオブジェクト</param>
		public LivetCollectionChangedEventListener( INotifyCollectionChanged source ) {
			_bag = new CollectionChangedEventHandlerBag( source );
			Initialize( h => source.CollectionChanged += h, h => source.CollectionChanged -= h, ( sender, e ) => _bag.ExecuteHandler( e ) );
		}

		/// <summary>
		/// コンストラクタ。リスナのインスタンスの作成と同時にハンドラを一つ登録します。
		/// </summary>
		/// <param name="source">INotifyCollectionChangedオブジェクト</param>
		/// <param name="handler">NotifyCollectionChangedイベントハンドラ</param>
		public LivetCollectionChangedEventListener( INotifyCollectionChanged source, NotifyCollectionChangedEventHandler handler ) {
			_bag = new CollectionChangedEventHandlerBag( source, handler );
			Initialize( h => source.CollectionChanged += h, h => source.CollectionChanged -= h, ( sender, e ) => _bag.ExecuteHandler( e ) );
		}

		/// <summary>
		/// このリスナインスタンスに新たなハンドラを追加します。
		/// </summary>
		/// <param name="handler">NotifyCollectionChangedイベントハンドラ</param>
		public void RegisterHandler( NotifyCollectionChangedEventHandler handler ) {
			ThrowExceptionIfDisposed();
			_bag.RegisterHandler( handler );
		}

		/// <summary>
		/// このリスナインスタンスにプロパティ名でフィルタリング済のハンドラを追加します。
		/// </summary>
		/// <param name="action">ハンドラを登録したいNotifyCollectionChangedAction</param>
		/// <param name="handler">actionで指定されたNotifyCollectionChangedActionに対応したNotifyCollectionChangedイベントハンドラ</param>
		public void RegisterHandler( NotifyCollectionChangedAction action, NotifyCollectionChangedEventHandler handler ) {
			ThrowExceptionIfDisposed();
			_bag.RegisterHandler( action, handler );
		}

		IEnumerator<KeyValuePair<NotifyCollectionChangedAction, List<NotifyCollectionChangedEventHandler>>> IEnumerable<KeyValuePair<NotifyCollectionChangedAction, List<NotifyCollectionChangedEventHandler>>>.GetEnumerator() {
			return ( (IEnumerable<KeyValuePair<NotifyCollectionChangedAction, List<NotifyCollectionChangedEventHandler>>>)_bag ).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return ( (IEnumerable<KeyValuePair<NotifyCollectionChangedAction, List<NotifyCollectionChangedEventHandler>>>)_bag ).GetEnumerator();
		}

		public void Add( NotifyCollectionChangedEventHandler handler ) {
			_bag.Add( handler );
		}

		public void Add( NotifyCollectionChangedAction action, NotifyCollectionChangedEventHandler handler ) {
			_bag.Add( action, handler );
		}


		public void Add( NotifyCollectionChangedAction action, params NotifyCollectionChangedEventHandler[] handlers ) {
			_bag.Add( action, handlers );
		}
	}
}
