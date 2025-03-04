using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using log4net;

namespace XWM.Ftp;

public class ClientConnection : IDisposable
{
	private class DataConnectionOperation
	{
		public Func<NetworkStream, string, string> Operation { get; set; }

		public string Arguments { get; set; }
	}

	private enum TransferType
	{
		Ascii,
		Ebcdic,
		Image,
		Local
	}

	private enum FormatControlType
	{
		NonPrint,
		Telnet,
		CarriageControl
	}

	private enum DataConnectionType
	{
		Passive,
		Active
	}

	private enum FileStructureType
	{
		File,
		Record,
		Page
	}

	private readonly ILog _log = LogManager.GetLogger(typeof(ClientConnection));

	private bool _disposed;

	private TcpListener _passiveListener;

	private TcpClient _controlClient;

	private TcpClient _dataClient;

	private NetworkStream _controlStream;

	private StreamReader _controlReader;

	private StreamWriter _controlWriter;

	private TransferType _connectionType;

	private FormatControlType _formatControlType;

	private DataConnectionType _dataConnectionType = DataConnectionType.Active;

	private FileStructureType _fileStructureType;

	private string _username;

	private string _root;

	private string _currentDirectory;

	private IPEndPoint _dataEndpoint;

	private IPEndPoint _remoteEndPoint;

	private X509Certificate _cert;

	private SslStream _sslStream;

	private string _clientIp;

	private User _currentUser;

	private readonly List<string> _validCommands;

	private static long CopyStream(Stream input, Stream output, int bufferSize)
	{
		byte[] array = new byte[bufferSize];
		long num = 0L;
		int num2;
		while ((num2 = input.Read(array, 0, array.Length)) > 0)
		{
			output.Write(array, 0, num2);
			num += num2;
		}
		return num;
	}

	private static long CopyStreamAscii(Stream input, Stream output, int bufferSize)
	{
		char[] array = new char[bufferSize];
		long num = 0L;
		using StreamReader streamReader = new StreamReader(input, Encoding.ASCII);
		using StreamWriter streamWriter = new StreamWriter(output, Encoding.ASCII);
		int num2;
		while ((num2 = streamReader.Read(array, 0, array.Length)) > 0)
		{
			streamWriter.Write(array, 0, num2);
			num += num2;
		}
		return num;
	}

	private long CopyStream(Stream input, Stream output)
	{
		if (_connectionType == TransferType.Image)
		{
			return CopyStream(input, output, 4096);
		}
		return CopyStreamAscii(input, output, 4096);
	}

	public ClientConnection(TcpClient client)
	{
		_controlClient = client;
		_validCommands = new List<string>();
	}

	private string CheckUser()
	{
		if (_currentUser == null)
		{
			return "530 Not logged in";
		}
		return null;
	}

	public void HandleClient(object obj)
	{
		_remoteEndPoint = (IPEndPoint)_controlClient.Client.RemoteEndPoint;
		_clientIp = _remoteEndPoint.Address.ToString();
		_controlStream = _controlClient.GetStream();
		_controlReader = new StreamReader(_controlStream);
		_controlWriter = new StreamWriter(_controlStream);
		_controlWriter.WriteLine("220 Service Ready.");
		_controlWriter.Flush();
		_validCommands.AddRange(new string[6] { "AUTH", "USER", "PASS", "QUIT", "HELP", "NOOP" });
		_dataClient = new TcpClient();
		string renameFrom = null;
		try
		{
			string text;
			while ((text = _controlReader.ReadLine()) != null)
			{
				string text2 = null;
				string[] array = text.Split(' ');
				string text3 = array[0].ToUpperInvariant();
				string text4 = ((array.Length > 1) ? text.Substring(array[0].Length + 1) : null);
				if (text4 != null && text4.Trim().Length == 0)
				{
					text4 = null;
				}
				LogEntry logEntry = new LogEntry
				{
					Date = DateTime.Now,
					Cip = _clientIp,
					CsUriStem = text4
				};
				if (!_validCommands.Contains(text3))
				{
					text2 = CheckUser();
				}
				if (text3 != "RNTO")
				{
					renameFrom = null;
				}
				if (text2 == null)
				{
					switch (text3)
					{
					case "USER":
						text2 = User(text4);
						break;
					case "PASS":
						text2 = Password(text4);
						logEntry.CsUriStem = "******";
						break;
					case "CWD":
						text2 = ChangeWorkingDirectory(text4);
						break;
					case "CDUP":
						text2 = ChangeWorkingDirectory("..");
						break;
					case "QUIT":
						text2 = "221 Service closing control connection";
						break;
					case "REIN":
						_currentUser = null;
						_username = null;
						_passiveListener = null;
						_dataClient = null;
						text2 = "220 Service ready for new user";
						break;
					case "PORT":
						text2 = Port(text4);
						logEntry.CPort = _dataEndpoint.Port.ToString();
						break;
					case "PASV":
						text2 = Passive();
						logEntry.SPort = ((IPEndPoint)_passiveListener.LocalEndpoint).Port.ToString();
						break;
					case "TYPE":
						text2 = Type(array[1], (array.Length == 3) ? array[2] : null);
						logEntry.CsUriStem = array[1];
						break;
					case "STRU":
						text2 = Structure(text4);
						break;
					case "MODE":
						text2 = Mode(text4);
						break;
					case "RNFR":
						renameFrom = text4;
						text2 = "350 Requested file action pending further information";
						break;
					case "RNTO":
						text2 = Rename(renameFrom, text4);
						break;
					case "DELE":
						text2 = Delete(text4);
						break;
					case "RMD":
						text2 = RemoveDir(text4);
						break;
					case "MKD":
						text2 = CreateDir(text4);
						break;
					case "PWD":
						text2 = PrintWorkingDirectory();
						break;
					case "RETR":
						text2 = Retrieve(text4);
						logEntry.Date = DateTime.Now;
						break;
					case "STOR":
						text2 = Store(text4);
						logEntry.Date = DateTime.Now;
						break;
					case "STOU":
						text2 = StoreUnique();
						logEntry.Date = DateTime.Now;
						break;
					case "APPE":
						text2 = Append(text4);
						logEntry.Date = DateTime.Now;
						break;
					case "LIST":
						text2 = List(_currentDirectory);
						logEntry.Date = DateTime.Now;
						break;
					case "SYST":
						text2 = "215 UNIX Type: L8";
						break;
					case "NOOP":
						text2 = "200 OK";
						break;
					case "ACCT":
						text2 = "200 OK";
						break;
					case "ALLO":
						text2 = "200 OK";
						break;
					case "NLST":
						text2 = "502 Command not implemented";
						break;
					case "SITE":
						text2 = "502 Command not implemented";
						break;
					case "STAT":
						text2 = "502 Command not implemented";
						break;
					case "HELP":
						text2 = "502 Command not implemented";
						break;
					case "SMNT":
						text2 = "502 Command not implemented";
						break;
					case "REST":
						text2 = "502 Command not implemented";
						break;
					case "ABOR":
						text2 = "502 Command not implemented";
						break;
					case "AUTH":
						text2 = Auth(text4);
						break;
					case "FEAT":
						text2 = FeatureList();
						break;
					case "OPTS":
						text2 = Options(text4);
						break;
					case "MDTM":
						text2 = FileModificationTime(text4);
						break;
					case "SIZE":
						text2 = FileSize(text4);
						break;
					case "EPRT":
						text2 = EPort(text4);
						logEntry.CPort = _dataEndpoint.Port.ToString();
						break;
					case "EPSV":
						text2 = EPassive();
						logEntry.SPort = ((IPEndPoint)_passiveListener.LocalEndpoint).Port.ToString();
						break;
					default:
						text2 = "502 Command not implemented";
						break;
					}
				}
				logEntry.CsMethod = text3;
				logEntry.CsUsername = _username;
				logEntry.ScStatus = text2.Substring(0, text2.IndexOf(' '));
				_log.Info(logEntry);
				if (_controlClient != null && _controlClient.Connected)
				{
					_controlWriter.WriteLine(text2);
					_controlWriter.Flush();
					if (!text2.StartsWith("221"))
					{
						if (text3 == "AUTH")
						{
							_cert = new X509Certificate2("server.pfx", "XAMPPWebsiteManager");
							_sslStream = new SslStream(_controlStream);
							_sslStream.AuthenticateAsServer(_cert, clientCertificateRequired: false, SslProtocols.Tls, checkCertificateRevocation: false);
							_controlReader = new StreamReader(_sslStream);
							_controlWriter = new StreamWriter(_sslStream);
						}
						continue;
					}
					break;
				}
				break;
			}
		}
		catch (Exception ex)
		{
			_log.Error(ex.Message);
		}
		Dispose();
	}

	private bool IsPathValid(string path)
	{
		return path.StartsWith(_root);
	}

	private string NormalizeFilename(string path)
	{
		if (path == null)
		{
			path = string.Empty;
		}
		if (path == "/")
		{
			return _root;
		}
		path = ((!path.StartsWith("/")) ? new FileInfo(Path.Combine(_currentDirectory, path)).FullName : new FileInfo(Path.Combine(_root, path.Substring(1))).FullName);
		if (!IsPathValid(path))
		{
			return null;
		}
		return path;
	}

	private string FeatureList()
	{
		_controlWriter.WriteLine("211- Extensions supported:");
		_controlWriter.WriteLine(" MDTM");
		_controlWriter.WriteLine(" SIZE");
		return "211 End";
	}

	private string Options(string arguments)
	{
		return "200 Looks good to me...";
	}

	private string Auth(string authMode)
	{
		if (authMode == "TLS")
		{
			return "234 Enabling TLS Connection";
		}
		return "504 Unrecognized AUTH mode";
	}

	private string User(string username)
	{
		_username = username;
		return "331 Username ok, need password";
	}

	private string Password(string password)
	{
		_currentUser = UserStore.Validate(_username, password);
		if (_currentUser != null)
		{
			_root = _currentUser.HomeDir;
			_currentDirectory = _root;
			return "230 User logged in";
		}
		return "530 Not logged in";
	}

	private string ChangeWorkingDirectory(string pathname)
	{
		if (pathname == "/")
		{
			_currentDirectory = _root;
		}
		else
		{
			string path;
			if (pathname.StartsWith("/"))
			{
				pathname = pathname.Substring(1).Replace('/', '\\');
				path = Path.Combine(_root, pathname);
			}
			else
			{
				pathname = pathname.Replace('/', '\\');
				path = Path.Combine(_currentDirectory, pathname);
			}
			if (Directory.Exists(path))
			{
				_currentDirectory = new DirectoryInfo(path).FullName;
				if (!IsPathValid(_currentDirectory))
				{
					_currentDirectory = _root;
				}
			}
			else
			{
				_currentDirectory = _root;
			}
		}
		return "250 Changed to new directory";
	}

	private string Port(string hostPort)
	{
		_dataConnectionType = DataConnectionType.Active;
		string[] array = hostPort.Split(',');
		byte[] array2 = new byte[4];
		byte[] array3 = new byte[2];
		for (int i = 0; i < 4; i++)
		{
			array2[i] = Convert.ToByte(array[i]);
		}
		for (int j = 4; j < 6; j++)
		{
			array3[j - 4] = Convert.ToByte(array[j]);
		}
		if (BitConverter.IsLittleEndian)
		{
			Array.Reverse(array3);
		}
		_dataEndpoint = new IPEndPoint(new IPAddress(array2), BitConverter.ToInt16(array3, 0));
		return "200 Data Connection Established";
	}

	private string EPort(string hostPort)
	{
		_dataConnectionType = DataConnectionType.Active;
		char c = hostPort[0];
		string[] array = hostPort.Split(new char[1] { c }, StringSplitOptions.RemoveEmptyEntries);
		_ = array[0][0];
		string ipString = array[1];
		string s = array[2];
		_dataEndpoint = new IPEndPoint(IPAddress.Parse(ipString), int.Parse(s));
		return "200 Data Connection Established";
	}

	private string Passive()
	{
		_dataConnectionType = DataConnectionType.Passive;
		IPAddress address = ((IPEndPoint)_controlClient.Client.LocalEndPoint).Address;
		_passiveListener = new TcpListener(address, 0);
		_passiveListener.Start();
		IPEndPoint obj = (IPEndPoint)_passiveListener.LocalEndpoint;
		byte[] addressBytes = obj.Address.GetAddressBytes();
		byte[] bytes = BitConverter.GetBytes((short)obj.Port);
		if (BitConverter.IsLittleEndian)
		{
			Array.Reverse(bytes);
		}
		return $"227 Entering Passive Mode ({addressBytes[0]},{addressBytes[1]},{addressBytes[2]},{addressBytes[3]},{bytes[0]},{bytes[1]})";
	}

	private string EPassive()
	{
		_dataConnectionType = DataConnectionType.Passive;
		IPAddress address = ((IPEndPoint)_controlClient.Client.LocalEndPoint).Address;
		_passiveListener = new TcpListener(address, 0);
		_passiveListener.Start();
		IPEndPoint iPEndPoint = (IPEndPoint)_passiveListener.LocalEndpoint;
		return $"229 Entering Extended Passive Mode (|||{iPEndPoint.Port}|)";
	}

	private string Type(string typeCode, string formatControl)
	{
		string text = typeCode.ToUpperInvariant();
		if (!(text == "A"))
		{
			if (!(text == "I"))
			{
				return "504 Command not implemented for that parameter";
			}
			_connectionType = TransferType.Image;
		}
		else
		{
			_connectionType = TransferType.Ascii;
		}
		if (!string.IsNullOrWhiteSpace(formatControl))
		{
			text = formatControl.ToUpperInvariant();
			if (!(text == "N"))
			{
				return "504 Command not implemented for that parameter";
			}
			_formatControlType = FormatControlType.NonPrint;
		}
		return $"200 Type set to {_connectionType}";
	}

	private string Delete(string pathname)
	{
		pathname = NormalizeFilename(pathname);
		if (pathname != null)
		{
			if (System.IO.File.Exists(pathname))
			{
				System.IO.File.Delete(pathname);
				return "250 Requested file action okay, completed";
			}
			return "550 File Not Found";
		}
		return "550 File Not Found";
	}

	private string RemoveDir(string pathname)
	{
		pathname = NormalizeFilename(pathname);
		if (pathname != null)
		{
			if (Directory.Exists(pathname))
			{
				Directory.Delete(pathname);
				return "250 Requested file action okay, completed";
			}
			return "550 Directory Not Found";
		}
		return "550 Directory Not Found";
	}

	private string CreateDir(string pathname)
	{
		pathname = NormalizeFilename(pathname);
		if (pathname != null)
		{
			if (!Directory.Exists(pathname))
			{
				Directory.CreateDirectory(pathname);
				return "250 Requested file action okay, completed";
			}
			return "550 Directory already exists";
		}
		return "550 Directory Not Found";
	}

	private string FileModificationTime(string pathname)
	{
		pathname = NormalizeFilename(pathname);
		if (pathname != null && System.IO.File.Exists(pathname))
		{
			return string.Format("213 {0}", System.IO.File.GetLastWriteTime(pathname).ToString("yyyyMMddHHmmss.fff"));
		}
		return "550 File Not Found";
	}

	private string FileSize(string pathname)
	{
		pathname = NormalizeFilename(pathname);
		if (pathname != null && System.IO.File.Exists(pathname))
		{
			long num = 0L;
			using (FileStream fileStream = System.IO.File.Open(pathname, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				num = fileStream.Length;
			}
			return $"213 {num}";
		}
		return "550 File Not Found";
	}

	private string Retrieve(string pathname)
	{
		pathname = NormalizeFilename(pathname);
		if (pathname != null && System.IO.File.Exists(pathname))
		{
			DataConnectionOperation state = new DataConnectionOperation
			{
				Arguments = pathname,
				Operation = RetrieveOperation
			};
			SetupDataConnectionOperation(state);
			return $"150 Opening {_dataConnectionType} mode data transfer for RETR";
		}
		return "550 File Not Found";
	}

	private string Store(string pathname)
	{
		pathname = NormalizeFilename(pathname);
		if (pathname != null)
		{
			DataConnectionOperation state = new DataConnectionOperation
			{
				Arguments = pathname,
				Operation = StoreOperation
			};
			SetupDataConnectionOperation(state);
			return $"150 Opening {_dataConnectionType} mode data transfer for STOR";
		}
		return "450 Requested file action not taken";
	}

	private string Append(string pathname)
	{
		pathname = NormalizeFilename(pathname);
		if (pathname != null)
		{
			DataConnectionOperation state = new DataConnectionOperation
			{
				Arguments = pathname,
				Operation = AppendOperation
			};
			SetupDataConnectionOperation(state);
			return $"150 Opening {_dataConnectionType} mode data transfer for APPE";
		}
		return "450 Requested file action not taken";
	}

	private string StoreUnique()
	{
		string arguments = NormalizeFilename(default(Guid).ToString());
		DataConnectionOperation state = new DataConnectionOperation
		{
			Arguments = arguments,
			Operation = StoreOperation
		};
		SetupDataConnectionOperation(state);
		return $"150 Opening {_dataConnectionType} mode data transfer for STOU";
	}

	private string PrintWorkingDirectory()
	{
		string text = _currentDirectory.Replace(_root, string.Empty).Replace('\\', '/');
		if (text.Length == 0)
		{
			text = "/";
		}
		return $"257 \"{text}\" is current directory.";
	}

	private string List(string pathname)
	{
		pathname = NormalizeFilename(pathname);
		if (pathname != null)
		{
			DataConnectionOperation state = new DataConnectionOperation
			{
				Arguments = pathname,
				Operation = ListOperation
			};
			SetupDataConnectionOperation(state);
			return $"150 Opening {_dataConnectionType} mode data transfer for LIST";
		}
		return "450 Requested file action not taken";
	}

	private string Structure(string structure)
	{
		switch (structure)
		{
		case "F":
			_fileStructureType = FileStructureType.File;
			return "200 Command OK";
		case "R":
		case "P":
			return $"504 STRU not implemented for \"{structure}\"";
		default:
			return $"501 Parameter {structure} not recognized";
		}
	}

	private string Mode(string mode)
	{
		if (mode.ToUpperInvariant() == "S")
		{
			return "200 OK";
		}
		return "504 Command not implemented for that parameter";
	}

	private string Rename(string renameFrom, string renameTo)
	{
		if (string.IsNullOrWhiteSpace(renameFrom) || string.IsNullOrWhiteSpace(renameTo))
		{
			return "450 Requested file action not taken";
		}
		renameFrom = NormalizeFilename(renameFrom);
		renameTo = NormalizeFilename(renameTo);
		if (renameFrom != null && renameTo != null)
		{
			if (System.IO.File.Exists(renameFrom))
			{
				System.IO.File.Move(renameFrom, renameTo);
			}
			else
			{
				if (!Directory.Exists(renameFrom))
				{
					return "450 Requested file action not taken";
				}
				Directory.Move(renameFrom, renameTo);
			}
			return "250 Requested file action okay, completed";
		}
		return "450 Requested file action not taken";
	}

	private void HandleAsyncResult(IAsyncResult result)
	{
		if (_dataConnectionType == DataConnectionType.Active)
		{
			_dataClient.EndConnect(result);
		}
		else
		{
			_dataClient = _passiveListener.EndAcceptTcpClient(result);
		}
	}

	private void SetupDataConnectionOperation(DataConnectionOperation state)
	{
		if (_dataConnectionType == DataConnectionType.Active)
		{
			_dataClient = new TcpClient(_dataEndpoint.AddressFamily);
			_dataClient.BeginConnect(_dataEndpoint.Address, _dataEndpoint.Port, DoDataConnectionOperation, state);
		}
		else
		{
			_passiveListener.BeginAcceptTcpClient(DoDataConnectionOperation, state);
		}
	}

	private void DoDataConnectionOperation(IAsyncResult result)
	{
		HandleAsyncResult(result);
		DataConnectionOperation dataConnectionOperation = result.AsyncState as DataConnectionOperation;
		string value;
		using (NetworkStream arg = _dataClient.GetStream())
		{
			value = dataConnectionOperation.Operation(arg, dataConnectionOperation.Arguments);
		}
		_dataClient.Close();
		_dataClient = null;
		_controlWriter.WriteLine(value);
		_controlWriter.Flush();
	}

	private string RetrieveOperation(NetworkStream dataStream, string pathname)
	{
		using (FileStream input = new FileStream(pathname, FileMode.Open, FileAccess.Read))
		{
			CopyStream(input, dataStream);
		}
		return "226 Closing data connection, file transfer successful";
	}

	private string StoreOperation(NetworkStream dataStream, string pathname)
	{
		long num = 0L;
		using (FileStream output = new FileStream(pathname, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, 4096, FileOptions.SequentialScan))
		{
			num = CopyStream(dataStream, output);
		}
		LogEntry message = new LogEntry
		{
			Date = DateTime.Now,
			Cip = _clientIp,
			CsMethod = "STOR",
			CsUsername = _username,
			ScStatus = "226",
			CsBytes = num.ToString()
		};
		_log.Info(message);
		return "226 Closing data connection, file transfer successful";
	}

	private string AppendOperation(NetworkStream dataStream, string pathname)
	{
		long num = 0L;
		using (FileStream output = new FileStream(pathname, FileMode.Append, FileAccess.Write, FileShare.None, 4096, FileOptions.SequentialScan))
		{
			num = CopyStream(dataStream, output);
		}
		LogEntry message = new LogEntry
		{
			Date = DateTime.Now,
			Cip = _clientIp,
			CsMethod = "APPE",
			CsUsername = _username,
			ScStatus = "226",
			CsBytes = num.ToString()
		};
		_log.Info(message);
		return "226 Closing data connection, file transfer successful";
	}

	private string ListOperation(NetworkStream dataStream, string pathname)
	{
		StreamWriter streamWriter = new StreamWriter(dataStream, Encoding.ASCII);
		foreach (string item in Directory.EnumerateDirectories(pathname))
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(item);
			string arg = ((directoryInfo.LastWriteTime < DateTime.Now - TimeSpan.FromDays(180.0)) ? directoryInfo.LastWriteTime.ToString("MMM dd  yyyy") : directoryInfo.LastWriteTime.ToString("MMM dd HH:mm"));
			string value = string.Format("drwxr-xr-x    2 2003     2003     {0,8} {1} {2}", "4096", arg, directoryInfo.Name);
			streamWriter.AutoFlush = true;
			streamWriter.WriteLine(value);
		}
		foreach (string item2 in Directory.EnumerateFiles(pathname))
		{
			FileInfo fileInfo = new FileInfo(item2);
			string arg2 = ((fileInfo.LastWriteTime < DateTime.Now - TimeSpan.FromDays(180.0)) ? fileInfo.LastWriteTime.ToString("MMM dd  yyyy") : fileInfo.LastWriteTime.ToString("MMM dd HH:mm"));
			string value2 = $"-rw-r--r--    2 2003     2003     {fileInfo.Length,8} {arg2} {fileInfo.Name}";
			streamWriter.WriteLine(value2);
			streamWriter.Flush();
		}
		LogEntry message = new LogEntry
		{
			Date = DateTime.Now,
			Cip = _clientIp,
			CsMethod = "LIST",
			CsUsername = _username,
			ScStatus = "226"
		};
		_log.Info(message);
		return "226 Transfer complete";
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!_disposed && disposing)
		{
			if (_controlClient != null)
			{
				_controlClient.Close();
			}
			if (_dataClient != null)
			{
				_dataClient.Close();
			}
			if (_controlStream != null)
			{
				_controlStream.Close();
			}
			if (_controlReader != null)
			{
				_controlReader.Close();
			}
			if (_controlWriter != null)
			{
				_controlWriter.Close();
			}
		}
		_disposed = true;
	}
}
