using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace LivetSmart.EventListeners {
	internal class CollectionChangedEventHandlerBag : IEnumerable<KeyValuePair<NotifyCollectionChangedAction, List<NotifyCollectionChangedEventHandler>>> {
		private readonly Dictionary<NotifyCollectionChangedAction, List<NotifyCollectionChangedEventHandler>> _handlerDictionary = new();
		private readonly WeakReference<INotifyCollectionChanged> _source;

		private readonly List<NotifyCollectionChangedEventHandler> _allHandlerList = new();

		private readonly Dictionary<List<NotifyCollectionChangedEventHandler>, object> _lockObjectDictionary = new();

		private readonly object _handlerDictionaryLockObject = new();
		private readonly object _allHandlerListLockObject = new();

		internal CollectionChangedEventHandlerBag( INotifyCollectionChanged source ) {
			_source = new WeakReference<INotifyCollectionChanged>( source );
		}

		internal CollectionChangedEventHandlerBag( INotifyCollectionChanged source, NotifyCollectionChangedEventHandler handler ) : this( source ) {
			RegisterHandler( handler );
		}

		internal void RegisterHandler( NotifyCollectionChangedEventHandler handler ) {
			lock( _allHandlerListLockObject ) {
				_allHandlerList.Add( handler );
			}
		}

		internal void RegisterHandler( NotifyCollectionChangedAction action, NotifyCollectionChangedEventHandler handler ) {
			lock( _handlerDictionaryLockObject ) {
				if( !_handlerDictionary.TryGetValue( action, out var bag ) ) {
					bag = new List<NotifyCollectionChangedEventHandler>();
					_lockObjectDictionary.Add( bag, new object() );
					_handlerDictionary[action] = bag;
				}
				bag.Add( handler );
			}
		}

		internal void ExecuteHandler( NotifyCollectionChangedEventArgs e ) {
			var result = _source.TryGetTarget( out var sourceResult );

			if( !result ) return;

			List<NotifyCollectionChangedEventHandler>? list;
			lock( _handlerDictionaryLockObject ) {
				_handlerDictionary.TryGetValue( e.Action, out list );
			}
			if( list != null ) {
				lock( _lockObjectDictionary[list] ) {
					foreach( var handler in list ) {
						handler( sourceResult, e );
					}
				}
			}

			lock( _allHandlerListLockObject ) {
				if( _allHandlerList.Any() ) {
					foreach( var handler in _allHandlerList ) {
						handler( sourceResult, e );
					}
				}
			}
		}

		IEnumerator<KeyValuePair<NotifyCollectionChangedAction, List<NotifyCollectionChangedEventHandler>>> IEnumerable<KeyValuePair<NotifyCollectionChangedAction, List<NotifyCollectionChangedEventHandler>>>.GetEnumerator() {
			return _handlerDictionary.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return _handlerDictionary.GetEnumerator();
		}

		internal void Add( NotifyCollectionChangedEventHandler handler ) {
			RegisterHandler( handler );
		}

		internal void Add( NotifyCollectionChangedAction action, NotifyCollectionChangedEventHandler handler ) {
			RegisterHandler( action, handler );
		}


		internal void Add( NotifyCollectionChangedAction action, params NotifyCollectionChangedEventHandler[] handlers ) {
			foreach( var handler in handlers ) {
				RegisterHandler( action, handler );
			}
		}
	}
}
