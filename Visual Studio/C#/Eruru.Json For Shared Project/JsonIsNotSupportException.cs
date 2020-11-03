using System;

namespace Eruru.Json {

	public class JsonIsNotSupportException : Exception {

		public JsonIsNotSupportException (object value) {
			JsonAPI.SetExceptionMessage (this, $"{value} is not supported");
		}

	}

}