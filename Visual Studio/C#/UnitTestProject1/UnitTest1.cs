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
			string json = JsonConvert.Serialize (account, false);
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
			string path = @"movie.json";
			File.WriteAllText (path, JsonConvert.Serialize (movie));
			using (StreamWriter file = File.CreateText (path)) {
				JsonConvert.Serialize (movie, file);
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
			Account account = JsonConvert.Deserialize<Account> (json);
			Console.WriteLine (account.Email);
			//james@example.com
			Assert.AreEqual ("james@example.com", account.Email);
		}

		[TestMethod]
		public void DeserializeJsonFromAFile () {
			string path = @"movie.json";
			Movie movie = JsonConvert.Deserialize<Movie> (File.ReadAllText (path));
			using (StreamReader file = File.OpenText (path)) {
				Movie movie2 = JsonConvert.Deserialize<Movie> (file);
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
			string jsonIncludeNullValues = JsonConvert.Serialize (person, false);
			Console.WriteLine (jsonIncludeNullValues);
			//{
			//	"Name": "Nigal Newborn",
			//	"Age": 1,
			//	"Partner": null,
			//	"Salary": null
			//}
			string jsonIgnoreNullValues = JsonConvert.Serialize (person, false, new JsonConfig {
				IgnoreNullValue = true
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

		[TestMethod]
		public void DefaultValueHandling () {
			Person person = new Person ();
			string jsonIncludeDefaultValues = JsonConvert.Serialize (person, false);
			Console.WriteLine (jsonIncludeDefaultValues);
			//{
			//	"Name": null,
			//	"Age": 0,
			//	"Partner": null,
			//	"Salary": null
			//}
			string jsonIgnoreDefaultValues = JsonConvert.Serialize (person, false, new JsonConfig {
				IgnoreDefaultValue = true
			});
			Console.WriteLine (jsonIgnoreDefaultValues);
			//{}
			Assert.AreEqual ($"{{{Environment.NewLine}" +
				$"\t\"Name\": null,{Environment.NewLine}" +
				$"\t\"Age\": 0,{Environment.NewLine}" +
				$"\t\"Partner\": null,{Environment.NewLine}" +
				$"\t\"Salary\": null{Environment.NewLine}" +
			"}", jsonIncludeDefaultValues);
			Assert.AreEqual ("{}", jsonIgnoreDefaultValues);
		}

		[TestMethod]
		public void QueryingJsonWithJsonPath () {
			JsonObject o = JsonObject.Parse (
			@"{
				'Stores': [
					'Lambton Quay',
					'Willis Street'
				],
				'Manufacturers': [
					{
						'Name': 'Acme Co',
						'Products': [
							{
								'Name': 'Anvil',
								'Price': 50
							}
						]
					},
					{
						'Name': 'Contoso',
						'Products': [
							{
								'Name': 'Elbow Grease',
								'Price': 99.95
							},
							{
								'Name': 'Headlight Fluid',
								'Price': 4
							}
						]
					}
				]
			}");
			string name = o.Select ("Manufacturers[0].Name");
			Console.WriteLine (name);
			// Acme Co
			decimal productPrice = o.Select ("Manufacturers[0].Products[0].Price");
			Console.WriteLine (productPrice);
			// 50
			string productName = o.Select ("Manufacturers[1].Products[0].Name");
			Console.WriteLine (productName);
			// Elbow Grease
			Assert.AreEqual ("Acme Co", name);
			Assert.AreEqual (50M, productPrice);
			Assert.AreEqual ("Elbow Grease", productName);
		}

	}

}