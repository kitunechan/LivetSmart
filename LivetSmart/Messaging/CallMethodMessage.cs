using System;
using System.Diagnostics.CodeAnalysis;

namespace LivetSmart.Messaging {

	/// <summary>
	/// メソッドの名称にアクセスできるオブジェクトを表します。
	/// </summary>
	public interface IMethodMessage {
		Type? MethodInvokeTarget { get; }
		string? MethodName { get; }
	}

	public interface IParameterMethodMessage : IMethodMessage {
		object? MethodParameter { get; }
	}

	public interface IResultMethodMessage : IMethodMessage {
		object? Result { get; set; }
	}

	/// <summary>
	/// 引数の無いメソッドを呼び出すメッセージです。
	/// </summary>
	public sealed class CallActionMessage : Message, IMethodMessage {
		public CallActionMessage() { }

		public CallActionMessage( string? methodName ) : this( null, methodName, null ) {
		}

		public CallActionMessage( Type? methodInvokeTarget, string? methodName ) : this( methodInvokeTarget, methodName, null ) {
		}

		public CallActionMessage( Type? methodInvokeTarget, string? methodName, string? messageKey ) : base( messageKey ) {
			this.MethodInvokeTarget = methodInvokeTarget;
			this.MethodName = methodName;
		}


		public Type? MethodInvokeTarget { get; init; }

		public string? MethodName { get; init; }
	}

	/// <summary>
	/// 引数のあるメソッドを呼び出すメッセージです。
	/// </summary>
	/// <typeparam name="TParameter">引数の型</typeparam>
	public sealed class CallActionMessage<TParameter> : Message, IParameterMethodMessage {

		public CallActionMessage() : base() { }

		[SetsRequiredMembers]
		public CallActionMessage( string? methodName, TParameter methodParameter ) : this( null, methodName, methodParameter, null ) {
		}

		[SetsRequiredMembers]
		public CallActionMessage( Type? methodInvokeTarget, string? methodName, TParameter methodParameter, string? messageKey ) : base( messageKey ) {
			this.MethodInvokeTarget = methodInvokeTarget;
			this.MethodName = methodName;
			this.MethodParameter = methodParameter;
		}


		public Type? MethodInvokeTarget { get; init; }
		public string? MethodName { get; init; }
		public required TParameter MethodParameter { get; init; }

		object? IParameterMethodMessage.MethodParameter => MethodParameter;

	}


	/// <summary>
	/// 引数がなく、返り値のあるメソッドを呼び出すメッセージです。
	/// </summary>
	/// <typeparam name="TResult">返り値の型</typeparam>
	public sealed class CallFuncMessage<TResult> : ResponsiveMessage<TResult>, IResultMethodMessage, IMethodMessage {
		public CallFuncMessage() {
		}

		public CallFuncMessage( string? methodName ) : this( null, methodName, null ) {
		}

		public CallFuncMessage( Type? methodInvokeTarget, string? methodName, string? messageKey ) : base( messageKey ) {
			if( methodName is null ) { throw new ArgumentNullException(); }

			this.MethodInvokeTarget = methodInvokeTarget;
			this.MethodName = methodName;
		}

		public Type? MethodInvokeTarget { get; init; }

		public string? MethodName { get; init; }

		object? IResultMethodMessage.Result {
			get => Response;
			set {
				this.Response = (TResult?)value;
			}
		}
	}

	/// <summary>
	/// 引数と、返り値のあるメソッドを呼び出すメッセージです。
	/// </summary>
	/// <typeparam name="TParameter">引数の型</typeparam>
	/// <typeparam name="TResult">返り値の型</typeparam>
	public sealed class CallFuncMessage<TParameter, TResult> : ResponsiveMessage<TResult>, IResultMethodMessage, IParameterMethodMessage {
		public CallFuncMessage() {
		}

		[SetsRequiredMembers]
		public CallFuncMessage( string? methodName, TParameter methodParameter ) : this( null, methodName, methodParameter, null ) {
		}

		[SetsRequiredMembers]
		public CallFuncMessage( Type? methodInvokeTarget, string? methodName, TParameter methodParameter, string? messageKey ) : base( messageKey ) {
			this.MethodInvokeTarget = methodInvokeTarget;
			this.MethodName = methodName;
			this.MethodParameter = methodParameter;
		}

		public Type? MethodInvokeTarget { get; init; }

		public string? MethodName { get; init; }

		public required TParameter MethodParameter { get; init; }
		
		object? IResultMethodMessage.Result {
			get => Response;
			set {
				this.Response = (TResult?)value;
			}
		}

		object? IParameterMethodMessage.MethodParameter => MethodParameter;
	}


}