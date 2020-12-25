using System;
using System.Reflection;

namespace Eruru.Json {

	class JsonConverter {

		public readonly Type BeforeType;

		readonly object Instance;
		readonly Type Type;
		readonly Type AfterType;
		readonly MethodInfo ReadMethod;
		readonly MethodInfo WriteMethod;
		readonly object[] ReadParameters = new object[1];
		readonly object[] WriteParameters = new object[1];

		public JsonConverter (Type type) {
			Type = type ?? throw new ArgumentNullException (nameof (type));
			Type interfaceType = Type.GetInterface (typeof (IJsonConverter<object, object>).Name);
			if (interfaceType is null) {
				throw new JsonException ($"{type}需要实现{typeof (IJsonConverter<object, object>)}接口");
			}
			Type[] types = interfaceType.GetGenericArguments ();
			BeforeType = types[0];
			AfterType = types[1];
			ReadMethod = interfaceType.GetMethod (nameof (IJsonConverter<object, object>.Read), new Type[] { BeforeType });
			WriteMethod = interfaceType.GetMethod (nameof (IJsonConverter<object, object>.Write), new Type[] { AfterType });
			Instance = JsonApi.CreateInstance (Type);
		}

		public object Read (object value, JsonConfig config) {
			if (config is null) {
				throw new ArgumentNullException (nameof (config));
			}
			ReadParameters[0] = JsonApi.ChangeType (value, BeforeType, config);
			return JsonApi.ChangeType (ReadMethod.Invoke (Instance, ReadParameters), ReadMethod.ReturnType, config);
		}

		public object Write (object value, JsonConfig config) {
			if (config is null) {
				throw new ArgumentNullException (nameof (config));
			}
			WriteParameters[0] = JsonApi.ChangeType (value, AfterType, config);
			return JsonApi.ChangeType (WriteMethod.Invoke (Instance, WriteParameters), WriteMethod.ReturnType, config);
		}

		public override int GetHashCode () {
			return Type.GetHashCode ();
		}

	}

}