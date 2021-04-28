using System;
using System.Collections.Generic;

namespace Eruru.Json {

	public class JsonValueBuilder : IJsonBuilder<JsonValue, JsonArray, JsonObject> {

		readonly IJsonReader Reader;
		readonly JsonConfig Config;
		readonly Stack<JsonArray> Stacks = new Stack<JsonArray> ();

		public JsonValueBuilder (IJsonReader reader, JsonConfig config = null) {
			Reader = reader ?? throw new ArgumentNullException (nameof (reader));
			Config = config ?? JsonConfig.Default;
		}

		#region IJsonBuilder<JsonValue, JsonArray, JsonObject>

		public JsonValue BuildValue (JsonValue value = null) {
			JsonValue returnValue = null;
			Reader.ReadValue (
				(instance, valueType) => {
					if (value is null) {
						returnValue = new JsonValue (instance, valueType);
						return;
					}
					value._Type = valueType;
					value._Value = instance;
					returnValue = value;
				},
				() => returnValue = BuildArray (value),
				() => returnValue = BuildObject (value)
			);
			return returnValue;
		}

		public JsonArray BuildArray (JsonArray array = null) {
			Stacks.Push (array ?? new JsonArray ());
			int oldCount = Stacks.Peek ().Count;
			int newCount = 0;
			Reader.ReadArray (i => {
				newCount = i + 1;
				if (i < oldCount) {
					Stacks.Peek ()[i] = BuildValue (Stacks.Peek ()[i]);
					return;
				}
				Stacks.Peek ().Add (BuildValue ());
			});
			if (Stacks.Peek ().Count > newCount) {
				Stacks.Peek ().RemoveRange (0, oldCount);
			}
			return Stacks.Pop ();
		}

		public JsonObject BuildObject (JsonObject jsonObject = null) {
			JsonObject returnValue = jsonObject ?? new JsonObject ();
			string key = null;
			Reader.ReadObject (name => {
				key = name;
				return true;
			}, () => returnValue.Add (JsonApi.Naming (key, Config.NamingType), BuildValue (returnValue.Get (key))));//todo 待测试复用是否正常
			return returnValue;
		}

		#endregion

	}

}