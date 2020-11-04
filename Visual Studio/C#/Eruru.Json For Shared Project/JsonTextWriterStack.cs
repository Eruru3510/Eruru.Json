namespace Eruru.Json {

	class JsonTextWriterStack {

		public bool HasValue;
		public JsonTextWriterStage Stage;

		public JsonTextWriterStack (JsonTextWriterStage stage) {
			Stage = stage;
		}

	}

}