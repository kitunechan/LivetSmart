using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Linq;
using System;

namespace LivetSmart.Messaging {
	/// <summary>
	/// メソッドの実行するアクションです。<see cref="CallActionMessage"/>、<see cref="CallActionMessage{TParameter}"/>、<see cref="CallFuncMessage{TResult}"/>、<see cref="CallFuncMessage{TParameter, TResult}"/>に対応します。
	/// </summary>
	public class CallMethodAction : TriggerAction<DependencyObject> {
		private static readonly MethodCache _method = new();

		#region Register MethodTarget
		/// <summary>
		/// メソッドを呼び出すオブジェクトを指定、または取得します。
		/// </summary>
		public object? MethodTarget {
			get { return GetValue( MethodTargetProperty ); }
			set { SetValue( MethodTargetProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for MethodInvokeTarget.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MethodTargetProperty =
			DependencyProperty.Register( "MethodInvokeTarget", typeof( object ), typeof( CallMethodAction ), new PropertyMetadata( default ) );
		#endregion

		#region Register MethodName
		/// <summary>
		/// 呼び出すメソッドの名前を指定、または取得します。
		/// </summary>
		public string? MethodName {
			get { return (string?)GetValue( MethodNameProperty ); }
			set { SetValue( MethodNameProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for MethodName.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MethodNameProperty =
			DependencyProperty.Register( "MethodName", typeof( string ), typeof( CallMethodAction ), new PropertyMetadata( default ) );
		#endregion

		#region Register MethodParameterType
		public Type? MethodParameterType {
			get => (Type?)GetValue( MethodParameterTypeProperty );
			set => SetValue( MethodParameterTypeProperty, value );
		}

		public static readonly DependencyProperty MethodParameterTypeProperty =
			DependencyProperty.Register( nameof( MethodParameterType ), typeof( Type ), typeof( CallMethodAction ), new PropertyMetadata( default ) );
		#endregion

		#region Register MethodParameter
		/// <summary>
		/// 呼び出すメソッドに渡す引数を指定、または取得します。
		/// </summary>
		public object? MethodParameter {
			get { return GetValue( MethodParameterProperty ); }
			set { SetValue( MethodParameterProperty, value ); }
		}

		// Using a DependencyProperty as the backing store for MethodParameter.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty MethodParameterProperty =
			DependencyProperty.Register( "MethodParameter", typeof( object ), typeof( CallMethodAction ), new PropertyMetadata( default ) );
		#endregion


		protected override void Invoke( object? parameter ) {
			if( parameter is IMessage message ) {
				Invoke( MethodTarget ?? AssociatedObject, MethodName, MethodParameterType, MethodParameter, message );
			}
		}

		static void Invoke( object methodTarget, string? methodName, Type? methodParameterType, object? methodParameter, IMessage message ) {
			if( message is IMethodMessage methodMessage ) {
				if( methodMessage?.MethodInvokeTarget is not null && methodMessage.MethodInvokeTarget != methodTarget.GetType() ) {
					System.Diagnostics.Debug.WriteLine( $"CallMethodAction({methodTarget.GetType().FullName}): {methodMessage.MethodInvokeTarget.FullName}" );
					return;
				}

				methodName = methodMessage?.MethodName ?? methodName;
			}

			if( methodName is null ) {
				throw new ArgumentNullException();
			}

			if( message is IParameterMethodMessage parameterMethodMessage ) {
				methodParameter = parameterMethodMessage.MethodParameter ?? methodParameter;
				methodParameterType = methodParameter?.GetType() ?? methodParameterType;

				var t = parameterMethodMessage.GetType();
				if( t.IsGenericType ) {
					var type = t.GetGenericTypeDefinition();
					if( type == typeof( CallActionMessage<> ) || type == typeof( CallFuncMessage<,> ) ) {
						methodParameterType = t.GenericTypeArguments.First();
					}
				}
			}
			methodParameterType ??= methodParameter?.GetType();

			// Invoke
			var result = _method.Invoke( methodTarget, methodName, methodParameterType, methodParameter );

			if( message is IResultMethodMessage resultMessage ) {
				resultMessage.Result = result;
			}

			message.IsHandled = true;
		}

		public static void Action( object methodTarget, IMessage message ) {
			Invoke( methodTarget, null, null, null, message );
		}
	}
}
