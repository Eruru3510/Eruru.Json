using System;
using System.Collections.Generic;

namespace Eruru.Json {

	[AttributeUsage (AttributeTargets.Field | AttributeTargets.Property)]
	public class JsonField : Attribute {

		public string Name { get; }
		public bool HasConverter {

			get => Converters?.Length > 0;

		}
		public Type ConverterReadType {

			get => Converters[0].BeforeType;

		}

		static readonly Dictionary<int, JsonConverter> CachedConverters = new Dictionary<int, JsonConverter> ();

		JsonConverter[] Converters;

		public JsonField () {

		}
		public JsonField (string name) {
			Name = name ?? throw new ArgumentNullException (nameof (name));
		}
		public JsonField (params Type[] converters) {
			if (converters is null) {
				throw new ArgumentNullException (nameof (converters));
			}
			SetConverters (converters);
		}
		public JsonField (string name, params Type[] converters) {
			if (converters is null) {
				throw new ArgumentNullException (nameof (converters));
			}
			Name = name;
			SetConverters (converters);
		}

		public object Read (object value, JsonConfig config) {
			if (config is null) {
				throw new ArgumentNullException (nameof (config));
			}
			if (HasConverter) {
				for (int i = 0; i < Converters.Length; i++) {
					value = Converters[i].Read (value, config);
				}
			}
			return value;
		}

		public object Write (object value, JsonConfig config) {
			if (config is null) {
				throw new ArgumentNullException (nameof (config));
			}
			if (HasConverter) {
				for (int i = Converters.Length - 1; i >= 0; i--) {
					value = Converters[i].Read (value, config);
				}
			}
			return value;
		}

		void SetConverters (Type[] converters) {
			if (converters is null) {
				throw new ArgumentNullException (nameof (converters));
			}
			Converters = Array.ConvertAll (converters, converter => {
				if (converter is null) {
					throw new ArgumentNullException (nameof (converter));
				}
				if (CachedConverters.TryGetValue (converter.GetHashCode (), out JsonConverter cachedConverter)) {
					return cachedConverter;
				}
				cachedConverter = new JsonConverter (converter);
				CachedConverters.Add (cachedConverter.GetHashCode (), cachedConverter);
				return cachedConverter;
			});
		}

	}

}