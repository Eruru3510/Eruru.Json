namespace Eruru.Json {

	public interface IJsonBuilder {

		void BuildValue ();

		void BuildArray ();

		void BuildObject ();

	}

	public interface IJsonBuilder<Value, Array, Object> {

		Value BuildValue (Value value = default);

		Array BuildArray (Array array = default);

		Object BuildObject (Object jsonObject = default);

	}

}