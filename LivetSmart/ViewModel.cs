using System;
using LivetSmart.Messaging;
using System.Xml.Serialization;

namespace LivetSmart {
	/// <summary>
	/// ViewModelの基底クラスです。
	/// </summary>
	[Serializable]
	public abstract class ViewModel : NotificationObject, IDisposable {
		/// <summary>
		/// このViewModelクラスの基本DisposableCollectionです。
		/// </summary>
		[XmlIgnore]
		public DisposableCollection DisposableCollection => _disposableCollection ??= new DisposableCollection();
		[NonSerialized]
		private DisposableCollection? _disposableCollection;

		/// <summary>
		/// このViewModelクラスの基本Messengerインスタンスです。
		/// </summary>
		[XmlIgnore]
		public Messenger Messenger => _messenger ??= new Messenger();
		[NonSerialized]
		private Messenger? _messenger;

		/// <summary>
		/// このインスタンスによって使用されているすべてのリソースを解放します。
		/// </summary>
		public void Dispose() {
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		[NonSerialized]
		private bool _disposed;
		protected virtual void Dispose( bool disposing ) {
			if( _disposed ) return;
			if( disposing ) {
				_disposableCollection?.Dispose();
			}
			_disposed = true;
		}
	}
}