#例程
##序列化对象到Json字符串
```C#
public class Account {

	public string Email { get; set; }
	public bool Active { get; set; }
	public DateTime CreatedDate { get; set; }
	public IList<string> Roles { get; set; }

}
```
```C#
Account account = new Account {
	Email = "james@example.com",
	Active = true,
	CreatedDate = new DateTime (2013, 1, 20, 0, 0, 0, DateTimeKind.Utc),
	Roles = new List<string> () {
		"User",
		"Admin"
	}
};
//参数二为是否压缩Json字符串
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
```
##序列化对象到Json文本文件
```C#
public class Movie {

	public string Name { get; set; }
	public int Year { get; set; }

}
```
```C#
Movie movie = new Movie {
	Name = "Bad Boys",
	Year = 1995
};
string path = @"d:\movie.json";
//序列化对象为Json字符串，然后写到文本文件
File.WriteAllText (path, JsonSerializer.Serialize (movie));
//以流的方式序列化对象到Json文本文件
using (StreamWriter file = File.CreateText (path)) {
	JsonSerializer.SerializeStreamWriter (file, movie);
}
```
##反序列化Json字符串到对象
```C#
public class Account {

	public string Email { get; set; }
	public bool Active { get; set; }
	public DateTime CreatedDate { get; set; }
	public IList<string> Roles { get; set; }

}
```
```C#
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
```
##反序列化Json文本文件到对象
```C#
public class Movie {

	public string Name { get; set; }
	public int Year { get; set; }

}
```
```C#
string path = @"d:\movie.json";
//读入Json字符串，然后反序列化Json字符串到对象
Movie movie = JsonDeserializer.Deserialize<Movie> (File.ReadAllText (path));
//以流的方式从Json文本文件反序列化到对象
using (StreamReader file = File.OpenText (path)) {
	Movie movie2 = JsonDeserializer.Deserialize<Movie> (file);
}
```
##使用JsonObject.Parse解析Json字符串
```C#
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
```