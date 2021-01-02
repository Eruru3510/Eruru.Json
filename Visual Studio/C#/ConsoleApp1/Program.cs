using System;
using System.Data;
using System.IO;
using Eruru.Json;

namespace ConsoleApp1 {

	class Program {

		class Player {

			public int ID;
			public string Name;

		}

		static void Main (string[] args) {
			Console.Title = nameof (ConsoleApp1);
			SerializeDataSet ();
			Console.ReadLine ();
		}

		static void SerializeDataSet () {
			DataSet dataSet = new DataSet ("Data Set");
			DataTable dataTable = new DataTable ("Player");
			dataTable.Columns.Add ("ID");
			dataTable.Columns.Add ("Name");
			dataTable.Rows.Add (1, "Jack");
			dataTable.Rows.Add (2, "Steve");
			dataSet.Tables.Add (dataTable);
			dataTable = new DataTable ("Item");
			dataTable.Columns.Add ("Name");
			dataTable.Rows.Add ("Block");
			dataTable.Rows.Add ("Grass");
			dataSet.Tables.Add (dataTable);
			string json = JsonConvert.Serialize (dataSet, false);
			Console.WriteLine (json);
			JsonObject jsonObject = JsonObject.Parse (json);
			dataSet = JsonConvert.Deserialize (jsonObject, dataSet);
			json = JsonConvert.Serialize (dataSet, false);
			Console.WriteLine (jsonObject.Serialize (false));
		}

		static void WriteJson () {
			JsonArray array = new JsonArray (1);
			Player player = new Player ();
			JsonTextWriter textWriter = new JsonTextWriter (new StringWriter (), false);
			textWriter.BeginObject ();
			textWriter.Write ("array");
			textWriter.Write (array);
			textWriter.Write ("object");
			textWriter.Write (player);
			textWriter.EndObject ();
			Console.WriteLine (textWriter);
		}

	}

}