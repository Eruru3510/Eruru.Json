using System;
using Eruru.Json;

namespace ConsoleApp1 {

	class Program {

		class Data {

			public int Number = 0;
			public Data ChildData = null;

		}

		static void Main (string[] args) {
			Console.Title = nameof (ConsoleApp1);
			JsonConfig jsonConfig = new JsonConfig () {
				IgnoreNull = true,
				Compress = false
			};
			Console.WriteLine (JsonConvert.Serialize (new Data (), jsonConfig));
			jsonConfig.IgnoreDefault = true;
			Console.WriteLine (JsonConvert.Serialize (new Data (), jsonConfig));
			Console.ReadLine ();
		}

	}

}