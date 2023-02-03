using System.ComponentModel;
using System.Collections.Concurrent;

namespace LivetSmart {
	/// <summary>
	///PropertyChangedEventArgs のキャッシュを行います。
	/// </summary>
	internal static class EventArgsFactory {
		private static readonly ConcurrentDictionary<string, PropertyChangedEventArgs> _propertyChangedEventArgsDictionary = new();

		public static PropertyChangedEventArgs GetPropertyChangedEventArgs( string propertyName ) {
			return _propertyChangedEventArgsDictionary.GetOrAdd( propertyName, name => new PropertyChangedEventArgs( name ) );
		}
	}
}
