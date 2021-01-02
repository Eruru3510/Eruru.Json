using System;
using System.Collections.Generic;
using System.IO;

namespace Eruru.Json {

	public class JsonTextWriter : IDisposable {

		readonly TextWriter TextWriter;
		readonly JsonConfig Config;
		readonly bool Compress;
		readonly Stack<JsonTextWriterStack> Stacks = new Stack<JsonTextWriterStack> ();

		int Indent;

		public JsonTextWriter (TextWriter textWriter, JsonConfig config = null) {
			TextWriter = textWriter ?? throw new ArgumentNullException (nameof (textWriter));
			Config = config ?? JsonConfig.Default;
			Compress = Config.Compress;
			Stacks.Push (new JsonTextWriterStack (JsonTextWriterStage.Value));
		}
		public JsonTextWriter (TextWriter textWriter, bool compress, JsonConfig config = null) {
			TextWriter = textWriter ?? throw new ArgumentNullException (nameof (textWriter));
			Compress = compress;
			Config = config ?? JsonConfig.Default;
			Stacks.Push (new JsonTextWriterStack (JsonTextWriterStage.Value));
		}

		public void Write (object value) {
			if (value is JsonValue jsonValue) {
				new JsonTextBuilder (new JsonValueReader (jsonValue), this).BuildValue ();
				return;
			}
			if (value is JsonArray array) {
				new JsonTextBuilder (new JsonValueReader (array), this).BuildArray ();
				return;
			}
			if (value is JsonObject jsonObject) {
				new JsonTextBuilder (new JsonValueReader (jsonObject), this).BuildObject ();
				return;
			}
			if (JsonApi.TryGetValueType (value, out JsonValueType valueType, Config)) {
				Write (value, valueType);
				return;
			}
			new JsonTextBuilder (new JsonSerializer (value, Config), this).BuildValue ();
		}

		public void BeginArray () {
			Begin (true, true);
		}

		public void EndArray () {
			Begin (false, true);
		}

		public void BeginObject () {
			Begin (true, false);
		}

		public void EndObject () {
			Begin (false, false);
		}

		public override string ToString () {
			Check ();
			return TextWriter.ToString ();
		}

		internal void Write (object value, JsonValueType valueType) {
			CheckEnd ();
			Head ();
			if (JsonApi.HasFlag (Stacks.Peek ().Stage, JsonTextWriterStage.Key)) {
				valueType = JsonValueType.String;
			}
			switch (valueType) {
				case JsonValueType.Null:
					TextWriter.Write (JsonKeyword.Null);
					break;
				case JsonValueType.Integer:
					TextWriter.Write (value);
					break;
				case JsonValueType.Decimal:
					TextWriter.Write (value);
					if (value.ToString ().IndexOf ('.') < 0) {
						TextWriter.Write (".0");
					}
					break;
				case JsonValueType.Bool:
					TextWriter.Write ((bool)value ? JsonKeyword.True : JsonKeyword.False);
					break;
				case JsonValueType.String:
					WriteString (JsonApi.CancelUnescape ((string)value));
					break;
				case JsonValueType.DateTime:
					DateTime dateTime = (DateTime)value;
					WriteString (Config.UTCTime ? dateTime.ToUniversalTime ().ToString ("yyyy-MM-ddTHH:mm:ssZ") : dateTime.ToString ());
					break;
				default:
					throw new JsonNotSupportException (valueType);
			}
			NextStage ();
		}

		void WriteString (string value) {
			TextWriter.Write (JsonKeyword.DoubleQuot);
			TextWriter.Write (value);
			TextWriter.Write (JsonKeyword.DoubleQuot);
		}

		void Begin (bool isBegin, bool isArray) {
			CheckEnd ();
			if (isBegin) {
				Head ();
				Indent++;
				if (isArray) {
					Stacks.Push (new JsonTextWriterStack (JsonTextWriterStage.FirstArrayValue));
					TextWriter.Write (JsonKeyword.LeftBracket);
					return;
				}
				Stacks.Push (new JsonTextWriterStack (JsonTextWriterStage.FirstObjectKey));
				TextWriter.Write (JsonKeyword.LeftBrace);
				return;
			}
			Indent--;
			if (Stacks.Peek ().HasValue) {
				NewLineIndent ();
			}
			TextWriter.Write (isArray ? JsonKeyword.RightBracket : JsonKeyword.RightBrace);
			Stacks.Pop ();
			NextStage ();
		}

		void CheckEnd () {
			if (Stacks.Peek ().Stage == JsonTextWriterStage.End) {
				throw new JsonException ("Json已写完");
			}
		}

		void Check () {
			if (Stacks.Peek ().Stage != JsonTextWriterStage.End) {
				throw new JsonException ($"Json未写完{Environment.NewLine}{TextWriter}");
			}
		}

		void Head () {
			switch (Stacks.Peek ().Stage) {
				case JsonTextWriterStage.FirstArrayValue:
				case JsonTextWriterStage.FirstObjectKey:
					NewLineIndent ();
					break;
				case JsonTextWriterStage.ArrayValue:
				case JsonTextWriterStage.ObjectKey:
					CommaNewLineIndent ();
					break;
				case JsonTextWriterStage.ObjectValue:
					TextWriter.Write (JsonKeyword.Semicolon);
					if (!Compress) {
						TextWriter.Write (JsonKeyword.Space);
					}
					break;
			}
			Stacks.Peek ().HasValue = true;
		}

		void NextStage () {
			switch (Stacks.Peek ().Stage) {
				case JsonTextWriterStage.Value:
					Stacks.Peek ().Stage = JsonTextWriterStage.End;
					break;
				case JsonTextWriterStage.FirstArrayValue:
				case JsonTextWriterStage.ArrayValue:
					Stacks.Peek ().Stage = JsonTextWriterStage.ArrayValue;
					break;
				case JsonTextWriterStage.FirstObjectKey:
				case JsonTextWriterStage.ObjectKey:
					Stacks.Peek ().Stage = JsonTextWriterStage.ObjectValue;
					break;
				case JsonTextWriterStage.ObjectValue:
					Stacks.Peek ().Stage = JsonTextWriterStage.ObjectKey;
					break;
				default:
					throw new JsonNotSupportException (Stacks.Peek ().Stage);
			}
		}

		void NewLineIndent () {
			if (Compress) {
				return;
			}
			TextWriter.WriteLine ();
			for (int i = 0; i < Indent; i++) {
				TextWriter.Write (Config.IndentString);
			}
		}

		void CommaNewLineIndent () {
			TextWriter.Write (JsonKeyword.Comma);
			NewLineIndent ();
		}

		#region IDisposable

		public void Dispose () {
			TextWriter.Dispose ();
			Check ();
		}

		#endregion

	}

}