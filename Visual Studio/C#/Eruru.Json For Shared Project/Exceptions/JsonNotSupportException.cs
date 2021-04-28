using System;

namespace Eruru.Json {

	public class JsonNotSupportException : Exception {

		public JsonNotSupportException (object value) {
			this.SetMessage ($"不支持{value}");
		}

	}

}