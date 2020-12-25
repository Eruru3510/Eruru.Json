namespace Eruru.Json {

	public enum JsonValueType {

		Null = 1 << 0,
		Decimal = 1 << 1,
		Integer = 1 << 2,
		Bool = 1 << 3,
		String = 1 << 4,
		DateTime = 1 << 5,
		Array = 1 << 6,
		Object = 1 << 7,
		Value = Null | Decimal | Integer | Bool | String | DateTime

	}

}