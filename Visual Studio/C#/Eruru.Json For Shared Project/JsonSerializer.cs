using System;
using System.Collections;
using System.Collections.Generic;
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
			Stacks.Peek ().Type = Stacks.Peek ().Instance.GetType ();
			if (isArray) {
				JsonAPI.TryGetArrayType (Stacks.Peek ().Type, out Stacks.Peek ().ArrayType);
				return;
			}
			JsonAPI.TryGetObjectType (Stacks.Peek ().Type, out Stacks.Peek ().ObjectType);
		}

		object ConverterWrite () {
			return Stacks.Peek ().Field?.Write (Stacks.Peek ().Instance, Config) ?? Stacks.Peek ().Instance;
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
			Stacks.Peek ().IsInitialized = true;
			if (Stacks.Peek ().Instance is null) {
				value (ConverterWrite (), JsonValueType.Null);
				return;
			}
			Stacks.Peek ().Type = Stacks.Peek ().Instance.GetType ();
			if (JsonAPI.TryGetValueType (Stacks.Peek ().Type, out JsonValueType valueType, Config)) {
				value (ConverterWrite (), valueType);
				return;
			}
			if (JsonAPI.TryGetArrayType (Stacks.Peek ().Type, out JsonArrayType arrayType)) {
				Stacks.Peek ().ArrayType = arrayType;
				readArray ();
				return;
			}
			if (JsonAPI.TryGetObjectType (Stacks.Peek ().Type, out JsonObjectType objectType)) {
				Stacks.Peek ().ObjectType = objectType;
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
									readValue (count);
									Stacks.Pop ();
									count++;
									continue;
								}
								dimension++;
								readValue (count);
								count++;
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
					Type elementType = Stacks.Peek ().Type.GetGenericArguments ()[0];
					int count = (int)Stacks.Peek ().Type.GetProperty (nameof (IList.Count)).GetValue (Stacks.Peek ().Instance, null);
					MethodInfo getItemMethod = Stacks.Peek ().Type.GetMethod ("get_Item");
					object[] getItemParameters = new object[1];
					for (int i = 0; i < count; i++) {
						getItemParameters[0] = i;
						Stacks.Push (new JsonSerializerStack (getItemMethod.Invoke (Stacks.Peek ().Instance, getItemParameters)));
						readValue (i);
						Stacks.Pop ();
					}
					break;
				}
				default:
					throw new JsonNotSupportException (Stacks.Peek ().ArrayType);
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
				case JsonObjectType.Class:
					JsonAPI.ForEachMembers (Stacks.Peek ().Type, (memberInfo, fieldInfo, propertyInfo, field) => {
						bool isRead = false;
						object instance = null;
						object Read () {
							if (isRead) {
								return instance;
							} else {
								isRead = true;
							}
							switch (memberInfo.MemberType) {
								case MemberTypes.Field:
									return instance = fieldInfo.GetValue (Stacks.Peek ().Instance);
								case MemberTypes.Property:
									return instance = propertyInfo.GetValue (Stacks.Peek ().Instance, null);
								default:
									throw new JsonNotSupportException (memberInfo.MemberType);
							}
						}
						if (Config.IgnoreNull && Read () is null) {
							return;
						}
						if (key (field?.Name ?? memberInfo.Name)) {
							Stacks.Push (new JsonSerializerStack (Read (), field));
							readValue ();
							Stacks.Pop ();
						}
					});
					break;
				default:
					throw new JsonNotSupportException (Stacks.Peek ().ObjectType);
			}
		}

		#endregion

	}

}