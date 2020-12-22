using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Eruru.Json {

	public delegate void JsonAction ();
	public delegate void JsonAction<in T1, in T2> (T1 arg1, T2 arg2);
	public delegate void JsonAction<in T1, in T2, in T3, in T4> (T1 arg1, T2 arg2, T3 arg3, T4 arg4);
	public delegate TResult JsonFunc<in T, out TResult> (T arg);

	static class JsonApi {

		static readonly BindingFlags BindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		static readonly KeyValuePair<char, char>[] Escapes = new KeyValuePair<char, char>[] {
			new KeyValuePair<char, char> ('\\', '\\'),
			new KeyValuePair<char, char> ('"', '"'),
			new KeyValuePair<char, char> ('r', '\r'),
			new KeyValuePair<char, char> ('n', '\n'),
			new KeyValuePair<char, char> ('t', '\t')
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
				case TypeCode.DBNull:
					valueType = JsonValueType.Null;
					return true;
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
			}
			valueType = JsonValueType.Null;
			return false;
		}
		public static bool TryGetValueType (object value, out JsonValueType valueType, JsonConfig config = null) {
			if (value is null) {
				valueType = JsonValueType.Null;
				return true;
			}
			return TryGetValueType (value.GetType (), out valueType, config);
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
			}
			arrayType = JsonArrayType.Unknown;
			return false;
		}

		public static bool TryGetObjectType (Type type, out JsonObjectType objectType) {
			if (type is null) {
				throw new ArgumentNullException (nameof (type));
			}
			if (type.IsClass) {
				objectType = JsonObjectType.Class;
				return true;
			}
			objectType = JsonObjectType.Unknown;
			return false;
		}

		public static void SetExceptionMessage (object instance, string message) {
			if (instance is null) {
				throw new ArgumentNullException (nameof (instance));
			}
			if (message is null) {
				throw new ArgumentNullException (nameof (message));
			}
			typeof (Exception).GetField ("_message", BindingFlags).SetValue (instance, message);
		}

		public static MemberInfo[] GetMembers (Type type) {
			if (type is null) {
				throw new ArgumentNullException (nameof (type));
			}
			return type.GetMembers (BindingFlags);
		}

		public static void ForEachMembers (Type type, JsonAction<MemberInfo, FieldInfo, PropertyInfo, JsonField> action) {
			if (type is null) {
				throw new ArgumentNullException (nameof (type));
			}
			if (action is null) {
				throw new ArgumentNullException (nameof (action));
			}
			foreach (MemberInfo memberInfo in GetMembers (type)) {
				if (CanSerialize (memberInfo, out FieldInfo fieldInfo, out PropertyInfo propertyInfo, out JsonField field)) {
					action (memberInfo, fieldInfo, propertyInfo, field);
				}
			}
		}

		public static bool CanSerialize (MemberInfo memberInfo, out FieldInfo fieldInfo, out PropertyInfo propertyInfo, out JsonField field) {
			if (memberInfo is null) {
				throw new ArgumentNullException (nameof (memberInfo));
			}
			switch (memberInfo.MemberType) {
				case MemberTypes.Field: {
					fieldInfo = (FieldInfo)memberInfo;
					if (GetCustomAttribute<JsonIgnoreField> (memberInfo) != null) {
						propertyInfo = null;
						field = null;
						return false;
					}
					field = GetCustomAttribute<JsonField> (memberInfo);
					if (!fieldInfo.IsPublic && field is null) {
						propertyInfo = null;
						return false;
					}
					propertyInfo = null;
					return true;
				}
				case MemberTypes.Property: {
					propertyInfo = (PropertyInfo)memberInfo;
					if (!propertyInfo.CanRead || !propertyInfo.CanWrite || GetCustomAttribute<JsonIgnoreField> (memberInfo) != null) {
						fieldInfo = null;
						field = null;
						return false;
					}
					field = GetCustomAttribute<JsonField> (memberInfo);
					fieldInfo = null;
					return true;
				}
				default:
					propertyInfo = null;
					fieldInfo = null;
					field = null;
					return false;
			}
		}

		public static bool CanSerialize (JsonConfig config, object instance) {
			if (config.IgnoreNullValue) {
				if (instance is null) {
					return false;
				}
			}
			if (config.IgnoreDefaultValue) {
				if (instance is null) {
					return false;
				}
				Type type = instance.GetType ();
				if (type.IsValueType) {
					if (Equals (instance, Activator.CreateInstance (type))) {
						return false;
					}
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

		public static object ChangeType (object value, Type type, JsonConfig config = null) {
			if (type is null) {
				return value;
			}
			if (value is null) {
				return default;
			}
			if (type.IsEnum) {
				if (config is null) {
					config = JsonConfig.Default;
				}
				if (config.StringEnum) {
					return Enum.Parse (type, value.ToString (), config.IgnoreCase);
				}
				return Enum.ToObject (type, Convert.ChangeType (value, TypeCode.Int32));
			}
			return Convert.ChangeType (value, type);
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

		public static bool Equals (string a, string b, JsonConfig config) {
			if (config is null) {
				throw new ArgumentNullException (nameof (config));
			}
			return string.Equals (a, b, GetStringComparison (config));
		}

		public static string Unescape (string text) {
			if (text is null) {
				throw new ArgumentNullException (nameof (text));
			}
			StringBuilder stringBuilder = new StringBuilder ();
			for (int i = 0; i < text.Length; i++) {
				int index = Array.FindIndex (Escapes, escape => escape.Value == text[i]);
				if (index > -1) {
					stringBuilder.Append (JsonKeyword.Backslash);
					stringBuilder.Append (Escapes[index].Key);
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

		static StringComparison GetStringComparison (JsonConfig config) {
			if (config is null) {
				throw new ArgumentNullException (nameof (config));
			}
			return config.IgnoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
		}

	}

}