using System;

namespace Eruru.Json {

	public class JsonKey : JsonValue {

		public string Name { get; internal set; }

		public JsonKey (string name) {
			Name = name ?? throw new ArgumentNullException (nameof (name));
		}
		public JsonKey (string name, object value) : base (value) {
			Name = name ?? throw new ArgumentNullException (nameof (name));
		}

	}

}