using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Eruru.Json;

namespace ConsoleApp1 {

	class Program {

		class Data {

			[JsonField (typeof (IntDatasConverter))]
			public List<object> Datas;

		}

		class IntDatasConverter : IJsonConverter<List<object>, List<object>> {

			public List<object> Read (List<object> datas) {
				for (int i = 0; i < datas.Count; i++) {
					if (Type.GetTypeCode (datas[i].GetType ()) == TypeCode.Int64) {
						datas[i] = Convert.ToInt32 (datas[i]);
					}
				}
				return datas;
			}

			public List<object> Write (List<object> value) {
				throw new NotImplementedException ();
			}

		}

		static void Main (string[] args) {
			Console.Title = nameof (ConsoleApp1);
			Data data = JsonDeserializer.Deserialize<Data> ("{'datas':[1,'a']}");
			foreach (object value in data.Datas) {
				Console.WriteLine ($"类型：{value.GetType ()} 值：{value}");
			}
			Console.ReadLine ();
		}

	}

}