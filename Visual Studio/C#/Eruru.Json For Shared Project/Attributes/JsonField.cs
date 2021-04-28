using System;
using System.Collections.Generic;

namespace Eruru.Json {

	[AttributeUsage (AttributeTargets.Field | AttributeTargets.Property)]
	public class JsonField : Attribute {

		public string Name { get; }
		public bool HasConverter { get; }
		public Type ConverterReadType { get; }
		public Type ConverterWriteType { get; }

		static readonly Dictionary<int, JsonConverter> CachedConverters = new Dictionary<int, JsonConverter> ();

		readonly JsonConverter[] Converters;

		public JsonField () {

		}
		public JsonField (string name) {
			Name = name ?? throw new ArgumentNullException (nameof (name));
		}
		public JsonField (params Type[] converters) {
			if (converters is null) {
				throw new ArgumentNullException (nameof (converters));
			}
			Converters = Array.ConvertAll (converters, converter => {
				if (converter is null) {
					throw new NullReferenceException (nameof (converter));
				}
				if (CachedConverters.TryGetValue (converter.GetHashCode (), out JsonConverter cachedConverter)) {
					return cachedConverter;
				}
				cachedConverter = new JsonConverter (converter);
				CachedConverters.Add (cachedConverter.GetHashCode (), cachedConverter);
				return cachedConverter;
			});
			HasConverter = Converters.Length > 0;
			ConverterReadType = Converters[0].BeforeType;
			ConverterWriteType = Converters[Converters.Length - 1].BeforeType;
		}
		public JsonField (string name, params Type[] converters) : this (converters) {
			Name = name ?? throw new ArgumentNullException (nameof (name));
		}

		public object ConverterRead (object value = null, JsonConfig config = null) {
			if (HasConverter) {
				if (config is null) {
					config = JsonConfig.Default;
				}
				for (int i = 0; i < Converters.Length; i++) {
					value = Converters[i].Read (value, config);
				}
			}
			return value;
		}

		public object ConverterWrite (object value = null, JsonConfig config = null) {
			if (HasConverter) {
				if (config is null) {
					config = JsonConfig.Default;
				}
				for (int i = Converters.Length - 1; i >= 0; i--) {
					value = Converters[i].Write (value, config);
				}
			}
			return value;
		}

	}

}