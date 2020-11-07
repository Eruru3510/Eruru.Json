using System;

namespace Eruru.Json {

	public class JsonNotSupportException : Exception {

		public JsonNotSupportException (object value) {
			JsonAPI.SetExceptionMessage (this, $"不支持{value}");
		}

	}

}