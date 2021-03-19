using System;
using System.Collections.Generic;
using Eruru.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1 {

	[TestClass]
	public class Serialize {

		[TestMethod]
		public void SerializeDictionary () {
			Dictionary<string, object> dictionary = new Dictionary<string, object> () {
				{ "A", 1 },
				{ "B", "2" },
				{ "C", 3.0 }
			};
			string expected = Format ("{'A':1,'B':'2','C':3.0}");
			string json = JsonConvert.Serialize (dictionary);
			Console.WriteLine (json);
			Assert.AreEqual (expected, json);

			dictionary.Add ("D", 4);
			json = JsonConvert.Serialize (JsonConvert.Deserialize (json, dictionary));
			Console.WriteLine (json);
			Assert.AreEqual (expected, json);

			dictionary.Remove ("C");
			json = JsonConvert.Serialize (JsonConvert.Deserialize (json, dictionary));
			Console.WriteLine (json);
			Assert.AreEqual (expected, json);
		}

		[TestMethod]
		public void SerializeSortedDictionary () {
			SortedDictionary<string, object> sortedDictionary = new SortedDictionary<string, object> () {
				{ "B", "2" },
				{ "C", 3.0 },
				{ "A", 1 }
			};
			string json = JsonConvert.Serialize (sortedDictionary);
			Console.WriteLine (json);
			Assert.AreEqual (Format ("{'A':1,'B':'2','C':3.0}"), json);
		}

		[TestMethod]
		public void SerializeList () {
			List<object> list = new List<object> () {
				1,
				2.0,
				"3"
			};
			string expected = Format ("[1,2.0,'3']");
			string json = JsonConvert.Serialize (list);
			Console.WriteLine (json);
			Assert.AreEqual (expected, json);

			list.Add (4);
			json = JsonConvert.Serialize (JsonConvert.Deserialize (json, list));
			Console.WriteLine (json);
			Assert.AreEqual (expected, json);

			list.RemoveAt (list.Count - 1);
			json = JsonConvert.Serialize (JsonConvert.Deserialize (json, list));
			Console.WriteLine (json);
			Assert.AreEqual (expected, json);
		}

		string Format (string text) {
			return text.Replace ('\'', '"');
		}

	}

}