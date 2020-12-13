using System;
using System.Collections;
using System.IO;
using Eruru.TextTokenizer;

namespace Eruru.Json {

	public class JsonValue : IJsonSerializable, IEnumerable, IConvertible, IComparable, IJsonArray, IJsonObject {

		public JsonValueType Type {

			get => _Type;

			set {
				if (_Type == value) {
					return;
				}
				switch (value) {
					case JsonValueType.Null:
						_Value = null;
						break;
					case JsonValueType.Integer:
						_Value = ToLong ();
						break;
					case JsonValueType.Decimal:
						_Value = ToDecimal ();
						break;
					case JsonValueType.Bool:
						_Value = ToBool ();
						break;
					case JsonValueType.String:
						_Value = ToString ();
						break;
					case JsonValueType.DateTime:
						_Value = ToDateTime ();
						break;
					case JsonValueType.Array:
						_Value = GetArray ();
						break;
					case JsonValueType.Object:
						_Value = GetObject ();
						break;
					default:
						throw new JsonNotSupportException (value);
				}
				_Type = value;
			}

		}
		public object Value {

			get => _Value;

			set {
				if (JsonApi.TryGetValueType (value, out JsonValueType valueType)) {
					_Type = valueType;
					_Value = value;
					return;
				}
				if (value is JsonArray) {
					_Type = JsonValueType.Array;
					_Value = value;
					return;
				}
				if (value is JsonObject) {
					_Type = JsonValueType.Object;
					_Value = value;
					return;
				}
				if (value is JsonValue jsonValue) {
					Value = jsonValue.Value;
					return;
				}
				throw new JsonNotSupportException (value);
			}

		}
		public byte Byte {

			get => ToByte ();

			set {
				_Type = JsonValueType.Integer;
				_Value = value;
			}

		}
		public ushort UShort {

			get => ToUShort ();

			set {
				_Type = JsonValueType.Integer;
				_Value = value;
			}

		}
		public uint UInt {

			get => ToUInt ();

			set {
				_Type = JsonValueType.Integer;
				_Value = value;
			}

		}
		public ulong ULong {

			get => ToULong ();

			set {
				_Type = JsonValueType.Integer;
				_Value = value;
			}

		}
		public sbyte SByte {

			get => ToSByte ();

			set {
				_Type = JsonValueType.Integer;
				_Value = value;
			}

		}
		public short Short {

			get => ToShort ();

			set {
				_Type = JsonValueType.Integer;
				_Value = value;
			}

		}
		public int Int {

			get => ToInt ();

			set {
				_Type = JsonValueType.Integer;
				_Value = value;
			}

		}
		public long Long {

			get => ToLong ();

			set {
				_Type = JsonValueType.Integer;
				_Value = value;
			}

		}
		public float Float {

			get => ToFloat ();

			set {
				_Type = JsonValueType.Decimal;
				_Value = value;
			}

		}
		public double Double {

			get => ToDouble ();

			set {
				_Type = JsonValueType.Decimal;
				_Value = value;
			}

		}
		public decimal Decimal {

			get => ToDecimal ();

			set {
				_Type = JsonValueType.Decimal;
				_Value = value;
			}

		}
		public bool Bool {

			get => ToBool ();

			set {
				_Type = JsonValueType.Bool;
				_Value = value;
			}

		}
		public char Char {

			get => ToChar ();

			set {
				_Type = JsonValueType.String;
				_Value = value;
			}

		}
		public string String {

			get => ToString ();

			set {
				_Type = value is null ? JsonValueType.Null : JsonValueType.String;
				_Value = value;
			}

		}
		public DateTime DateTime {

			get => ToDateTime ();

			set {
				_Type = JsonValueType.DateTime;
				_Value = value;
			}

		}
		public JsonArray Array {

			get => GetArray ();

			set {
				_Type = value is null ? JsonValueType.Null : JsonValueType.Array;
				_Value = value;
			}

		}
		public JsonObject Object {

			get => GetObject ();

			set {
				_Type = value is null ? JsonValueType.Null : JsonValueType.Object;
				_Value = value;
			}

		}

		internal JsonValueType _Type;
		internal object _Value;

		public JsonValue () {
			_Type = JsonValueType.Null;
		}
		public JsonValue (object value) {
			Value = value;
		}
		public JsonValue (byte value) {
			Byte = value;
		}
		public JsonValue (ushort value) {
			UShort = value;
		}
		public JsonValue (uint value) {
			UInt = value;
		}
		public JsonValue (ulong value) {
			ULong = value;
		}
		public JsonValue (sbyte value) {
			SByte = value;
		}
		public JsonValue (short value) {
			Short = value;
		}
		public JsonValue (int value) {
			Int = value;
		}
		public JsonValue (long value) {
			Long = value;
		}
		public JsonValue (float value) {
			Float = value;
		}
		public JsonValue (double value) {
			Double = value;
		}
		public JsonValue (decimal value) {
			Decimal = value;
		}
		public JsonValue (bool value) {
			Bool = value;
		}
		public JsonValue (char value) {
			Char = value;
		}
		public JsonValue (string value) {
			String = value;
		}
		public JsonValue (DateTime value) {
			DateTime = value;
		}
		public JsonValue (JsonArray value) {
			Array = value;
		}
		public JsonValue (JsonObject value) {
			Object = value;
		}

		internal JsonValue (object value, JsonValueType valueType) {
			_Value = value;
			_Type = valueType;
		}

		public static JsonValue Parse (string text, JsonConfig config = null) {
			if (text is null) {
				throw new ArgumentNullException (nameof (text));
			}
			return Load (new StringReader (text), null, config);
		}
		public static JsonValue Parse (string text, JsonValue value, JsonConfig config = null) {
			if (text is null) {
				throw new ArgumentNullException (nameof (text));
			}
			return Load (new StringReader (text), value, config);
		}

		public static JsonValue Load (string path, JsonConfig config = null) {
			if (JsonApi.IsNullOrWhiteSpace (path)) {
				throw new ArgumentException ($"“{nameof (path)}”不能是 Null 或空白", nameof (path));
			}
			return Load (new StreamReader (path), null, config);
		}
		public static JsonValue Load (TextReader textReader, JsonConfig config = null) {
			if (textReader is null) {
				throw new ArgumentNullException (nameof (textReader));
			}
			return Load (textReader, null, config);
		}
		public static JsonValue Load (string path, JsonValue value, JsonConfig config = null) {
			if (JsonApi.IsNullOrWhiteSpace (path)) {
				throw new ArgumentException ($"“{nameof (path)}”不能是 Null 或空白", nameof (path));
			}
			return Load (new StreamReader (path), value, config);
		}
		public static JsonValue Load (TextReader textReader, JsonValue value, JsonConfig config = null) {
			if (textReader is null) {
				throw new ArgumentNullException (nameof (textReader));
			}
			using (JsonTextReader reader = new JsonTextReader (textReader, config)) {
				return new JsonValueBuilder (reader, config).BuildValue (value);
			}
		}

		public byte ToByte (byte defaultValue = default) {
			try {
				return Convert.ToByte (Value);
			} catch {
				return defaultValue;
			}
		}
		public ushort ToUShort (ushort defaultValue = default) {
			try {
				return Convert.ToUInt16 (Value);
			} catch {
				return defaultValue;
			}
		}
		public uint ToUInt (uint defaultValue = default) {
			try {
				return Convert.ToUInt32 (Value);
			} catch {
				return defaultValue;
			}
		}
		public ulong ToULong (ulong defaultValue = default) {
			try {
				return Convert.ToUInt64 (Value);
			} catch {
				return defaultValue;
			}
		}
		public sbyte ToSByte (sbyte defaultValue = default) {
			try {
				return Convert.ToSByte (Value);
			} catch {
				return defaultValue;
			}
		}
		public short ToShort (short defaultValue = default) {
			try {
				return Convert.ToInt16 (Value);
			} catch {
				return defaultValue;
			}
		}
		public int ToInt (int defaultValue = default) {
			try {
				return Convert.ToInt32 (Value);
			} catch {
				return defaultValue;
			}
		}
		public long ToLong (long defaultValue = default) {
			try {
				return Convert.ToInt64 (Value);
			} catch {
				return defaultValue;
			}
		}
		public float ToFloat (float defaultValue = default) {
			try {
				return Convert.ToSingle (Value);
			} catch {
				return defaultValue;
			}
		}
		public double ToDouble (double defaultValue = default) {
			try {
				return Convert.ToDouble (Value);
			} catch {
				return defaultValue;
			}
		}
		public decimal ToDecimal (decimal defaultValue = default) {
			try {
				return Convert.ToDecimal (Value);
			} catch {
				return defaultValue;
			}
		}
		public bool ToBool (bool defaultValue = default) {
			try {
				return Convert.ToBoolean (Value);
			} catch {
				return defaultValue;
			}
		}
		public char ToChar (char defaultValue = default) {
			try {
				return Convert.ToChar (Value);
			} catch {
				return defaultValue;
			}
		}
		public string ToString (string defaultValue = default) {
			try {
				return Convert.ToString (Value);
			} catch {
				return defaultValue;
			}
		}
		public DateTime ToDateTime (DateTime defaultValue = default) {
			try {
				return Convert.ToDateTime (Value);
			} catch {
				return defaultValue;
			}
		}
		public JsonArray ToArray (JsonArray defaultValue = default) {
			return Value as JsonArray ?? defaultValue;
		}
		public JsonObject ToObject (JsonObject defaultValue = default) {
			return Value as JsonObject ?? defaultValue;
		}

		public JsonValue Select (string path) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			return Select (this, path);
		}

		public static JsonValue Select (JsonValue value, string path) {
			if (value is null) {
				throw new ArgumentNullException (nameof (value));
			}
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			JsonValue current = value;
			using (var reader = new TextTokenizer<JsonTokenType> (
				new StringReader (path),
				JsonTokenType.Unknown,
				JsonTokenType.Integer,
				JsonTokenType.Decimal,
				JsonTokenType.String
			) {
				{ JsonKeyword.Dot, JsonTokenType.Dot },
				{ JsonKeyword.LeftBracket, JsonTokenType.LeftBracket },
				{ JsonKeyword.RightBracket, JsonTokenType.RightBracket }
			}) {
				while (reader.MoveNext ()) {
					switch (reader.Current.Type) {
						case JsonTokenType.Unknown:
						case JsonTokenType.Dot:
							if (reader.Current.Type == JsonTokenType.Dot) {
								reader.MoveNext ();
							}
							current = current[Convert.ToString (reader.Current.Value)];
							continue;
						case JsonTokenType.LeftBracket:
							reader.MoveNext ();
							switch (reader.Current.Type) {
								case JsonTokenType.Integer:
									current = current[Convert.ToInt32 (reader.Current.Value)];
									break;
								default:
									throw new JsonTextReaderException (reader.Buffer, reader.Current, "整数");
							}
							reader.MoveNext ();
							if (reader.Current.Type != JsonTokenType.RightBracket) {
								throw new JsonTextReaderException (reader.Buffer, reader.Current, JsonKeyword.RightBracket);
							}
							continue;
					}
					throw new JsonTextReaderException (reader.Buffer, reader.Current, "键名", JsonKeyword.Dot, JsonKeyword.LeftBracket);
				}
			}
			return current;
		}

		JsonArray GetArray () {
			JsonArray array = ToArray ();
			if (array is null) {
				array = new JsonArray ();
				_Type = JsonValueType.Array;
				_Value = array;
			}
			return array;
		}

		JsonObject GetObject () {
			JsonObject jsonObject = ToObject ();
			if (jsonObject is null) {
				jsonObject = new JsonObject ();
				_Type = JsonValueType.Object;
				_Value = jsonObject;
			}
			return jsonObject;
		}

		#region Override

		public override bool Equals (object obj) {
			if (obj is JsonValue value) {
				return Equals (value.Value);
			}
			if (obj is JsonArray array) {
				return ToArray ().Equals (array);
			}
			if (obj is JsonObject jsonObject) {
				return ToObject ().Equals (jsonObject);
			}
			if (JsonApi.TryGetValueType (obj, out JsonValueType valueType)) {
				switch (valueType) {
					case JsonValueType.Null:
						return Value == obj;
					case JsonValueType.Integer:
						return ToLong ().Equals (Convert.ToInt64 (obj));
					case JsonValueType.Decimal:
						return ToDecimal ().Equals (Convert.ToDecimal (obj));
					case JsonValueType.Bool:
						return ToBool ().Equals ((bool)obj);
					case JsonValueType.String:
						if (obj is char character) {
							return ToChar ().Equals (character);
						}
						return ToString ().Equals ((string)obj);
					case JsonValueType.DateTime:
						return ToDateTime ().Equals ((DateTime)obj);
				}
			}
			return false;
		}

		public override int GetHashCode () {
			return Value?.GetHashCode () ?? default;
		}

		public override string ToString () {
			return Serialize ();
		}

		#endregion

		#region Implicit Operator

		public static implicit operator byte (JsonValue value) {
			return value?.ToByte () ?? default;
		}
		public static implicit operator ushort (JsonValue value) {
			return value?.ToUShort () ?? default;
		}
		public static implicit operator uint (JsonValue value) {
			return value?.ToUInt () ?? default;
		}
		public static implicit operator ulong (JsonValue value) {
			return value?.ToULong () ?? default;
		}
		public static implicit operator sbyte (JsonValue value) {
			return value?.ToSByte () ?? default;
		}
		public static implicit operator short (JsonValue value) {
			return value?.ToShort () ?? default;
		}
		public static implicit operator int (JsonValue value) {
			return value?.ToInt () ?? default;
		}
		public static implicit operator long (JsonValue value) {
			return value?.ToLong () ?? default;
		}
		public static implicit operator float (JsonValue value) {
			return value?.ToFloat () ?? default;
		}
		public static implicit operator double (JsonValue value) {
			return value?.ToDouble () ?? default;
		}
		public static implicit operator decimal (JsonValue value) {
			return value?.ToDecimal () ?? default;
		}
		public static implicit operator bool (JsonValue value) {
			return value?.ToBool () ?? default;
		}
		public static implicit operator char (JsonValue value) {
			return value?.ToChar () ?? default;
		}
		public static implicit operator string (JsonValue value) {
			return value?.ToString () ?? default;
		}
		public static implicit operator DateTime (JsonValue value) {
			return value?.ToDateTime () ?? default;
		}
		public static implicit operator JsonArray (JsonValue value) {
			return value?.GetArray () ?? default;
		}
		public static implicit operator JsonObject (JsonValue value) {
			return value?.GetObject () ?? default;
		}

		public static implicit operator JsonValue (byte value) {
			return new JsonValue (value);
		}
		public static implicit operator JsonValue (ushort value) {
			return new JsonValue (value);
		}
		public static implicit operator JsonValue (uint value) {
			return new JsonValue (value);
		}
		public static implicit operator JsonValue (ulong value) {
			return new JsonValue (value);
		}
		public static implicit operator JsonValue (sbyte value) {
			return new JsonValue (value);
		}
		public static implicit operator JsonValue (short value) {
			return new JsonValue (value);
		}
		public static implicit operator JsonValue (int value) {
			return new JsonValue (value);
		}
		public static implicit operator JsonValue (long value) {
			return new JsonValue (value);
		}
		public static implicit operator JsonValue (float value) {
			return new JsonValue (value);
		}
		public static implicit operator JsonValue (double value) {
			return new JsonValue (value);
		}
		public static implicit operator JsonValue (decimal value) {
			return new JsonValue (value);
		}
		public static implicit operator JsonValue (bool value) {
			return new JsonValue (value);
		}
		public static implicit operator JsonValue (char value) {
			return new JsonValue (value);
		}
		public static implicit operator JsonValue (string value) {
			return new JsonValue (value);
		}
		public static implicit operator JsonValue (DateTime value) {
			return new JsonValue (value);
		}
		public static implicit operator JsonValue (JsonArray value) {
			return new JsonValue (value);
		}
		public static implicit operator JsonValue (JsonObject value) {
			return new JsonValue (value);
		}

		#endregion

		#region Operator ==

		public static bool operator == (JsonValue a, byte b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.Equals (b);
		}
		public static bool operator == (JsonValue a, ushort b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.Equals (b);
		}
		public static bool operator == (JsonValue a, uint b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.Equals (b);
		}
		public static bool operator == (JsonValue a, ulong b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.Equals (b);
		}
		public static bool operator == (JsonValue a, sbyte b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.Equals (b);
		}
		public static bool operator == (JsonValue a, short b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.Equals (b);
		}
		public static bool operator == (JsonValue a, int b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.Equals (b);
		}
		public static bool operator == (JsonValue a, long b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.Equals (b);
		}
		public static bool operator == (JsonValue a, float b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.Equals (b);
		}
		public static bool operator == (JsonValue a, double b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.Equals (b);
		}
		public static bool operator == (JsonValue a, decimal b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.Equals (b);
		}
		public static bool operator == (JsonValue a, bool b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.Equals (b);
		}
		public static bool operator == (JsonValue a, char b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.Equals (b);
		}
		public static bool operator == (JsonValue a, string b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.Equals (b);
		}
		public static bool operator == (JsonValue a, DateTime b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.Equals (b);
		}
		public static bool operator == (JsonValue a, JsonValue b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return a.Equals (b);
		}
		public static bool operator == (JsonValue a, JsonArray b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return a.Equals (b);
		}
		public static bool operator == (JsonValue a, JsonObject b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return a.Equals (b);
		}

		public static bool operator == (byte a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return b.Equals (a);
		}
		public static bool operator == (ushort a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return b.Equals (a);
		}
		public static bool operator == (uint a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return b.Equals (a);
		}
		public static bool operator == (ulong a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return b.Equals (a);
		}
		public static bool operator == (sbyte a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return b.Equals (a);
		}
		public static bool operator == (short a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return b.Equals (a);
		}
		public static bool operator == (int a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return b.Equals (a);
		}
		public static bool operator == (long a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return b.Equals (a);
		}
		public static bool operator == (float a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return b.Equals (a);
		}
		public static bool operator == (double a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return b.Equals (a);
		}
		public static bool operator == (decimal a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return b.Equals (a);
		}
		public static bool operator == (bool a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return b.Equals (a);
		}
		public static bool operator == (char a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return b.Equals (a);
		}
		public static bool operator == (string a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return b.Equals (a);
		}
		public static bool operator == (DateTime a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return b.Equals (a);
		}
		public static bool operator == (JsonArray a, JsonValue b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return b.Equals (a);
		}
		public static bool operator == (JsonObject a, JsonValue b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return b.Equals (a);
		}

		#endregion

		#region Operator !=

		public static bool operator != (JsonValue a, byte b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return !a.Equals (b);
		}
		public static bool operator != (JsonValue a, ushort b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return !a.Equals (b);
		}
		public static bool operator != (JsonValue a, uint b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return !a.Equals (b);
		}
		public static bool operator != (JsonValue a, ulong b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return !a.Equals (b);
		}
		public static bool operator != (JsonValue a, sbyte b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return !a.Equals (b);
		}
		public static bool operator != (JsonValue a, short b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return !a.Equals (b);
		}
		public static bool operator != (JsonValue a, int b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return !a.Equals (b);
		}
		public static bool operator != (JsonValue a, long b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return !a.Equals (b);
		}
		public static bool operator != (JsonValue a, float b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return !a.Equals (b);
		}
		public static bool operator != (JsonValue a, double b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return !a.Equals (b);
		}
		public static bool operator != (JsonValue a, decimal b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return !a.Equals (b);
		}
		public static bool operator != (JsonValue a, bool b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return !a.Equals (b);
		}
		public static bool operator != (JsonValue a, char b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return !a.Equals (b);
		}
		public static bool operator != (JsonValue a, string b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return !a.Equals (b);
		}
		public static bool operator != (JsonValue a, DateTime b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return !a.Equals (b);
		}
		public static bool operator != (JsonValue a, JsonValue b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return !a.Equals (b);
		}
		public static bool operator != (JsonValue a, JsonArray b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return !a.Equals (b);
		}
		public static bool operator != (JsonValue a, JsonObject b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return !a.Equals (b);
		}

		public static bool operator != (byte a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return !b.Equals (a);
		}
		public static bool operator != (ushort a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return !b.Equals (a);
		}
		public static bool operator != (uint a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return !b.Equals (a);
		}
		public static bool operator != (ulong a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return !b.Equals (a);
		}
		public static bool operator != (sbyte a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return !b.Equals (a);
		}
		public static bool operator != (short a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return !b.Equals (a);
		}
		public static bool operator != (int a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return !b.Equals (a);
		}
		public static bool operator != (long a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return !b.Equals (a);
		}
		public static bool operator != (float a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return !b.Equals (a);
		}
		public static bool operator != (double a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return !b.Equals (a);
		}
		public static bool operator != (decimal a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return !b.Equals (a);
		}
		public static bool operator != (bool a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return !b.Equals (a);
		}
		public static bool operator != (char a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return !b.Equals (a);
		}
		public static bool operator != (string a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return !b.Equals (a);
		}
		public static bool operator != (DateTime a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return !b.Equals (a);
		}
		public static bool operator != (JsonArray a, JsonValue b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return !b.Equals (a);
		}
		public static bool operator != (JsonObject a, JsonValue b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return !b.Equals (a);
		}

		#endregion

		#region Operator >

		public static bool operator > (JsonValue a, byte b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.CompareTo (b) > 0;
		}
		public static bool operator > (JsonValue a, ushort b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.CompareTo (b) > 0;
		}
		public static bool operator > (JsonValue a, uint b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.CompareTo (b) > 0;
		}
		public static bool operator > (JsonValue a, ulong b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.CompareTo (b) > 0;
		}
		public static bool operator > (JsonValue a, sbyte b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.CompareTo (b) > 0;
		}
		public static bool operator > (JsonValue a, short b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.CompareTo (b) > 0;
		}
		public static bool operator > (JsonValue a, int b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.CompareTo (b) > 0;
		}
		public static bool operator > (JsonValue a, long b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.CompareTo (b) > 0;
		}
		public static bool operator > (JsonValue a, float b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.CompareTo (b) > 0;
		}
		public static bool operator > (JsonValue a, double b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.CompareTo (b) > 0;
		}
		public static bool operator > (JsonValue a, decimal b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.CompareTo (b) > 0;
		}
		public static bool operator > (JsonValue a, bool b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.CompareTo (b) > 0;
		}
		public static bool operator > (JsonValue a, char b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.CompareTo (b) > 0;
		}
		public static bool operator > (JsonValue a, string b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.CompareTo (b) > 0;
		}
		public static bool operator > (JsonValue a, DateTime b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.CompareTo (b) > 0;
		}
		public static bool operator > (JsonValue a, JsonValue b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return a.CompareTo (b) > 0;
		}

		public static bool operator > (byte a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return b.CompareTo (a) < 0;
		}
		public static bool operator > (ushort a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return b.CompareTo (a) < 0;
		}
		public static bool operator > (uint a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return b.CompareTo (a) < 0;
		}
		public static bool operator > (ulong a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return b.CompareTo (a) < 0;
		}
		public static bool operator > (sbyte a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return b.CompareTo (a) < 0;
		}
		public static bool operator > (short a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return b.CompareTo (a) < 0;
		}
		public static bool operator > (int a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return b.CompareTo (a) < 0;
		}
		public static bool operator > (long a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return b.CompareTo (a) < 0;
		}
		public static bool operator > (float a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return b.CompareTo (a) < 0;
		}
		public static bool operator > (double a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return b.CompareTo (a) < 0;
		}
		public static bool operator > (decimal a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return b.CompareTo (a) < 0;
		}
		public static bool operator > (bool a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return b.CompareTo (a) < 0;
		}
		public static bool operator > (char a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return b.CompareTo (a) < 0;
		}
		public static bool operator > (string a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return b.CompareTo (a) < 0;
		}
		public static bool operator > (DateTime a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return b.CompareTo (a) < 0;
		}

		#endregion

		#region Operator <

		public static bool operator < (JsonValue a, byte b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.CompareTo (b) < 0;
		}
		public static bool operator < (JsonValue a, ushort b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.CompareTo (b) < 0;
		}
		public static bool operator < (JsonValue a, uint b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.CompareTo (b) < 0;
		}
		public static bool operator < (JsonValue a, ulong b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.CompareTo (b) < 0;
		}
		public static bool operator < (JsonValue a, sbyte b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.CompareTo (b) < 0;
		}
		public static bool operator < (JsonValue a, short b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.CompareTo (b) < 0;
		}
		public static bool operator < (JsonValue a, int b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.CompareTo (b) < 0;
		}
		public static bool operator < (JsonValue a, long b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.CompareTo (b) < 0;
		}
		public static bool operator < (JsonValue a, float b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.CompareTo (b) < 0;
		}
		public static bool operator < (JsonValue a, double b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.CompareTo (b) < 0;
		}
		public static bool operator < (JsonValue a, decimal b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.CompareTo (b) < 0;
		}
		public static bool operator < (JsonValue a, bool b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.CompareTo (b) < 0;
		}
		public static bool operator < (JsonValue a, char b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.CompareTo (b) < 0;
		}
		public static bool operator < (JsonValue a, string b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.CompareTo (b) < 0;
		}
		public static bool operator < (JsonValue a, DateTime b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.CompareTo (b) < 0;
		}
		public static bool operator < (JsonValue a, JsonValue b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return a.CompareTo (b) < 0;
		}

		public static bool operator < (byte a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return b.CompareTo (a) > 0;
		}
		public static bool operator < (ushort a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return b.CompareTo (a) > 0;
		}
		public static bool operator < (uint a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return b.CompareTo (a) > 0;
		}
		public static bool operator < (ulong a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return b.CompareTo (a) > 0;
		}
		public static bool operator < (sbyte a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return b.CompareTo (a) > 0;
		}
		public static bool operator < (short a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return b.CompareTo (a) > 0;
		}
		public static bool operator < (int a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return b.CompareTo (a) > 0;
		}
		public static bool operator < (long a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return b.CompareTo (a) > 0;
		}
		public static bool operator < (float a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return b.CompareTo (a) > 0;
		}
		public static bool operator < (double a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return b.CompareTo (a) > 0;
		}
		public static bool operator < (decimal a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return b.CompareTo (a) > 0;
		}
		public static bool operator < (bool a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return b.CompareTo (a) > 0;
		}
		public static bool operator < (char a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return b.CompareTo (a) > 0;
		}
		public static bool operator < (string a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return b.CompareTo (a) > 0;
		}
		public static bool operator < (DateTime a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return b.CompareTo (a) > 0;
		}

		#endregion

		#region Operator +

		public static int operator + (JsonValue a, byte b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToByte () + b;
		}
		public static int operator + (JsonValue a, ushort b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToUShort () + b;
		}
		public static uint operator + (JsonValue a, uint b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToUInt () + b;
		}
		public static ulong operator + (JsonValue a, ulong b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToULong () + b;
		}
		public static int operator + (JsonValue a, sbyte b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToSByte () + b;
		}
		public static int operator + (JsonValue a, short b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToShort () + b;
		}
		public static int operator + (JsonValue a, int b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToInt () + b;
		}
		public static long operator + (JsonValue a, long b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToLong () + b;
		}
		public static float operator + (JsonValue a, float b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToFloat () + b;
		}
		public static double operator + (JsonValue a, double b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToDouble () + b;
		}
		public static decimal operator + (JsonValue a, decimal b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToDecimal () + b;
		}
		public static int operator + (JsonValue a, char b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToChar () + b;
		}
		public static string operator + (JsonValue a, string b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToString () + b;
		}

		public static int operator + (byte a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a + b.ToByte ();
		}
		public static int operator + (ushort a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a + b.ToUShort ();
		}
		public static uint operator + (uint a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a + b.ToUInt ();
		}
		public static ulong operator + (ulong a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a + b.ToULong ();
		}
		public static int operator + (sbyte a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a + b.ToSByte ();
		}
		public static int operator + (short a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a + b.ToShort ();
		}
		public static int operator + (int a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a + b.ToInt ();
		}
		public static long operator + (long a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a + b.ToLong ();
		}
		public static float operator + (float a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a + b.ToFloat ();
		}
		public static double operator + (double a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a + b.ToDouble ();
		}
		public static decimal operator + (decimal a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a + b.ToDecimal ();
		}
		public static bool operator + (bool a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a != b.Bool;
		}
		public static int operator + (char a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a + b.ToChar ();
		}
		public static string operator + (string a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a + b.ToString ();
		}

		#endregion

		#region Operator -

		public static int operator - (JsonValue a, byte b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToByte () - b;
		}
		public static int operator - (JsonValue a, ushort b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToUShort () - b;
		}
		public static uint operator - (JsonValue a, uint b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToUInt () - b;
		}
		public static ulong operator - (JsonValue a, ulong b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToULong () - b;
		}
		public static int operator - (JsonValue a, sbyte b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToSByte () - b;
		}
		public static int operator - (JsonValue a, short b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToShort () - b;
		}
		public static int operator - (JsonValue a, int b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToInt () - b;
		}
		public static long operator - (JsonValue a, long b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToLong () - b;
		}
		public static float operator - (JsonValue a, float b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToFloat () - b;
		}
		public static double operator - (JsonValue a, double b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToDouble () - b;
		}
		public static decimal operator - (JsonValue a, decimal b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToDecimal () - b;
		}
		public static int operator - (JsonValue a, char b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToChar () - b;
		}

		public static int operator - (byte a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a - b.ToByte ();
		}
		public static int operator - (ushort a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a - b.ToUShort ();
		}
		public static uint operator - (uint a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a - b.ToUInt ();
		}
		public static ulong operator - (ulong a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a - b.ToULong ();
		}
		public static int operator - (sbyte a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a - b.ToSByte ();
		}
		public static int operator - (short a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a - b.ToShort ();
		}
		public static int operator - (int a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a - b.ToInt ();
		}
		public static long operator - (long a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a - b.ToLong ();
		}
		public static float operator - (float a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a - b.ToFloat ();
		}
		public static double operator - (double a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a - b.ToDouble ();
		}
		public static decimal operator - (decimal a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a - b.ToDecimal ();
		}
		public static int operator - (char a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a - b.ToChar ();
		}

		#endregion

		#region Operator *

		public static int operator * (JsonValue a, byte b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToByte () * b;
		}
		public static int operator * (JsonValue a, ushort b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToUShort () * b;
		}
		public static uint operator * (JsonValue a, uint b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToUInt () * b;
		}
		public static ulong operator * (JsonValue a, ulong b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToULong () * b;
		}
		public static int operator * (JsonValue a, sbyte b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToSByte () * b;
		}
		public static int operator * (JsonValue a, short b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToShort () * b;
		}
		public static int operator * (JsonValue a, int b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToInt () * b;
		}
		public static long operator * (JsonValue a, long b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToLong () * b;
		}
		public static float operator * (JsonValue a, float b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToFloat () * b;
		}
		public static double operator * (JsonValue a, double b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToDouble () * b;
		}
		public static decimal operator * (JsonValue a, decimal b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToDecimal () * b;
		}
		public static int operator * (JsonValue a, char b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToChar () * b;
		}

		public static int operator * (byte a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a * b.ToByte ();
		}
		public static int operator * (ushort a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a * b.ToUShort ();
		}
		public static uint operator * (uint a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a * b.ToUInt ();
		}
		public static ulong operator * (ulong a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a * b.ToULong ();
		}
		public static int operator * (sbyte a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a * b.ToSByte ();
		}
		public static int operator * (short a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a * b.ToShort ();
		}
		public static int operator * (int a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a * b.ToInt ();
		}
		public static long operator * (long a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a * b.ToLong ();
		}
		public static float operator * (float a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a * b.ToFloat ();
		}
		public static double operator * (double a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a * b.ToDouble ();
		}
		public static decimal operator * (decimal a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a * b.ToDecimal ();
		}
		public static int operator * (char a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a * b.ToChar ();
		}

		#endregion

		#region Operator /

		public static int operator / (JsonValue a, byte b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToByte () / b;
		}
		public static int operator / (JsonValue a, ushort b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToUShort () / b;
		}
		public static uint operator / (JsonValue a, uint b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToUInt () / b;
		}
		public static ulong operator / (JsonValue a, ulong b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToULong () / b;
		}
		public static int operator / (JsonValue a, sbyte b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToSByte () / b;
		}
		public static int operator / (JsonValue a, short b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToShort () / b;
		}
		public static int operator / (JsonValue a, int b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToInt () / b;
		}
		public static long operator / (JsonValue a, long b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToLong () / b;
		}
		public static float operator / (JsonValue a, float b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToFloat () / b;
		}
		public static double operator / (JsonValue a, double b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToDouble () / b;
		}
		public static decimal operator / (JsonValue a, decimal b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToDecimal () / b;
		}
		public static int operator / (JsonValue a, char b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.ToChar () / b;
		}

		public static int operator / (byte a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a / b.ToByte ();
		}
		public static int operator / (ushort a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a / b.ToUShort ();
		}
		public static uint operator / (uint a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a / b.ToUInt ();
		}
		public static ulong operator / (ulong a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a / b.ToULong ();
		}
		public static int operator / (sbyte a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a / b.ToSByte ();
		}
		public static int operator / (short a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a / b.ToShort ();
		}
		public static int operator / (int a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a / b.ToInt ();
		}
		public static long operator / (long a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a / b.ToLong ();
		}
		public static float operator / (float a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a / b.ToFloat ();
		}
		public static double operator / (double a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a / b.ToDouble ();
		}
		public static decimal operator / (decimal a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a / b.ToDecimal ();
		}
		public static int operator / (char a, JsonValue b) {
			if (b is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a / b.ToChar ();
		}

		#endregion

		#region IJsonSerializable

		public string Serialize (JsonConfig config = null) {
			using (JsonTextBuilder builder = new JsonTextBuilder (new JsonValueReader (this), new StringWriter (), config)) {
				builder.BuildValue ();
				return builder.ToString ();
			}
		}
		public void Serialize (string path, JsonConfig config = null) {
			if (JsonApi.IsNullOrWhiteSpace (path)) {
				throw new ArgumentException ($"“{nameof (path)}”不能为 Null 或空白", nameof (path));
			}
			Serialize (new StreamWriter (path), config);
		}
		public void Serialize (TextWriter textWriter, JsonConfig config = null) {
			if (textWriter is null) {
				throw new ArgumentNullException (nameof (textWriter));
			}
			using (JsonTextBuilder builder = new JsonTextBuilder (new JsonValueReader (this), textWriter, config)) {
				builder.BuildValue ();
			}
		}
		public string Serialize (bool compress, JsonConfig config = null) {
			using (JsonTextBuilder builder = new JsonTextBuilder (new JsonValueReader (this), new StringWriter (), compress, config)) {
				builder.BuildValue ();
				return builder.ToString ();
			}
		}
		public void Serialize (string path, bool compress, JsonConfig config = null) {
			if (JsonApi.IsNullOrWhiteSpace (path)) {
				throw new ArgumentException ($"“{nameof (path)}”不能为 Null 或空白", nameof (path));
			}
			Serialize (new StreamWriter (path), compress, config);
		}
		public void Serialize (TextWriter textWriter, bool compress, JsonConfig config = null) {
			if (textWriter is null) {
				throw new ArgumentNullException (nameof (textWriter));
			}
			using (JsonTextBuilder builder = new JsonTextBuilder (new JsonValueReader (this), textWriter, compress, config)) {
				builder.BuildValue ();
			}
		}

		#endregion

		#region IEnumerable

		public IEnumerator GetEnumerator () {
			switch (Type) {
				default:
				case JsonValueType.String:
					return String.GetEnumerator ();
				case JsonValueType.Array:
					return GetArray ().GetEnumerator ();
				case JsonValueType.Object:
					return GetObject ().GetEnumerator ();
			}
		}

		#endregion

		#region IConvertible

		public TypeCode GetTypeCode () {
			return System.Type.GetTypeCode (Value?.GetType ());
		}

		public object ToType (Type conversionType, IFormatProvider provider) {
			return JsonApi.ChangeType (Value, conversionType);
		}

		public bool ToBoolean (IFormatProvider provider) {
			return ToBool ();
		}
		public char ToChar (IFormatProvider provider) {
			return ToChar ();
		}
		public sbyte ToSByte (IFormatProvider provider) {
			return ToSByte ();
		}
		public byte ToByte (IFormatProvider provider) {
			return ToByte ();
		}
		public short ToInt16 (IFormatProvider provider) {
			return ToShort ();
		}
		public ushort ToUInt16 (IFormatProvider provider) {
			return ToUShort ();
		}
		public int ToInt32 (IFormatProvider provider) {
			return ToInt ();
		}
		public uint ToUInt32 (IFormatProvider provider) {
			return ToUInt ();
		}
		public long ToInt64 (IFormatProvider provider) {
			return ToLong ();
		}
		public ulong ToUInt64 (IFormatProvider provider) {
			return ToULong ();
		}
		public float ToSingle (IFormatProvider provider) {
			return ToFloat ();
		}
		public double ToDouble (IFormatProvider provider) {
			return ToDouble ();
		}
		public decimal ToDecimal (IFormatProvider provider) {
			return ToDecimal ();
		}
		public DateTime ToDateTime (IFormatProvider provider) {
			return ToDateTime ();
		}
		public string ToString (IFormatProvider provider) {
			return ToString ();
		}

		#endregion

		#region IComparable

		public int CompareTo (object obj) {
			if (obj is JsonValue value) {
				return CompareTo (value.Value);
			}
			if (JsonApi.TryGetValueType (obj, out JsonValueType valueType)) {
				switch (valueType) {
					case JsonValueType.Integer:
						return ToLong ().CompareTo (Convert.ToInt64 (obj));
					case JsonValueType.Decimal:
						return ToDecimal ().CompareTo (Convert.ToDecimal (obj));
					case JsonValueType.Bool:
						return ToBool ().CompareTo ((bool)obj);
					case JsonValueType.String:
						if (obj is char character) {
							return ToChar ().CompareTo (character);
						}
						return ToString ().CompareTo ((string)obj);
					case JsonValueType.DateTime:
						return ToDateTime ().CompareTo ((DateTime)obj);
				}
			}
			return 0;
		}

		#endregion

		#region IJsonArray 

		public JsonValue this[int index] {

			get => GetArray ()[index];

			set => GetArray ()[index] = value;

		}

		public JsonValue Get (int index) {
			return GetArray ().Get (index);
		}

		#endregion

		#region IJsonObject 

		public JsonValue this[string name] {

			get {
				if (name is null) {
					throw new ArgumentNullException (nameof (name));
				}
				return GetObject ()[name];
			}

			set {
				if (name is null) {
					throw new ArgumentNullException (nameof (name));
				}
				GetObject ()[name] = value;
			}

		}

		public JsonKey Add (string name) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return GetObject ().Add (name);
		}
		public JsonKey Add (string name, object value) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return GetObject ().Add (name, value);
		}

		public JsonKey Get (string name) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return GetObject ().Add (name);
		}

		public JsonKey Rename (string oldName, string newName) {
			if (oldName is null) {
				throw new ArgumentNullException (nameof (oldName));
			}
			if (newName is null) {
				throw new ArgumentNullException (nameof (newName));
			}
			return GetObject ().Rename (oldName, newName);
		}

		#endregion

	}

}