using System;
using System.Dynamic;
using System.Reflection;

namespace Eruru.Json {

	public class JsonDynamicValue : DynamicObject {

		public JsonValue Value { get; set; }

		public JsonDynamicValue () {
			Value = new JsonValue ();
		}
		public JsonDynamicValue (JsonValue value) {
			Value = value ?? throw new ArgumentNullException (nameof (value));
		}

		public override bool TrySetMember (SetMemberBinder binder, object value) {
			Value[binder.Name] = value as JsonValue ?? new JsonValue (value);
			return true;
		}

		public override bool TryGetMember (GetMemberBinder binder, out object result) {
			result = (JsonDynamicValue)Value[binder.Name];
			return true;
		}

		public override bool TryGetIndex (GetIndexBinder binder, object[] indexes, out object result) {
			result = (JsonDynamicValue)Value[Convert.ToInt32 (indexes[0])];
			return true;
		}

		public override bool TrySetIndex (SetIndexBinder binder, object[] indexes, object value) {
			Value[Convert.ToInt32 (indexes[0])] = value as JsonValue ?? new JsonValue (value);
			return true;
		}

		public override bool TryInvokeMember (InvokeMemberBinder binder, object[] args, out object result) {
			Type type = Value.GetType ();
			Type[] types = Array.ConvertAll (args, arg => {
				return arg.GetType ();
			});
			MethodInfo methodInfo = type.GetMethod (binder.Name, types);
			result = methodInfo?.Invoke (Value, args);
			return true;
		}

		public override string ToString () {
			return Value.ToString ();
		}

		public static implicit operator JsonDynamicValue (JsonValue value) {
			if (value is null) {
				throw new ArgumentNullException (nameof (value));
			}
			return new JsonDynamicValue (value);
		}

	}

}