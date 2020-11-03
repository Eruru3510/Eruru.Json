using System.IO;

namespace Eruru.Json {

	public interface IJsonTextualization {

		string ToText (JsonConfig config = null);

		void Save (Stream stream, JsonConfig config = null);
		void Save (string path, JsonConfig config = null);

	}

}