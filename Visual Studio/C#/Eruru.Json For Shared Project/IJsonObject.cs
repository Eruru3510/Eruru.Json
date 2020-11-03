namespace Eruru.Json {

	public interface IJsonObject {

		JsonValue this[string name] { get; set; }

		JsonKey Add (string name);
		JsonKey Add (string name, object value);

		JsonKey Get (string name);

	}

}