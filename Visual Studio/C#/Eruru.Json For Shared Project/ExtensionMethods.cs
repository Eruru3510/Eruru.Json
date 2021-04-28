using System;
using System.Reflection;

namespace Eruru.Json {

	static class ExtensionMethods {

		static readonly FieldInfo ExceptionMessageFieldInfo = typeof (Exception).GetField ("_message", JsonApi.BindingFlags);

		public static void SetMessage (this Exception exception, string message) {
			if (exception is null) {
				throw new ArgumentNullException (nameof (exception));
			}
			if (message is null) {
				throw new ArgumentNullException (nameof (message));
			}
			ExceptionMessageFieldInfo.SetValue (exception, message);
		}

		public static FieldInfo GetRuntimeField (this Type type, string name) {
			if (type is null) {
				throw new ArgumentNullException (nameof (type));
			}
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return type.GetField (name, JsonApi.BindingFlags);
		}

	}

}