using System;
using System.IO;
using Eruru.TextTokenizer;

namespace Eruru.Json {

	public class JsonSelector : TextTokenizer<JsonTokenType> {

		JsonValue Root;

		public JsonSelector (JsonValue root) : base (
			JsonTokenType.End,
			JsonTokenType.String,
			JsonTokenType.Integer,
			JsonTokenType.Decimal,
			JsonTokenType.String
		) {
			Root = root ?? throw new ArgumentNullException (nameof (root));
			Add (JsonKeyword.Dot, JsonTokenType.Dot);
			Add (JsonKeyword.LeftBracket, JsonTokenType.LeftBracket);
			Add (JsonKeyword.RightBracket, JsonTokenType.RightBracket);
		}

		public JsonValue Select (string path) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			JsonValue Value = Root;
			TextReader = new StringReader (path);
			while (MoveNext ()) {
				switch (Current.Type) {
					case JsonTokenType.String:
					case JsonTokenType.Integer:
					case JsonTokenType.Decimal:
					case JsonTokenType.Dot:
						if (Current.Type == JsonTokenType.Dot) {
							MoveNext ();
						}
						Value = Value[Current.String];
						break;
					case JsonTokenType.LeftBracket:
						MoveNext ();
						Value = Value[Current.Int];
						MoveNext ();
						if (Current.Type != JsonTokenType.RightBracket) {
							throw new JsonTextReaderException (Buffer, Current, JsonKeyword.RightBracket);
						}
						break;
					default:
						throw new JsonTextReaderException (Buffer, Current, "键名", JsonKeyword.Dot, JsonKeyword.LeftBracket);
				}
			}
			return Value;
		}

	}

}