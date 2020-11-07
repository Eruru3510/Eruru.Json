using System;
using System.Collections.Generic;
using System.Text;

namespace Eruru.Json {

	public class JsonTextReaderException : Exception {

		public JsonTextReaderException (string message, Queue<int> buffer, JsonToken token) {
			if (message is null) {
				throw new ArgumentNullException (nameof (message));
			}
			if (buffer is null) {
				throw new ArgumentNullException (nameof (buffer));
			}
			StringBuilder stringBuilder = new StringBuilder ();
			stringBuilder.AppendLine (message);
			stringBuilder.AppendLine ($"类型：{token.Type} 索引：{token.Index} 长度：{token.Length} 值：{token.Value}");
			stringBuilder.AppendLine (new string (Array.ConvertAll (buffer.ToArray (), character => (char)character)));
			JsonAPI.SetExceptionMessage (this, stringBuilder.ToString ());
		}

	}

}