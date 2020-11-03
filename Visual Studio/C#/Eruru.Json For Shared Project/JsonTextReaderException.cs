using System;
using System.Collections.Generic;
using System.Text;

namespace Eruru.Json {

	public class JsonTokenReaderException : Exception {

		public JsonTokenReaderException (string message, Queue<int> buffer, JsonToken token) {
			if (message is null) {
				throw new ArgumentNullException (nameof (message));
			}
			if (buffer is null) {
				throw new ArgumentNullException (nameof (buffer));
			}
			StringBuilder stringBuilder = new StringBuilder ();
			stringBuilder.AppendLine (message);
			stringBuilder.AppendLine ($"Type: {token.Type} Index: {token.Index} Length: {token.Length} Value: {token.Value}");
			stringBuilder.AppendLine (new string (Array.ConvertAll (buffer.ToArray (), character => (char)character)));
			JsonAPI.SetExceptionMessage (this, stringBuilder.ToString ());
		}

	}

}