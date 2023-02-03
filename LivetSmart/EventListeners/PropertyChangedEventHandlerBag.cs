using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace LivetSmart.EventListeners {
	internal class PropertyChangedEventHandlerBag : IEnumerable<KeyValuePair<string, List<PropertyChangedEventHandler>>> {
		private readonly Dictionary<string, List<PropertyChangedEventHandler>> _handlerDictionary = new();
		private readonly WeakReference<INotifyPropertyChanged> _source;

		private readonly object _handlerDictionaryLockObject = new();
		private readonly Dictionary<List<PropertyChangedEventHandler>, object> _lockObjectDictionary = new();

		internal PropertyChangedEventHandlerBag( INotifyPropertyChanged source ) {
			_source = new WeakReference<INotifyPropertyChanged>( source );
		}

		internal PropertyChangedEventHandlerBag( INotifyPropertyChanged source, PropertyChangedEventHandler handler ) : this( source ) {
			RegisterHandler( handler );
		}

		internal void RegisterHandler( PropertyChangedEventHandler handler ) {
			RegisterHandler( string.Empty, handler );
		}

		internal void RegisterHandler( string propertyName, PropertyChangedEventHandler handler ) {
			lock( _handlerDictionaryLockObject ) {
				if( !_handlerDictionary.TryGetValue( propertyName, out var bag ) ) {
					bag = new List<PropertyChangedEventHandler>();
					_lockObjectDictionary.Add( bag, new object() );
					_handlerDictionary[propertyName] = bag;
				}
				bag.Add( handler );
			}
		}

		[Obsolete( "RegisterHandler( nameof() ) の仕様を検討してください。" )]
		internal void RegisterHandler<T>( Expression<Func<T>> propertyExpression, PropertyChangedEventHandler handler ) {
			var memberExpression = propertyExpression.Body as MemberExpression
										?? throw new NotSupportedException( "このメソッドでは ()=>プロパティ の形式のラムダ式以外許可されません" );
			RegisterHandler( memberExpression.Member.Name, handler );
		}

		internal void ExecuteHandler( PropertyChangedEventArgs e ) {
			var result = _source.TryGetTarget( out var sourceResult );

			if( !result ) return;

			if( e.PropertyName != null ) {
				List<PropertyChangedEventHandler>? list;
				lock( _handlerDictionaryLockObject ) {
					_handlerDictionary.TryGetValue( e.PropertyName, out list );
				}

				if( list != null ) {
					lock( _lockObjectDictionary[list] ) {
						foreach( var handler in list ) {
							handler( sourceResult, e );
						}
					}
				}
			}

			lock( _handlerDictionaryLockObject ) {
				_handlerDictionary.TryGetValue( string.Empty, out var allList );
				if( allList != null ) {
					lock( _lockObjectDictionary[allList] ) {
						foreach( var handler in allList ) {
							handler( sourceResult, e );
						}
					}
				}
			}
		}

		IEnumerator<KeyValuePair<string, List<PropertyChangedEventHandler>>> IEnumerable<KeyValuePair<string, List<PropertyChangedEventHandler>>>.GetEnumerator() {
			return _handlerDictionary.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return _handlerDictionary.GetEnumerator();
		}

		internal void Add( PropertyChangedEventHandler handler ) {
			RegisterHandler( handler );
		}

		internal void Add( string propertyName, PropertyChangedEventHandler handler ) {
			RegisterHandler( propertyName, handler );
		}


		internal void Add( string propertyName, params PropertyChangedEventHandler[] handlers ) {
			foreach( var handler in handlers ) {
				RegisterHandler( propertyName, handler );
			}
		}

		[Obsolete( "nameof()の仕様を検討してください。" )]
		internal void Add<T>( Expression<Func<T>> propertyExpression, PropertyChangedEventHandler handler ) {
			var memberExpression = propertyExpression.Body as MemberExpression
										?? throw new NotSupportedException( "このメソッドでは ()=>プロパティ の形式のラムダ式以外許可されません" );

			Add( memberExpression.Member.Name, handler );
		}


		[Obsolete( "nameof()の仕様を検討してください。" )]
		internal void Add<T>( Expression<Func<T>> propertyExpression, params PropertyChangedEventHandler[] handlers ) {
			var memberExpression = propertyExpression.Body as MemberExpression
										?? throw new NotSupportedException( "このメソッドでは ()=>プロパティ の形式のラムダ式以外許可されません" );

			Add( memberExpression.Member.Name, handlers );
		}
	}
}
