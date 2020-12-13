using System;
using System.Collections.Generic;
using System.Text;
using Eruru.TextTokenizer;

namespace Eruru.Json {

	public class JsonTextReaderException : Exception {

		public JsonTextReaderException (string message, Queue<char> buffer, TextTokenizerToken<JsonTokenType> token) {
			if (JsonApi.IsNullOrWhiteSpace (message)) {
				throw new ArgumentException ($"“{nameof (message)}”不能为 Null 或空白", nameof (message));
			}
			if (buffer is null) {
				throw new ArgumentNullException (nameof (buffer));
			}
			if (JsonApi.IsNullOrWhiteSpace (message)) {
				throw new ArgumentException ($"“{nameof (message)}”不能为 Null 或空白", nameof (message));
			}
			StringBuilder stringBuilder = new StringBuilder ();
			stringBuilder.AppendLine (message);
			stringBuilder.AppendLine ($"类型：{token.Type} 位置：{token.Index} 长度：{token.Length} 值：{token.Value}");
			stringBuilder.AppendLine (new string (buffer.ToArray ()));
			JsonApi.SetExceptionMessage (this, stringBuilder.ToString ());
		}
		public JsonTextReaderException (Queue<char> buffer, TextTokenizerToken<JsonTokenType> token, params object[] values) : this (
			$"期望是{string.Join ("或", Array.ConvertAll (values, value => value.ToString ()))}",
			buffer,
			token
		) {
			if (buffer is null) {
				throw new ArgumentNullException (nameof (buffer));
			}
			if (values is null) {
				throw new ArgumentNullException (nameof (values));
			}
		}

	}

}