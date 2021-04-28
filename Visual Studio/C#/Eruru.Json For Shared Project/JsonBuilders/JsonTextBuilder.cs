using System;
using System.IO;

namespace Eruru.Json {

	public class JsonTextBuilder : IDisposable, IJsonBuilder {

		readonly JsonConfig Config;
		readonly JsonTextWriter TextWriter;
		readonly IJsonReader Reader;

		public JsonTextBuilder (IJsonReader reader, TextWriter textWriter, JsonConfig config = null) {
			if (textWriter is null) {
				throw new ArgumentNullException (nameof (textWriter));
			}
			Reader = reader ?? throw new ArgumentNullException (nameof (reader));
			TextWriter = new JsonTextWriter (textWriter, config);
			Config = config ?? JsonConfig.Default;
		}
		public JsonTextBuilder (IJsonReader reader, TextWriter textWriter, bool compress, JsonConfig config = null) {
			if (textWriter is null) {
				throw new ArgumentNullException (nameof (textWriter));
			}
			Reader = reader ?? throw new ArgumentNullException (nameof (reader));
			Config = config ?? JsonConfig.Default;
			TextWriter = new JsonTextWriter (textWriter, compress, config);
		}
		public JsonTextBuilder (IJsonReader reader, JsonTextWriter textWriter, JsonConfig config = null) {
			Reader = reader ?? throw new ArgumentNullException (nameof (reader));
			TextWriter = textWriter ?? throw new ArgumentNullException (nameof (textWriter));
			Config = config ?? JsonConfig.Default;
		}

		public override string ToString () {
			return TextWriter.ToString ();
		}

		#region IDisposable

		public void Dispose () {
			TextWriter.Dispose ();
		}

		#endregion

		#region IJsonBuilder

		public void BuildValue () {
			Reader.ReadValue (
				(value, valueType) => TextWriter.Write (value, valueType),
				() => BuildArray (),
				() => BuildObject ()
			);
		}

		public void BuildArray () {
			TextWriter.BeginArray ();
			Reader.ReadArray (i => BuildValue ());
			TextWriter.EndArray ();
		}

		public void BuildObject () {
			TextWriter.BeginObject ();
			Reader.ReadObject (
				name => {
					TextWriter.Write (JsonApi.Naming (name, Config.NamingType), JsonValueType.String);
					return true;
				},
				() => BuildValue ()
			);
			TextWriter.EndObject ();
		}

		#endregion

	}

}