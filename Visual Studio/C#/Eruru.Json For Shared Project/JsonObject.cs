using System;
using System.Collections.Generic;
using System.IO;

namespace Eruru.Json {

	public class JsonObject : List<JsonKey>, IJsonSerializable, IJsonObject {

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
			return Serialize ();
		}

		static JsonObject Build (TextReader textReader) {
			if (textReader is null) {
				throw new ArgumentNullException (nameof (textReader));
			}
			using (JsonTextReader reader = new JsonTextReader (textReader)) {
				return new JsonValueBuilder (reader).BuildObject ();
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
				builder.BuildObject ();
				return builder;
			}
		}
		JsonTextBuilder Build (TextWriter textWriter, bool compress, JsonConfig config) {
			if (textWriter is null) {
				throw new ArgumentNullException (nameof (textWriter));
			}
			using (JsonTextBuilder builder = new JsonTextBuilder (new JsonValueReader (this), textWriter, compress, config)) {
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