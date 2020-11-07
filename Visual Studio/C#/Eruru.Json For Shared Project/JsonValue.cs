using System;
using System.Collections;
using System.IO;

namespace Eruru.Json {

	public class JsonValue : IJsonSerializable, IEnumerable, IJsonArray, IJsonObject {

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
					case JsonValueType.Long:
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
				if (JsonAPI.TryGetValueType (value, JsonConfig.Default, out JsonValueType valueType)) {
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
				if (value is JsonKey key) {
					Value = key.Value;
					return;
				}
				throw new JsonNotSupportException (value);
			}

		}
		public byte Byte {

			get => ToByte ();

			set {
				_Type = JsonValueType.Long;
				_Value = value;
			}

		}
		public ushort UShort {

			get => ToUShort ();

			set {
				_Type = JsonValueType.Long;
				_Value = value;
			}

		}
		public uint UInt {

			get => ToUInt ();

			set {
				_Type = JsonValueType.Long;
				_Value = value;
			}

		}
		public ulong ULong {

			get => ToULong ();

			set {
				_Type = JsonValueType.Long;
				_Value = value;
			}

		}
		public sbyte SByte {

			get => ToSByte ();

			set {
				_Type = JsonValueType.Long;
				_Value = value;
			}

		}
		public short Short {

			get => ToShort ();

			set {
				_Type = JsonValueType.Long;
				_Value = value;
			}

		}
		public int Int {

			get => ToInt ();

			set {
				_Type = JsonValueType.Long;
				_Value = value;
			}

		}
		public long Long {

			get => ToLong ();

			set {
				_Type = JsonValueType.Long;
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
				_Type = value is JsonArray ? JsonValueType.Array : JsonValueType.Null;
				_Value = value;
			}

		}
		public JsonObject Object {

			get => GetObject ();

			set {
				_Type = value is JsonObject ? JsonValueType.Object : JsonValueType.Null;
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

		public static JsonValue Parse (string text) {
			if (text is null) {
				throw new ArgumentNullException (nameof (text));
			}
			return Build (new StringReader (text));
		}

		public static JsonValue Load (Stream stream) {
			if (stream is null) {
				throw new ArgumentNullException (nameof (stream));
			}
			return Build (new StreamReader (stream));
		}
		public static JsonValue Load (string path) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			return Build (new StreamReader (path));
		}

		public static JsonValue Parse (string text, JsonValue value = null) {
			if (text is null) {
				throw new ArgumentNullException (nameof (text));
			}
			return Build (new StringReader (text), value);
		}

		static JsonValue Build (TextReader textReader, JsonValue value = null) {
			if (textReader is null) {
				throw new ArgumentNullException (nameof (textReader));
			}
			using (JsonTextReader reader = new JsonTextReader (textReader)) {
				return new JsonValueBuilder (reader).BuildValue (value);
			}
		}

		public override string ToString () {
			return Serialize ();
		}

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
				_Type = JsonValueType.Array;
				_Value = jsonObject;
			}
			return jsonObject;
		}

		#region IJsonSerializable

		public string Serialize (JsonConfig config = null) {
			return Build (new StringWriter (), config).ToString ();
		}
		public void Serialize (string path, JsonConfig config = null) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			Build (new StreamWriter (path), config);
		}
		public void Serialize (Stream stream, JsonConfig config = null) {
			if (stream is null) {
				throw new ArgumentNullException (nameof (stream));
			}
			Build (new StreamWriter (stream), config);
		}
		public void Serialize (StreamWriter streamWriter, JsonConfig config = null) {
			if (streamWriter is null) {
				throw new ArgumentNullException (nameof (streamWriter));
			}
			Build (streamWriter, config);
		}
		public string Serialize (bool compress, JsonConfig config = null) {
			return Build (new StringWriter (), compress, config).ToString ();
		}
		public void Serialize (string path, bool compress, JsonConfig config = null) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			Build (new StreamWriter (path), compress, config);
		}
		public void Serialize (Stream stream, bool compress, JsonConfig config = null) {
			if (stream is null) {
				throw new ArgumentNullException (nameof (stream));
			}
			Build (new StreamWriter (stream), compress, config);
		}
		public void Serialize (StreamWriter streamWriter, bool compress, JsonConfig config = null) {
			if (streamWriter is null) {
				throw new ArgumentNullException (nameof (streamWriter));
			}
			Build (streamWriter, compress, config);
		}

		JsonTextBuilder Build (TextWriter textWriter, JsonConfig config) {
			if (textWriter is null) {
				throw new ArgumentNullException (nameof (textWriter));
			}
			using (JsonTextBuilder builder = new JsonTextBuilder (new JsonValueReader (this), textWriter, config)) {
				builder.BuildValue ();
				return builder;
			}
		}
		JsonTextBuilder Build (TextWriter textWriter, bool compress, JsonConfig config) {
			if (textWriter is null) {
				throw new ArgumentNullException (nameof (textWriter));
			}
			using (JsonTextBuilder builder = new JsonTextBuilder (new JsonValueReader (this), textWriter, compress, config)) {
				builder.BuildValue ();
				return builder;
			}
		}

		#endregion

		#region IEnumerable

		public IEnumerator GetEnumerator () {
			if (Type == JsonValueType.Array) {
				return GetArray ().GetEnumerator ();
			}
			return GetObject ().GetEnumerator ();
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

			get => GetObject ()[name];

			set => GetObject ()[name] = value;

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

		#endregion

	}

}