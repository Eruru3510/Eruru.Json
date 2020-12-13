using System;
using System.IO;
using System.Text.RegularExpressions;
using Eruru.TextTokenizer;

namespace Eruru.Json {

	public class JsonTextReader : TextTokenizer<JsonTokenType>, IJsonReader {

		readonly JsonConfig Config;

		public JsonTextReader (TextReader textReader, JsonConfig config = null) : base (
			JsonTokenType.Unknown,
			JsonTokenType.Integer,
			JsonTokenType.Decimal,
			JsonTokenType.String
		) {
			if (textReader is null) {
				throw new ArgumentNullException (nameof (textReader));
			}
			TextReader = textReader;
			Config = config ?? JsonConfig.Default;
			Add (JsonKeyword.LeftBrace, JsonTokenType.LeftBrace);
			Add (JsonKeyword.RightBrace, JsonTokenType.RightBrace);
			Add (JsonKeyword.LeftBracket, JsonTokenType.LeftBracket);
			Add (JsonKeyword.RightBracket, JsonTokenType.RightBracket);
			Add (JsonKeyword.Comma, JsonTokenType.Comma);
			Add (JsonKeyword.Semicolon, JsonTokenType.Semicolon);
			Add (JsonKeyword.Null, JsonTokenType.Null);
			Add (JsonKeyword.True, JsonTokenType.True);
			Add (JsonKeyword.False, JsonTokenType.False);
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
					string text = (string)Current.Value;
					if (DateTime.TryParse (text, out DateTime dateTime)) {
						value?.Invoke (dateTime, JsonValueType.DateTime);
						return;
					}
					value?.Invoke (Regex.Unescape (text), JsonValueType.String);
					return;
				}
				case JsonTokenType.Null:
					value?.Invoke (null, JsonValueType.Null);
					return;
				case JsonTokenType.True:
					value?.Invoke (true, JsonValueType.Bool);
					return;
				case JsonTokenType.False:
					value?.Invoke (false, JsonValueType.Bool);
					return;
				case JsonTokenType.LeftBracket:
					readArray ();
					return;
				case JsonTokenType.LeftBrace:
					readObject ();
					return;
			}
			throw new JsonTextReaderException (
				Buffer,
				Current,
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
				throw new JsonTextReaderException (Buffer, Current, isArray ? JsonKeyword.LeftBracket : JsonKeyword.LeftBrace);
			}
			JsonTokenType end = isArray ? JsonTokenType.RightBracket : JsonTokenType.RightBrace;
			bool isFirst = true;
			for (int i = 0; MoveNext (); i++) {
				if (Current.Type == end) {
					return;
				}
				if (isFirst) {
					isFirst = false;
				} else {
					if (Current.Type != JsonTokenType.Comma) {
						throw new JsonTextReaderException (Buffer, Current, JsonKeyword.Comma);
					}
					MoveNext ();
				}
				bool needReadValue = true;
				if (!isArray) {
					if (Current.Type != JsonTokenType.String) {
						throw new JsonTextReaderException (Buffer, Current, JsonTokenType.String);
					}
					needReadValue = key ((string)Current.Value);
					MoveNext ();
					if (Current.Type != JsonTokenType.Semicolon) {
						throw new JsonTextReaderException (Buffer, Current, JsonKeyword.Semicolon);
					}
					MoveNext ();
				}
				if (needReadValue) {
					readValue (i);
					continue;
				}
				ConsumptionValue ();
			}
			throw new JsonTextReaderException (Buffer, Current, isArray ? JsonKeyword.RightBracket : JsonKeyword.RightBrace);
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