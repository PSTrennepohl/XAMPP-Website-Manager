using System;
using System.Xml.Serialization;

namespace XWM.Ftp;

[Serializable]
public class User
{
	[XmlAttribute("username")]
	public string Username { get; set; }

	[XmlAttribute("password")]
	public string Password { get; set; }

	[XmlAttribute("homedir")]
	public string HomeDir { get; set; }
}
