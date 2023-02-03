using LivetSmart.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace LivetSmart.Tests {
	[TestClass()]
	public class MessageTests {

		List<string> AssertList = new List<string>();
		Messenger messenger = new Messenger();

		public MessageTests() {
			var triggers = Interaction.GetTriggers( new WindowEx( AssertList ) );
			triggers.Add( new MessageTrigger( messenger ) {
				Actions = {
					new CallMethodAction() { MethodName = "Test" },
					new CallMethodAction() { MethodName = "Test", MethodParameter = "string" },
					new CallMethodAction() { MethodName = "Test", MethodParameter = 999 },
					new CallMethodAction() { MethodName = "Test", MethodParameter = "obj" },

					new CallMethodAction() { MethodName = "TestResult" },
					new CallMethodAction() { MethodName = "TestResult", MethodParameter = "string" },
					new CallMethodAction() { MethodName = "TestResult", MethodParameter = 999 },
					new CallMethodAction() { MethodName = "TestResult", MethodParameter = "obj" },

					new CallMethodAction() { MethodName = "TestResultInt" },
					new CallMethodAction() { MethodName = "TestResultInt", MethodParameter = "string" },
					new CallMethodAction() { MethodName = "TestResultInt", MethodParameter = 999 },
					new CallMethodAction() { MethodName = "TestResultInt", MethodParameter = "obj" },

					new CallMethodAction() { MethodName = "TestResultObject" },
					new CallMethodAction() { MethodName = "TestResultObject", MethodParameter = "string" },
					new CallMethodAction() { MethodName = "TestResultObject", MethodParameter = 999 },
					new CallMethodAction() { MethodName = "TestResultObject", MethodParameter = "obj" },
				}
			} );
		}


		[TestCleanup()]
		public void TestCleanup() {
			AssertList.Clear();
		}

		[TestMethod()]
		public void MessageTest() {
			messenger.Send( new Message() );

			var TrueList = new List<string>(){
				 "Test()" ,
				 "Test( string )" ,
				 "Test( int )" ,
				 "Test( string )" ,
				 "TestResult()" ,
				 "TestResult( string ): string" ,
				 "TestResult( int ): 999" ,
				 "TestResult( string ): obj" ,
				 "TestResultInt()" ,
				 "TestResultInt( string ): string" ,
				 "TestResultInt( int ): 999" ,
				 "TestResultInt( string ): obj" ,
				 "TestResultObject()" ,
				 "TestResultObject( string ): string" ,
				 "TestResultObject( int ): 999" ,
				 "TestResultObject( string ): obj" ,
			};

			Assert.IsTrue( AssertList.SequenceEqual( TrueList ) );

		}

		[TestMethod()]
		public void MethodCacheTest() {
			var messengerAction = new Messenger();
			var messengerFunc = new Messenger();

			var triggers = Interaction.GetTriggers( new WindowEx( AssertList ) );
			triggers.Add( new MessageTrigger( messengerAction ) {
				Actions = {
					new CallMethodAction() { MethodName = "Test" },
				}
			} );
			triggers.Add( new MessageTrigger( messengerFunc ) {
				Actions = {
					new CallMethodAction() { MethodName = "Test", MethodParameterType = typeof(string) },
				}
			} );

			messengerAction.Send( new Message() );
			Task.WaitAll( MethodCache.Tasks.ToArray() );
			messengerAction.Send( new Message() );

			messengerFunc.Send( new Message() );
			Task.WaitAll( MethodCache.Tasks.ToArray() );
			messengerFunc.Send( new Message() );

			Task.WaitAll( MethodCache.Tasks.ToArray() );

			messengerAction.Send( new Message() );
			messengerFunc.Send( new Message() );

			messengerAction.Send( new Message() );
			messengerFunc.Send( new Message() );

			var TrueList = new List<string>(){
				 "Test()" ,
				 "Test()" ,
				 "Test( string )" ,
				 "Test( string )" ,

				 "Test()" ,
				 "Test( string )" ,
				 "Test()" ,
				 "Test( string )" ,
			};

			Assert.IsTrue( AssertList.SequenceEqual( TrueList ) );

		}


		[TestMethod()]
		public void CallMethodMessageTest() {
			messenger.Send( new CallActionMessage() );

			var TrueList = new List<string>(){
				 "Test()" ,
				 "Test( string )" ,
				 "Test( int )" ,
				 "Test( string )" ,
				 "TestResult()" ,
				 "TestResult( string ): string" ,
				 "TestResult( int ): 999" ,
				 "TestResult( string ): obj" ,
				 "TestResultInt()" ,
				 "TestResultInt( string ): string" ,
				 "TestResultInt( int ): 999" ,
				 "TestResultInt( string ): obj" ,
				 "TestResultObject()" ,
				 "TestResultObject( string ): string" ,
				 "TestResultObject( int ): 999" ,
				 "TestResultObject( string ): obj" ,
			};

			Assert.IsTrue( AssertList.SequenceEqual( TrueList ) );
		}


		[TestMethod()]
		public void CallMethodMessageForMultiMessageActionTest() {
			var window = new WindowEx( AssertList ) { Width = 50, Height = 50 };

			var messenger = new Messenger();

			var triggers = Interaction.GetTriggers( window );
			triggers.Add( new MessageTrigger( messenger ) {
				Actions = {
					new MultiMessageAction(),
				}
			} );

			window.Show();


			Assert.ThrowsException<ArgumentNullException>( (Action)( () => {
				messenger.Send( new CallActionMessage() );
			} ) );

			var result = new DebugList<object>();

			//-----------------
			{
				messenger.Send( new CallActionMessage( "Test" ) );
				messenger.Send( new CallActionMessage( "TestResult" ) );
				messenger.Send( new CallActionMessage( "TestResultInt" ) );
				messenger.Send( new CallActionMessage( "TestResultObject" ) );

				var trueList = new[]{
					"Test()",
					"TestResult()",
					"TestResultInt()",
					"TestResultObject()",
				};

				Assert.IsTrue( AssertList.SequenceEqual( trueList ) );
				AssertList.Clear();
			}
			{
				messenger.Send( new CallActionMessage<string>( "Test", "str" ) );
				messenger.Send( new CallActionMessage<int>( "Test", 100 ) );
				messenger.Send( new CallActionMessage<object>( "Test", "obj" ) );

				messenger.Send( new CallActionMessage<object>( "Test", null ) );
				messenger.Send( new CallActionMessage<string>( "Test", null ) );
				messenger.Send( new CallActionMessage<object>( "Test", 200 ) );

				var trueList = new[]{
					"Test( string )",
					"Test( int )",
					"Test( object )",

					"Test( object )",
					"Test( string )",
					"Test( object )",
				};

				Assert.IsTrue( AssertList.SequenceEqual( trueList ) );
				AssertList.Clear();
			}
			{
				messenger.Send( new CallActionMessage<string>( "TestResult", "str" ) );
				messenger.Send( new CallActionMessage<object>( "TestResult", "obj" ) );
				messenger.Send( new CallActionMessage<int>( "TestResult", 100 ) );

				messenger.Send( new CallActionMessage<string>( "TestResult", null ) );
				messenger.Send( new CallActionMessage<object>( "TestResult", null ) );
				messenger.Send( new CallActionMessage<object>( "TestResult", 200 ) );

				var trueList = new[]{
					"TestResult( string ): str",
					"TestResult( object ): obj",
					"TestResult( int ): 100",

					"TestResult( string ): ",
					"TestResult( object ): ",
					"TestResult( object ): 200",
				};

				Assert.IsTrue( AssertList.SequenceEqual( trueList ) );
				AssertList.Clear();
			}
			{
				messenger.Send( new CallActionMessage<string>( "TestResultInt", "str" ) );
				messenger.Send( new CallActionMessage<object>( "TestResultInt", "obj" ) );
				messenger.Send( new CallActionMessage<int>( "TestResultInt", 100 ) );

				messenger.Send( new CallActionMessage<string>( "TestResultInt", null ) );
				messenger.Send( new CallActionMessage<object>( "TestResultInt", null ) );
				messenger.Send( new CallActionMessage<object>( "TestResultInt", 200 ) );

				var trueList = new[]{
					"TestResultInt( string ): str",
					"TestResultInt( object ): obj",
					"TestResultInt( int ): 100",

					"TestResultInt( string ): ",
					"TestResultInt( object ): ",
					"TestResultInt( object ): 200",
				};

				Assert.IsTrue( AssertList.SequenceEqual( trueList ) );
				AssertList.Clear();
			}
			{
				messenger.Send( new CallActionMessage<string>( "TestResultObject", "str" ) );
				messenger.Send( new CallActionMessage<object>( "TestResultObject", "obj" ) );
				messenger.Send( new CallActionMessage<int>( "TestResultObject", 100 ) );

				messenger.Send( new CallActionMessage<string>( "TestResultObject", null ) );
				messenger.Send( new CallActionMessage<object>( "TestResultObject", null ) );
				messenger.Send( new CallActionMessage<object>( "TestResultObject", 200 ) );

				var trueList = new[]{
					"TestResultObject( string ): str",
					"TestResultObject( object ): obj",
					"TestResultObject( int ): 100",

					"TestResultObject( string ): ",
					"TestResultObject( object ): ",
					"TestResultObject( object ): 200",
				};

				Assert.IsTrue( AssertList.SequenceEqual( trueList ) );
				AssertList.Clear();
			}

			//---------------

			{
				result.Push = messenger.Send( new CallFuncMessage<object>( "Test" ) );
				result.Push = messenger.Send( new CallFuncMessage<object>( "TestResult" ) );
				result.Push = messenger.Send( new CallFuncMessage<object>( "TestResultInt" ) );
				result.Push = messenger.Send( new CallFuncMessage<object>( "TestResultObject" ) );

				var trueList = new[]{
					"Test()",
					"TestResult()",
					"TestResultInt()",
					"TestResultObject()",
				};
				Assert.IsTrue( AssertList.SequenceEqual( trueList ) );
				AssertList.Clear();

				var resultTrueList = new object[] {
					null,
					"TestResult",
					1,
					"TestResultObject",
				};
				Assert.IsTrue( result.SequenceEqual( resultTrueList ) );
				result.Clear();
			}
			{
				result.Push = messenger.Send( new CallFuncMessage<string, object>( "Test", "str" ) );
				result.Push = messenger.Send( new CallFuncMessage<int, object>( "Test", 100 ) );
				result.Push = messenger.Send( new CallFuncMessage<object, object>( "Test", "obj" ) );

				result.Push = messenger.Send( new CallFuncMessage<object, object>( "Test", null ) );
				result.Push = messenger.Send( new CallFuncMessage<string, object>( "Test", null ) );
				result.Push = messenger.Send( new CallFuncMessage<object, object>( "Test", 200 ) );

				var trueList = new[]{
					"Test( string )",
					"Test( int )",
					"Test( object )",

					"Test( object )",
					"Test( string )",
					"Test( object )",
				};
				Assert.IsTrue( AssertList.SequenceEqual( trueList ) );
				AssertList.Clear();

				var resultTrueList = new object[] {
					null,
					null,
					null,

					null,
					null,
					null,
				};
				Assert.IsTrue( result.SequenceEqual( resultTrueList ) );
				result.Clear();
			}
			{
				result.Push = messenger.Send( new CallFuncMessage<string, string>( "TestResult", "str" ) );
				result.Push = messenger.Send( new CallFuncMessage<object, string>( "TestResult", "obj" ) );
				result.Push = messenger.Send( new CallFuncMessage<int, string>( "TestResult", 100 ) );

				result.Push = messenger.Send( new CallFuncMessage<string, string>( "TestResult", null ) );
				result.Push = messenger.Send( new CallFuncMessage<object, string>( "TestResult", null ) );
				result.Push = messenger.Send( new CallFuncMessage<object, string>( "TestResult", 200 ) );

				var trueList = new[]{
					"TestResult( string ): str",
					"TestResult( object ): obj",
					"TestResult( int ): 100",

					"TestResult( string ): ",
					"TestResult( object ): ",
					"TestResult( object ): 200",
				};
				Assert.IsTrue( AssertList.SequenceEqual( trueList ) );
				AssertList.Clear();

				var resultTrueList = new object[] {
					"TestResult: str",
					"TestResult: obj",
					"TestResult: 100",

					"TestResult: ",
					"TestResult: ",
					"TestResult: 200",
				};
				Assert.IsTrue( result.SequenceEqual( resultTrueList ) );
				result.Clear();
			}
			{
				result.Push = messenger.Send( new CallFuncMessage<string, int>( "TestResultInt", "str" ) );
				result.Push = messenger.Send( new CallFuncMessage<object, int>( "TestResultInt", "obj" ) );
				result.Push = messenger.Send( new CallFuncMessage<int, int>( "TestResultInt", 100 ) );

				result.Push = messenger.Send( new CallFuncMessage<string, int>( "TestResultInt", null ) );
				result.Push = messenger.Send( new CallFuncMessage<object, int>( "TestResultInt", null ) );
				result.Push = messenger.Send( new CallFuncMessage<object, int>( "TestResultInt", 200 ) );

				var trueList = new[]{
					"TestResultInt( string ): str",
					"TestResultInt( object ): obj",
					"TestResultInt( int ): 100",

					"TestResultInt( string ): ",
					"TestResultInt( object ): ",
					"TestResultInt( object ): 200",
				};
				Assert.IsTrue( AssertList.SequenceEqual( trueList ) );
				AssertList.Clear();

				var resultTrueList = new object[] {
					2,
					4,
					100,

					2,
					4,
					4,
				};
				Assert.IsTrue( result.SequenceEqual( resultTrueList ) );
				result.Clear();
			}
			{
				result.Push = messenger.Send( new CallFuncMessage<string, object>( "TestResultObject", "str" ) );
				result.Push = messenger.Send( new CallFuncMessage<object, object>( "TestResultObject", "obj" ) );
				result.Push = messenger.Send( new CallFuncMessage<int, object>( "TestResultObject", 100 ) );

				result.Push = messenger.Send( new CallFuncMessage<string, object>( "TestResultObject", null ) );
				result.Push = messenger.Send( new CallFuncMessage<object, object>( "TestResultObject", null ) );
				result.Push = messenger.Send( new CallFuncMessage<object, object>( "TestResultObject", 200 ) );

				var trueList = new[]{
					"TestResultObject( string ): str",
					"TestResultObject( object ): obj",
					"TestResultObject( int ): 100",

					"TestResultObject( string ): ",
					"TestResultObject( object ): ",
					"TestResultObject( object ): 200",
				};
				Assert.IsTrue( AssertList.SequenceEqual( trueList ) );
				AssertList.Clear();

				var resultTrueList = new object[] {
					"TestResultObject: str",
					"TestResultObject: obj",
					100,

					"TestResultObject: ",
					"TestResultObject: ",
					"TestResultObject: 200",
				};
				Assert.IsTrue( result.SequenceEqual( resultTrueList ) );
				result.Clear();
			}
		}


	}

	class DebugList<T> : List<T> {

		public T Push {
			set {
				this.Add( value );
			}
		}

	}

	class WindowEx : Window {
		public WindowEx( List<string> assertList ) {
			this.AssertList = assertList;
		}

		readonly List<string> AssertList;

		#region CallMethod

		void Test() {
			AssertList.Add( "Test()" );
		}

		void Test( object value ) {
			AssertList.Add( "Test( object )" );
		}

		void Test( string value ) {
			AssertList.Add( "Test( string )" );
		}

		void Test( int value ) {
			AssertList.Add( "Test( int )" );
		}



		string TestResult() {
			AssertList.Add( "TestResult()" );
			return "TestResult";
		}

		string TestResult( string value ) {
			AssertList.Add( "TestResult( string ): " + value );
			return "TestResult: " + value;
		}
		string TestResult( int value ) {
			AssertList.Add( "TestResult( int ): " + value );
			return "TestResult: " + value;
		}
		string TestResult( object value ) {
			AssertList.Add( "TestResult( object ): " + value );
			return "TestResult: " + value;
		}


		int TestResultInt() {
			AssertList.Add( "TestResultInt()" );
			return 1;
		}

		int TestResultInt( string value ) {
			AssertList.Add( "TestResultInt( string ): " + value );
			return 2;
		}
		int TestResultInt( int value ) {
			AssertList.Add( "TestResultInt( int ): " + value );
			return value;
		}
		int TestResultInt( object value ) {
			AssertList.Add( "TestResultInt( object ): " + value );
			return 4;
		}


		object TestResultObject() {
			AssertList.Add( "TestResultObject()" );
			return "TestResultObject";
		}

		object TestResultObject( string value ) {
			AssertList.Add( "TestResultObject( string ): " + value );
			return "TestResultObject: " + value;
		}
		object TestResultObject( int value ) {
			AssertList.Add( "TestResultObject( int ): " + value );
			return value;
		}
		object TestResultObject( object value ) {
			AssertList.Add( "TestResultObject( object ): " + value );
			return "TestResultObject: " + value;
		}

		#endregion


	}
}