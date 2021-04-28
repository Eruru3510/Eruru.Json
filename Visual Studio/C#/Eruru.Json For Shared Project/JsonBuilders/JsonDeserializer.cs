using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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

		public object BuildValue (Type type = null, object instance = null) {
			object returnValue = null;
			Reader.ReadValue (
				(value, _) => returnValue = JsonApi.ChangeType (value, type, Config),
				() => returnValue = BuildArray (type, instance),
				() => returnValue = BuildObject (type, instance)
			);
			return returnValue;
		}

		public object BuildArray (Type type, object instance = null) {
			if (type is null) {
				throw new ArgumentNullException (nameof (type));
			}
			if (!JsonApi.TryGetArrayType (type, out JsonArrayType arrayType)) {
				throw new JsonNotSupportException (type);
			}
			switch (arrayType) {
				default:
					throw new JsonNotSupportException (arrayType);
				case JsonArrayType.Array: {
					JsonArray jsonArray = new JsonValueBuilder (Reader, Config).BuildArray ();
					List<int> bounds = new List<int> ();
					MeasureArray (jsonArray, ref bounds);
					if (bounds.Count != type.GetArrayRank ()) {
						throw new JsonException ("数组维数不匹配");
					}
					Type elementType = type.GetElementType ();
					Array array = null;
					bool create = false;
					if (instance?.GetType () != type) {
						create = true;
					} else {
						array = (Array)instance;
						for (int i = 0; i < array.Rank; i++) {
							if (array.GetLength (i) != bounds[i]) {
								create = true;
								break;
							}
						}
					}
					if (create) {
						if (bounds.Count == 1) {
							array = Array.CreateInstance (elementType, bounds[0]);
						} else {
							array = Array.CreateInstance (elementType, bounds.ToArray ());
						}
					}
					if (array.Rank == 1) {
						for (int i = 0; i < jsonArray.Count; i++) {
							JsonDeserializer deserializer = new JsonDeserializer (new JsonValueReader (jsonArray[i], Config), Config);
							array.SetValue (deserializer.BuildValue (elementType, array.GetValue (i)), i);
						}
						return array;
					}
					int[] indices = new int[bounds.Count];
					int dimension = 0;
					void ForEachArray (JsonArray currentArray) {
						int length = bounds[dimension];
						for (int i = 0; i < length; i++) {
							indices[dimension] = i;
							if (dimension == indices.Length - 1) {
								JsonDeserializer deserializer = new JsonDeserializer (new JsonValueReader (currentArray[i], Config), Config);
								array.SetValue (deserializer.BuildValue (elementType, array.GetValue (indices)), indices);
								continue;
							}
							dimension++;
							ForEachArray (currentArray[i]);
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
					if (instance?.GetType () != type) {
						if (type.IsInterface) {
							switch (arrayType) {
								default:
									throw new JsonNotSupportException (arrayType);
								case JsonArrayType.GenericIList:
									instance = JsonApi.CreateInstance (typeof (List<>).MakeGenericType (elementType));
									break;
							}
						} else {
							instance = JsonApi.CreateInstance (type);
						}
					}
					IList list = (IList)instance;
					int oldCount = list.Count;
					int newCount = 0;
					Reader.ReadArray (i => {
						newCount = i + 1;
						if (i < oldCount) {
							list[i] = BuildValue (elementType, list[i]);
							return;
						}
						list.Add (BuildValue (elementType));
					});
					while (list.Count > newCount) {
						list.RemoveAt (list.Count - 1);
					}
					return list;
				}
				case JsonArrayType.DataTable: {
					if (instance?.GetType () != type) {
						instance = JsonApi.CreateInstance (type);
					}
					DataTable dataTable = (DataTable)instance;
					int columnNumber = dataTable.Columns.Count;
					int columnIndex;
					ArrayList arrayList = null;
					object[] values = null;
					Reader.ReadArray (i => {
						bool hasRow = dataTable.Rows.Count > i;
						if (!hasRow) {
							if (i == 0) {
								arrayList = new ArrayList ();
							} else if (values is null) {
								values = new object[columnNumber];
							}
						}
						columnIndex = 0;
						Reader.ReadObject (columnName => {
							if (!hasRow && i == 0) {
								columnNumber++;
								dataTable.Columns.Add (columnName);
							}
							return true;
						}, () => {
							object value = BuildValue ();
							if (hasRow) {
								dataTable.Rows[i].ItemArray[columnIndex] = value;
								return;
							}
							if (i == 0) {
								arrayList.Add (value);
								return;
							}
							values[columnIndex] = value;
							columnIndex++;
						});
						if (!hasRow) {
							dataTable.Rows.Add (i == 0 ? arrayList.ToArray () : values);
						}
					});
					return dataTable;
				}
			}
		}

		public object BuildObject (Type type, object instance = null) {
			if (type is null) {
				throw new ArgumentNullException (nameof (type));
			}
			if (!JsonApi.TryGetObjectType (type, out JsonObjectType objectType)) {
				throw new JsonNotSupportException (type);
			}
			if (instance?.GetType () != type) {
				instance = JsonApi.CreateInstance (type);
			}
			switch (objectType) {
				default:
					throw new JsonNotSupportException (objectType);
				case JsonObjectType.Class: {
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
							default:
								throw new JsonNotSupportException (memberInfo.MemberType);
							case MemberTypes.Field:
								value = ConverterRead (fieldInfo.FieldType, fieldInfo.GetValue (instance), field);
								break;
							case MemberTypes.Property:
								value = ConverterRead (propertyInfo.PropertyType, propertyInfo.GetValue (instance, null), field);
								break;
						}
						if (!JsonApi.CanSerializeValue (value, Config)) {
							return;
						}
						switch (memberInfo.MemberType) {
							default:
								throw new JsonNotSupportException (memberInfo.MemberType);
							case MemberTypes.Field:
								fieldInfo.SetValue (instance, value);
								break;
							case MemberTypes.Property:
								propertyInfo.SetValue (instance, value, null);
								break;
						}
					});
					return instance;
				}
				case JsonObjectType.DataSet: {
					DataSet dataSet = (DataSet)instance;
					string tableName = null;
					Reader.ReadObject (name => {
						tableName = name;
						return true;
					}, () => {
						DataTable dataTable = dataSet.Tables[tableName];
						bool hasTable = dataTable != null;
						dataTable = BuildArray<DataTable> (dataTable);
						if (!hasTable) {
							dataSet.Tables.Add (dataTable);
						}
						dataTable.TableName = JsonApi.Naming (tableName, Config.NamingType);
					});
					return instance;
				}
				case JsonObjectType.GenericDictionary:
				case JsonObjectType.GenericSortedDictionary:
				case JsonObjectType.GenericSortedList: {
					Type keyType = type.GetGenericArguments ()[0];
					Type valueType = type.GetGenericArguments ()[1];
					IDictionary dictionary = (IDictionary)instance;
					dictionary.Clear ();
					object key = null;
					Reader.ReadObject (name => {
						key = JsonApi.ChangeType (JsonApi.Naming (name, Config.NamingType), keyType, Config);
						return true;
					}, () => {
						dictionary[key] = BuildValue (valueType);
					});
					return instance;
				}
				case JsonObjectType.GenericKeyValuePair: {
					Type keyType = type.GetGenericArguments ()[0];
					Type valueType = type.GetGenericArguments ()[1];
					Reader.ReadObject (name => {
						type.GetRuntimeField ("key").SetValue (instance, JsonApi.ChangeType (JsonApi.Naming (name, Config.NamingType), keyType, Config));
						return true;
					}, () => {
						FieldInfo fieldInfo = type.GetRuntimeField ("value");
						fieldInfo.SetValue (instance, BuildValue (valueType, fieldInfo.GetValue (instance)));
					});
					return instance;
				}
			}
		}

		object ConverterRead (Type type, object instance, JsonField field) {
			if (type is null) {
				throw new ArgumentNullException (nameof (type));
			}
			if (field?.HasConverter ?? false) {
				Type readType;
				if (field.ConverterReadType == type.BaseType || JsonApi.GetElementType (field.ConverterReadType) == JsonApi.GetElementType (type).BaseType) {//todo 让多个转换器之间也支持
					readType = type;
				} else {
					readType = field.ConverterReadType;
				}
				return JsonApi.ChangeType (field.ConverterRead (BuildValue (readType, instance), Config), type, Config);
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