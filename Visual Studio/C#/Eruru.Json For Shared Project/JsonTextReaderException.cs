using System;
using System.Collections.Generic;
using System.Text;

namespace Eruru.Json {

	public class JsonTextReaderException : Exception {

		public JsonTextReaderException (string message, Queue<int> buffer, JsonToken token) {
			if (JsonApi.IsNullOrWhiteSpace (message)) {
				throw new ArgumentException ($"“{nameof (message)}”不能为 Null 或空白", nameof (message));
			}
			if (buffer is null) {
				throw new ArgumentNullException (nameof (buffer));
			}
			Initialize (message, buffer, token);
		}
		public JsonTextReaderException (Queue<int> buffer, JsonToken token, params object[] values) {
			if (values is null) {
				throw new ArgumentNullException (nameof (values));
			}
			if (buffer is null) {
				throw new ArgumentNullException (nameof (buffer));
			}
			Initialize ($"期望是{string.Join ("或", Array.ConvertAll (values, value => value.ToString ()))}", buffer, token);
		}

		void Initialize (string message, Queue<int> buffer, JsonToken token) {
			if (JsonApi.IsNullOrWhiteSpace (message)) {
				throw new ArgumentException ($"“{nameof (message)}”不能为 Null 或空白", nameof (message));
			}
			StringBuilder stringBuilder = new StringBuilder ();
			stringBuilder.AppendLine (message);
			stringBuilder.AppendLine ($"类型：{token.Type} 索引：{token.Index} 长度：{token.Length} 值：{token.Value}");
			stringBuilder.AppendLine (new string (Array.ConvertAll (buffer.ToArray (), character => (char)character)));
			JsonApi.SetExceptionMessage (this, stringBuilder.ToString ());
		}

	}

}