using System;
using System.Collections.Generic;
using Eruru.Json;

namespace ConsoleApp1 {

	class Program {

		static void Main (string[] args) {
			Console.Title = nameof (ConsoleApp1);
			KeyValuePair<string, int> keyValuePair = JsonConvert.Deserialize<KeyValuePair<string, int>> ("{'A':1}");
			Console.ReadLine ();
		}

	}

}