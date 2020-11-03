using System;
using System.Collections.Generic;
using Eruru.Json;

namespace ConsoleApp1 {

	class Program {

		class Data {

			public string Name { get; set; }
			public DateTime Date;
			public List<int> List;
			public List<List<int>> JaggedList;
			public int[] Array;
			public int[,] MultidimensionalArray;
			public int[][][] JaggedArray;

		}

		static void Main (string[] args) {
			Console.Title = nameof (ConsoleApp1);
			//JsonConfig.Default.Compress = false;
			Data data = new Data () {
				Name = "Eruru",
				Date = DateTime.Now,
				List = new List<int> () { 1, 2 },
				JaggedList = new List<List<int>> () {
					new List<int> () { 1 },
					new List<int> () { 1, 2 }
				},
				Array = new int[] { 1, 2 },
				MultidimensionalArray = new int[,] {
					{ 1, 2 },
					{ 3, 4 }
				},
				JaggedArray = new int[][][] {
					new int[][] { new int[] { 1 } },
					new int[][] { new int[] { 1, 2 } }
				}
			};
			string json = JsonSerializer.Serialize (data);
			data = JsonDeserializer.Deserialize<Data> (json);
			Console.WriteLine (JsonSerializer.Serialize (data));
			data = JsonDeserializer.Deserialize (json, data);
			Console.WriteLine (JsonSerializer.Serialize (data));
			Console.ReadLine ();
		}

	}

}