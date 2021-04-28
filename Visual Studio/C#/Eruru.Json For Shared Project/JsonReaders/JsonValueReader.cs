using System;
using System.Collections.Generic;

namespace Eruru.Json {

	public class JsonValueReader : IJsonReader {

		readonly JsonConfig Config;
		readonly Stack<JsonValue> Values = new Stack<JsonValue> ();

		public JsonValueReader (JsonValue value, JsonConfig config = null) {
			if (value is null) {
				throw new ArgumentNullException (nameof (value));
			}
			Config = config ?? JsonConfig.Default;
			Values.Push (value);
		}

		#region IJsonReader

		public void ReadValue (JsonAction<object, JsonValueType> value, JsonAction readArray, JsonAction readObject) {
			if (value is null) {
				throw new ArgumentNullException (nameof (value));
			}
			if (readArray is null) {
				throw new ArgumentNullException (nameof (readArray));
			}
			if (readObject is null) {
				throw new ArgumentNullException (nameof (readObject));
			}
			switch (Values.Peek ().Type) {
				case JsonValueType.Array:
					readArray ();
					return;
				case JsonValueType.Object:
					readObject ();
					return;
			}
			if (JsonApi.HasFlag (Values.Peek ().Type, JsonValueType.Value)) {
				value (Values.Peek ().Value, Values.Peek ().Type);
				return;
			}
			throw new JsonNotSupportException (Values.Peek ().Type);
		}

		public void ReadArray (Action<int> readValue) {
			if (readValue is null) {
				throw new ArgumentNullException (nameof (readValue));
			}
			JsonArray array = Values.Peek ();
			for (int i = 0; i < array.Count; i++) {
				Values.Push (array[i]);
				readValue (i);
				Values.Pop ();
			}
		}

		public void ReadObject (JsonFunc<string, bool> key, JsonAction readValue) {
			if (key is null) {
				throw new ArgumentNullException (nameof (key));
			}
			if (readValue is null) {
				throw new ArgumentNullException (nameof (readValue));
			}
			foreach (JsonKey jsonKey in Values.Peek ()) {
				if (key (jsonKey.Name)) {
					Values.Push (jsonKey);
					readValue ();
					Values.Pop ();
				}
			}
		}

		#endregion

	}

}