using System;

namespace LivetSmart.Messaging {
	/// <summary>
	/// ViewModelで使用するMessengerクラスです。
	/// </summary>
	public class Messenger {

		/// <summary>
		/// 指定された相互作用メッセージを同期的に送信します。
		/// </summary>
		/// <param name="message">相互作用メッセージ</param>
		public void Send( IMessage message ) {
			if( message is null ) {
				throw new ArgumentException( $"{nameof( message )}はnullにできません", nameof( message ) );
			}

			if( this.SendMessage != null ) {
				this.SendMessage?.Invoke( this, new MessageSendEventArgs( message ) );
			}

			if( this.SendLater != null ) {
				this.SendLater?.Invoke( this, new MessageSendEventArgs( message ) );
			}
		}

		/// <summary>
		/// 戻り値のあるメッセージを送信します。
		/// </summary>
		/// <typeparam name="TResult">戻り値の型</typeparam>
		/// <param name="message">メッセージ</param>
		/// <returns>呼び出したメッセージの戻り値</returns>
		public TResult? Send<TResult>( ResponsiveMessage<TResult> message ) {
			if( message is null ) {
				throw new ArgumentException( $"{nameof( message )}はnullにできません", nameof( message ) );
			}

			var hasEvent = false;
			if( this.SendMessage != null ) {
				this.SendMessage?.Invoke( this, new MessageSendEventArgs( message ) );
				hasEvent = true;
			}

			if( this.SendLater != null ) {
				this.SendLater?.Invoke( this, new MessageSendEventArgs( message ) );
				hasEvent = true;
			}

			return hasEvent ? message.Response : default;
		}

		/// <summary>
		/// 相互作用メッセージが送信された時に発生するイベントです。
		/// </summary>
		public event EventHandler<MessageSendEventArgs>? SendMessage;


		public event EventHandler<MessageSendEventArgs>? SendLater;


	}


	/// <summary>
	/// 相互作用メッセージ送信時イベント用のイベント引数です。
	/// </summary>
	public class MessageSendEventArgs : EventArgs {
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="message">Message</param>
		public MessageSendEventArgs( IMessage message ) {
			if( message is null ) {
				throw new ArgumentException( $"{nameof( message )}はnullにできません", nameof( message ) );
			}

			this.Message = message;
		}

		/// <summary>
		/// 送信されたメッセージ
		/// </summary>
		public IMessage Message { get; }
	}
}