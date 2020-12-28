﻿using System;
using System.Collections;
using System.Collections.Generic;
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
				(value, valueType) => instanceValue = JsonApi.ChangeType (value, type, Config),
				() => instanceValue = BuildArray (type, instance),
				() => instanceValue = BuildObject (type, instance)
			);
			return instanceValue;
		}

		object BuildArray (Type type, object instance = null) {
			if (type is null) {
				throw new ArgumentNullException (nameof (type));
			}
			if (JsonApi.TryGetArrayType (type, out JsonArrayType arrayType)) {
				switch (arrayType) {
					case JsonArrayType.Array: {
						JsonArray jsonArray = new JsonValueBuilder (Reader).BuildArray ();
						List<int> bounds = new List<int> ();
						MeasureArray (jsonArray, ref bounds);
						if (bounds.Count < type.GetArrayRank ()) {
							throw new JsonException ("数组维数不匹配");
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
										instance = JsonApi.CreateInstance (typeof (List<>).MakeGenericType (elementType));
										break;
									default:
										throw new JsonNotSupportException (arrayType);
								}
							} else {
								instance = JsonApi.CreateInstance (type);
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
			throw new JsonException ($"不支持将数组转为{type}");
		}

		object BuildObject (Type type, object instance = null) {
			if (type is null) {
				throw new ArgumentNullException (nameof (type));
			}
			if (JsonApi.TryGetObjectType (type, out JsonObjectType objectType)) {
				switch (objectType) {
					case JsonObjectType.Class: {
						if (instance is null) {
							instance = JsonApi.CreateInstance (type);
						}
						MemberInfo memberInfo = null;
						FieldInfo fieldInfo = null;
						PropertyInfo propertyInfo = null;
						JsonField field = null;
						Reader.ReadObject (name => {
							foreach (MemberInfo current in JsonApi.GetMembers (type)) {
								if (JsonApi.CanSerializeMember (current, out fieldInfo, out propertyInfo, out field)) {
									if (JsonApi.Equals (name, field?.Name ?? current.Name, Config)) {
										memberInfo = current;
										return true;
									}
								}
							}
							return false;
						}, () => {
							object value;
							switch (memberInfo.MemberType) {
								case MemberTypes.Field:
									value = ConverterRead (fieldInfo.FieldType, fieldInfo.GetValue (instance), field);
									break;
								case MemberTypes.Property:
									value = ConverterRead (propertyInfo.PropertyType, propertyInfo.GetValue (instance, null), field);
									break;
								default:
									throw new JsonNotSupportException (memberInfo.MemberType);
							}
							if (!JsonApi.CanSerializeValue (value, Config)) {
								return;
							}
							if (memberInfo.MemberType == MemberTypes.Property) {
								propertyInfo.SetValue (instance, value, null);
								return;
							}
							fieldInfo.SetValue (instance, value);
						});
						return instance;
					}
					default:
						throw new JsonNotSupportException (objectType);
				}
			}
			throw new JsonException ($"不支持将对象转为{type}");
		}

		object ConverterRead (Type type, object instance, JsonField field) {
			if (type is null) {
				throw new ArgumentNullException (nameof (type));
			}
			if (field?.HasConverter ?? false) {
				return JsonApi.ChangeType (field.Read (BuildValue (field.ConverterReadType, instance), Config), type, Config);
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