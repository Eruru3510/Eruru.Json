using System;
using System.Collections.Generic;
using System.IO;

namespace Eruru.Json {

	public class JsonArray : List<JsonValue>, IJsonSerializable, IJsonArray {

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

		public static JsonArray Parse (string text) {
			if (text is null) {
				throw new ArgumentNullException (nameof (text));
			}
			return Build (new StringReader (text));
		}

		public static JsonArray Load (Stream stream) {
			if (stream is null) {
				throw new ArgumentNullException (nameof (stream));
			}
			return Build (new StreamReader (stream));
		}
		public static JsonArray Load (string path) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			return Build (new StreamReader (path));
		}

		public override string ToString () {
			return Serialize ();
		}

		static JsonArray Build (TextReader textReader) {
			if (textReader is null) {
				throw new ArgumentNullException (nameof (textReader));
			}
			using (JsonTextReader reader = new JsonTextReader (textReader)) {
				return new JsonValueBuilder (reader).BuildArray ();
			}
		}

		#region IJsonSerializable

		public string Serialize (JsonConfig config = null) {
			return Build (new StringWriter (), config).ToString ();
		}
		public void Serialize (string path, JsonConfig config = null) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			Build (new StreamWriter (path), config);
		}
		public void Serialize (Stream stream, JsonConfig config = null) {
			if (stream is null) {
				throw new ArgumentNullException (nameof (stream));
			}
			Build (new StreamWriter (stream), config);
		}
		public void Serialize (StreamWriter streamWriter, JsonConfig config = null) {
			if (streamWriter is null) {
				throw new ArgumentNullException (nameof (streamWriter));
			}
			Build (streamWriter, config);
		}
		public string Serialize (bool compress, JsonConfig config = null) {
			return Build (new StringWriter (), compress, config).ToString ();
		}
		public void Serialize (string path, bool compress, JsonConfig config = null) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			Build (new StreamWriter (path), compress, config);
		}
		public void Serialize (Stream stream, bool compress, JsonConfig config = null) {
			if (stream is null) {
				throw new ArgumentNullException (nameof (stream));
			}
			Build (new StreamWriter (stream), compress, config);
		}
		public void Serialize (StreamWriter streamWriter, bool compress, JsonConfig config = null) {
			if (streamWriter is null) {
				throw new ArgumentNullException (nameof (streamWriter));
			}
			Build (streamWriter, compress, config);
		}

		JsonTextBuilder Build (TextWriter textWriter, JsonConfig config) {
			if (textWriter is null) {
				throw new ArgumentNullException (nameof (textWriter));
			}
			using (JsonTextBuilder builder = new JsonTextBuilder (new JsonValueReader (this), textWriter, config)) {
				builder.BuildArray ();
				return builder;
			}
		}
		JsonTextBuilder Build (TextWriter textWriter, bool compress, JsonConfig config) {
			if (textWriter is null) {
				throw new ArgumentNullException (nameof (textWriter));
			}
			using (JsonTextBuilder builder = new JsonTextBuilder (new JsonValueReader (this), textWriter, compress, config)) {
				builder.BuildArray ();
				return builder;
			}
		}

		#endregion

		#region IJsonArray

		public new JsonValue this[int index] {

			get {
				while (index >= Count) {
					Add (new JsonValue ());
				}
				return base[index];
			}

			set => base[index] = value;

		}

		#endregion

	}

}