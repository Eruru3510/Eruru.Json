using System;

namespace Eruru.Json {

	public class JsonValueBuilder : IJsonBuilder<JsonValue, JsonArray, JsonObject> {

		readonly IJsonReader Reader;

		public JsonValueBuilder (IJsonReader reader) {
			Reader = reader ?? throw new ArgumentNullException (nameof (reader));
		}

		#region IJsonBuilder<JsonValue, JsonArray, JsonObject>

		public JsonValue BuildValue () {
			JsonValue jsonValue = null;
			Reader.ReadValue (
				(value, valueType) => jsonValue = new JsonValue (value, valueType),
				() => jsonValue = BuildArray (),
				() => jsonValue = BuildObject ()
			);
			return jsonValue;
		}

		public JsonArray BuildArray () {
			JsonArray array = new JsonArray ();
			Reader.ReadArray (() => array.Add (BuildValue ()));
			return array;
		}

		public JsonObject BuildObject () {
			JsonObject jsonObject = new JsonObject ();
			string keyName = null;
			Reader.ReadObject (name => {
				keyName = name;
				return true;
			}, () => jsonObject.Add (keyName, BuildValue ()));
			return jsonObject;
		}

		#endregion

	}

}