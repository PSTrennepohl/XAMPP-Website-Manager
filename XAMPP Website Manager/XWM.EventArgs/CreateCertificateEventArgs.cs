using System;

namespace XWM.EventArgs;

public class CreateCertificateEventArgs : System.EventArgs
{
	public string CertificateFilePath { get; set; }

	public string CertificateKeyPath { get; set; }
}
