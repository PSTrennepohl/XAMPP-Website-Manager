using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace XWM.Ftp;

public class RateLimitingStream : Stream
{
	private readonly Stream _baseStream;

	private readonly Stopwatch _watch;

	private readonly int _speedLimit;

	private long _transmitted;

	private readonly double _resolution;

	public override bool CanRead => false;

	public override bool CanSeek => false;

	public override bool CanWrite => true;

	public override long Length => _baseStream.Length;

	public override long Position
	{
		get
		{
			return _baseStream.Position;
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public RateLimitingStream(Stream baseStream, int speedLimit)
		: this(baseStream, speedLimit, 1.0)
	{
	}

	public RateLimitingStream(Stream baseStream, int speedLimit, double resolution)
	{
		_baseStream = baseStream;
		_watch = new Stopwatch();
		_speedLimit = speedLimit;
		_resolution = resolution;
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		if (!_watch.IsRunning)
		{
			_watch.Start();
		}
		int num = 0;
		while (_speedLimit > 0 && (double)_transmitted >= (double)_speedLimit * _resolution)
		{
			Thread.Sleep(10);
			if ((double)_watch.ElapsedMilliseconds > 1000.0 * _resolution)
			{
				_transmitted = 0L;
				_watch.Restart();
			}
		}
		_baseStream.Write(buffer, offset, count);
		_transmitted += count;
		num += count;
		if ((double)_watch.ElapsedMilliseconds > 1000.0 * _resolution)
		{
			_transmitted = 0L;
			_watch.Restart();
		}
	}

	public override void Flush()
	{
		_baseStream.Flush();
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		throw new NotImplementedException();
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		throw new NotImplementedException();
	}

	public override void SetLength(long value)
	{
		throw new NotImplementedException();
	}

	protected override void Dispose(bool disposing)
	{
		_watch.Stop();
		base.Dispose(disposing);
	}
}
