using System.IO;

namespace Eruru.Json {

	public interface IJsonSerializable {

		string Serialize (JsonConfig config = null);
		void Serialize (string path, JsonConfig config = null);
		void Serialize (TextWriter textWriter, JsonConfig config = null);
		string Serialize (bool compress, JsonConfig config = null);
		void Serialize (string path, bool compress, JsonConfig config = null);
		void Serialize (TextWriter textWriter, bool compress, JsonConfig config = null);

	}

}