using System;
using System.Collections.Generic;
using System.IO;

namespace Eruru.Json {

	public class JsonObject : Dictionary<string, JsonKey>, IJsonSerializable, IEquatable<JsonObject>, IJsonObject, IEnumerable<JsonKey> {

		public JsonObject () {

		}

		public static JsonObject Parse (string text, JsonConfig config = null) {
			if (text is null) {
				throw new ArgumentNullException (nameof (text));
			}
			return Load (new StringReader (text), null, config);
		}
		public static JsonObject Parse (string text, JsonObject jsonObject, JsonConfig config = null) {
			if (text is null) {
				throw new ArgumentNullException (nameof (text));
			}
			return Load (new StringReader (text), jsonObject, config);
		}

		public static JsonObject Load (string path, JsonConfig config = null) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			return Load (new StreamReader (path), null, config);
		}
		public static JsonObject Load (TextReader textReader, JsonConfig config = null) {
			if (textReader is null) {
				throw new ArgumentNullException (nameof (textReader));
			}
			return Load (textReader, null, config);
		}
		public static JsonObject Load (string path, JsonObject jsonObject, JsonConfig config = null) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			return Load (new StreamReader (path), jsonObject, config);
		}
		public static JsonObject Load (TextReader textReader, JsonObject jsonObject, JsonConfig config = null) {
			if (textReader is null) {
				throw new ArgumentNullException (nameof (textReader));
			}
			using (JsonTextReader reader = new JsonTextReader (textReader, config)) {
				return new JsonValueBuilder (reader, config).BuildObject (jsonObject);
			}
		}

		public JsonValue Select (string path) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			using (JsonSelector selector = new JsonSelector (this)) {
				return selector.Select (path);
			}
		}

		public override string ToString () {
			return Serialize ();
		}

		public static implicit operator JsonObject (string text) {
			if (text is null) {
				throw new ArgumentNullException (nameof (text));
			}
			return Parse (text);
		}

		public static implicit operator string (JsonObject jsonObject) {
			if (jsonObject is null) {
				throw new ArgumentNullException (nameof (jsonObject));
			}
			return jsonObject.ToString ();
		}

		#region IJsonSerializable

		public string Serialize (JsonConfig config = null) {
			using (JsonTextBuilder builder = new JsonTextBuilder (new JsonValueReader (this), new StringWriter (), config)) {
				builder.BuildObject ();
				return builder.ToString ();
			}
		}
		public void Serialize (string path, JsonConfig config = null) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			Serialize (new StreamWriter (path), config);
		}
		public void Serialize (TextWriter textWriter, JsonConfig config = null) {
			if (textWriter is null) {
				throw new ArgumentNullException (nameof (textWriter));
			}
			using (JsonTextBuilder builder = new JsonTextBuilder (new JsonValueReader (this), textWriter, config)) {
				builder.BuildObject ();
			}
		}
		public string Serialize (bool compress, JsonConfig config = null) {
			using (JsonTextBuilder builder = new JsonTextBuilder (new JsonValueReader (this), new StringWriter (), compress, config)) {
				builder.BuildObject ();
				return builder.ToString ();
			}
		}
		public void Serialize (string path, bool compress, JsonConfig config = null) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			Serialize (new StreamWriter (path), compress, config);
		}
		public void Serialize (TextWriter textWriter, bool compress, JsonConfig config = null) {
			if (textWriter is null) {
				throw new ArgumentNullException (nameof (textWriter));
			}
			using (JsonTextBuilder builder = new JsonTextBuilder (new JsonValueReader (this), textWriter, compress, config)) {
				builder.BuildObject ();
			}
		}

		#endregion

		#region IEquatable<JsonObject>

		public bool Equals (JsonObject other) {
			if (other is null) {
				return false;
			}
			if (Count != other.Count) {
				return false;
			}
			foreach (JsonKey key in Values) {
				if (!other.TryGetValue (key.Name, out JsonKey otherKey) || !key.Equals (otherKey)) {
					return false;
				}
			}
			return true;
		}

		#endregion

		#region IJsonObject

		public new JsonValue this[string name] {

			get {
				if (name is null) {
					throw new ArgumentNullException (nameof (name));
				}
				return GetOrCreate (name);
			}

			set {
				if (name is null) {
					throw new ArgumentNullException (nameof (name));
				}
				GetOrCreate (name).Value = value;
			}

		}

		public JsonKey Add (string name) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			if (TryGetValue (name, out JsonKey key)) {
				return key;
			}
			key = new JsonKey (name);
			base.Add (name, key);
			return key;
		}
		public JsonKey Add (string name, object value) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			if (TryGetValue (name, out JsonKey key)) {
				key.Value = value;
				return key;
			}
			key = new JsonKey (name, value);
			base.Add (name, key);
			return key;
		}

		public JsonKey Get (string name) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			TryGetValue (name, out JsonKey key);
			return key;
		}

		public JsonKey Rename (string oldName, string newName) {
			if (oldName is null) {
				throw new ArgumentNullException (nameof (oldName));
			}
			if (newName is null) {
				throw new ArgumentNullException (nameof (newName));
			}
			if (TryGetValue (oldName, out JsonKey key)) {
				Remove (oldName);
				base.Add (newName, key);
				key.Name = newName;
				return key;
			}
			return null;
		}

		JsonKey GetOrCreate (string name) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return Get (name) ?? Add (name);
		}

		#endregion

		#region IEnumerable<JsonKey>

		public new IEnumerator<JsonKey> GetEnumerator () {
			return Values.GetEnumerator ();
		}

		#endregion

	}

}