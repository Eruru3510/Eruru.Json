using System;
using System.Text;
using Eruru.TextTokenizer;

namespace Eruru.Json {

	public class JsonTextReaderException : Exception {

		public JsonTextReaderException (TextTokenizer<JsonTokenType> textTokenizer, params object[] values) {
			if (textTokenizer is null) {
				throw new ArgumentNullException (nameof (textTokenizer));
			}
			if (values is null) {
				throw new ArgumentNullException (nameof (values));
			}
			StringBuilder stringBuilder = new StringBuilder ();
			stringBuilder.AppendLine ($"期望是{string.Join ("或", Array.ConvertAll (values, value => value.ToString ()))}");
			stringBuilder.AppendLine (
				$"类型：{textTokenizer.Current.Type} " +
				$"位置：{textTokenizer.Current.StartIndex} " +
				$"长度：{textTokenizer.Current.Length} " +
				$"值：{textTokenizer.Current.Value}"
			);
			stringBuilder.AppendLine (new string (textTokenizer.Buffer.ToArray ()));
			this.SetMessage (stringBuilder.ToString ());
		}

	}

}