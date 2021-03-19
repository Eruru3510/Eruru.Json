using System;
using System.IO;
using System.Text.RegularExpressions;
using Eruru.TextTokenizer;

namespace Eruru.Json {

	public class JsonTextReader : TextTokenizer<JsonTokenType>, IJsonReader {

		readonly JsonConfig Config;

		public JsonTextReader (TextReader textReader, JsonConfig config = null) :
			base (textReader, JsonTokenType.End, JsonTokenType.Unknown, JsonTokenType.Integer, JsonTokenType.Decimal, JsonTokenType.String) {
			if (textReader is null) {
				throw new ArgumentNullException (nameof (textReader));
			}
			Config = config ?? JsonConfig.Default;
			AddSymbol (JsonKeyword.LeftBrace, JsonTokenType.LeftBrace);
			AddSymbol (JsonKeyword.RightBrace, JsonTokenType.RightBrace);
			AddSymbol (JsonKeyword.LeftBracket, JsonTokenType.LeftBracket);
			AddSymbol (JsonKeyword.RightBracket, JsonTokenType.RightBracket);
			AddSymbol (JsonKeyword.Comma, JsonTokenType.Comma);
			AddSymbol (JsonKeyword.Semicolon, JsonTokenType.Semicolon);
			AddKeyword (JsonKeyword.Null, JsonTokenType.Null, null);
			AddKeyword (JsonKeyword.True, JsonTokenType.Bool, true);
			AddKeyword (JsonKeyword.False, JsonTokenType.Bool, false);
		}

		#region IJsonReader

		public void ReadValue (JsonAction<object, JsonValueType> value, JsonAction readArray, JsonAction readObject) {
			if (readArray is null) {
				throw new ArgumentNullException (nameof (readArray));
			}
			if (readObject is null) {
				throw new ArgumentNullException (nameof (readObject));
			}
			switch (Current.Type) {
				case JsonTokenType.Integer:
					value?.Invoke (Current.Value, JsonValueType.Integer);
					return;
				case JsonTokenType.Decimal:
					value?.Invoke (Current.Value, JsonValueType.Decimal);
					return;
				case JsonTokenType.String: {
					string text = Current.String;
					if (DateTime.TryParse (text, out DateTime dateTime)) {
						value?.Invoke (dateTime, JsonValueType.DateTime);
						return;
					}
					value?.Invoke (Regex.Unescape (text), JsonValueType.String);
					return;
				}
				case JsonTokenType.Null:
					value?.Invoke (Current.Value, JsonValueType.Null);
					return;
				case JsonTokenType.Bool:
					value?.Invoke (Current.Value, JsonValueType.Bool);
					return;
				case JsonTokenType.LeftBracket:
					readArray ();
					return;
				case JsonTokenType.LeftBrace:
					readObject ();
					return;
			}
			throw new JsonTextReaderException (
				this,
				JsonKeyword.Null, "整数", "小数", JsonKeyword.True, JsonKeyword.False, "字符串", JsonKeyword.LeftBracket, JsonKeyword.LeftBrace
			);
		}

		public void ReadArray (Action<int> readValue) {
			if (readValue is null) {
				throw new ArgumentNullException (nameof (readValue));
			}
			Read (true, null, readValue);
		}

		public void ReadObject (JsonFunc<string, bool> key, JsonAction readValue) {
			if (key is null) {
				throw new ArgumentNullException (nameof (key));
			}
			if (readValue is null) {
				throw new ArgumentNullException (nameof (readValue));
			}
			Read (false, key, i => readValue ());
		}

		void Read (bool isArray, JsonFunc<string, bool> key, Action<int> readValue) {
			if (!isArray && key is null) {
				throw new ArgumentNullException (nameof (key));
			}
			if (readValue is null) {
				throw new ArgumentNullException (nameof (readValue));
			}
			JsonTokenType start = isArray ? JsonTokenType.LeftBracket : JsonTokenType.LeftBrace;
			if (Current.Type != start) {
				throw new JsonTextReaderException (this, isArray ? JsonKeyword.LeftBracket : JsonKeyword.LeftBrace);
			}
			JsonTokenType end = isArray ? JsonTokenType.RightBracket : JsonTokenType.RightBrace;
			bool isFirst = true;
			int i = 0;
			while (MoveNext ()) {
				if (Current.Type == end) {
					return;
				}
				if (isFirst) {
					isFirst = false;
				} else {
					if (Current.Type != JsonTokenType.Comma) {
						throw new JsonTextReaderException (this, JsonKeyword.Comma);
					}
					MoveNext ();
				}
				bool needReadValue = true;
				if (!isArray) {
					if (Current.Type != JsonTokenType.String) {
						throw new JsonTextReaderException (this, "键名");
					}
					needReadValue = key (Regex.Unescape (Current.String));
					MoveNext ();
					if (Current.Type != JsonTokenType.Semicolon) {
						throw new JsonTextReaderException (this, JsonKeyword.Semicolon);
					}
					MoveNext ();
				}
				if (needReadValue) {
					readValue (i++);
					continue;
				}
				ConsumptionValue ();
			}
			if (isFirst) {
				throw new JsonTextReaderException (this, isArray ? "值" : "键名", isArray ? JsonKeyword.RightBracket : JsonKeyword.RightBrace);
			}
			throw new JsonTextReaderException (this, JsonKeyword.Comma, isArray ? JsonKeyword.RightBracket : JsonKeyword.RightBrace);
		}

		void ConsumptionValue () {
			ReadValue (
				null,
				() => ReadArray (i => ConsumptionValue ()),
				() => ReadObject (name => true, ConsumptionValue)
			);
		}

		#endregion

	}

}