using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using log4net;

namespace XWM.Ftp;

public class FtpServer : IDisposable
{
	private readonly ILog _log = LogManager.GetLogger(typeof(FtpServer));

	private bool _disposed;

	private bool _listening;

	private TcpListener _listener;

	private List<ClientConnection> _activeConnections;

	private readonly IPEndPoint _localEndPoint;

	public FtpServer()
		: this(IPAddress.Any, 21)
	{
	}

	public FtpServer(IPAddress ipAddress, int port)
	{
		_localEndPoint = new IPEndPoint(ipAddress, port);
	}

	public void Start()
	{
		_listener = new TcpListener(_localEndPoint);
		_log.Info("#Version: 1.0");
		_log.Info("#Fields: date time c-ip c-port cs-username cs-method cs-uri-stem sc-status sc-bytes cs-bytes s-name s-port");
		_listening = true;
		_listener.Start();
		_activeConnections = new List<ClientConnection>();
		_listener.BeginAcceptTcpClient(HandleAcceptTcpClient, _listener);
	}

	public void Stop()
	{
		_log.Info("Stopping FtpServer");
		_listening = false;
		if (_listener != null)
		{
			_listener.Stop();
			_listener = null;
		}
	}

	private void HandleAcceptTcpClient(IAsyncResult result)
	{
		if (_listening)
		{
			_listener.BeginAcceptTcpClient(HandleAcceptTcpClient, _listener);
			TcpClient tcpClient = _listener.EndAcceptTcpClient(result);
			ClientConnection clientConnection = new ClientConnection(tcpClient);
			_activeConnections.Add(clientConnection);
			ThreadPool.QueueUserWorkItem(clientConnection.HandleClient, tcpClient);
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!_disposed && disposing)
		{
			Stop();
			foreach (ClientConnection activeConnection in _activeConnections)
			{
				activeConnection.Dispose();
			}
		}
		_disposed = true;
	}
}
