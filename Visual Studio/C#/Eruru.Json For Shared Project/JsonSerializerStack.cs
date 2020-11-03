using System;

namespace Eruru.Json {

	class JsonSerializerStack {

		public bool IsInitialized;
		public object Instance;
		public JsonField Field;
		public Type Type;
		public JsonArrayType ArrayType = JsonArrayType.Unknown;
		public JsonObjectType ObjectType = JsonObjectType.Unknown;

		public JsonSerializerStack (object instance) {
			Instance = instance;
		}
		public JsonSerializerStack (object instance, JsonField field) {
			Instance = instance;
			Field = field;
		}

	}

}