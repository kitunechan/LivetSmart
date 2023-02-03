using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Runtime.InteropServices;
using System.Linq.Expressions;

namespace LivetSmart.Messaging {
	/// <summary>
	/// ビヘイビア・トリガー・アクションでのメソッド直接バインディングを可能にするためのクラスです。<br/>
	/// 引数の無いメソッドを実行します。メソッドの実行はキャッシュされます。
	/// </summary>
	public class MethodCache {
		private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, ConcurrentDictionary<Type, Action<object, object?[]>>>> _ActionCacheDictionary = new();
		private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, ConcurrentDictionary<Type, Func<object, object?[], object?>>>> _FuncCacheDictionary = new();

		private static readonly List<Task> taskList = new();

		public static IEnumerable<Task> Tasks => taskList.ToArray();

		private static bool TryGetActionCache( Type targetType, string methodName, Type? argumentType, [MaybeNullWhen( false )] out Action<object, object?[]> action ) {
			if( _ActionCacheDictionary.TryGetValue( targetType, out var _cache1 ) ) {
				if( _cache1.TryGetValue( methodName, out var _cache2 ) ) {
					if( _cache2.TryGetValue( argumentType ?? typeof( void ), out action ) ) {
						return true;
					}
				}
			}
			action = null;
			return false;
		}

		private static bool TryGetFuncCache( Type targetType, string methodName, Type? argumentType, [MaybeNullWhen( false )] out Func<object, object?[], object?> func ) {
			if( _FuncCacheDictionary.TryGetValue( targetType, out var _cache1 ) ) {
				if( _cache1.TryGetValue( methodName, out var _cache2 ) ) {
					if( _cache2.TryGetValue( argumentType ?? typeof( void ), out func ) ) {
						return true;
					}
				}
			}
			func = null;
			return false;
		}

		private readonly object syncObject = new();
		private Type? _targetTypeCache;
		private string? _methodNameCache;
		private Type? _argumentTypeCache;
		private Action<object, object?[]>? _actionCache;
		private Func<object, object?[], object?>? _functionCache;

		public object? Invoke( object target, string methodName, Type? argumentType, object? argumentValue ) {
			var targetType = target.GetType();
			Type[] types = argumentType is null ? new Type[0] : new[] { argumentType };
			object?[] args = types.Any() ? new object?[] { argumentValue } : new object[0];

			object? result = default;
			MethodInfo methodInfo;

			bool taken = false;
			try {
				Monitor.Enter( syncObject, ref taken );

				if( targetType == _targetTypeCache && methodName == _methodNameCache && argumentType == _argumentTypeCache ) {
					if( _actionCache is not null ) {
						var cache = _actionCache;
						if( taken ) {
							taken = false;
							Monitor.Exit( syncObject );
						}

						cache( target, args );
						return null;
					}
					if( _functionCache is not null ) {
						var cache = _functionCache;
						if( taken ) {
							taken = false;
							Monitor.Exit( syncObject );
						}

						return cache( target, args );
					}

					throw new Exception( "Cache Error" );

				} else {
					_actionCache = null;
					_functionCache = null;

					if( TryGetActionCache( targetType, methodName, argumentType, out _actionCache ) ) {
						_targetTypeCache = targetType;
						_methodNameCache = methodName;
						_argumentTypeCache = argumentType;

						var cache = _actionCache;
						if( taken ) {
							taken = false;
							Monitor.Exit( syncObject );
						}
						cache( target, args );
						return null;
					}
					if( TryGetFuncCache( targetType, methodName, argumentType, out _functionCache ) ) {
						_targetTypeCache = targetType;
						_methodNameCache = methodName;
						_argumentTypeCache = argumentType;

						var cache = _functionCache;
						if( taken ) {
							taken = false;
							Monitor.Exit( syncObject );
						}
						return cache( target, args );
					}
				}

				methodInfo = targetType.GetMethod( methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic, null, types, null )
								?? throw new ArgumentException();

			} finally {
				if( taken ) Monitor.Exit( syncObject );
			}

			result = methodInfo.Invoke( target, args );

			// メソッドのキャッシュ処理
			if( methodInfo.ReturnType == typeof( void ) ) {
				var t = Task.Run( () => {
					lock( syncObject ) {
						_targetTypeCache = targetType;
						_methodNameCache = methodName;
						_argumentTypeCache = argumentType;

						var pTarget = Expression.Parameter( typeof( object ), "target" );
						var pArgs = Expression.Parameter( typeof( object[] ), "args" );
						var parameters = methodInfo.GetParameters()
											.Select( ( x, index ) => Expression.Convert( Expression.ArrayIndex( pArgs, Expression.Constant( index ) ), x.ParameterType ) )
											.ToArray();

						var method = Expression.Lambda<Action<object, object?[]>>(
										Expression.Call( Expression.Convert( pTarget, targetType ), methodInfo, parameters ),
										pTarget, pArgs
									).Compile();

						_actionCache = method;

						_ActionCacheDictionary
							.GetOrAdd( targetType, _ => new() )
							.GetOrAdd( methodInfo.Name, _ => new() )
							.TryAdd( argumentType ?? typeof( void ), method );
					}
				} );

				taskList.Add( t );
				t.ContinueWith( _ => {
					taskList.Remove( _ );
				} );

			} else {
				var t = Task.Run( () => {
					lock( syncObject ) {
						_targetTypeCache = targetType;
						_methodNameCache = methodName;
						_argumentTypeCache = argumentType;

						var pTarget = Expression.Parameter( typeof( object ), "target" );
						var pArgs = Expression.Parameter( typeof( object[] ), "args" );
						var parameters = methodInfo.GetParameters()
											.Select( ( x, index ) => Expression.Convert( Expression.ArrayIndex( pArgs, Expression.Constant( index ) ), x.ParameterType ) )
											.ToArray();

						var method = Expression.Lambda<Func<object, object?[], object?>>(
										Expression.Convert(
											Expression.Call( Expression.Convert( pTarget, targetType ), methodInfo, parameters ),
											typeof( object )
										),
										pTarget, pArgs
									).Compile();

						_functionCache = method;

						_FuncCacheDictionary
							.GetOrAdd( targetType, _ => new() )
							.GetOrAdd( methodInfo.Name, _ => new() )
							.GetOrAdd( argumentType ?? typeof( void ), method );
					}
				} );

				taskList.Add( t );
				t.ContinueWith( _ => {
					taskList.Remove( _ );
				} );
			}

			return result;
		}
	}
}
