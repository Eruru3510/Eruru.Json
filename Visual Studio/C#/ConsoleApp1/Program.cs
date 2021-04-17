using System;
using System.Reflection;
using Eruru.Json;

namespace ConsoleApp1 {

	class Data {

		public BindingFlags BindingFlags;

	}

	class Program {

		static void Main (string[] args) {
			Console.Title = nameof (ConsoleApp1);
			Console.WriteLine (JsonConvert.Serialize (new Data (), false));
			Console.ReadLine ();
		}

	}

}