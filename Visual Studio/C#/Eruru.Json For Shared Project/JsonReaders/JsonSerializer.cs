﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Eruru.Json {

	public class JsonSerializer : IJsonReader {

		readonly Stack<JsonSerializerStack> Stacks = new Stack<JsonSerializerStack> ();
		readonly JsonConfig Config;

		JsonAction ForEachArray;

		public JsonSerializer (object instance, JsonConfig config = null) {
			Config = config ?? JsonConfig.Default;
			Stacks.Push (new JsonSerializerStack (instance));
		}

		void Initialize (bool isArray) {
			if (Stacks.Peek ().IsInitialized) {
				return;
			}
			Stacks.Peek ().IsInitialized = true;
			if (Stacks.Peek ().Instance is null) {
				return;
			}
			if (Stacks.Peek ().Type is null) {
				Stacks.Peek ().Type = Stacks.Peek ().Instance.GetType ();
			}
			if (isArray) {
				JsonApi.TryGetArrayType (Stacks.Peek ().Type, out Stacks.Peek ().ArrayType);
				return;
			}
			JsonApi.TryGetObjectType (Stacks.Peek ().Type, out Stacks.Peek ().ObjectType);
		}

		#region IJsonReader

		public void ReadValue (JsonAction<object, JsonValueType> value, JsonAction readArray, JsonAction readObject) {
			if (value is null) {
				throw new ArgumentNullException (nameof (value));
			}
			if (readArray is null) {
				throw new ArgumentNullException (nameof (readArray));
			}
			if (readObject is null) {
				throw new ArgumentNullException (nameof (readObject));
			}
			if (Stacks.Peek ().ArrayType != JsonArrayType.Unknown) {
				readArray ();
				return;
			}
			if (Stacks.Peek ().ObjectType != JsonObjectType.Unknown) {
				readObject ();
				return;
			}
			Stacks.Peek ().IsInitialized = true;
			if (Stacks.Peek ().Instance is null) {
				value (null, JsonValueType.Null);
				return;
			}
			if (Stacks.Peek ().Type is null) {
				Stacks.Peek ().Type = Stacks.Peek ().Instance.GetType ();
			}
			if (JsonApi.TryGetValueType (Stacks.Peek ().Type, out JsonValueType valueType, Config)) {
				value (Stacks.Peek ().Instance, valueType);
				return;
			}
			if (JsonApi.TryGetArrayType (Stacks.Peek ().Type, out Stacks.Peek ().ArrayType)) {
				readArray ();
				return;
			}
			if (JsonApi.TryGetObjectType (Stacks.Peek ().Type, out Stacks.Peek ().ObjectType)) {
				readObject ();
				return;
			}
			throw new JsonNotSupportException (Stacks.Peek ().Type);
		}

		public void ReadArray (Action<int> readValue) {
			if (readValue is null) {
				throw new ArgumentNullException (nameof (readValue));
			}
			Initialize (true);
			switch (Stacks.Peek ().ArrayType) {
				default:
					throw new JsonNotSupportException (Stacks.Peek ().ArrayType);
				case JsonArrayType.Array: {
					if (ForEachArray is null) {
						Array array = (Array)Stacks.Peek ().Instance;
						if (array.Rank == 1) {
							for (int i = 0; i < array.Length; i++) {
								Stacks.Push (new JsonSerializerStack (array.GetValue (i)));
								readValue (i);
								Stacks.Pop ();
							}
							return;
						}
						int[] indices = new int[array.Rank];
						int dimension = 0;
						int count = 0;
						ForEachArray = () => {
							int length = array.GetLength (dimension);
							for (int i = 0; i < length; i++) {
								indices[dimension] = i;
								if (dimension == indices.Length - 1) {
									Stacks.Push (new JsonSerializerStack (array.GetValue (indices)));
									readValue (count++);
									Stacks.Pop ();
									continue;
								}
								dimension++;
								readValue (count++);
							}
							dimension--;
							if (dimension < 0) {
								ForEachArray = null;
							}
						};
					}
					ForEachArray ();
					break;
				}
				case JsonArrayType.GenericList:
				case JsonArrayType.GenericIList:
				case JsonArrayType.GenericObservableCollection: {
					IList list = (IList)Stacks.Peek ().Instance;
					for (int i = 0; i < list.Count; i++) {
						Stacks.Push (new JsonSerializerStack (list[i]));
						readValue (i);
						Stacks.Pop ();
					}
					break;
				}
				case JsonArrayType.DataTable: {
					DataTable dataTable = (DataTable)Stacks.Peek ().Instance;
					for (int i = 0; i < dataTable.Rows.Count; i++) {
						Stacks.Push (new JsonSerializerStack (dataTable.Rows[i]));
						readValue (i);
						Stacks.Pop ();
					}
					break;
				}
			}
		}

		public void ReadObject (JsonFunc<string, bool> key, JsonAction readValue) {
			if (key is null) {
				throw new ArgumentNullException (nameof (key));
			}
			if (readValue is null) {
				throw new ArgumentNullException (nameof (readValue));
			}
			Initialize (false);
			switch (Stacks.Peek ().ObjectType) {
				default:
					throw new JsonNotSupportException (Stacks.Peek ().ObjectType);
				case JsonObjectType.Class:
					JsonApi.ForEachSerializableMembers (Stacks.Peek ().Type, (memberInfo, fieldInfo, propertyInfo, field) => {
						bool isReaded = false;
						object instance = null;
						Type fieldType = null;
						object Read () {
							if (isReaded) {
								return instance;
							}
							isReaded = true;
							switch (memberInfo.MemberType) {
								default:
									throw new JsonNotSupportException (memberInfo.MemberType);
								case MemberTypes.Field:
									fieldType = fieldInfo.FieldType;
									return instance = fieldInfo.GetValue (Stacks.Peek ().Instance);
								case MemberTypes.Property:
									fieldType = propertyInfo.PropertyType;
									return instance = propertyInfo.GetValue (Stacks.Peek ().Instance, null);
							}
						}
						object ConverterWrite (object value) {
							if (field?.HasConverter ?? false) {
								if (field.ConverterWriteType != fieldType.BaseType &&
									JsonApi.GetElementType (field.ConverterWriteType) != JsonApi.GetElementType (fieldType).BaseType
								) {
									fieldType = field.ConverterWriteType;
								}
								return field.ConverterWrite (value, Config);
							}
							return value;
						}
						if (!JsonApi.CanSerializeValue (Read (), Config)) {
							return;
						}
						if (key (field?.Name ?? memberInfo.Name)) {
							Stacks.Push (new JsonSerializerStack (ConverterWrite (Read ()), field));
							Stacks.Peek ().Type = fieldType;
							readValue ();
							Stacks.Pop ();
						}
					});
					break;
				case JsonObjectType.DataRow: {
					DataRow dataRow = (DataRow)Stacks.Peek ().Instance;
					for (int i = 0; i < dataRow.Table.Columns.Count; i++) {
						if (key (dataRow.Table.Columns[i].ColumnName)) {
							Stacks.Push (new JsonSerializerStack (dataRow[i]));
							readValue ();
							Stacks.Pop ();
						}
					}
					break;
				}
				case JsonObjectType.DataSet: {
					DataSet dataSet = (DataSet)Stacks.Peek ().Instance;
					foreach (DataTable dataTable in dataSet.Tables) {
						if (key (dataTable.TableName)) {
							Stacks.Push (new JsonSerializerStack (dataTable));
							readValue ();
							Stacks.Pop ();
						}
					}
					break;
				}
				case JsonObjectType.GenericDictionary:
				case JsonObjectType.GenericSortedDictionary:
				case JsonObjectType.GenericSortedList: {
					IDictionary dictionary = (IDictionary)Stacks.Peek ().Instance;
					foreach (DictionaryEntry entry in dictionary) {
						if (key (Convert.ToString (entry.Key))) {
							Stacks.Push (new JsonSerializerStack (entry.Value));
							readValue ();
							Stacks.Pop ();
						}
					}
					break;
				}
				case JsonObjectType.GenericKeyValuePair: {
					if (key (Convert.ToString (Stacks.Peek ().Type.GetProperty ("Key").GetValue (Stacks.Peek ().Instance, null)))) {
						Stacks.Push (new JsonSerializerStack (Stacks.Peek ().Type.GetProperty ("Value").GetValue (Stacks.Peek ().Instance, null)));
						readValue ();
						Stacks.Pop ();
					}
					break;
				}
			}
		}

		#endregion

	}

}