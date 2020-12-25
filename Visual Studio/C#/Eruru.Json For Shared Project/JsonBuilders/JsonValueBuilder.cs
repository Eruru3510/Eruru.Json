using System;
using System.Collections.Generic;

namespace Eruru.Json {

	public class JsonValueBuilder : IJsonBuilder<JsonValue, JsonArray, JsonObject> {

		readonly IJsonReader Reader;
		readonly JsonConfig Config;
		readonly Stack<JsonArray> Stacks = new Stack<JsonArray> ();

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
			if (array is null) {
				Stacks.Push (new JsonArray ());
			} else {
				Stacks.Push (array);
			}
			int count = Stacks.Peek ().Count;
			Reader.ReadArray (i => {
				JsonValue value = null;
				if (i < count) {
					value = Stacks.Peek ().Get (i);
				}
				Stacks.Peek ().Add (BuildValue (value));
			});
			Stacks.Peek ().RemoveRange (0, count);
			return Stacks.Pop ();
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
			}, () => currentObject.Add (keyName, BuildValue (currentObject.Get (keyName))));//todo 待测试复用是否正常
			return currentObject;
		}

		#endregion

	}

}