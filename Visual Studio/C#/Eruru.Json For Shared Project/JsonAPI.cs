﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Eruru.Json {

	public delegate void JsonAction ();
	public delegate void JsonAction<in T1, in T2> (T1 arg1, T2 arg2);
	public delegate void JsonAction<in T1, in T2, in T3, in T4> (T1 arg1, T2 arg2, T3 arg3, T4 arg4);
	public delegate TResult JsonFunc<in T, out TResult> (T arg);

	static class JsonApi {

		public static readonly BindingFlags BindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		static readonly Dictionary<char, char> Unescapes = new Dictionary<char, char> {
			{ '"', '"' },
			{ '\\', '\\' },
			{ '\b', 'b' },
			{ '\f', 'f' },
			{ '\n', 'n' },
			{ '\r', 'r' },
			{ '\t', 't' }
		};

		public static bool HasFlag (Enum a, Enum b) {
			if (a is null) {
				throw new ArgumentNullException (nameof (a));
			}
			if (b is null) {
				throw new ArgumentNullException (nameof (b));
			}
			return (a.GetHashCode () & b.GetHashCode ()) != 0;
		}

		public static bool TryGetValueType (Type type, out JsonValueType valueType, JsonConfig config = null) {
			if (type is null) {
				throw new ArgumentNullException (nameof (type));
			}
			if (config is null) {
				config = JsonConfig.Default;
			}
			if (type.IsEnum && config.StringEnum) {
				valueType = JsonValueType.String;
				return true;
			}
			switch (Type.GetTypeCode (type)) {
				case TypeCode.Byte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.SByte:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
					valueType = JsonValueType.Integer;
					return true;
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
					valueType = JsonValueType.Decimal;
					return true;
				case TypeCode.Boolean:
					valueType = JsonValueType.Bool;
					return true;
				case TypeCode.Char:
				case TypeCode.String:
					valueType = JsonValueType.String;
					return true;
				case TypeCode.DateTime:
					valueType = JsonValueType.DateTime;
					return true;
				case TypeCode.DBNull:
				default:
					valueType = JsonValueType.Null;
					return false;
			}
		}
		public static bool TryGetValueType (object value, out JsonValueType valueType, JsonConfig config = null) {
			if (value is null) {
				valueType = JsonValueType.Null;
				return true;
			}
			return TryGetValueType (value.GetType (), out valueType, config ?? JsonConfig.Default);
		}

		public static bool TryGetArrayType (Type type, out JsonArrayType arrayType) {
			if (type is null) {
				throw new ArgumentNullException (nameof (type));
			}
			if (type.IsArray) {
				arrayType = JsonArrayType.Array;
				return true;
			}
			switch (type.Name) {
				case "List`1":
					arrayType = JsonArrayType.GenericList;
					return true;
				case "IList`1":
					arrayType = JsonArrayType.GenericIList;
					return true;
				case "ObservableCollection`1":
					arrayType = JsonArrayType.GenericObservableCollection;
					return true;
				case nameof (DataTable):
					arrayType = JsonArrayType.DataTable;
					return true;
			}
			arrayType = JsonArrayType.Unknown;
			return false;
		}

		public static bool TryGetObjectType (Type type, out JsonObjectType objectType) {
			if (type is null) {
				throw new ArgumentNullException (nameof (type));
			}
			switch (type.Name) {
				case nameof (DataRow):
					objectType = JsonObjectType.DataRow;
					return true;
				case nameof (DataSet):
					objectType = JsonObjectType.DataSet;
					return true;
				case "Dictionary`2":
					objectType = JsonObjectType.GenericDictionary;
					return true;
				case "SortedDictionary`2":
					objectType = JsonObjectType.GenericSortedDictionary;
					return true;
				case "SortedList`2":
					objectType = JsonObjectType.GenericSortedList;
					return true;
				case "KeyValuePair`2":
					objectType = JsonObjectType.GenericKeyValuePair;
					return true;
			}
			if (type.IsClass) {
				objectType = JsonObjectType.Class;
				return true;
			}
			objectType = JsonObjectType.Unknown;
			return false;
		}

		public static MemberInfo[] GetMembers (Type type) {
			if (type is null) {
				throw new ArgumentNullException (nameof (type));
			}
			return type.GetMembers (BindingFlags);
		}

		public static void ForEachSerializableMembers (Type type, JsonAction<MemberInfo, FieldInfo, PropertyInfo, JsonField> action) {
			if (type is null) {
				throw new ArgumentNullException (nameof (type));
			}
			if (action is null) {
				throw new ArgumentNullException (nameof (action));
			}
			foreach (MemberInfo memberInfo in GetMembers (type)) {
				if (CanSerializeMember (memberInfo, out FieldInfo fieldInfo, out PropertyInfo propertyInfo, out JsonField field)) {
					action (memberInfo, fieldInfo, propertyInfo, field);
				}
			}
		}

		public static bool CanSerializeMember (MemberInfo memberInfo, out FieldInfo fieldInfo, out PropertyInfo propertyInfo, out JsonField field) {
			if (memberInfo is null) {
				throw new ArgumentNullException (nameof (memberInfo));
			}
			switch (memberInfo.MemberType) {
				case MemberTypes.Field: {
					fieldInfo = (FieldInfo)memberInfo;
					propertyInfo = null;
					if (GetCustomAttribute<JsonIgnoreField> (memberInfo) != null) {
						field = null;
						return false;
					}
					field = GetCustomAttribute<JsonField> (memberInfo);
					if (!fieldInfo.IsPublic && field is null) {
						return false;
					}
					return true;
				}
				case MemberTypes.Property: {
					propertyInfo = (PropertyInfo)memberInfo;
					fieldInfo = null;
					if (!propertyInfo.CanRead || !propertyInfo.CanWrite || GetCustomAttribute<JsonIgnoreField> (memberInfo) != null) {
						field = null;
						return false;
					}
					field = GetCustomAttribute<JsonField> (memberInfo);
					return true;
				}
				default:
					propertyInfo = null;
					fieldInfo = null;
					field = null;
					return false;
			}
		}

		public static bool CanSerializeValue (object value = null, JsonConfig config = null) {
			if (config is null) {
				config = JsonConfig.Default;
			}
			if (config.IgnoreNullValue) {
				if (value is null) {
					return false;
				}
			}
			if (config.IgnoreDefaultValue) {
				if (value is null) {
					return false;
				}
				if (Equals (value, GetDefaultValue (value.GetType ()))) {
					return false;
				}
			}
			return true;
		}

		public static T GetCustomAttribute<T> (MemberInfo memberInfo) where T : Attribute {
			if (memberInfo is null) {
				throw new ArgumentNullException (nameof (memberInfo));
			}
			object[] attributes = memberInfo.GetCustomAttributes (typeof (T), false);
			return attributes.Length == 0 ? null : (T)attributes[0];
		}

		public static Type GetElementType (Type type) {
			if (type is null) {
				throw new ArgumentNullException (nameof (type));
			}
			return type.IsArray ? type.GetElementType () : type;
		}

		public static object ChangeType (object value, Type type = null, JsonConfig config = null) {
			if (type is null) {
				return value;
			}
			try {
				if (type.IsEnum) {
					if (config is null) {
						config = JsonConfig.Default;
					}
					if (config.StringEnum) {
						return Enum.Parse (type, value?.ToString (), config.IgnoreCase);
					}
					return Enum.ToObject (type, Convert.ChangeType (value, TypeCode.Int32));
				}
				return Convert.ChangeType (value, type);
			} catch {
				return GetDefaultValue (type);
			}
		}

		public static object GetDefaultValue (Type type) {
			if (type is null) {
				throw new ArgumentNullException (nameof (type));
			}
			if (type.IsValueType) {
				return CreateInstance (type);
			}
			return null;
		}

		public static object CreateInstance (Type type) {
			if (type is null) {
				throw new ArgumentNullException (nameof (type));
			}
			if (type.GetConstructor (Type.EmptyTypes) is null) {
				return FormatterServices.GetUninitializedObject (type);
			}
			return Activator.CreateInstance (type);
		}

		public static bool Equals (string a = null, string b = null, JsonConfig config = null) {
			if (config is null) {
				config = JsonConfig.Default;
			}
			return string.Equals (a, b, config.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		}

		public static string CancelUnescape (string text) {
			if (text is null) {
				throw new ArgumentNullException (nameof (text));
			}
			StringBuilder stringBuilder = new StringBuilder ();
			for (int i = 0; i < text.Length; i++) {
				if (Unescapes.TryGetValue (text[i], out char value)) {
					stringBuilder.Append (JsonKeyword.Backslash);
					stringBuilder.Append (value);
					continue;
				}
				stringBuilder.Append (text[i]);
			}
			return stringBuilder.ToString ();
		}

		public static bool IsNullOrWhiteSpace (string text) {
			if (text is null) {
				return true;
			}
			if (text.Length == 0) {
				return true;
			}
			foreach (char character in text) {
				if (!char.IsWhiteSpace (character)) {
					return false;
				}
			}
			return true;
		}

		public static string Naming (string name, JsonNamingType type) {
			switch (type) {
				case JsonNamingType.Default:
					return name;
				case JsonNamingType.Lowercase:
					return name?.ToLowerInvariant ();
				case JsonNamingType.Uppercase:
					return name?.ToUpperInvariant ();
				default:
					throw new NotImplementedException ();
			}
		}

	}

}