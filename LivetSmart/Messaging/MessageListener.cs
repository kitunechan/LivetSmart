using System.Collections.Concurrent;
using System.Windows.Threading;
using System;
using System.Collections.Generic;
using LivetSmart.WeakEventListeners;

namespace LivetSmart.Messaging {
	public sealed class MessageListener : IDisposable, IEnumerable<KeyValuePair<string, ConcurrentBag<Action<IMessage>>>> {
		private LivetWeakEventListener<EventHandler<MessageSendEventArgs>, MessageSendEventArgs> _listener;
		private WeakReference<Messenger> _source;
		private ConcurrentDictionary<string, ConcurrentBag<Action<IMessage>>> _actionDictionary = new ConcurrentDictionary<string, ConcurrentBag<Action<IMessage>>>();

		public MessageListener( Messenger messenger ) {
			Dispatcher = Dispatcher.CurrentDispatcher;
			_source = new WeakReference<Messenger>( messenger );
			_listener = new LivetWeakEventListener<EventHandler<MessageSendEventArgs>, MessageSendEventArgs>
				(
					h => h,
					h => messenger.SendLater += h,
					h => messenger.SendLater -= h,
					MessageReceived
				);
		}
		public MessageListener( Messenger messenger, Action<IMessage> action ) : this( messenger, string.Empty, action ) {
		}

		public MessageListener( Messenger sendMessenger, Messenger receiveMessenger ) : this( sendMessenger ) {
			RegisterAction( message => {
				receiveMessenger.Send( message );
			} );
		}

		public MessageListener( Messenger messenger, string messageKey, Action<IMessage> action ) : this( messenger ) {
			RegisterAction( messageKey, action );
		}

		public void RegisterAction( Action<IMessage> action ) {
			ThrowExceptionIfDisposed();
			_actionDictionary.GetOrAdd( string.Empty, _ => new ConcurrentBag<Action<IMessage>>() ).Add( action );
		}

		public void RegisterAction( string messageKey, Action<IMessage> action ) {
			ThrowExceptionIfDisposed();
			_actionDictionary.GetOrAdd( messageKey, _ => new ConcurrentBag<Action<IMessage>>() ).Add( action );
		}

		private void MessageReceived( object? sender, MessageSendEventArgs e ) {
			if( _disposed ) return;

			var message = e.Message;
			if( !message.IsHandled ) {
				DoActionOnDispatcher( () => {
					GetValue( e, message );
				} );

				if( message is ShowWindowMessage showWindowMessage ) {
					showWindowMessage.ViewModel = ( (ShowWindowMessage)message ).ViewModel;
				}
			}
		}

		private void GetValue( MessageSendEventArgs e, IMessage message ) {
			if( _source.TryGetTarget( out var _ ) ) {
				if( e.Message.MessageKey != null ) {
					if( _actionDictionary.TryGetValue( e.Message.MessageKey, out var list ) ) {
						foreach( var action in list ) {
							action( message );
						}
					}
				}

				if( _actionDictionary.TryGetValue( string.Empty, out var allList ) ) {
					foreach( var action in allList ) {
						action( message );
					}
				}
			}
		}

		private void DoActionOnDispatcher( Action action ) {
			if( Dispatcher.CheckAccess() ) {
				action();
			} else {
				Dispatcher.Invoke( action );
			}
		}

		IEnumerator<KeyValuePair<string, ConcurrentBag<Action<IMessage>>>> IEnumerable<KeyValuePair<string, ConcurrentBag<Action<IMessage>>>>.GetEnumerator() {
			ThrowExceptionIfDisposed();
			return _actionDictionary.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			ThrowExceptionIfDisposed();
			return _actionDictionary.GetEnumerator();
		}

		public Dispatcher Dispatcher { get; set; }

		public void Add( Action<IMessage> action ) {
			RegisterAction( action );
		}

		public void Add( string messageKey, Action<IMessage> action ) {
			RegisterAction( messageKey, action );
		}


		public void Add( string messageKey, params Action<IMessage>[] actions ) {
			foreach( var action in actions ) {
				RegisterAction( messageKey, action );
			}
		}

		private void ThrowExceptionIfDisposed() {
			if( _disposed ) {
				throw new ObjectDisposedException( "EventListener" );
			}
		}

		private bool _disposed;

		public void Dispose() {
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		private void Dispose( bool disposing ) {
			if( _disposed ) return;

			if( disposing ) {
				_listener.Dispose();
			}
			_disposed = true;
		}
	}
}
