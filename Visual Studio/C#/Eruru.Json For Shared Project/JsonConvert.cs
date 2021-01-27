using System;
using System.IO;

namespace Eruru.Json {

	public static class JsonConvert {

		public static JsonValue SerializeToValue (object instance, JsonConfig config = null) {
			return new JsonValueBuilder (new JsonSerializer (instance, config), config).BuildValue ();
		}

		public static JsonArray SerializeToArray (object instance, JsonConfig config = null) {
			return new JsonValueBuilder (new JsonSerializer (instance, config), config).BuildArray ();
		}

		public static JsonObject SerializeToObject (object instance, JsonConfig config = null) {
			return new JsonValueBuilder (new JsonSerializer (instance, config), config).BuildObject ();
		}

		public static string Serialize (object instance, JsonConfig config = null) {
			using (JsonTextBuilder builder = new JsonTextBuilder (new JsonSerializer (instance, config), new StringWriter (), config)) {
				builder.BuildValue ();
				return builder.ToString ();
			}
		}

		public static void Serialize (object instance, string path, JsonConfig config = null) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			using (JsonTextBuilder builder = new JsonTextBuilder (new JsonSerializer (instance, config), new StreamWriter (path), config)) {
				builder.BuildValue ();
			}
		}

		public static void Serialize (object instance, TextWriter textWriter, JsonConfig config = null) {
			if (textWriter is null) {
				throw new ArgumentNullException (nameof (textWriter));
			}
			using (JsonTextBuilder builder = new JsonTextBuilder (new JsonSerializer (instance, config), textWriter, config)) {
				builder.BuildValue ();
			}
		}

		public static string Serialize (object instance, bool compress, JsonConfig config = null) {
			using (JsonTextBuilder builder = new JsonTextBuilder (new JsonSerializer (instance, config), new StringWriter (), compress, config)) {
				builder.BuildValue ();
				return builder.ToString ();
			}
		}

		public static void Serialize (object instance, string path, bool compress, JsonConfig config = null) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			using (JsonTextBuilder builder = new JsonTextBuilder (new JsonSerializer (instance, config), new StreamWriter (path), compress, config)) {
				builder.BuildValue ();
			}
		}

		public static void Serialize (object instance, TextWriter textWriter, bool compress, JsonConfig config = null) {
			if (textWriter is null) {
				throw new ArgumentNullException (nameof (textWriter));
			}
			using (JsonTextBuilder builder = new JsonTextBuilder (new JsonSerializer (instance, config), textWriter, compress, config)) {
				builder.BuildValue ();
			}
		}

		public static T Deserialize<T> (JsonValue value, JsonConfig config = null) {
			if (value is null) {
				throw new ArgumentNullException (nameof (value));
			}
			return new JsonDeserializer (new JsonValueReader (value), config).BuildValue<T> ();
		}

		public static T Deserialize<T> (JsonArray array, JsonConfig config = null) {
			if (array is null) {
				throw new ArgumentNullException (nameof (array));
			}
			return new JsonDeserializer (new JsonValueReader (array), config).BuildArray<T> ();
		}

		public static T Deserialize<T> (JsonObject jsonObject, JsonConfig config = null) {
			if (jsonObject is null) {
				throw new ArgumentNullException (nameof (jsonObject));
			}
			return new JsonDeserializer (new JsonValueReader (jsonObject), config).BuildObject<T> ();
		}

		public static T Deserialize<T> (string text, JsonConfig config = null) {
			if (text is null) {
				throw new ArgumentNullException (nameof (text));
			}
			using (JsonTextReader reader = new JsonTextReader (new StringReader (text), config)) {
				return new JsonDeserializer (reader, config).BuildValue<T> ();
			}
		}

		public static T Deserialize<T> (TextReader textReader, JsonConfig config = null) {
			if (textReader is null) {
				throw new ArgumentNullException (nameof (textReader));
			}
			using (JsonTextReader reader = new JsonTextReader (textReader, config)) {
				return new JsonDeserializer (reader, config).BuildValue<T> ();
			}
		}

		public static T DeserializeFile<T> (string path, JsonConfig config = null) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			using (JsonTextReader reader = new JsonTextReader (new StreamReader (path), config)) {
				return new JsonDeserializer (reader, config).BuildValue<T> ();
			}
		}

		public static T Deserialize<T> (JsonValue value, T instance, JsonConfig config = null) {
			if (value is null) {
				throw new ArgumentNullException (nameof (value));
			}
			return new JsonDeserializer (new JsonValueReader (value), config).BuildValue<T> (instance);
		}

		public static T Deserialize<T> (JsonArray array, T instance, JsonConfig config = null) {
			if (array is null) {
				throw new ArgumentNullException (nameof (array));
			}
			return new JsonDeserializer (new JsonValueReader (array), config).BuildArray<T> (instance);
		}

		public static T Deserialize<T> (JsonObject jsonObject, T instance, JsonConfig config = null) {
			if (jsonObject is null) {
				throw new ArgumentNullException (nameof (jsonObject));
			}
			return new JsonDeserializer (new JsonValueReader (jsonObject), config).BuildObject<T> (instance);
		}

		public static T Deserialize<T> (string text, T instance, JsonConfig config = null) {
			if (text is null) {
				throw new ArgumentNullException (nameof (text));
			}
			using (JsonTextReader reader = new JsonTextReader (new StringReader (text), config)) {
				return new JsonDeserializer (reader, config).BuildValue<T> (instance);
			}
		}

		public static T Deserialize<T> (TextReader textReader, T instance, JsonConfig config = null) {
			if (textReader is null) {
				throw new ArgumentNullException (nameof (textReader));
			}
			using (JsonTextReader reader = new JsonTextReader (textReader, config)) {
				return new JsonDeserializer (reader, config).BuildValue<T> (instance);
			}
		}

		public static T DeserializeFile<T> (string path, T instance, JsonConfig config = null) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			using (JsonTextReader reader = new JsonTextReader (new StreamReader (path), config)) {
				return new JsonDeserializer (reader, config).BuildValue<T> (instance);
			}
		}

	}

}