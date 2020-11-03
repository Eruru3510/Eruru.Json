using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Eruru.Json {

	public class JsonTextReader : IDisposable, IEnumerable<JsonToken>, IEnumerator<JsonToken>, IJsonReader {

		public int BufferLength { get; set; } = 100;

		readonly TextReader TextReader;
		readonly Queue<int> Buffer = new Queue<int> ();
		int Index;

		public JsonTextReader (TextReader textReader) {
			TextReader = textReader ?? throw new ArgumentNullException (nameof (textReader));
		}

		int Read () {
			Buffer.Enqueue (TextReader.Read ());
			if (Buffer.Count > BufferLength) {
				Buffer.Dequeue ();
			}
			Index++;
			return Buffer.Peek ();
		}

		string ReadString (char end) {
			StringBuilder stringBuilder = new StringBuilder ();
			while (TextReader.Peek () > -1) {
				char character = (char)TextReader.Peek ();
				if (character == JsonKeyword.Escape) {
					stringBuilder.Append (character);
					Read ();
					if (TextReader.Peek () > -1) {
						stringBuilder.Append ((char)TextReader.Peek ());
						Read ();
					}
					continue;
				}
				if (character == end) {
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
			throw new JsonTokenReaderException ("String has no closing quotation marks", Buffer, token);
		}

		string ReadNumber (out bool isFloat) {
			isFloat = false;
			StringBuilder stringBuilder = new StringBuilder ();
			while (TextReader.Peek () > -1) {
				char character = (char)TextReader.Peek ();
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
				char character = (char)TextReader.Peek ();
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
					NeedMoveNext = false;
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
			while (TextReader.Peek () > -1) {
				char character = (char)TextReader.Peek ();
				if (char.IsWhiteSpace (character)) {
					Read ();
					continue;
				}
				JsonToken token = new JsonToken {
					Type = JsonTokenType.Unknown,
					Index = Index
				};
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
						Read ();
						text = ReadString (character);
						token.Type = JsonTokenType.String;
						token.Length = text.Length + 2;
						token.Value = JsonAPI.Escape (text);
						Read ();
						break;
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
				}
				if (token.Type != JsonTokenType.Unknown) {
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
			return false;
		}

		public void Reset () {
			throw new JsonException ($"{nameof (TextReader)} cannot {nameof (Reset)}");
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
			if (JsonAPI.HasFlag (Current.Type, JsonTokenType.Value)) {
				if (Current.Type == JsonTokenType.String && DateTime.TryParse (Current.Value.ToString (), out DateTime dateTime)) {
					value?.Invoke (dateTime, JsonValueType.DateTime);
					return;
				}
				value?.Invoke (Current.Value, JsonAPI.TokenTypeToValueType (Current.Type));
				return;
			}
			throw new JsonTokenReaderException (
				$"The expectation is {JsonTokenType.Value} or {JsonTokenType.LeftBracket} or {JsonTokenType.LeftBrace}",
				Buffer,
				Current
			);
		}

		public void ReadArray (JsonAction readValue) {
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
			Read (false, key, readValue);
		}

		void Read (bool isArray, JsonFunc<string, bool> key, JsonAction readValue) {
			if (!isArray && key is null) {
				throw new ArgumentNullException (nameof (key));
			}
			if (readValue is null) {
				throw new ArgumentNullException (nameof (readValue));
			}
			JsonTokenType start = isArray ? JsonTokenType.LeftBracket : JsonTokenType.LeftBrace;
			if (Current.Type != start) {
				throw new JsonTokenReaderException ($"The expectation is {start}", Buffer, Current);
			}
			JsonTokenType end = isArray ? JsonTokenType.RightBracket : JsonTokenType.RightBrace;
			bool isFirst = true;
			while (MoveNext ()) {
				if (Current.Type == end) {
					return;
				}
				if (isFirst) {
					isFirst = false;
				} else {
					if (Current.Type != JsonTokenType.Comma) {
						throw new JsonTokenReaderException ($"The expectation is {JsonTokenType.Comma}", Buffer, Current);
					}
					MoveNext ();
				}
				bool needReadValue = true;
				if (!isArray) {
					if (Current.Type != JsonTokenType.String) {
						throw new JsonTokenReaderException ($"The expectation is {JsonTokenType.String}", Buffer, Current);
					}
					needReadValue = key ((string)Current.Value);
					MoveNext ();
					if (Current.Type != JsonTokenType.Semicolon) {
						throw new JsonTokenReaderException ($"The expectation is {JsonTokenType.Semicolon}", Buffer, Current);
					}
					MoveNext ();
				}
				if (needReadValue) {
					readValue ();
					continue;
				}
				ConsumptionValue ();
			}
			throw new JsonTokenReaderException ($"The expectation is {end}", Buffer, Current);
		}

		void ConsumptionValue () {
			ReadValue (
				null,
				() => ReadArray (ConsumptionValue),
				() => ReadObject (name => true, ConsumptionValue)
			);
		}

		#endregion

	}

}