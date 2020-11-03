namespace Eruru.Json {

	public interface IJsonConverter<Before, After> {

		After Read (Before value);

		Before Write (After value);

	}

}