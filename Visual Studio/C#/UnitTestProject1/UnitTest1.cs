using System;
using System.Collections.Generic;
using System.IO;
using Eruru.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1 {

	[TestClass]
	public class UnitTest1 {

		[TestInitialize]
		public void Initialize () {

		}

		public class Account {

			public string Email { get; set; }
			public bool Active { get; set; }
			public DateTime CreatedDate { get; set; }
			public IList<string> Roles { get; set; }

		}

		public class Movie {

			public string Name { get; set; }
			public int Year { get; set; }

		}

		[TestMethod]
		public void SerializeAnObject () {
			Account account = new Account {
				Email = "james@example.com",
				Active = true,
				CreatedDate = new DateTime (2013, 1, 20, 0, 0, 0, DateTimeKind.Utc),
				Roles = new List<string> () {
					"User",
					"Admin"
				}
			};
			string json = JsonSerializer.Serialize (account, false);
			//{
			//	"Email": "james@example.com",
			//	"Active": true,
			//	"CreatedDate": "2013-01-20T00:00:00Z",
			//	"Roles": [
			//		"User",
			//		"Admin"
			//	]
			//}
			Console.WriteLine (json);
			Assert.AreEqual (
			$"{{{Environment.NewLine}" +
				$"\t\"Email\": \"james@example.com\",{Environment.NewLine}" +
				$"\t\"Active\": true,{Environment.NewLine}" +
				$"\t\"CreatedDate\": \"2013-01-20T00:00:00Z\",{Environment.NewLine}" +
				$"\t\"Roles\": [{Environment.NewLine}" +
				$"\t\t\"User\",{Environment.NewLine}" +
				$"\t\t\"Admin\"{Environment.NewLine}" +
				$"\t]{Environment.NewLine}" +
			"}", json);
		}

		[TestMethod]
		public void SerializeJsonToAFile () {
			Movie movie = new Movie {
				Name = "Bad Boys",
				Year = 1995
			};
			string path = @"d:\movie.json";
			File.WriteAllText (path, JsonSerializer.Serialize (movie));
			using (StreamWriter file = File.CreateText (path)) {
				JsonSerializer.SerializeStreamWriter (file, movie);
			}
			string json = File.ReadAllText (path);
			Console.WriteLine (json);
			Assert.AreEqual ("{\"Name\":\"Bad Boys\",\"Year\":1995}", json);
		}

		[TestMethod]
		public void DeserializeAnObject () {
			string json = @"{
				'Email': 'james@example.com',
				'Active': true,
				'CreatedDate': '2013-01-20T00:00:00Z',
				'Roles': [
					'User',
					'Admin'
				]
			}";
			Account account = JsonDeserializer.Deserialize<Account> (json);
			Console.WriteLine (account.Email);
			//james@example.com
			Assert.AreEqual ("james@example.com", account.Email);
		}

		[TestMethod]
		public void DeserializeJsonFromAFile () {
			string path = @"d:\movie.json";
			Movie movie = JsonDeserializer.Deserialize<Movie> (File.ReadAllText (path));
			using (StreamReader file = File.OpenText (path)) {
				Movie movie2 = JsonDeserializer.Deserialize<Movie> (file);
			}
		}

		[TestMethod]
		public void ParsingJsonObjectUsingJsonObject_Parse () {
			string json = @"{
				'CPU': 'Intel',
				'Drives': [
					'DVD read/writer',
					'500 gigabyte hard drive'
				]
			}";
			JsonObject jsonObject = JsonObject.Parse (json);
			Console.WriteLine (jsonObject.ToString ());
			//{
			//	"CPU": "Intel",
			//	"Drives": [
			//		"DVD read/writer",
			//		"500 gigabyte hard drive"
			//	]
			//}
			Assert.AreEqual ("{\"CPU\":\"Intel\",\"Drives\":[\"DVD read/writer\",\"500 gigabyte hard drive\"]}", jsonObject.ToString ());
		}

	}

}