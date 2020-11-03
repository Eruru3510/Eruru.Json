using System;

namespace Eruru.Json {

	[Flags]
	public enum JsonTokenType {

		Unknown = 1 << 0,
		Null = 1 << 1,
		Decimal = 1 << 2,
		Long = 1 << 3,
		Bool = 1 << 4,
		String = 1 << 5,
		Comma = 1 << 6,
		Semicolon = 1 << 7,
		LeftBracket = 1 << 8,
		RightBracket = 1 << 9,
		LeftBrace = 1 << 10,
		RightBrace = 1 << 11,
		Value = Null | Long | Decimal | Bool | String

	}

}