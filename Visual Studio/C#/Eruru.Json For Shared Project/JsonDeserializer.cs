using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Eruru.Json {

	public class JsonDeserializer {

		readonly IJsonReader Reader;
		readonly JsonConfig Config;

		public JsonDeserializer (IJsonReader reader, JsonConfig config = null) {
			Reader = reader ?? throw new ArgumentNullException (nameof (reader));
			Config = config ?? JsonConfig.Default;
		}

		public static T Deserialize<T> (string text, JsonConfig config = null) {
			if (text is null) {
				throw new ArgumentNullException (nameof (text));
			}
			return Build<T> (new StringReader (text), config);
		}
		public static T Deserialize<T> (Stream stream, JsonConfig config = null) {
			if (stream is null) {
				throw new ArgumentNullException (nameof (stream));
			}
			return Build<T> (new StreamReader (stream), config);
		}
		public static T Deserialize<T> (StreamReader streamReader, JsonConfig config = null) {
			if (streamReader is null) {
				throw new ArgumentNullException (nameof (streamReader));
			}
			return Build<T> (streamReader, config);
		}
		public static T Deserialize<T> (JsonValue value, JsonConfig config = null) {
			if (value is null) {
				throw new ArgumentNullException (nameof (value));
			}
			return (T)Build (value, config).BuildValue (typeof (T));
		}
		public static T Deserialize<T> (JsonArray array, JsonConfig config = null) {
			if (array is null) {
				throw new ArgumentNullException (nameof (array));
			}
			return (T)Build (array, config).BuildArray (typeof (T));
		}
		public static T Deserialize<T> (JsonObject jsonObject, JsonConfig config = null) {
			if (jsonObject is null) {
				throw new ArgumentNullException (nameof (jsonObject));
			}
			return (T)Build (jsonObject, config).BuildObject (typeof (T));
		}
		public static T Deserialize<T> (string text, T instance, JsonConfig config = null) {
			if (text is null) {
				throw new ArgumentNullException (nameof (text));
			}
			return Build (new StringReader (text), config, instance);
		}
		public static T Deserialize<T> (Stream stream, T instance, JsonConfig config = null) {
			if (stream is null) {
				throw new ArgumentNullException (nameof (stream));
			}
			return Build (new StreamReader (stream), config, instance);
		}
		public static T Deserialize<T> (StreamReader streamReader, T instance, JsonConfig config = null) {
			if (streamReader is null) {
				throw new ArgumentNullException (nameof (streamReader));
			}
			return Build (streamReader, config, instance);
		}
		public static T Deserialize<T> (JsonValue value, T instance, JsonConfig config = null) {
			if (value is null) {
				throw new ArgumentNullException (nameof (value));
			}
			return (T)Build (value, config).BuildValue (typeof (T), instance);
		}
		public static T Deserialize<T> (JsonArray array, T instance, JsonConfig config = null) {
			if (array is null) {
				throw new ArgumentNullException (nameof (array));
			}
			return (T)Build (array, config).BuildArray (typeof (T), instance);
		}
		public static T Deserialize<T> (JsonObject jsonObject, T instance, JsonConfig config = null) {
			if (jsonObject is null) {
				throw new ArgumentNullException (nameof (jsonObject));
			}
			return (T)Build (jsonObject, config).BuildObject (typeof (T), instance);
		}

		public static T DeserializeFile<T> (string path, JsonConfig config = null) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			return Build<T> (new StreamReader (path), config);
		}
		public static T DeserializeFile<T> (string path, T instance, JsonConfig config = null) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			return Build (new StreamReader (path), config, instance);
		}

		static T Build<T> (TextReader textReader, JsonConfig config, T instance = default) {
			if (textReader is null) {
				throw new ArgumentNullException (nameof (textReader));
			}
			using (JsonTextReader reader = new JsonTextReader (textReader)) {
				return (T)new JsonDeserializer (reader, config).BuildValue (typeof (T), instance);
			}
		}

		static JsonDeserializer Build (JsonValue value, JsonConfig config) {
			if (value is null) {
				throw new ArgumentNullException (nameof (value));
			}
			return new JsonDeserializer (new JsonValueReader (value), config);
		}

		object BuildValue (Type type, object instance = null) {
			object instanceValue = null;
			Reader.ReadValue (
				(value, valueType) => instanceValue = JsonAPI.ChangeType (value, type, Config),
				() => instanceValue = BuildArray (type, instance),
				() => instanceValue = BuildObject (type, instance)
			);
			return instanceValue;
		}

		object BuildArray (Type type, object instance = null) {
			if (type is null) {
				throw new ArgumentNullException (nameof (type));
			}
			if (JsonAPI.TryGetArrayType (type, out JsonArrayType arrayType)) {
				switch (arrayType) {
					case JsonArrayType.Array: {
						JsonArray jsonArray = new JsonValueBuilder (Reader).BuildArray ();
						List<int> bounds = new List<int> ();
						MeasureArray (jsonArray, ref bounds);
						if (bounds.Count < type.GetArrayRank ()) {
							throw new JsonException ("Array rank mismatch");
						}
						Type elementType = type.GetElementType ();
						Array array = (Array)instance;
						bool create = false;
						if (array is null) {
							create = true;
						} else {
							for (int i = 0; i < array.Rank; i++) {
								if (array.GetLength (i) != bounds[i]) {
									create = true;
									break;
								}
							}
						}
						if (create) {
							if (type.GetArrayRank () == 1) {
								array = Array.CreateInstance (elementType, bounds[0]);
							} else {
								array = Array.CreateInstance (elementType, bounds.ToArray ());
							}
						}
						if (array.Rank == 1) {
							for (int i = 0; i < jsonArray.Count; i++) {
								JsonDeserializer deserializer = new JsonDeserializer (new JsonValueReader (jsonArray[i]), Config);
								array.SetValue (deserializer.BuildValue (elementType, array.GetValue (i)), i);
							}
							return array;
						}
						int[] indices = new int[bounds.Count];
						int dimension = 0;
						void ForEachArray (JsonArray current) {
							int length = bounds[dimension];
							for (int i = 0; i < length; i++) {
								indices[dimension] = i;
								if (dimension == indices.Length - 1) {
									JsonDeserializer deserializer = new JsonDeserializer (new JsonValueReader (current[i]), Config);
									array.SetValue (deserializer.BuildValue (elementType, array.GetValue (indices)), indices);
									continue;
								}
								dimension++;
								ForEachArray (current[i]);
							}
							dimension--;
						}
						ForEachArray (jsonArray);
						return array;
					}
					case JsonArrayType.GenericList:
					case JsonArrayType.GenericIList:
					case JsonArrayType.GenericObservableCollection: {
						Type elementType = type.GetGenericArguments ()[0];
						if (instance is null) {
							if (type.IsInterface) {
								switch (arrayType) {
									case JsonArrayType.GenericIList:
										instance = JsonAPI.CreateInstance (typeof (List<>).MakeGenericType (elementType));
										break;
									default:
										throw new JsonIsNotSupportException (arrayType);
								}
							} else {
								instance = JsonAPI.CreateInstance (type);
							}
						}
						IList list = (IList)instance;
						int count = list.Count;
						Reader.ReadArray (() => {
							object instanceValue = null;
							if (count > 0) {
								instanceValue = list[0];
								list.RemoveAt (0);
							}
							list.Add (BuildValue (elementType, instanceValue));
						});
						return instance;
					}
					default:
						throw new JsonIsNotSupportException (arrayType);
				}
			}
			throw new JsonIsNotSupportException (type);
		}

		object BuildObject (Type type, object instance = null) {
			if (type is null) {
				throw new ArgumentNullException (nameof (type));
			}
			if (JsonAPI.TryGetObjectType (type, out JsonObjectType objectType)) {
				switch (objectType) {
					case JsonObjectType.Class: {
						if (instance is null) {
							instance = JsonAPI.CreateInstance (type);
						}
						MemberInfo memberInfo = null;
						FieldInfo fieldInfo = null;
						PropertyInfo propertyInfo = null;
						JsonField field = null;
						Reader.ReadObject (name => {
							foreach (MemberInfo current in JsonAPI.GetMembers (type)) {
								if (JsonAPI.CanSerialize (current, out fieldInfo, out propertyInfo, out field)) {
									if (JsonAPI.Equals (name, field?.Name ?? current.Name, Config)) {
										memberInfo = current;
										return true;
									}
								}
							}
							return false;
						}, () => {
							switch (memberInfo.MemberType) {
								case MemberTypes.Field:
									fieldInfo.SetValue (instance, Read (fieldInfo.FieldType, fieldInfo.GetValue (instance), field));
									break;
								case MemberTypes.Property:
									propertyInfo.SetValue (instance, Read (propertyInfo.PropertyType, propertyInfo.GetValue (instance, null), field), null);
									break;
								default:
									throw new JsonIsNotSupportException (memberInfo.MemberType);
							}
						});
						return instance;
					}
					default:
						throw new JsonIsNotSupportException (objectType);
				}
			}
			throw new JsonIsNotSupportException (type);
		}

		object Read (Type type, object instance, JsonField field) {
			if (type is null) {
				throw new ArgumentNullException (nameof (type));
			}
			if (field?.HasConverter ?? false) {
				return JsonAPI.ChangeType (field.Read (BuildValue (null, instance), Config), type, Config);
			}
			return BuildValue (type, instance);
		}

		void MeasureArray (JsonArray array, ref List<int> bounds) {
			if (array is null) {
				throw new ArgumentNullException (nameof (array));
			}
			if (bounds is null) {
				throw new ArgumentNullException (nameof (bounds));
			}
			bounds.Add (array.Count);
			if (array.Count > 0 && array[0].Type == JsonValueType.Array) {
				MeasureArray (array[0], ref bounds);
			}
		}

	}

}