namespace Eruru.Json {

	public interface IJsonReader {

		void ReadValue (JsonAction<object, JsonValueType> value, JsonAction readArray, JsonAction readObject);

		void ReadArray (JsonAction readValue);

		void ReadObject (JsonFunc<string, bool> key, JsonAction readValue);

	}

}