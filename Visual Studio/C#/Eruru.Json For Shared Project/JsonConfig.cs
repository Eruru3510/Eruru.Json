namespace Eruru.Json {

	public class JsonConfig {

		public static JsonConfig Default { get; } = new JsonConfig ();

		public bool Compress { get; set; } = true;
		public bool IgnoreCase { get; set; } = true;
		public bool IgnoreNull { get; set; } = false;
		public bool StringEnum { get; set; } = true;
		public bool UTCTime { get; set; } = true;
		public string IndentString { get; set; } = "\t";

		public JsonConfig () {

		}
		public JsonConfig (bool compress) {
			Compress = compress;
		}

	}

}