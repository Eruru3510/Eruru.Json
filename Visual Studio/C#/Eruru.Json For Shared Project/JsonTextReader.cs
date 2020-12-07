using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Eruru.Json {

	public class JsonTextReader : IDisposable, IEnumerable<JsonToken>, IEnumerator<JsonToken>, IJsonReader {

		public Queue<int> Buffer { get; } = new Queue<int> ();
		public int BufferLength { get; set; } = 500;

		readonly TextReader TextReader;
		readonly JsonConfig Config;
		int Index;

		public JsonTextReader (TextReader textReader, JsonConfig config = null) {
			TextReader = textReader ?? throw new ArgumentNullException (nameof (textReader));
			Config = config;
		}

		int Read () {
			while (Buffer.Count >= BufferLength) {
				Buffer.Dequeue ();
			}
			Buffer.Enqueue (TextReader.Read ());
			Index++;
			return Buffer.Peek ();
		}

		char Peek () {
			return (char)TextReader.Peek ();
		}

		char SkipWhiteSpace () {
			while (TextReader.Peek () > -1) {
				char character = Peek ();
				if (char.IsWhiteSpace (character)) {
					Read ();
					continue;
				}
				return character;
			}
			return Peek ();
		}

		string ReadString (char end) {
			Read ();
			StringBuilder stringBuilder = new StringBuilder ();
			while (TextReader.Peek () > -1) {
				char character = Peek ();
				if (character == JsonKeyword.Backslash) {
					stringBuilder.Append (character);
					Read ();
					if (TextReader.Peek () > -1) {
						stringBuilder.Append (Peek ());
						Read ();
					}
					continue;
				}
				if (character == end) {
					Read ();
					return stringBuilder.ToString ();
				}
				stringBuilder.Append (character);
				Read ();
			}
			JsonToken token = new JsonToken {
				Type = JsonTokenType.String,
				Index = Current.Index + Current.Length,
				Length = stringBuilder.Length,
				Value = stringBuilder.ToString ()
			};
			throw new JsonTextReaderException ("字符串没有引号结束", Buffer, token);
		}

		string ReadNumber (out bool isFloat) {
			isFloat = false;
			StringBuilder stringBuilder = new StringBuilder ();
			while (TextReader.Peek () > -1) {
				char character = Peek ();
				if (!Array.Exists (JsonKeyword.Numbers, value => character == value)) {
					break;
				}
				if (!isFloat && character == JsonKeyword.Dot) {
					isFloat = true;
				}
				stringBuilder.Append (character);
				Read ();
			}
			return stringBuilder.ToString ();
		}

		string ReadValue () {
			StringBuilder stringBuilder = new StringBuilder ();
			while (TextReader.Peek () > -1) {
				char character = Peek ();
				if (!char.IsLetter (character)) {
					break;
				}
				stringBuilder.Append (character);
				Read ();
			}
			return stringBuilder.ToString ();
		}

		#region IDisposable

		public void Dispose () {
			TextReader.Dispose ();
		}

		#endregion

		#region IEnumerable<JsonToken>

		public IEnumerator<JsonToken> GetEnumerator () {
			return this;
		}

		IEnumerator IEnumerable.GetEnumerator () {
			return this;
		}

		#endregion

		#region IEnumerator<JsonToken>

		public JsonToken Current {

			get {
				if (NeedMoveNext) {
					MoveNext ();
				}
				return _Current;
			}

		}

		object IEnumerator.Current {

			get => Current;

		}

		JsonToken _Current;
		bool NeedMoveNext = true;

		public bool MoveNext () {
			NeedMoveNext = false;
			JsonToken token = new JsonToken () {
				Type = JsonTokenType.Unknown,
				Index = Index
			};
			while (TextReader.Peek () > -1) {
				char character = SkipWhiteSpace ();
				switch (character) {
					case JsonKeyword.Comma:
						token.Type = JsonTokenType.Comma;
						break;
					case JsonKeyword.Semicolon:
						token.Type = JsonTokenType.Semicolon;
						break;
					case JsonKeyword.LeftBracket:
						token.Type = JsonTokenType.LeftBracket;
						break;
					case JsonKeyword.RightBracket:
						token.Type = JsonTokenType.RightBracket;
						break;
					case JsonKeyword.LeftBrace:
						token.Type = JsonTokenType.LeftBrace;
						break;
					case JsonKeyword.RightBrace:
						token.Type = JsonTokenType.RightBrace;
						break;
					case JsonKeyword.Dot:
						token.Type = JsonTokenType.Dot;
						break;
				}
				if (token.Type != JsonTokenType.Unknown) {
					token.Length = 1;
					token.Value = character;
					_Current = token;
					Read ();
					return true;
				}
				string text;
				switch (character) {
					case JsonKeyword.SingleQuot:
					case JsonKeyword.DoubleQuot:
						text = ReadString (character);
						token.Type = JsonTokenType.String;
						token.Length = text.Length + 2;
						token.Value = Regex.Unescape (text);
						_Current = token;
						return true;
				}
				if (Array.Exists (JsonKeyword.Numbers, value => character == value)) {
					text = ReadNumber (out bool isFloat);
					if (isFloat) {
						if (decimal.TryParse (text, out decimal result)) {
							token.Type = JsonTokenType.Decimal;
							token.Value = result;
						}
					} else {
						if (long.TryParse (text, out long result)) {
							token.Type = JsonTokenType.Long;
							token.Value = result;
						}
					}
					token.Length = text.Length;
					if (token.Type == JsonTokenType.Unknown) {
						token.Value = text;
						_Current = token;
						return true;
					}
					_Current = token;
					return true;
				}
				text = ReadValue ();
				token.Length = text.Length;
				switch (text) {
					case JsonKeyword.Null:
						token.Type = JsonTokenType.Null;
						break;
					case JsonKeyword.True:
						token.Type = JsonTokenType.Bool;
						token.Value = true;
						break;
					case JsonKeyword.False:
						token.Type = JsonTokenType.Bool;
						token.Value = false;
						break;
				}
				if (token.Type == JsonTokenType.Unknown) {
					token.Value = text;
				}
				_Current = token;
				return true;
			}
			_Current = token;
			return false;
		}

		public void Reset () {
			throw new JsonException ($"{nameof (TextReader)}无法{nameof (Reset)}");
		}

		#endregion

		#region IJsonReader

		public void ReadValue (JsonAction<object, JsonValueType> value, JsonAction readArray, JsonAction readObject) {
			if (readArray is null) {
				throw new ArgumentNullException (nameof (readArray));
			}
			if (readObject is null) {
				throw new ArgumentNullException (nameof (readObject));
			}
			switch (Current.Type) {
				case JsonTokenType.LeftBracket:
					readArray ();
					return;
				case JsonTokenType.LeftBrace:
					readObject ();
					return;
			}
			if (JsonApi.HasFlag (Current.Type, JsonTokenType.Value)) {
				if (Current.Type == JsonTokenType.String && DateTime.TryParse (Current.Value.ToString (), out DateTime dateTime)) {
					value?.Invoke (dateTime, JsonValueType.DateTime);
					return;
				}
				value?.Invoke (Current.Value, JsonApi.TokenTypeToValueType (Current.Type));
				return;
			}
			throw new JsonTextReaderException (
				Buffer,
				Current,
				JsonTokenType.Null, JsonTokenType.Long, JsonTokenType.Decimal, JsonTokenType.Bool, JsonTokenType.String,
				JsonTokenType.LeftBracket, JsonTokenType.LeftBrace
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
				throw new JsonTextReaderException (Buffer, Current, start);
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
						throw new JsonTextReaderException (Buffer, Current, JsonTokenType.Comma);
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
						throw new JsonTextReaderException (Buffer, Current, JsonTokenType.Semicolon);
					}
					MoveNext ();
				}
				if (needReadValue) {
					readValue (i);
					continue;
				}
				ConsumptionValue ();
			}
			throw new JsonTextReaderException (Buffer, Current, end);
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