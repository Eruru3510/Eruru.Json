using System;
using System.Data;
using Eruru.Json;

namespace ConsoleApp1 {

	class Program {

		class Data {

			[JsonField (typeof (Converter))]
			public Student[] Students;
			[JsonField (typeof (Converter))]
			public Teacher[] Teachers;

		}

		abstract class People {

			public virtual void Say () {
				Console.WriteLine ("我是人");
			}

		}

		class Student : People {

			public string Name;

			public override void Say () {
				Console.WriteLine ("我是学生");
			}

		}

		class Teacher : People {

			public string Class;

			public override void Say () {
				Console.WriteLine ("我是老师");
			}

		}

		class Converter : IJsonConverter<People[], People[]> {

			public People[] Read (People[] value) {
				value[0].Say ();
				return value;
			}

			public People[] Write (People[] value) {
				value[0].Say ();
				return value;
			}

		}

		static void Main (string[] args) {
			Console.Title = nameof (ConsoleApp1);
			Test ();
			Console.ReadLine ();
		}

		static void Test () {
			Data data = JsonConvert.Deserialize<Data> ("{'Students':[{'Name':'学生'},{'Name':'学生'}],'Teachers':[{'Class':'教师'},{'Class':'教师'}]}");
			Console.WriteLine (JsonConvert.Serialize (data));
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

		/*
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
		*/
	}

}