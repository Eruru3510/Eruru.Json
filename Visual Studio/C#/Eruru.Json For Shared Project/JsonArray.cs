using System;
using System.Collections.Generic;
using System.IO;

namespace Eruru.Json {

	public class JsonArray : List<JsonValue>, IJsonTextualization, IJsonArray {

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
			return ToText ();
		}

		static JsonArray Build (TextReader textReader) {
			if (textReader is null) {
				throw new ArgumentNullException (nameof (textReader));
			}
			using (JsonTextReader reader = new JsonTextReader (textReader)) {
				return new JsonValueBuilder (reader).BuildArray ();
			}
		}

		#region IJsonTextualization

		public string ToText (JsonConfig config = null) {
			return BuildText (new StringWriter (), config).ToString ();
		}

		public void Save (Stream stream, JsonConfig config = null) {
			if (stream is null) {
				throw new ArgumentNullException (nameof (stream));
			}
			BuildText (new StreamWriter (stream), config);
		}
		public void Save (string path, JsonConfig config = null) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			BuildText (new StreamWriter (path), config);
		}

		JsonTextBuilder BuildText (TextWriter textWriter, JsonConfig config) {
			if (textWriter is null) {
				throw new ArgumentNullException (nameof (textWriter));
			}
			using (JsonTextBuilder builder = new JsonTextBuilder (new JsonValueReader (this), textWriter, config)) {
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