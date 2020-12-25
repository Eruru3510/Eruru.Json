namespace Eruru.Json {

	public interface IJsonArray {

		JsonValue this[int index] { get; set; }

		JsonValue Get (int index);

	}

}