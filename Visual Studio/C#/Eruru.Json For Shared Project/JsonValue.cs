using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

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
				if (JsonAPI.TryGetValueType (value, out JsonValueType valueType)) {
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

		int GetHashCode (JsonValueType valueType) {
			switch (valueType) {
				case JsonValueType.Long:
					if (Type != JsonValueType.Long) {
						return ToLong ().GetHashCode ();
					}
					return GetHashCode ();
				case JsonValueType.Decimal:
					if (Type != JsonValueType.Decimal) {
						return ToDecimal ().GetHashCode ();
					}
					return GetHashCode ();
				case JsonValueType.Bool:
					if (Type != JsonValueType.Bool) {
						return ToBool ().GetHashCode ();
					}
					return GetHashCode ();
				case JsonValueType.String:
					if (Type != JsonValueType.String) {
						return ToString ().GetHashCode ();
					}
					return GetHashCode ();
				case JsonValueType.DateTime:
					if (Type != JsonValueType.DateTime) {
						return ToDateTime ().GetHashCode ();
					}
					return GetHashCode ();
				default:
					throw new JsonNotSupportException (valueType);
			}
		}

		#region Override

		public override bool Equals (object obj) {
			if (Value is null || obj is null) {
				return Value == obj;
			}
			if (obj is JsonValue value) {
				return Equals (value.Value);
			}
			if (obj is JsonArray array) {
				if (Type != JsonValueType.Array) {
					return false;
				}
				return ToArray ().Equals (array);
			}
			if (obj is JsonObject jsonObject) {
				if (Type != JsonValueType.Object) {
					return false;
				}
				throw new NotImplementedException ();
				return ToObject ().Equals (jsonObject);
			}
			if (JsonAPI.TryGetValueType (obj, out JsonValueType valueType)) {
				switch (valueType) {
					case JsonValueType.Long:
					case JsonValueType.Decimal:
					case JsonValueType.Bool:
					case JsonValueType.String:
					case JsonValueType.DateTime:
						return GetHashCode (valueType) == obj.GetHashCode ();
					default:
						throw new JsonNotSupportException (valueType);
				}
			}
			throw new JsonNotSupportException (obj);
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
			return a.Equals (b);
		}
		public static bool operator == (JsonValue a, JsonArray b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.Equals (b);
		}
		public static bool operator == (JsonValue a, JsonObject b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
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
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return b.Equals (a);
		}
		public static bool operator == (JsonObject a, JsonValue b) {
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
			return !a.Equals (b);
		}
		public static bool operator != (JsonValue a, JsonArray b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return !a.Equals (b);
		}
		public static bool operator != (JsonValue a, JsonObject b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
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
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return !b.Equals (a);
		}
		public static bool operator != (JsonObject a, JsonValue b) {
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
			return a.CompareTo (b) > 0;
		}
		public static bool operator > (JsonValue a, JsonArray b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.CompareTo (b) > 0;
		}
		public static bool operator > (JsonValue a, JsonObject b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.CompareTo (b) > 0;
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
			return a.CompareTo (b) < 0;
		}
		public static bool operator < (JsonValue a, JsonArray b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.CompareTo (b) < 0;
		}
		public static bool operator < (JsonValue a, JsonObject b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			return a.CompareTo (b) < 0;
		}

		#endregion

		#region Operator +

		public static int operator + (JsonValue a, byte b) {
			return a.Byte + b;
		}
		public static int operator + (JsonValue a, ushort b) {
			return a.UShort + b;
		}
		public static uint operator + (JsonValue a, uint b) {
			return a.UInt + b;
		}
		public static ulong operator + (JsonValue a, ulong b) {
			return a.ULong + b;
		}
		public static int operator + (JsonValue a, sbyte b) {
			return a.SByte + b;
		}
		public static int operator + (JsonValue a, short b) {
			return a.Short + b;
		}
		public static int operator + (JsonValue a, int b) {
			return a.Int + b;
		}
		public static long operator + (JsonValue a, long b) {
			return a.Long + b;
		}
		public static float operator + (JsonValue a, float b) {
			return a.Float + b;
		}
		public static double operator + (JsonValue a, double b) {
			return a.Double + b;
		}
		public static decimal operator + (JsonValue a, decimal b) {
			return a.Decimal + b;
		}
		public static bool operator + (JsonValue a, bool b) {
			return a.Bool != b;
		}
		public static int operator + (JsonValue a, char b) {
			return a.Char + b;
		}
		public static string operator + (JsonValue a, string b) {
			return a.String + b;
		}
		public static DateTime operator + (JsonValue a, DateTime b) {
			return new DateTime (a.ToDateTime ().Ticks + b.Ticks);
		}
		public static JsonValue operator + (JsonValue a, JsonValue b) {
			throw new NotImplementedException ();
		}
		public static JsonArray operator + (JsonValue a, JsonArray b) {
			throw new NotImplementedException ();
		}
		public static JsonObject operator + (JsonValue a, JsonObject b) {
			throw new NotImplementedException ();
		}

		#endregion

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

		#region IConvertible

		public TypeCode GetTypeCode () {
			return System.Type.GetTypeCode (Value?.GetType ());
		}

		public object ToType (Type conversionType, IFormatProvider provider) {
			return JsonAPI.ChangeType (Value, conversionType);
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
			if (Value is null) {
				return obj is null ? 0 : 1;
			}
			if (obj is null) {
				return Value is null ? 0 : -1;
			}
			if (obj is JsonValue value) {
				return CompareTo (value.Value);
			}
			if (obj is JsonArray) {
				return 0;
			}
			if (obj is JsonObject) {
				return 0;
			}
			if (JsonAPI.TryGetValueType (obj, out JsonValueType valueType)) {
				switch (valueType) {
					case JsonValueType.Long:
						return ToLong ().CompareTo (Convert.ToInt64 (obj));
					case JsonValueType.Decimal:
						return ToDecimal ().CompareTo (Convert.ToDecimal (obj));
					case JsonValueType.Bool:
						return ToBool ().CompareTo (Convert.ToBoolean (obj));
					case JsonValueType.String:
						return ToString ().CompareTo (Convert.ToString (obj));
					case JsonValueType.DateTime:
						return ToDateTime ().CompareTo (Convert.ToDateTime (obj));
					default:
						throw new JsonNotSupportException (valueType);
				}
			}
			throw new JsonNotSupportException (obj);
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