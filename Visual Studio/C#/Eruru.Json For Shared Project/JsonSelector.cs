using System;
using System.IO;
using Eruru.TextTokenizer;

namespace Eruru.Json {

	public class JsonSelector : TextTokenizer<JsonTokenType> {

		readonly JsonValue Root;

		public JsonSelector (JsonValue root) : base (JsonTokenType.End, JsonTokenType.String, JsonTokenType.Integer, JsonTokenType.Decimal, JsonTokenType.String) {
			Root = root ?? throw new ArgumentNullException (nameof (root));
			AddSymbol (JsonKeyword.Dot, JsonTokenType.Dot);
			AddSymbol (JsonKeyword.LeftBracket, JsonTokenType.LeftBracket);
			AddSymbol (JsonKeyword.RightBracket, JsonTokenType.RightBracket);
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
							throw new JsonTextReaderException (this, JsonKeyword.RightBracket);
						}
						break;
					default:
						throw new JsonTextReaderException (this, "键名", JsonKeyword.Dot, JsonKeyword.LeftBracket);
				}
			}
			return Value;
		}

	}

}