namespace Eruru.Json {

	public class JsonConfig {

		public bool Compress = true;
		public bool IgnoreCase = true;
		public bool StringEnum = true;
		public bool UTCTime = true;

		public static JsonConfig Default { get; } = new JsonConfig ();

		public JsonConfig () {

		}
		public JsonConfig (bool compress) {
			Compress = compress;
		}

	}

}