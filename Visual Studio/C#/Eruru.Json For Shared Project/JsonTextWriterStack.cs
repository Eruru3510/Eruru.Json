namespace Eruru.Json {

	class JsonTextWriterStack {

		public JsonTextWriterStage Stage;

		public JsonTextWriterStack (JsonTextWriterStage stage) {
			Stage = stage;
		}

	}

}