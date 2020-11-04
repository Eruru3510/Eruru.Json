using System;
using System.IO;

namespace Eruru.Json {

	public class JsonTextBuilder : JsonTextWriter, IJsonBuilder {

		readonly IJsonReader Reader;

		public JsonTextBuilder (IJsonReader reader, TextWriter textWriter, JsonConfig config = null) : base (textWriter, config) {
			if (textWriter is null) {
				throw new ArgumentNullException (nameof (textWriter));
			}
			Reader = reader ?? throw new ArgumentNullException (nameof (reader));
		}
		public JsonTextBuilder (IJsonReader reader, TextWriter textWriter, bool compress, JsonConfig config = null) : base (textWriter, compress, config) {
			if (textWriter is null) {
				throw new ArgumentNullException (nameof (textWriter));
			}
			Reader = reader ?? throw new ArgumentNullException (nameof (reader));
		}

		#region IJsonBuilder

		public void BuildValue () {
			Reader.ReadValue ((value, valueType) => Write (value, valueType), () => BuildArray (), () => BuildObject ());
		}

		public void BuildArray () {
			BeginArray ();
			Reader.ReadArray (() => BuildValue ());
			EndArray ();
		}

		public void BuildObject () {
			BeginObject ();
			Reader.ReadObject (name => {
				Write (name, JsonValueType.String);
				return true;
			}, () => BuildValue ());
			EndObject ();
		}

		#endregion

	}

}