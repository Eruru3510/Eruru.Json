using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

		public static string Serialize (object instance, JsonConfig config = null) {
			return BuildText (instance, new StringWriter (), config).ToString ();
		}
		public static void Serialize (object instance, string path, JsonConfig config = null) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			BuildText (instance, new StreamWriter (path), config);
		}
		public static void Serialize (object instance, Stream stream, JsonConfig config = null) {
			if (stream is null) {
				throw new ArgumentNullException (nameof (stream));
			}
			BuildText (instance, new StreamWriter (stream), config);
		}
		public static void Serialize (object instance, StreamWriter streamWriter, JsonConfig config = null) {
			if (streamWriter is null) {
				throw new ArgumentNullException (nameof (streamWriter));
			}
			BuildText (instance, streamWriter, config);
		}
		public static string Serialize (object instance, bool compress, JsonConfig config = null) {
			return BuildText (instance, new StringWriter (), compress, config).ToString ();
		}
		public static void Serialize (object instance, string path, bool compress, JsonConfig config = null) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			BuildText (instance, new StreamWriter (path), compress, config);
		}
		public static void Serialize (object instance, Stream stream, bool compress, JsonConfig config = null) {
			if (stream is null) {
				throw new ArgumentNullException (nameof (stream));
			}
			BuildText (instance, new StreamWriter (stream), compress, config);
		}
		public static void Serialize (object instance, StreamWriter streamWriter, bool compress, JsonConfig config = null) {
			if (streamWriter is null) {
				throw new ArgumentNullException (nameof (streamWriter));
			}
			BuildText (instance, streamWriter, compress, config);
		}

		public static JsonValue SerializeValue (object instance, JsonConfig config = null) {
			return BuildValue (instance, config).BuildValue ();
		}

		public static JsonArray SerializeArray (object instance, JsonConfig config = null) {
			return BuildValue (instance, config).BuildArray ();
		}

		public static JsonObject SerializeObject (object instance, JsonConfig config = null) {
			return BuildValue (instance, config).BuildObject ();
		}

		static JsonTextBuilder BuildText (object instance, TextWriter textWriter, JsonConfig config) {
			if (textWriter is null) {
				throw new ArgumentNullException (nameof (textWriter));
			}
			using (JsonTextBuilder builder = new JsonTextBuilder (new JsonSerializer (instance, config), textWriter)) {
				builder.BuildValue ();
				return builder;
			}
		}
		static JsonTextBuilder BuildText (object instance, TextWriter textWriter, bool compress, JsonConfig config) {
			if (textWriter is null) {
				throw new ArgumentNullException (nameof (textWriter));
			}
			using (JsonTextBuilder builder = new JsonTextBuilder (new JsonSerializer (instance, config), textWriter, compress)) {
				builder.BuildValue ();
				return builder;
			}
		}

		static JsonValueBuilder BuildValue (object instance, JsonConfig config) {
			return new JsonValueBuilder (new JsonSerializer (instance, config));
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
				value (Write (), JsonValueType.Null);
				return;
			}
			Stacks.Peek ().Type = Stacks.Peek ().Instance.GetType ();
			if (JsonAPI.TryGetValueType (Stacks.Peek ().Type, Config, out JsonValueType valueType)) {
				value (Write (), valueType);
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
			throw new JsonIsNotSupportException (Stacks.Peek ().Instance);
		}

		public void ReadArray (JsonAction readValue) {
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
								readValue ();
								Stacks.Pop ();
							}
							return;
						}
						int[] indices = new int[array.Rank];
						int dimension = 0;
						ForEachArray = () => {
							int length = array.GetLength (dimension);
							for (int i = 0; i < length; i++) {
								indices[dimension] = i;
								if (dimension == indices.Length - 1) {
									Stacks.Push (new JsonSerializerStack (array.GetValue (indices)));
									readValue ();
									Stacks.Pop ();
									continue;
								}
								dimension++;
								readValue ();
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
						readValue ();
						Stacks.Pop ();
					}
					break;
				}
				default:
					throw new JsonIsNotSupportException (Stacks.Peek ().Instance);
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
							}
							isRead = true;
							switch (memberInfo.MemberType) {
								case MemberTypes.Field:
									return instance = fieldInfo.GetValue (Stacks.Peek ().Instance);
								case MemberTypes.Property:
									return instance = propertyInfo.GetValue (Stacks.Peek ().Instance, null);
								default:
									throw new JsonIsNotSupportException (memberInfo.MemberType);
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
					throw new JsonIsNotSupportException (Stacks.Peek ().Instance);
			}
		}

		#endregion

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

		object Write () {
			return Stacks.Peek ().Field?.Write (Stacks.Peek ().Instance, Config) ?? Stacks.Peek ().Instance;
		}

	}

}