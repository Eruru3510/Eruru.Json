namespace Eruru.Json {

	public interface IJsonBuilder {

		void BuildValue ();

		void BuildArray ();

		void BuildObject ();

	}

	public interface IJsonBuilder<Value, Array, Object> {

		Value BuildValue ();

		Array BuildArray ();

		Object BuildObject ();

	}

}