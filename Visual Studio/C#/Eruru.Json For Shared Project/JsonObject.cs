using System;
using System.Collections.Generic;
using System.IO;

namespace Eruru.Json {

	public class JsonObject : List<JsonKey>, IJsonTextualization, IJsonObject {

		public JsonObject () {

		}
		public JsonObject (params JsonKey[] keys) {
			AddRange (keys);
		}
		public JsonObject (IEnumerable<JsonKey> keys) {
			AddRange (keys);
		}

		public static JsonObject Parse (string text) {
			if (text is null) {
				throw new ArgumentNullException (nameof (text));
			}
			return Build (new StringReader (text));
		}

		public static JsonObject Load (Stream stream) {
			if (stream is null) {
				throw new ArgumentNullException (nameof (stream));
			}
			return Build (new StreamReader (stream));
		}
		public static JsonObject Load (string path) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			return Build (new StreamReader (path));
		}

		public override string ToString () {
			return ToText ();
		}

		static JsonObject Build (TextReader textReader) {
			if (textReader is null) {
				throw new ArgumentNullException (nameof (textReader));
			}
			using (JsonTextReader reader = new JsonTextReader (textReader)) {
				return new JsonValueBuilder (reader).BuildObject ();
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
				builder.BuildObject ();
				return builder;
			}
		}

		#endregion

		#region IJsonObject

		public JsonValue this[string name] {

			get => GetOrCreate (name);

			set {
				for (int i = 0; i < Count; i++) {
					if (JsonAPI.Equals (name, this[i].Name)) {
						this[i].Value = value;
						return;
					}
				}
				Add (name, value);
			}

		}

		public JsonKey Add (string name) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			JsonKey key = new JsonKey (name);
			base.Add (key);
			return key;
		}
		public JsonKey Add (string name, object value) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			JsonKey key = new JsonKey (name, value);
			base.Add (key);
			return key;
		}

		public JsonKey Get (string name) {
			foreach (JsonKey key in this) {
				if (JsonAPI.Equals (name, key.Name)) {
					return key;
				}
			}
			return null;
		}

		JsonKey GetOrCreate (string name) {
			return Get (name) ?? Add (name);
		}

		#endregion

	}

}