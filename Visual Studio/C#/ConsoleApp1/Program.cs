using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using Eruru.Json;

namespace ConsoleApp1 {

	class Program {

		static void Main (string[] args) {
			Console.Title = nameof (ConsoleApp1);
			Console.WriteLine (new JsonValue ("100") + 200);
			Console.WriteLine (new JsonValue ("100") + (byte)200 + 100);
			Console.ReadLine ();
		}

	}

}