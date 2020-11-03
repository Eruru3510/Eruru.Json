namespace Eruru.Json {

	public struct JsonToken {

		public JsonTokenType Type { get; set; }
		public object Value { get; set; }
		public int Index { get; set; }
		public int Length { get; set; }

	}

}