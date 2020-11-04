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

		public class Person {

			public string Name { get; set; }
			public int Age { get; set; }
			public Person Partner { get; set; }
			public decimal? Salary { get; set; }

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
				JsonSerializer.Serialize (movie, file);
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
			Console.WriteLine (jsonObject.Serialize (false));
			//{
			//	"CPU": "Intel",
			//	"Drives": [
			//		"DVD read/writer",
			//		"500 gigabyte hard drive"
			//	]
			//}
			Assert.AreEqual (
			$"{{{Environment.NewLine}" +
				$"\t\"CPU\": \"Intel\",{Environment.NewLine}" +
				$"\t\"Drives\": [{Environment.NewLine}" +
					$"\t\t\"DVD read/writer\",{Environment.NewLine}" +
					$"\t\t\"500 gigabyte hard drive\"{Environment.NewLine}" +
				$"\t]{Environment.NewLine}" +
			"}", jsonObject.Serialize (false));
		}

		[TestMethod]
		public void NullValueHandling () {
			Person person = new Person {
				Name = "Nigal Newborn",
				Age = 1
			};
			string jsonIncludeNullValues = JsonSerializer.Serialize (person, false);
			Console.WriteLine (jsonIncludeNullValues);
			//{
			//	"Name": "Nigal Newborn",
			//	"Age": 1,
			//	"Partner": null,
			//	"Salary": null
			//}
			string jsonIgnoreNullValues = JsonSerializer.Serialize (person, false, new JsonConfig {
				IgnoreNull = true
			});
			Console.WriteLine (jsonIgnoreNullValues);
			//{
			//	"Name": "Nigal Newborn",
			//	"Age": 1
			//}
			Assert.AreEqual (
			$"{{{Environment.NewLine}" +
				$"\t\"Name\": \"Nigal Newborn\",{Environment.NewLine}" +
				$"\t\"Age\": 1,{Environment.NewLine}" +
				$"\t\"Partner\": null,{Environment.NewLine}" +
				$"\t\"Salary\": null{Environment.NewLine}" +
			"}", jsonIncludeNullValues);
			Assert.AreEqual (
			$"{{{Environment.NewLine}" +
				$"\t\"Name\": \"Nigal Newborn\",{Environment.NewLine}" +
				$"\t\"Age\": 1{Environment.NewLine}" +
			"}", jsonIgnoreNullValues);
		}

	}

}