using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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

		public T BuildValue<T> (object instance = null) {
			return (T)BuildValue (typeof (T), instance);
		}

		public T BuildArray<T> (object instance = null) {
			return (T)BuildArray (typeof (T), instance);
		}

		public T BuildObject<T> (object instance = null) {
			return (T)BuildObject (typeof (T), instance);
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
						Array array = instance as Array;
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
										throw new JsonNotSupportException (arrayType);
								}
							} else {
								instance = JsonAPI.CreateInstance (type);
							}
						}
						IList list = (IList)instance;
						int count = list.Count;
						Reader.ReadArray (i => {
							object instanceValue = null;
							if (i < count) {
								instanceValue = list[0];
								list.RemoveAt (0);
							}
							list.Add (BuildValue (elementType, instanceValue));
						});
						return instance;
					}
					default:
						throw new JsonNotSupportException (arrayType);
				}
			}
			throw new JsonNotSupportException (type);
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
									fieldInfo.SetValue (instance, ConverterRead (fieldInfo.FieldType, fieldInfo.GetValue (instance), field));
									break;
								case MemberTypes.Property:
									propertyInfo.SetValue (instance, ConverterRead (propertyInfo.PropertyType, propertyInfo.GetValue (instance, null), field), null);
									break;
								default:
									throw new JsonNotSupportException (memberInfo.MemberType);
							}
						});
						return instance;
					}
					default:
						throw new JsonNotSupportException (objectType);
				}
			}
			throw new JsonNotSupportException (type);
		}

		object ConverterRead (Type type, object instance, JsonField field) {
			if (type is null) {
				throw new ArgumentNullException (nameof (type));
			}
			if (field?.HasConverter ?? false) {
				return JsonAPI.ChangeType (field.Read (BuildValue (field.ConverterReadType, instance), Config), type, Config);
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