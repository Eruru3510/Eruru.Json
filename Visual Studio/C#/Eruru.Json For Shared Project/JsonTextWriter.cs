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
			if (JsonAPI.TryGetValueType (value, Config, out JsonValueType valueType)) {
				Write (value, valueType);
				return;
			}
			throw new JsonIsNotSupportException (valueType);
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

		public void Write (object value, JsonValueType valueType) {
			CheckEnd ();
			Head ();
			if (JsonAPI.HasFlag (Stacks.Peek ().Stage, JsonTextWriterStage.Key)) {
				valueType = JsonValueType.String;
			}
			switch (valueType) {
				case JsonValueType.Null:
					TextWriter.Write (JsonKeyword.Null);
					break;
				case JsonValueType.Long:
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
					WriteString (JsonAPI.Unescape (value.ToString ()));
					break;
				case JsonValueType.DateTime:
					DateTime dateTime = (DateTime)value;
					WriteString (Config.UTCTime ? dateTime.ToUniversalTime ().ToString ("yyyy-MM-ddTHH:mm:ssZ") : dateTime.ToString ());
					break;
				default:
					throw new JsonIsNotSupportException (valueType);
			}
			NextStage ();
		}

		public override string ToString () {
			Check ();
			return TextWriter.ToString ();
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
				throw new JsonException ("Json is finished");
			}
		}

		void Check () {
			if (Stacks.Peek ().Stage != JsonTextWriterStage.End) {
				throw new JsonException ($"Json is not finished{Environment.NewLine}{TextWriter}");
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
					throw new JsonIsNotSupportException (Stacks.Peek ());
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