using System.Collections.Generic;
using System.IO;
using Eruru.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1 {

	[TestClass]
	public class UnitTest1 {

		[TestMethod]
		public void WarframeStat () {
			string path = @"..\..\..\Assets\warframestat.json";
			string json = File.ReadAllText (path);
			Assert.AreEqual (json, JsonObject.Load (path).Serialize (false));
		}

		[TestMethod]
		public void TestMethod1 () {
			Assert.AreEqual (true, JsonValue.Parse ("true").Bool);
		}

		[TestMethod]
		public void SerializeDictionary () {
			Dictionary<string, KeyValuePair<string, object>> dictionary = new Dictionary<string, KeyValuePair<string, object>> () {
				{ "Jack", new KeyValuePair<string, object> ("Age", 12 ) },
				{ "Steve", new KeyValuePair<string, object> ("Color", "Red" ) }
			};
			string json = JsonConvert.Serialize (dictionary);
			Assert.AreEqual ("{\"Jack\":{\"Age\":12},\"Steve\":{\"Color\":\"Red\"}}", json);
			Dictionary<string, KeyValuePair<string, object>> newDictionary = new Dictionary<string, KeyValuePair<string, object>> ();
			newDictionary = JsonConvert.Deserialize (json, newDictionary);
			Assert.AreEqual (json, JsonConvert.Serialize (newDictionary));
		}

	}

}