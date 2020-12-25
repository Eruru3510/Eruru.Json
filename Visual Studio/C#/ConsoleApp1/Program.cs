using System;
using Eruru.Json;

namespace ConsoleApp1 {

	class Program {

		static void Main (string[] args) {
			Console.Title = nameof (ConsoleApp1);
			try {
				Test ();
			} catch (Exception exception) {
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine (exception);
			}
			Console.ReadLine ();
		}

		class Data {

			public int[,] ints = new int[1, 2];

		}

		static void Test () {
			foreach (JsonObject jsonObject in new JsonValue (new JsonArray (new JsonObject (), new JsonObject ()))) {
				Console.WriteLine (jsonObject.Serialize ());
			}
		}

	}

}