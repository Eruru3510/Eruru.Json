using System;

namespace Eruru.Json {

	public class JsonValueBuilder : IJsonBuilder<JsonValue, JsonArray, JsonObject> {

		readonly IJsonReader Reader;
		readonly JsonConfig Config;

		public JsonValueBuilder (IJsonReader reader, JsonConfig config = null) {
			Reader = reader ?? throw new ArgumentNullException (nameof (reader));
			Config = config;
		}

		#region IJsonBuilder<JsonValue, JsonArray, JsonObject>

		public JsonValue BuildValue (JsonValue value = null) {
			JsonValue currentValue = null;
			Reader.ReadValue (
				(instanceValue, valueType) => {
					if (value is null) {
						currentValue = new JsonValue (instanceValue, valueType);
						return;
					}
					value._Type = valueType;
					value._Value = instanceValue;
					currentValue = value;
				},
				() => currentValue = BuildArray (value),
				() => currentValue = BuildObject (value)
			);
			return currentValue;
		}

		public JsonArray BuildArray (JsonArray array = null) {
			JsonArray currentArray = array;
			if (currentArray is null) {
				currentArray = new JsonArray ();
			}
			int count = currentArray.Count;
			Reader.ReadArray (i => {
				JsonValue value = null;
				if (i < count) {
					value = currentArray.Get (i);
				}
				currentArray.Add (BuildValue (value));
			});
			currentArray.RemoveRange (0, count);
			return currentArray;
		}

		public JsonObject BuildObject (JsonObject jsonObject = null) {
			JsonObject currentObject = jsonObject;
			if (currentObject is null) {
				currentObject = new JsonObject ();
			}
			string keyName = null;
			Reader.ReadObject (name => {
				keyName = name;
				return true;
			}, () => currentObject.Add (keyName, BuildValue ()));
			return currentObject;
		}

		#endregion

	}

}