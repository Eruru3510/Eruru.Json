using System;

namespace Eruru.Json {

	[AttributeUsage (AttributeTargets.Field | AttributeTargets.Property)]
	public class JsonIgnoreField : Attribute {

	}

}