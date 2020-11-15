using System;

namespace Eruru.Json {

	public enum JsonTextWriterStage {

		End = 1 << 0,
		Value = 1 << 1,
		FirstArrayValue = 1 << 2,
		ArrayValue = 1 << 3,
		FirstObjectKey = 1 << 4,
		ObjectKey = 1 << 5,
		ObjectValue = 1 << 6,
		Key = FirstObjectKey | ObjectKey

	}

}