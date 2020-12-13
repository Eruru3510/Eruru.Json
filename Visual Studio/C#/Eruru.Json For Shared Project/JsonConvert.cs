using System;
using System.IO;

namespace Eruru.Json {

	public static class JsonConvert {

		/// <summary>
		/// 将对象序列化为JsonValue
		/// </summary>
		/// <param name="instance">对象实例</param>
		/// <param name="config">配置实例（为空时使用JsonConfig.Default）</param>
		/// <returns>JsonValue</returns>
		public static JsonValue SerializeValue (object instance, JsonConfig config = null) {
			return new JsonValueBuilder (new JsonSerializer (instance, config), config).BuildValue ();
		}

		/// <summary>
		/// 将对象序列化为JsonArray
		/// </summary>
		/// <param name="instance">对象实例</param>
		/// <param name="config">配置实例（为空时使用JsonConfig.Default）</param>
		/// <returns>JsonArray</returns>
		public static JsonArray SerializeArray (object instance, JsonConfig config = null) {
			return new JsonValueBuilder (new JsonSerializer (instance, config), config).BuildArray ();
		}

		/// <summary>
		/// 将对象序列化为JsonValue
		/// </summary>
		/// <param name="instance">对象实例</param>
		/// <param name="config">配置实例（为空时使用JsonConfig.Default）</param>
		/// <returns>Json字符串</returns>
		public static JsonObject SerializeObject (object instance, JsonConfig config = null) {
			return new JsonValueBuilder (new JsonSerializer (instance, config), config).BuildObject ();
		}

		/// <summary>
		/// 将对象序列化为Json字符串
		/// </summary>
		/// <param name="instance">对象实例</param>
		/// <param name="config">配置实例（为空时使用JsonConfig.Default）</param>
		/// <returns>Json字符串</returns>
		public static string Serialize (object instance, JsonConfig config = null) {
			using (JsonTextBuilder builder = new JsonTextBuilder (new JsonSerializer (instance, config), new StringWriter (), config)) {
				builder.BuildValue ();
				return builder.ToString ();
			}
		}

		/// <summary>
		/// 将对象序列化到文件
		/// </summary>
		/// <param name="instance">对象实例</param>
		/// <param name="path">文件保存路径</param>
		/// <param name="config">配置实例（为空时使用JsonConfig.Default）</param>
		public static void Serialize (object instance, string path, JsonConfig config = null) {
			if (JsonApi.IsNullOrWhiteSpace (path)) {
				throw new ArgumentException ($"“{nameof (path)}”不能为 Null 或空白", nameof (path));
			}
			using (JsonTextBuilder builder = new JsonTextBuilder (new JsonSerializer (instance, config), new StreamWriter (path), config)) {
				builder.BuildValue ();
			}
		}

		/// <summary>
		/// 将对象序列化到TextWriter
		/// </summary>
		/// <param name="instance">对象实例</param>
		/// <param name="textWriter">TextWriter实例</param>
		/// <param name="config">配置实例（为空时使用JsonConfig.Default）</param>
		public static void Serialize (object instance, TextWriter textWriter, JsonConfig config = null) {
			if (textWriter is null) {
				throw new ArgumentNullException (nameof (textWriter));
			}
			using (JsonTextBuilder builder = new JsonTextBuilder (new JsonSerializer (instance, config), textWriter, config)) {
				builder.BuildValue ();
			}
		}

		/// <summary>
		/// 将对象序列化为Json字符串
		/// </summary>
		/// <param name="instance">对象实例</param>
		/// <param name="compress">是否压缩（缩进）Json字符串</param>
		/// <param name="config">配置实例（为空时使用JsonConfig.Default）</param>
		/// <returns>Json字符串</returns>
		public static string Serialize (object instance, bool compress, JsonConfig config = null) {
			using (JsonTextBuilder builder = new JsonTextBuilder (new JsonSerializer (instance, config), new StringWriter (), compress, config)) {
				builder.BuildValue ();
				return builder.ToString ();
			}
		}

		/// <summary>
		/// 将对象序列化到文件
		/// </summary>
		/// <param name="instance">对象实例</param>
		/// <param name="path"></param>
		/// <param name="compress">是否压缩（缩进）Json字符串</param>
		/// <param name="config">配置实例（为空时使用JsonConfig.Default）</param>
		public static void Serialize (object instance, string path, bool compress, JsonConfig config = null) {
			if (JsonApi.IsNullOrWhiteSpace (path)) {
				throw new ArgumentException ($"“{nameof (path)}”不能为 Null 或空白", nameof (path));
			}
			using (JsonTextBuilder builder = new JsonTextBuilder (new JsonSerializer (instance, config), new StreamWriter (path), compress, config)) {
				builder.BuildValue ();
			}
		}

		/// <summary>
		/// 将对象序列化到TextWriter
		/// </summary>
		/// <param name="instance">对象实例</param>
		/// <param name="textWriter">TextWriter实例</param>
		/// <param name="compress">是否压缩（缩进）Json字符串</param>
		/// <param name="config">配置实例（为空时使用JsonConfig.Default）</param>
		public static void Serialize (object instance, TextWriter textWriter, bool compress, JsonConfig config = null) {
			if (textWriter is null) {
				throw new ArgumentNullException (nameof (textWriter));
			}
			using (JsonTextBuilder builder = new JsonTextBuilder (new JsonSerializer (instance, config), textWriter, compress, config)) {
				builder.BuildValue ();
			}
		}

		/// <summary>
		/// 将JsonValue反序列化为对象
		/// </summary>
		/// <typeparam name="T">对象类型</typeparam>
		/// <param name="value">JsonValue</param>
		/// <param name="config">配置实例（为空时使用JsonConfig.Default）</param>
		/// <returns>对象实例</returns>
		public static T Deserialize<T> (JsonValue value, JsonConfig config = null) {
			if (value is null) {
				throw new ArgumentNullException (nameof (value));
			}
			return new JsonDeserializer (new JsonValueReader (value), config).BuildValue<T> ();
		}

		/// <summary>
		/// 将JsonArray反序列化为对象
		/// </summary>
		/// <typeparam name="T">对象类型</typeparam>
		/// <param name="array">JsonArray</param>
		/// <param name="config">配置实例（为空时使用JsonConfig.Default）</param>
		/// <returns>对象实例</returns>
		public static T Deserialize<T> (JsonArray array, JsonConfig config = null) {
			if (array is null) {
				throw new ArgumentNullException (nameof (array));
			}
			return new JsonDeserializer (new JsonValueReader (array), config).BuildArray<T> ();
		}

		/// <summary>
		/// 将JsonObject反序列化为对象
		/// </summary>
		/// <typeparam name="T">对象类型</typeparam>
		/// <param name="jsonObject">JsonObject</param>
		/// <param name="config">配置实例（为空时使用JsonConfig.Default）</param>
		/// <returns>对象实例</returns>
		public static T Deserialize<T> (JsonObject jsonObject, JsonConfig config = null) {
			if (jsonObject is null) {
				throw new ArgumentNullException (nameof (jsonObject));
			}
			return new JsonDeserializer (new JsonValueReader (jsonObject), config).BuildObject<T> ();
		}

		/// <summary>
		/// 将Json字符串反序列化为对象
		/// </summary>
		/// <typeparam name="T">对象类型</typeparam>
		/// <param name="text">Json字符串</param>
		/// <param name="config">配置实例（为空时使用JsonConfig.Default）</param>
		/// <returns>对象实例</returns>
		public static T Deserialize<T> (string text, JsonConfig config = null) {
			if (text is null) {
				throw new ArgumentNullException (nameof (text));
			}
			using (JsonTextReader reader = new JsonTextReader (new StringReader (text), config)) {
				return new JsonDeserializer (reader, config).BuildValue<T> ();
			}
		}

		/// <summary>
		/// 将TextReader反序列化为对象
		/// </summary>
		/// <typeparam name="T">对象类型</typeparam>
		/// <param name="textReader">TextReader</param>
		/// <param name="config">配置实例（为空时使用JsonConfig.Default）</param>
		/// <returns>对象实例</returns>
		public static T Deserialize<T> (TextReader textReader, JsonConfig config = null) {
			if (textReader is null) {
				throw new ArgumentNullException (nameof (textReader));
			}
			using (JsonTextReader reader = new JsonTextReader (textReader, config)) {
				return new JsonDeserializer (reader, config).BuildValue<T> ();
			}
		}

		/// <summary>
		/// 将Json字符串反序列化为对象
		/// </summary>
		/// <typeparam name="T">对象类型</typeparam>
		/// <param name="path">文件路径</param>
		/// <param name="config">配置实例（为空时使用JsonConfig.Default）</param>
		/// <returns>对象实例</returns>
		public static T DeserializeFile<T> (string path, JsonConfig config = null) {
			if (JsonApi.IsNullOrWhiteSpace (path)) {
				throw new ArgumentException ($"“{nameof (path)}”不能为 Null 或空白", nameof (path));
			}
			using (JsonTextReader reader = new JsonTextReader (new StreamReader (path), config)) {
				return new JsonDeserializer (reader, config).BuildValue<T> ();
			}
		}

		/// <summary>
		/// 将JsonValue反序列化为对象
		/// </summary>
		/// <typeparam name="T">对象类型</typeparam>
		/// <param name="value">JsonValue</param>
		/// <param name="instance">复用对象实例</param>
		/// <param name="config">配置实例（为空时使用JsonConfig.Default）</param>
		/// <returns>对象实例</returns>
		public static T Deserialize<T> (JsonValue value, T instance, JsonConfig config = null) {
			if (value is null) {
				throw new ArgumentNullException (nameof (value));
			}
			return new JsonDeserializer (new JsonValueReader (value), config).BuildValue<T> (instance);
		}

		/// <summary>
		/// 将JsonArray反序列化为对象
		/// </summary>
		/// <typeparam name="T">对象类型</typeparam>
		/// <param name="array">JsonArray</param>
		/// <param name="instance">复用对象实例</param>
		/// <param name="config">配置实例（为空时使用JsonConfig.Default）</param>
		/// <returns>对象实例</returns>
		public static T Deserialize<T> (JsonArray array, T instance, JsonConfig config = null) {
			if (array is null) {
				throw new ArgumentNullException (nameof (array));
			}
			return new JsonDeserializer (new JsonValueReader (array), config).BuildArray<T> (instance);
		}

		/// <summary>
		/// 将JsonObject反序列化为对象
		/// </summary>
		/// <typeparam name="T">对象类型</typeparam>
		/// <param name="jsonObject">JsonObject</param>
		/// <param name="instance">复用对象实例</param>
		/// <param name="config">配置实例（为空时使用JsonConfig.Default）</param>
		/// <returns>对象实例</returns>
		public static T Deserialize<T> (JsonObject jsonObject, T instance, JsonConfig config = null) {
			if (jsonObject is null) {
				throw new ArgumentNullException (nameof (jsonObject));
			}
			return new JsonDeserializer (new JsonValueReader (jsonObject), config).BuildObject<T> (instance);
		}

		/// <summary>
		/// 将Json字符串反序列化为对象
		/// </summary>
		/// <typeparam name="T">对象类型</typeparam>
		/// <param name="text">Json字符串</param>
		/// <param name="instance">复用对象实例</param>
		/// <param name="config">配置实例（为空时使用JsonConfig.Default）</param>
		/// <returns>对象实例</returns>
		public static T Deserialize<T> (string text, T instance, JsonConfig config = null) {
			if (text is null) {
				throw new ArgumentNullException (nameof (text));
			}
			using (JsonTextReader reader = new JsonTextReader (new StringReader (text), config)) {
				return new JsonDeserializer (reader, config).BuildValue<T> (instance);
			}
		}

		/// <summary>
		/// 将TextReader反序列化为对象
		/// </summary>
		/// <typeparam name="T">对象类型</typeparam>
		/// <param name="textReader">TextReader</param>
		/// <param name="instance">复用对象实例</param>
		/// <param name="config">配置实例（为空时使用JsonConfig.Default）</param>
		/// <returns>对象实例</returns>
		public static T Deserialize<T> (TextReader textReader, T instance, JsonConfig config = null) {
			if (textReader is null) {
				throw new ArgumentNullException (nameof (textReader));
			}
			using (JsonTextReader reader = new JsonTextReader (textReader, config)) {
				return new JsonDeserializer (reader, config).BuildValue<T> (instance);
			}
		}

		/// <summary>
		/// 将Json字符串反序列化为对象
		/// </summary>
		/// <typeparam name="T">对象类型</typeparam>
		/// <param name="path">文件路径</param>
		/// /// <param name="instance">复用对象实例</param>
		/// <param name="config">配置实例（为空时使用JsonConfig.Default）</param>
		/// <returns>对象实例</returns>
		public static T DeserializeFile<T> (string path, T instance, JsonConfig config = null) {
			if (JsonApi.IsNullOrWhiteSpace (path)) {
				throw new ArgumentException ($"“{nameof (path)}”不能为 Null 或空白", nameof (path));
			}
			using (JsonTextReader reader = new JsonTextReader (new StreamReader (path), config)) {
				return new JsonDeserializer (reader, config).BuildValue<T> (instance);
			}
		}

	}

}