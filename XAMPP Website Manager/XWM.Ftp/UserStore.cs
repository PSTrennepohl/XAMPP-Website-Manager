using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace XWM.Ftp;

public static class UserStore
{
	public static List<User> Users;

	private static string _filename;

	static UserStore()
	{
		_filename = "ftp_users.xml";
		Users = new List<User>();
		XmlSerializer xmlSerializer = new XmlSerializer(Users.GetType(), new XmlRootAttribute("Users"));
		if (System.IO.File.Exists(_filename))
		{
			Users = xmlSerializer.Deserialize(new StreamReader(_filename)) as List<User>;
		}
	}

	public static void Save()
	{
		XmlSerializer xmlSerializer = new XmlSerializer(Users.GetType(), new XmlRootAttribute("Users"));
		using StreamWriter textWriter = new StreamWriter(_filename);
		xmlSerializer.Serialize(textWriter, Users);
	}

	public static User Validate(string username, string password)
	{
		return Users.Where((User u) => u.Username == username && u.Password == password).SingleOrDefault();
	}
}
