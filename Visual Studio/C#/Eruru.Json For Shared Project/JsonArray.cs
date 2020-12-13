using System;
using System.Collections.Generic;
using System.IO;

namespace Eruru.Json {

	public class JsonArray : List<JsonValue>, IJsonSerializable, IJsonArray, IEquatable<JsonArray> {

		public JsonArray () {

		}
		public JsonArray (params object[] values) {
			if (values is null) {
				throw new ArgumentNullException (nameof (values));
			}
			AddRange (Array.ConvertAll (values, value => new JsonValue (value)));
		}
		public JsonArray (params JsonValue[] values) {
			if (values is null) {
				throw new ArgumentNullException (nameof (values));
			}
			AddRange (values);
		}
		public JsonArray (IEnumerable<JsonValue> values) {
			if (values is null) {
				throw new ArgumentNullException (nameof (values));
			}
			AddRange (values);
		}

		public static JsonArray Parse (string text, JsonConfig config = null) {
			if (text is null) {
				throw new ArgumentNullException (nameof (text));
			}
			return Load (new StringReader (text), null, config);
		}
		public static JsonArray Parse (string text, JsonArray array, JsonConfig config = null) {
			if (text is null) {
				throw new ArgumentNullException (nameof (text));
			}
			return Load (new StringReader (text), array, config);
		}

		public static JsonArray Load (string path, JsonConfig config = null) {
			if (JsonApi.IsNullOrWhiteSpace (path)) {
				throw new ArgumentException ($"“{nameof (path)}”不能是 Null 或空白", nameof (path));
			}
			return Load (new StreamReader (path), null, config);
		}
		public static JsonArray Load (TextReader textReader, JsonConfig config = null) {
			if (textReader is null) {
				throw new ArgumentNullException (nameof (textReader));
			}
			return Load (textReader, null, config);
		}
		public static JsonArray Load (string path, JsonArray array, JsonConfig config = null) {
			if (JsonApi.IsNullOrWhiteSpace (path)) {
				throw new ArgumentException ($"“{nameof (path)}”不能是 Null 或空白", nameof (path));
			}
			return Load (new StreamReader (path), array, config);
		}
		public static JsonArray Load (TextReader textReader, JsonArray array, JsonConfig config = null) {
			if (textReader is null) {
				throw new ArgumentNullException (nameof (textReader));
			}
			using (JsonTextReader reader = new JsonTextReader (textReader, config)) {
				return new JsonValueBuilder (reader, config).BuildArray (array);
			}
		}

		public JsonValue Select (string path) {
			if (JsonApi.IsNullOrWhiteSpace (path)) {
				throw new ArgumentException ($"“{nameof (path)}”不能是 Null 或空白", nameof (path));
			}
			return JsonValue.Select (this, path);
		}

		public override string ToString () {
			return Serialize ();
		}

		public static implicit operator JsonArray (string text) {
			if (text is null) {
				throw new ArgumentNullException (nameof (text));
			}
			return Parse (text);
		}

		public static implicit operator string (JsonArray array) {
			if (array is null) {
				throw new ArgumentNullException (nameof (array));
			}
			return array.ToString ();
		}

		#region IJsonSerializable

		public string Serialize (JsonConfig config = null) {
			using (JsonTextBuilder builder = new JsonTextBuilder (new JsonValueReader (this), new StringWriter (), config)) {
				builder.BuildArray ();
				return builder.ToString ();
			}
		}
		public void Serialize (string path, JsonConfig config = null) {
			if (JsonApi.IsNullOrWhiteSpace (path)) {
				throw new ArgumentException ($"“{nameof (path)}”不能为 Null 或空白", nameof (path));
			}
			Serialize (new StreamWriter (path), config);
		}
		public void Serialize (TextWriter textWriter, JsonConfig config = null) {
			if (textWriter is null) {
				throw new ArgumentNullException (nameof (textWriter));
			}
			using (JsonTextBuilder builder = new JsonTextBuilder (new JsonValueReader (this), textWriter, config)) {
				builder.BuildArray ();
			}
		}
		public string Serialize (bool compress, JsonConfig config = null) {
			using (JsonTextBuilder builder = new JsonTextBuilder (new JsonValueReader (this), new StringWriter (), compress, config)) {
				builder.BuildArray ();
				return builder.ToString ();
			}
		}
		public void Serialize (string path, bool compress, JsonConfig config = null) {
			if (JsonApi.IsNullOrWhiteSpace (path)) {
				throw new ArgumentException ($"“{nameof (path)}”不能为 Null 或空白", nameof (path));
			}
			Serialize (new StreamWriter (path), compress, config);
		}
		public void Serialize (TextWriter textWriter, bool compress, JsonConfig config = null) {
			if (textWriter is null) {
				throw new ArgumentNullException (nameof (textWriter));
			}
			using (JsonTextBuilder builder = new JsonTextBuilder (new JsonValueReader (this), textWriter, compress, config)) {
				builder.BuildArray ();
			}
		}

		#endregion

		#region IJsonArray

		public new JsonValue this[int index] {

			get => GetOrCreate (index);

			set => GetOrCreate (index).Value = value;

		}

		public JsonValue Get (int index) {
			return base[index];
		}

		JsonValue GetOrCreate (int index) {
			while (index >= Count) {
				Add (new JsonValue ());
			}
			return base[index];
		}

		#endregion

		#region IEquatable<JsonArray>

		public bool Equals (JsonArray other) {
			if (other is null) {
				return false;
			}
			if (Count != other.Count) {
				return false;
			}
			for (int i = 0; i < Count; i++) {
				if (base[i] != other[i]) {
					return false;
				}
			}
			return true;
		}

		#endregion

	}

}