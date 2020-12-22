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

	}

}